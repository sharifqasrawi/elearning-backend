﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Learning.Helpers;
using E_Learning.Models;
using E_Learning.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_Learning.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ClassesController : ControllerBase
    {
        private readonly IClassRepository _classRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationRepository _notificationRepository;
        public ClassesController(IClassRepository classRepository, 
                                 ICourseRepository courseRepository,
                                 ISessionRepository sessionRepository,
                                 UserManager<ApplicationUser> userManager,
                                 INotificationRepository notificationRepository)
        {
            _classRepository = classRepository;
            _courseRepository = courseRepository;
            _userManager = userManager;
            _notificationRepository = notificationRepository;
            _sessionRepository = sessionRepository;
        }


        [AllowAnonymous]
        [HttpPost]
        public IActionResult CreateClass([FromBody] Class cls)
        {
            var errorMessages = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(cls.Name_EN) || cls.CourseId == null)
                {
                    errorMessages.Add("Class name and course Id are required to create class.");
                    return BadRequest(new { errors = errorMessages });
                }

                if (_classRepository.IsExistsInCourse(cls.CourseId.Value))
                {
                    errorMessages.Add("This course already has a class.");
                    return BadRequest(new { errors = errorMessages });
                }


                var course = _courseRepository.FindById(cls.CourseId.Value);
                if (course == null)
                {
                    errorMessages.Add("Error 404. Could not find course.");
                    return BadRequest(new { errors = errorMessages });
                }

                var newCls = new Class()
                {
                    Id = $"{course.Id.ToString()}-{Guid.NewGuid().ToString()}",
                    Name_EN = cls.Name_EN,
                    Course = course,
                    CourseId = course.Id
                };

                var createdClass = _classRepository.Create(newCls);

                if (createdClass == null)
                {
                    errorMessages.Add("Error creating class.");
                    return BadRequest(new { errors = errorMessages });
                }

                return Ok(new { course = ResponseGenerator.GenerateCourseResponse(course) });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpPost("enroll")]
        public async Task<IActionResult> EnrollInClass([FromBody] ClassUser classUser, [FromQuery] string action)
        {
            var errorMessages = new List<string>();
            try
            {
                if(string.IsNullOrEmpty(classUser.ClassId) || classUser.UserId == null)
                {
                    errorMessages.Add("Error joining class.");
                    return BadRequest(new { errors = errorMessages });
                }

                var cls = _classRepository.FindById(classUser.ClassId);
                var member = await _userManager.FindByIdAsync(classUser.UserId);

                if (cls == null || member == null)
                {
                    errorMessages.Add("Error joining class.");
                    return BadRequest(new { errors = errorMessages });
                }
                if (action == "enroll")
                {
                    var newClassUser = new ClassUser()
                    {
                        Class = cls,
                        ClassId = classUser.ClassId,
                        User = member,
                        UserId = classUser.UserId,
                        EnrollDateTime = DateTime.Now
                    };

                    cls.ClassUsers.Add(newClassUser);
                }
                else if (action == "disenroll")
                {
                    var enrollement = cls.ClassUsers
                       .SingleOrDefault(e => e.ClassId == classUser.ClassId
                                          && e.UserId == classUser.UserId);

                    cls.ClassUsers.Remove(enrollement);
                }

                var updatedClass = _classRepository.Update(cls);
                if (updatedClass != null)
                {
                    if(action == "enroll")
                    {
                        await _notificationRepository.Create(new Notification()
                        {
                            Type="ENROLLEMENT",
                            Text=$"{member.FirstName} {member.LastName} enrolled in class [ {cls.Name_EN} ]",
                            DateTime = DateTime.Now,
                            Info = null,
                            IsSeen = false,
                        });
                    }
                    else if (action == "disenroll")
                    {
                        await _notificationRepository.Create(new Notification()
                        {
                            Type = "ENROLLEMENT",
                            Text = $"{member.FirstName} {member.LastName} disenrolled from class [ {cls.Name_EN} ]",
                            DateTime = DateTime.Now,
                            Info = null,
                            IsSeen = false,
                        });
                    }
                }

                var course = _courseRepository.FindByClassId(updatedClass.Id);

                return Ok(new { course = ResponseGenerator.GenerateCourseResponse(course) });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpPut("set-current-session")]
        public async Task<IActionResult> SetCurrentSession([FromBody] ClassUser classUser)
        {
            var errorMessages = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(classUser.ClassId) || classUser.UserId == null || classUser.CurrentSessionId == null)
                {
                    errorMessages.Add("An error occured. Please try again later.");
                    return BadRequest(new { errors = errorMessages });
                }

                var cls = _classRepository.FindById(classUser.ClassId);
                var member = await _userManager.FindByIdAsync(classUser.UserId);

                if (cls == null || member == null)
                {
                    errorMessages.Add("An error occured. Invalid data.");
                    return BadRequest(new { errors = errorMessages });
                }


                var enrollement = cls.ClassUsers
                       .SingleOrDefault(e => e.ClassId == classUser.ClassId
                                          && e.UserId == classUser.UserId);

                var session = _sessionRepository.FindById(classUser.CurrentSessionId.Value);

                if (enrollement == null || session == null)
                {
                    errorMessages.Add("An error occured. Invalid data.");
                    return BadRequest(new { errors = errorMessages });
                }

                enrollement.CurrentSession = session;
                enrollement.CurrentSessionId = classUser.CurrentSessionId;
                enrollement.CurrentSessionSlug = session.Slug_EN;

                var updatedClass = _classRepository.Update(cls);

                return Ok(true);
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }
    }
}