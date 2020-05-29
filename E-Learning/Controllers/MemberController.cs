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
        private readonly IUserQuizRepository _userQuizRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public MemberController(ICourseRepository courseRepository,
                                IClassRepository classRepository,
                                IUserQuizRepository userQuizRepository,
                                 UserManager<ApplicationUser>  userManager)
        {
            _courseRepository = courseRepository;
            _classRepository = classRepository;
            _userManager = userManager;
            _userQuizRepository = userQuizRepository;
        }

        [AllowAnonymous]
        [HttpGet("courses")]
        public async Task<IActionResult> GetMemberCourses([FromQuery] string userId)
        {
            var errorMessages = new List<string>();
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

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
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpGet("get-user-quizzes")]
        public IActionResult GetUserQuiz([FromQuery] string userId)
        {
            var errorMessages = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    errorMessages.Add("Error fetching data");
                    return BadRequest(new { errors = errorMessages });
                }

                var userQuizzes = _userQuizRepository.GetUserQuizzesByUserId(userId)
                                                        .OrderByDescending(x => x.TakeDateTime)
                                                       .Select(x => new
                                                       {
                                                           id = x.Id,
                                                           quizId = x.QuizId,
                                                           quizTitle = x.Quiz.title_EN,
                                                           takeDateTime = x.TakeDateTime.Value,
                                                           userId = x.UserId,
                                                           isStarted = x.IsStarted.Value,
                                                           isOngoing = x.IsOngoing.Value,
                                                           isSubmitted = x.IsSubmitted.Value,
                                                           result = x.Result
                                                       }).ToList();


                return Ok(new { userQuizzes });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }
    }
}