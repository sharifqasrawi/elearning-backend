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
using Slugify;

namespace E_Learning.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IClassRepository _classRepository;
        private readonly ITranslator _translator;
        public CoursesController(ICourseRepository courseRepository,
                                 ITagRepository tagRepository,
                                 ISectionRepository sectionRepository,
                                 ISessionRepository sessionRepository,
                                 UserManager<ApplicationUser> userManager,
                                 ICategoryRepository categoryRepository,
                                 IClassRepository classRepository,
                                 ITranslator translator)
        {
            _courseRepository = courseRepository;
            _tagRepository = tagRepository;
            _userManager = userManager;
            _categoryRepository = categoryRepository;
            _sectionRepository = sectionRepository;
            _sessionRepository = sessionRepository;
            _classRepository = classRepository;
            _translator = translator;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetCourses([FromQuery] int? categoryId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var courses = _courseRepository.GetCourses()
                                                .Where(c => c.DeletedAt == null 
                                                            && (c.Category.Id == categoryId || categoryId == null))
                                                .OrderBy(c => c.Title_EN)
                                                .ThenBy(c => c.Category)
                                                .ThenBy(c => c.CreatedAt);

                var response = new List<object>();

                foreach (var course in courses)
                {
                    var res = ResponseGenerator.GenerateCourseResponse(course, true);
                  
                    response.Add(res);
                }

                return Ok(new { courses = response });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("course")]
        public IActionResult GetCourse([FromQuery] long id)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var course = _courseRepository.FindById(id);

                var response = ResponseGenerator.GenerateCourseResponse(course, true);

                return Ok(new { course = response });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("deleted")]
        public IActionResult GetDeletedCourses()
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var courses = _courseRepository.GetCourses()
                                              .Where(c => c.DeletedAt != null);

                var response = new List<object>();

                foreach (var course in courses)
                {
                    var res = ResponseGenerator.GenerateCourseResponse(course,true);

                    response.Add(res);
                }

                return Ok(new { courses = response });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create-course")]
        public IActionResult CreateCourse([FromBody] Course course)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();

            try
            {
                //var author = await _userManager.FindByIdAsync(course.Author.Id);
                var category = _categoryRepository.GetCategory(course.Category.Id);

                var slugHelper = new SlugHelper();
                var newCourse = new Course()
                {
                    Title_EN = course.Title_EN,
                    Title_FR = course.Title_FR ?? course.Title_EN,
                    Slug_EN = slugHelper.GenerateSlug(course.Title_EN),
                    Slug_FR = course.Title_FR != null ? slugHelper.GenerateSlug(course.Title_FR) : slugHelper.GenerateSlug(course.Title_EN),
                    Description_EN = course.Description_EN,
                    Description_FR = course.Description_FR ?? course.Description_EN,
                    Prerequisites_EN = course.Prerequisites_EN,
                    Prerequisites_FR = course.Prerequisites_FR ?? course.Prerequisites_EN,
                    Duration = course.Duration,
                    ImagePath = course.ImagePath,
                    IsFree = course.IsFree == null ? false : course.IsFree.Value,
                    Price = course.Price,
                    IsPublished = course.IsPublished == null ? false : course.IsPublished.Value,
                    CreatedBy = course.CreatedBy,
                    UpdatedBy = course.UpdatedBy,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    PublishedAt = null,
                    DeletedAt = null,
                    DeletedBy = null,
                    //Author = author,
                    Category = category,
                    Languages = course.Languages,
                    Level = course.Level,
                    Class = null
                };

                if (newCourse.IsPublished.Value == true)
                    newCourse.PublishedAt = DateTime.Now;

                var createdCourse = _courseRepository.Create(newCourse);

                var response = ResponseGenerator.GenerateCourseResponse(createdCourse, true);


                if (createdCourse != null)
                {

                    return Ok(new { createdCourse = response });
                }

                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new
                {
                    errors = errorMessages
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update-course")]
        public IActionResult UpdateCourse([FromBody] Course course)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();

            try
            {
                //var author = await _userManager.FindByIdAsync(course.Author.Id);
                var category = _categoryRepository.GetCategory(course.Category.Id);
                var newCourse = _courseRepository.FindById(course.Id);


                newCourse.Title_EN = course.Title_EN;
                newCourse.Title_FR = course.Title_FR ?? course.Title_EN;
                newCourse.Slug_EN = new SlugHelper().GenerateSlug(course.Title_EN);
                newCourse.Slug_FR = course.Title_FR  != null ?  new SlugHelper().GenerateSlug(course.Title_FR): new SlugHelper().GenerateSlug(course.Title_EN);
                newCourse.Description_EN = course.Description_EN;
                newCourse.Description_FR = course.Description_FR ?? course.Description_EN;
                newCourse.Prerequisites_EN = course.Prerequisites_EN;
                newCourse.Prerequisites_FR = course.Prerequisites_FR ?? course.Prerequisites_EN;
                newCourse.Duration = course.Duration;
                newCourse.ImagePath = course.ImagePath;
                newCourse.IsFree = course.IsFree == null ? false : course.IsFree.Value;
                newCourse.Price = course.Price;
                newCourse.IsPublished = course.IsPublished == null ? false : course.IsPublished.Value;
                newCourse.UpdatedBy = course.UpdatedBy;
                newCourse.UpdatedAt = DateTime.Now;
                newCourse.PublishedAt = null;
                //newCourse.Author = author;
                newCourse.Category = category;
                newCourse.Languages = course.Languages;
                newCourse.Level = course.Level;

                if (newCourse.IsPublished.Value == true)
                    newCourse.PublishedAt = DateTime.Now;

                var updatedCourse = _courseRepository.Update(newCourse);

                var response = ResponseGenerator.GenerateCourseResponse(updatedCourse, true);


                if (updatedCourse != null)
                {

                    return Ok(new { updatedCourse = response });
                }

                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("trash-restore-course")]
        public IActionResult TrashCourse([FromBody] Course course, [FromQuery] string action)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var crs = _courseRepository.FindById(course.Id);

                if (action == "trash")
                {
                    crs.DeletedAt = DateTime.Now;
                    crs.DeletedBy = course.DeletedBy;
                }
                else if (action == "restore")
                {
                    crs.DeletedAt = null;
                    crs.DeletedBy = null;
                }
                var updatedCourse = _courseRepository.Update(crs);

                var response = ResponseGenerator.GenerateCourseResponse(updatedCourse, true);

                return Ok(new { updatedCourse = response });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("publish")]
        public IActionResult PublishCourse([FromBody] Course course, [FromQuery] string action)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var crs = _courseRepository.FindById(course.Id);
                if (crs == null)
                    return NotFound();

                if (action == "publish")
                {
                    crs.IsPublished = true;
                    crs.PublishedAt = DateTime.Now;
                }
                else if (action == "unpublish")
                {
                    crs.IsPublished = false;
                    crs.PublishedAt = null;

                }
                var updatedCourse = _courseRepository.Update(crs);

                var response = ResponseGenerator.GenerateCourseResponse(updatedCourse, true);

                return Ok(new { updatedCourse = response });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        ////// Tags //////

        [Authorize(Roles = "Admin")]
        [HttpPut("tag-course")]
        public IActionResult AddTagToCourse([FromBody] CourseTag courseTag, [FromQuery] string action)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var course = _courseRepository.FindById(courseTag.CourseId);
                var courseTags = course.CourseTags;

                if (action == "add")
                {
                    var newTag = _tagRepository.FindById(courseTag.TagId);
                    var newCourseTag = new CourseTag()
                    {
                        CourseId = courseTag.CourseId,
                        Course = course,
                        TagId = courseTag.TagId,
                        Tag = newTag
                    };
                    courseTags.Add(newCourseTag);
                }
                else if(action == "remove")
                {
                    var currentCourseTag = courseTags
                                 .SingleOrDefault(x => x.TagId == courseTag.TagId && x.CourseId == courseTag.CourseId);

                    courseTags.Remove(currentCourseTag);
                }
                course.CourseTags = courseTags;

                var updatedCourse = _courseRepository.Update(course);

                var response = ResponseGenerator.GenerateCourseResponse(updatedCourse, true);

                return Ok(new { updatedCourse = response });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        ////// Sections //////


        /////// Sessions
        [Authorize(Roles = "Admin")]
        [HttpPost("edit-session")]
        public IActionResult CreateSession([FromBody] Session session, [FromQuery] string action)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();

            try
            {
                var section = _sectionRepository.FindById(session.Section.Id);

                if (action == "add")
                {
                    var newSession = new Session()
                    {
                        Section = section,
                        Title_EN = session.Title_EN,
                        Slug_EN = new SlugHelper().GenerateSlug(session.Title_EN),
                        Order = session.Order,
                        Duration = session.Duration,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        CreatedBy = session.CreatedBy,
                        UpdatedBy = session.UpdatedBy
                    };

                    _sessionRepository.Create(newSession);
                }
                else if (action == "edit")
                {
                    var s = _sessionRepository.FindById(session.Id);
                    s.Title_EN = session.Title_EN;
                    s.Slug_EN = new SlugHelper().GenerateSlug(session.Title_EN);
                    s.Duration = session.Duration;
                    s.UpdatedAt = DateTime.Now;
                    s.UpdatedBy = session.UpdatedBy;

                    if (s.Order != session.Order)
                    {
                        var oldOrder = s.Order;

                        // Previous
                        var oldSession = section.Sessions.SingleOrDefault(s => s.Order == session.Order);
                        if (oldSession != null)
                        {
                            oldSession.Order = oldOrder;
                            var updatedOldSession = _sessionRepository.Update(oldSession);
                        }

                        // New
                        s.Order = session.Order;

                    }

                    _sessionRepository.Update(s);
                }
                var course = _courseRepository.FindById(session.Section.Course.Id);

                var response = ResponseGenerator.GenerateCourseResponse(course, true);

                return Ok(new { updatedCourse = response });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-session")]
        public IActionResult DeleteSession( [FromQuery] long id)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();

            try
            {
                var courseId = _sessionRepository.FindById(id).Section.Course.Id;
                var deletedSession = _sessionRepository.Delete(id);

                var course = _courseRepository.FindById(courseId);

                var response = ResponseGenerator.GenerateCourseResponse(course, true);

                return Ok(new { updatedCourse = response });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("edit-session-content")]
        public IActionResult EditSessionContent([FromBody] SessionContent sessionContent, [FromQuery] string action)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var session = _sessionRepository.FindById(sessionContent.Session.Id);

                if(action == "add")
                {
                    var newContent = new SessionContent()
                    {
                        Session = session,
                        Type = sessionContent.Type,
                        Content = sessionContent.Content,
                        Order = sessionContent.Order
                    };

                    
                }

                return Ok();
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }


        ////////////////////////////////////////////////////
        
       
       
    }
}