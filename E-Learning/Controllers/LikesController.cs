using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class LikesController : ControllerBase
    {
        private readonly ILikeRepository _likeRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public LikesController(ILikeRepository likeRepository,
                               ICourseRepository courseRepository,
                               UserManager<ApplicationUser> userManager)
        {
            _likeRepository = likeRepository;
            _courseRepository = courseRepository;
            _userManager = userManager;
        }

        [HttpPost("like-course")]
        public async Task<IActionResult> LikeCourse([FromBody] Like like, [FromQuery] string action)
        {
            var errorMessages = new List<string>();
            try
            {
                var course = _courseRepository.FindById(like.CourseId);
                var user = await _userManager.FindByIdAsync(like.UserId);

                if (action == "like")
                {
                    var newLike = new Like()
                    {
                        Course = course,
                        CourseId = like.CourseId,
                        User = user,
                        UserId = user.Id,
                        UserFullName = $"{user.FirstName} {user.LastName}",
                        LikeDateTime = DateTime.Now
                    };

                    var createdLike = _likeRepository.Create(newLike);
                }
                else if (action == "unlike")
                {
                    var deletedLike = _likeRepository.Delete(like.CourseId, like.UserId);
                }

                return Ok(new { course });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }


    }
}