using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Learning.Helpers;
using E_Learning.Hubs;
using E_Learning.Models;
using E_Learning.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

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
        private readonly IHubContext<SignalHub> _hubContext;
        private readonly ITranslator _translator;

        public LikesController(ILikeRepository likeRepository,
                               ICourseRepository courseRepository,
                               ICommentRepository commentRepository,
                               INotificationRepository notificationRepository,
                               UserManager<ApplicationUser> userManager,
                               IHubContext<SignalHub> hubContext,
                               ITranslator translator)
        {
            _likeRepository = likeRepository;
            _courseRepository = courseRepository;
            _commentRepository = commentRepository;
            _notificationRepository = notificationRepository;
            _userManager = userManager;
            _hubContext = hubContext;
            _translator = translator;
        }

        [Authorize]
        [HttpPost("like-course")]
        public async Task<IActionResult> LikeCourse([FromBody] Like like, [FromQuery] string action)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var course = _courseRepository.FindById(like.CourseId.Value);
                var user = await _userManager.FindByIdAsync(like.UserId);
                Like updatedLike = null;
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

                    updatedLike = _likeRepository.Create(newLike);

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
                    updatedLike = _likeRepository.Delete(like.CourseId.Value, like.UserId, "course");

                    var newNotification = new Notification()
                    {
                        Type = "LIKE",
                        Text = $"{user.FirstName} {user.LastName} Unliked course [ {course.Title_EN} ]",
                        DateTime = DateTime.Now,
                        IsSeen = false
                    };

                    var createdNotification = await _notificationRepository.Create(newNotification);
                }

                //return Ok(new { course = ResponseGenerator.GenerateCourseResponse(course,false) });
                return Ok(new { like = updatedLike , action }) ;
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize]
        [HttpPost("like-comment")]
        public async Task<IActionResult> LikeComment([FromBody] Like like, [FromQuery] string action)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var comment = _commentRepository.FindById(like.CommentId.Value);
                var user = await _userManager.FindByIdAsync(like.UserId);
                Like updatedLike = null;

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

                    updatedLike = _likeRepository.Create(newLike);

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
                    updatedLike = _likeRepository.Delete(like.CommentId.Value, like.UserId, "comment");

                    var newNotification = new Notification()
                    {
                        Type = "LIKE",
                        Text = $"{user.FirstName} {user.LastName} Unliked [ {comment.UserFullName}'s comment ]",
                        DateTime = DateTime.Now,
                        IsSeen = false
                    };

                    var createdNotification = await _notificationRepository.Create(newNotification);
                }

         
                if (comment != null)
                {
                    var commentResponse = new Comment()
                    {
                        Id = comment.Id,
                        Text = comment.Text,
                        CommentId = comment.CommentId,
                        UserId = comment.UserId,
                        UserFullName = comment.UserFullName,
                        UserGender = comment.UserGender,
                        CourseId = comment.CourseId,
                        CommentDateTime = comment.CommentDateTime,
                        Replies = comment.Replies,
                        Likes = comment.Likes
                    };

                    await _hubContext.Clients.All.SendAsync("SignalCommentLikeReceived", ResponseGenerator.GenerateCommentResponse(commentResponse));
                }

                return Ok(new { comment = ResponseGenerator.GenerateCommentResponse(comment) });

            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpGet("count")]
        public IActionResult LikesCount()
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var count = _likeRepository.GetLikes().Count;


                return Ok(new { count });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }


       
    }
}