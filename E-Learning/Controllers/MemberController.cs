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
    public class MemberController : ControllerBase
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IClassRepository _classRepository;
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IUserQuizRepository _userQuizRepository;
        private readonly ISavedSessionRepository _savedSessionRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITranslator _translator;

        public MemberController(ICourseRepository courseRepository,
                                IClassRepository classRepository,
                                IUserQuizRepository userQuizRepository,
                                IFavoriteRepository favoriteRepository,
                                ISavedSessionRepository savedSessionRepository,
                                 UserManager<ApplicationUser>  userManager,
                                 ITranslator translator)
        {
            _courseRepository = courseRepository;
            _classRepository = classRepository;
            _userManager = userManager;
            _favoriteRepository = favoriteRepository;
            _savedSessionRepository = savedSessionRepository;
            _userQuizRepository = userQuizRepository;
            _translator = translator;
        }

        [Authorize]
        [HttpGet("courses")]
        public IActionResult GetMemberCourses([FromQuery] string userId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var courses = new List<object>();

                var allCourses = _courseRepository.GetCourses();

                foreach (var course in allCourses)
                {
                    if (course.Class != null)
                    {
                        foreach (var member in course.Class.ClassUsers)
                        {
                            if (member.UserId == userId)
                            {
                                courses.Add(ResponseGenerator.GenerateCourseResponse(course, false));
                            }
                        }
                    }
                }

                return Ok(new { courses });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize]
        [HttpGet("get-user-quizzes")]
        public IActionResult GetUserQuiz([FromQuery] string userId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }

                var userQuizzes = _userQuizRepository.GetUserQuizzesByUserId(userId)
                                                        .OrderByDescending(x => x.TakeDateTime)
                                                       .Select(x => new
                                                       {
                                                           id = x.Id,
                                                           quizId = x.QuizId,
                                                           quizTitle_EN = x.Quiz.title_EN,
                                                           quizTitle_FR = x.Quiz.title_FR,
                                                           takeDateTime = x.TakeDateTime.Value,
                                                           userId = x.UserId,
                                                           isStarted = x.IsStarted.Value,
                                                           isOngoing = x.IsOngoing.Value,
                                                           isSubmitted = x.IsSubmitted.Value,
                                                           result = x.Result
                                                       }).ToList();


                return Ok(new { userQuizzes });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize]
        [HttpGet("dashboard")]
        public IActionResult Dashboard([FromQuery] string userId)
        {

            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var coursesCount = 0;
                var favoritesCount = _favoriteRepository.GetFavoritesByUserId(userId).Count;
                var savedSessionsCount = _savedSessionRepository.GetSavedSessionsByUserId(userId).Count;
                var userQuizzesCount = _userQuizRepository.GetUserQuizzesByUserId(userId).Count;

                var courses = new List<object>();

                var allCourses = _courseRepository.GetCourses();

                foreach (var course in allCourses)
                {
                    if (course.Class != null)
                    {
                        foreach (var member in course.Class.ClassUsers)
                        {
                            if (member.UserId == userId)
                            {
                                coursesCount++;
                            }
                        }
                    }
                }


                return Ok(new { coursesCount, favoritesCount, savedSessionsCount, userQuizzesCount });
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
    }
}