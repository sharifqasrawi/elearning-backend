using System;
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
        private readonly IDoneSessionRepository _doneSessionRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationRepository _notificationRepository;
        private readonly ITranslator _translator;
        public ClassesController(IClassRepository classRepository,
                                 ICourseRepository courseRepository,
                                 ISessionRepository sessionRepository,
                                 IDoneSessionRepository doneSessionRepository,
                                 UserManager<ApplicationUser> userManager,
                                 INotificationRepository notificationRepository,
                                 ITranslator translator)
        {
            _classRepository = classRepository;
            _courseRepository = courseRepository;
            _userManager = userManager;
            _notificationRepository = notificationRepository;
            _sessionRepository = sessionRepository;
            _doneSessionRepository = doneSessionRepository;
            _translator = translator;
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult CreateClass([FromBody] Class cls)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(cls.Name_EN) || cls.CourseId == null)
                {
                    errorMessages.Add(_translator.GetTranslation("VALIDATION.CLASSNAME_COURSEID_REQUIRED", lang));
                    return BadRequest(new { errors = errorMessages });
                }

                if (_classRepository.IsExistsInCourse(cls.CourseId.Value))
                {
                    errorMessages.Add(_translator.GetTranslation("CLASSES.COURSE_HAS_CLASS", lang));
                    return BadRequest(new { errors = errorMessages });
                }


                var course = _courseRepository.FindById(cls.CourseId.Value);
                if (course == null)
                {
                    return NotFound();
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
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }

                return Ok(new { course = ResponseGenerator.GenerateCourseResponse(course, true) });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin, Author, User")]
        [HttpPost("enroll")]
        public async Task<IActionResult> EnrollInClass([FromBody] ClassUser classUser, [FromQuery] string action, [FromQuery] string userId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(classUser.ClassId) || classUser.UserId == null)
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }

                var cls = _classRepository.FindById(classUser.ClassId);
                var member = await _userManager.FindByIdAsync(classUser.UserId);

                if (cls == null || member == null)
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
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

                    _doneSessionRepository.DeleteAllByCourseUser(cls.Course.Id, classUser.UserId);
                  
                    cls.ClassUsers.Remove(enrollement);

                }

                var updatedClass = _classRepository.Update(cls);
                if (updatedClass != null)
                {
                    if (action == "enroll")
                    {
                        await _notificationRepository.Create(new Notification()
                        {
                            Type = "ENROLLEMENT",
                            Text = $"{member.FirstName} {member.LastName} enrolled in class [ {cls.Name_EN} ]",
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

                var user = await _userManager.FindByIdAsync(userId);
                bool isAdmin = false;
                if (user != null)
                    isAdmin = user.IsAdmin && (await _userManager.IsInRoleAsync(user, "Admin"));

                return Ok(new { updatedClass = ResponseGenerator.GenerateClassResponse(cls, isAdmin) });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin, Author, User")]
        [HttpPut("set-current-session")]
        public async Task<IActionResult> SetCurrentSession([FromBody] ClassUser classUser)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(classUser.ClassId) || classUser.UserId == null || classUser.CurrentSessionId == null)
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }

                var cls = _classRepository.FindById(classUser.ClassId);
                var member = await _userManager.FindByIdAsync(classUser.UserId);

                if (cls == null || member == null)
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }


                var enrollement = cls.ClassUsers
                       .SingleOrDefault(e => e.ClassId == classUser.ClassId
                                          && e.UserId == classUser.UserId);

                var session = _sessionRepository.FindById(classUser.CurrentSessionId.Value);

                if (enrollement == null || session == null)
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }

                enrollement.CurrentSession = session;
                enrollement.CurrentSessionId = classUser.CurrentSessionId;
                enrollement.CurrentSessionSlug = session.Slug_EN;

                var updatedClass = _classRepository.Update(cls);

                return Ok(true);
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin, Author, User")]
        [HttpGet("non-members")]
        public IActionResult GetNonMembers([FromQuery] string classId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var cls = _classRepository.FindById(classId);
                var clsMembers = cls.ClassUsers;
                var allUsers = _userManager.Users;
                var nonMembers = new List<object>();

                foreach (var user in allUsers)
                {
                    var newMember = new
                    {
                        id = user.Id,
                        fullName = $"{user.FirstName} {user.LastName}"
                    };

                    nonMembers.Add(newMember);

                }

                return Ok(new { nonMembers  });

            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }
    }
}