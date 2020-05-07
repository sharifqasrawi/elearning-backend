using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Learning.Models;
using E_Learning.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Learning.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ISessionRepository _sessionRepository;

        public HomeController(ICategoryRepository categoryRepository,
                              ICourseRepository courseRepository,
                              ISessionRepository sessionRepository)
        {
            _categoryRepository = categoryRepository;
            _courseRepository = courseRepository;
            _sessionRepository = sessionRepository;
        }

        [AllowAnonymous]
        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            var errorMessages = new List<string>();
            try
            {
                var categories = _categoryRepository.GetCategories()
                                                    .Where(c => c.DeletedAt == null
                                                             && c.Courses.Count > 0)
                                                    .ToList();

                return Ok(new { categories });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpGet("courses")]
        public IActionResult GetCourses([FromQuery] int? categoryId, [FromQuery] long? courseId)
        {

            var errorMessages = new List<string>();
            try
            {
                var courses = _courseRepository.GetCourses()
                                                .Where(c => c.DeletedAt == null
                                                            && (c.Category.Id == categoryId || categoryId == null)
                                                            && (c.Id == courseId || courseId == null)
                                                            && c.IsPublished == true)
                                                .OrderBy(c => c.Title_EN)
                                                .ThenBy(c => c.Category)
                                                .ThenBy(c => c.CreatedAt)
                                                .ToList();

                var response = new List<object>();

                foreach (var course in courses)
                {
                    var res = GenerateCourseResponse(course);

                    response.Add(res);
                }

                return Ok(new { courses = response });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpGet("course-session")]
        public IActionResult GetSession([FromQuery] long sessionId)
        {
            var errorMessages = new List<string>();
            try
            {
                var session = _sessionRepository.FindById(sessionId);

                return Ok(new { session });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }




        ////////////////////////
        private object GenerateCourseResponse(Course course)
        {
            var tags = new List<Tag>();

            foreach (var courseTag in course.CourseTags)
            {
                tags.Add(new Tag() { Id = courseTag.TagId, Name = courseTag.Tag.Name });
            }

            var sections = new List<Section>();

            foreach (var courseSection in course.Sections)
            {
                var sessions = new List<Session>();

                foreach (var sectionSession in courseSection.Sessions)
                {
                    sessions.Add(new Session()
                    {
                        Id = sectionSession.Id,
                        Title_EN = sectionSession.Title_EN,
                        Slug_EN = sectionSession.Slug_EN,
                        Duration = sectionSession.Duration,
                        Order = sectionSession.Order,
                        CreatedAt = sectionSession.CreatedAt,
                        CreatedBy = sectionSession.CreatedBy,
                        UpdatedAt = sectionSession.UpdatedAt,
                        UpdatedBy = sectionSession.UpdatedBy
                    });
                }

                sections.Add(new Section()
                {
                    Id = courseSection.Id,
                    Name_EN = courseSection.Name_EN,
                    Slug_EN = courseSection.Slug_EN,
                    Order = courseSection.Order,
                    CreatedAt = courseSection.CreatedAt,
                    CreatedBy = courseSection.CreatedBy,
                    UpdatedAt = courseSection.UpdatedAt,
                    UpdatedBy = courseSection.UpdatedBy,
                    DeletedAt = courseSection.DeletedAt,
                    DeletedBy = courseSection.DeletedBy,
                    Sessions = sessions.OrderBy(s => s.Order).ToList()
                });
            }


            var response = new
            {
                course.Id,
                course.Title_EN,
                course.Slug_EN,
                course.Description_EN,
                course.Prerequisites_EN,
                course.Duration,
                course.ImagePath,
                course.IsFree,
                course.Price,
                course.IsPublished,
                course.Languages,
                course.Level,
                course.Category,
                course.CreatedAt,
                course.CreatedBy,
                course.DeletedAt,
                course.DeletedBy,
                course.PublishedAt,
                course.UpdatedAt,
                course.UpdatedBy,
                tags,
                sections = sections.OrderBy(s => s.Order),
                course.Likes,
                course.Comments
            };

            return response;
        }
    }
}