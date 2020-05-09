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
        private readonly ICommentRepository _commentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public LikesController(ILikeRepository likeRepository,
                               ICourseRepository courseRepository,
                               ICommentRepository commentRepository,
                               INotificationRepository notificationRepository,
                               UserManager<ApplicationUser> userManager)
        {
            _likeRepository = likeRepository;
            _courseRepository = courseRepository;
            _commentRepository = commentRepository;
            _notificationRepository = notificationRepository;
            _userManager = userManager;
        }

        [HttpPost("like-course")]
        public async Task<IActionResult> LikeCourse([FromBody] Like like, [FromQuery] string action)
        {
            var errorMessages = new List<string>();
            try
            {
                var course = _courseRepository.FindById(like.CourseId.Value);
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

                    var newNotification = new Notification()
                    {
                        Type = "LIKE",
                        Text = $"{user.FirstName} {user.LastName} Liked course [ {course.Title_EN} ]",
                        DateTime = DateTime.Now,
                        IsSeen = false
                    };

                    var createdNotification = await _notificationRepository.Create(newNotification);
                }
                else if (action == "unlike")
                {
                    var deletedLike = _likeRepository.Delete(like.CourseId.Value, like.UserId, "course");

                    var newNotification = new Notification()
                    {
                        Type = "LIKE",
                        Text = $"{user.FirstName} {user.LastName} Unliked course [ {course.Title_EN} ]",
                        DateTime = DateTime.Now,
                        IsSeen = false
                    };

                    var createdNotification = await _notificationRepository.Create(newNotification);
                }

                return Ok(new { course });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpPost("like-comment")]
        public async Task<IActionResult> LikeComment([FromBody] Like like, [FromQuery] string action)
        {
            var errorMessages = new List<string>();
            try
            {
                var comment = _commentRepository.FindById(like.CommentId.Value);
                var user = await _userManager.FindByIdAsync(like.UserId);

                if (action == "like")
                {
                    var newLike = new Like()
                    {
                        Comment = comment,
                        CommentId = like.CommentId,
                        User = user,
                        UserId = user.Id,
                        UserFullName = $"{user.FirstName} {user.LastName}",
                        LikeDateTime = DateTime.Now
                    };

                    var createdLike = _likeRepository.Create(newLike);

                    var newNotification = new Notification()
                    {
                        Type = "LIKE",
                        Text = $"{user.FirstName} {user.LastName} Liked [ {comment.UserFullName}'s comment ]",
                        DateTime = DateTime.Now,
                        IsSeen = false
                    };

                    var createdNotification = await _notificationRepository.Create(newNotification);
                }
                else if (action == "unlike")
                {
                    var deletedLike = _likeRepository.Delete(like.CommentId.Value, like.UserId, "comment");

                    var newNotification = new Notification()
                    {
                        Type = "LIKE",
                        Text = $"{user.FirstName} {user.LastName} Unliked [ {comment.UserFullName}'s comment ]",
                        DateTime = DateTime.Now,
                        IsSeen = false
                    };

                    var createdNotification = await _notificationRepository.Create(newNotification);
                }

                

                return Ok(new { comment });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpGet("count")]
        public IActionResult LikesCount()
        {
            var errorMessages = new List<string>();
            try
            {
                var count = _likeRepository.GetLikes().Count;


                return Ok(new { count });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }
    }
}