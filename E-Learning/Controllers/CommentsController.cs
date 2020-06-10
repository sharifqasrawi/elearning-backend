using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Learning.Helpers;
using E_Learning.Hubs;
using E_Learning.Models;
using E_Learning.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace E_Learning.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<SignalHub> _hubContext;
        private readonly ITranslator _translator;

        public CommentsController(ICommentRepository commentRepository,
                                  ICourseRepository courseRepository,
                                  INotificationRepository notificationRepository,
                                  UserManager<ApplicationUser> userManager,
                                  IHubContext<SignalHub> hubContext,
                                  ITranslator translator)
        {
            _commentRepository = commentRepository;
            _courseRepository = courseRepository;
            _notificationRepository = notificationRepository;
            _userManager = userManager;
            _hubContext = hubContext;
            _translator = translator;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] Comment comment)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var course = _courseRepository.FindById(comment.CourseId);
                var user = await _userManager.FindByIdAsync(comment.UserId);

                var newComment = new Comment()
                {
                    User = user,
                    UserId = user.Id,
                    UserFullName = $"{user.FirstName} {user.LastName}",
                    UserGender = user.Gender,
                    Course = course,
                    CourseId = course.Id,
                    Text = comment.Text,
                    CommentId = comment.CommentId ?? null,
                    CommentDateTime = DateTime.Now,
                    Replies = new List<Comment>(),
                    Likes = new List<Like>()
                };

                var createdComment = _commentRepository.Create(newComment);

                if(createdComment != null)
                {
                    await _hubContext.Clients.All.SendAsync("SignalCommentReceived", ResponseGenerator.GenerateCommentResponse(createdComment));
                }

                if (createdComment.CommentId == null)
                {

                    var newNotification = new Notification()
                    {
                        Type = "COMMENT",
                        Text = $"{user.FirstName} {user.LastName} Commented on [ {course.Title_EN} ]",
                        DateTime = DateTime.Now,
                        IsSeen = false
                    };

                    var createdNotification = await _notificationRepository.Create(newNotification);
                }
                else
                {
                    var newNotification = new Notification()
                    {
                        Type = "COMMENT REPLY",
                        Text = $"{user.FirstName} {user.LastName} Replied to a comment on [ {course.Title_EN} ]",
                        DateTime = DateTime.Now,
                        IsSeen = false
                    };

                    var createdNotification = await _notificationRepository.Create(newNotification);
                }

                return Ok(new { createdComment = ResponseGenerator.GenerateCommentResponse(createdComment) });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetCommentsByCourseId([FromQuery] long courseId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
               
                var _comments = _commentRepository.GetCommentsByCourseId(courseId)
                        .OrderByDescending(c => c.CommentDateTime)
                        .ToList();

                var comments = new List<object>();
                foreach(var comment in _comments)
                {
                    comments.Add(ResponseGenerator.GenerateCommentResponse(comment));
                }

                return Ok(new { comments });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteComment([FromQuery] long id)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var comment = _commentRepository.FindById(id);
                if (comment.Replies.Count > 0)
                {
                    foreach (var reply in comment.Replies.ToList())
                    {
                        _commentRepository.Delete(reply.Id);
                    }
                }

                var deletedComment = _commentRepository.Delete(id);

                if (deletedComment != null)
                {
                    await _hubContext.Clients.All.SendAsync("SignalCommentDeletedReceived", ResponseGenerator.GenerateCommentResponse(deletedComment));
                }

                return Ok(new { deletedComment = ResponseGenerator.GenerateCommentResponse(deletedComment) });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateComment([FromBody] Comment comment)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var commentToUpdate = _commentRepository.FindById(comment.Id);
                commentToUpdate.Text = comment.Text;
                var updatedComment = _commentRepository.Update(commentToUpdate);

                if (updatedComment != null)
                {
                    await _hubContext.Clients.All.SendAsync("SignalCommentUpdatedReceived", ResponseGenerator.GenerateCommentResponse(updatedComment));
                }

                return Ok(new { updatedComment = ResponseGenerator.GenerateCommentResponse(updatedComment)  });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize]
        [HttpGet("count")]
        public IActionResult CommentsCount()
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var count = _commentRepository.GetComments().Count;
           

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