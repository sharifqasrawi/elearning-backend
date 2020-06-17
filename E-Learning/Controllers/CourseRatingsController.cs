using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using E_Learning.Helpers;
using E_Learning.Hubs;
using E_Learning.Models;
using E_Learning.Repositories;

namespace E_Learning.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CourseRatingsController : ControllerBase
    {
        private readonly ICourseRatingRepository _courseRatingRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICourseRepository _courseRepository;
        private readonly IHubContext<SignalHub> _hubContext;
        private readonly ITranslator _translator;
        public CourseRatingsController(ICourseRatingRepository courseRatingRepository,
                                       ICourseRepository courseRepository,
                                       UserManager<ApplicationUser> userManager,
                                       IHubContext<SignalHub> hubContext,
                                       ITranslator translator)
        {
            _courseRatingRepository = courseRatingRepository;
            _courseRepository = courseRepository;
            _userManager = userManager;
            _hubContext = hubContext;
            _translator = translator;
        }

        [Authorize(Roles = "Admin, Author, User")]
        [HttpPost]
        public async Task<IActionResult> RateCourse([FromBody] CourseRating courseRating)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var user = await _userManager.FindByIdAsync(courseRating.UserId);
                var course = _courseRepository.FindById(courseRating.CourseId);

                if(user == null || course == null)
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }


                var allowedValues = new List<float>() { 1, 2, 3, 4, 5 };

                if (!allowedValues.Contains(courseRating.Value))
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }

                var rating = _courseRatingRepository.FindByUserCourse(user.Id, course.Id);
                CourseRating createdCourseRating;
                
                if (rating == null)
                {

                    var newRating = new CourseRating()
                    {
                        Course = course,
                        CourseId = course.Id,
                        User = user,
                        UserId = user.Id,
                        Value = courseRating.Value,
                        RateDateTime = DateTime.Now
                    };

                    createdCourseRating = _courseRatingRepository.Create(newRating);
                    
                }
                else
                {
                    rating.OldValue = rating.Value;
                    rating.Value = courseRating.Value;
                    rating.RateDateTimeUpdated = DateTime.Now;

                    createdCourseRating = _courseRatingRepository.Update(rating);
                }

                if (createdCourseRating == null)
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }

                //await _hubContext.Clients.All.SendAsync("SignalCourseRateReceived", ResponseGenerator.GenerateCourseResponse(course));

                return Ok(new { course = ResponseGenerator.GenerateCourseResponse(course,false) });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }
    }
}