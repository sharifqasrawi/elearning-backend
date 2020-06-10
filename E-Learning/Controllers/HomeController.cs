using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Learning.Helpers;
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
        private readonly ITranslator _translator;

        public HomeController(ICategoryRepository categoryRepository,
                              ICourseRepository courseRepository,
                              ISessionRepository sessionRepository,
                              ITranslator translator)
        {
            _categoryRepository = categoryRepository;
            _courseRepository = courseRepository;
            _sessionRepository = sessionRepository;
            _translator = translator;
        }

        [AllowAnonymous]
        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var categories = _categoryRepository.GetCategories()
                                                    .Where(c => c.DeletedAt == null
                                                             && c.Courses.Count > 0)
                                                    .ToList();

                return Ok(new { categories });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpGet("courses")]
        public IActionResult GetCourses([FromQuery] int? categoryId, [FromQuery] long? courseId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var courses = _courseRepository.GetCourses()
                                                .Where(c => c.DeletedAt == null
                                                            && (c.Category.Id == categoryId || categoryId == null)
                                                            && (c.Id == courseId || courseId == null)
                                                            && c.IsPublished == true)
                                                .OrderBy(c => c.Title_EN)
                                                .ThenBy(c => c.CreatedAt)
                                                .ToList();

                var response = new List<object>();

                foreach (var course in courses)
                {
                    var res = ResponseGenerator.GenerateCourseResponse(course, false);

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

        [AllowAnonymous]
        [HttpGet("latest-courses")]
        public IActionResult GetLatestCourses()
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var courses = _courseRepository.GetCourses()
                                                .Where(c => c.DeletedAt == null && c.IsPublished == true)
                                                .OrderByDescending(c => c.PublishedAt)
                                                .ThenBy(c => c.Title_EN)
                                                .ThenBy(c => c.Category.Title_EN)
                                                .Take(5)
                                                .ToList();

                var response = new List<object>();

                foreach (var course in courses)
                {
                    var res = ResponseGenerator.GenerateCourseResponse(course, false);

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

        [AllowAnonymous]
        [HttpGet("course-session")]
        public IActionResult GetSession([FromQuery] long sessionId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var session = _sessionRepository.FindById(sessionId);

                return Ok(new {session = ResponseGenerator.GenerateSessionResponse(session) });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

    }
}