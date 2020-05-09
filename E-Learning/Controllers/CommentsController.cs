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
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentsController(ICommentRepository commentRepository,
                                  ICourseRepository courseRepository,
                                  UserManager<ApplicationUser> userManager)
        {
            _commentRepository = commentRepository;
            _courseRepository = courseRepository;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] Comment comment)
        {
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
                    Replies = new List<Comment>()
                };

                var createdComment = _commentRepository.Create(newComment);

                return Ok(new { createdComment });
            }
            catch(Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetCommentsByCourseId([FromQuery] long courseId)
        {
            var errorMessages = new List<string>();
            try
            {
               
                var comments = _commentRepository.GetCommentsByCourseId(courseId)
                        .OrderByDescending(c => c.CommentDateTime)
                        .ToList();

                return Ok(new { comments });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpDelete]
        public IActionResult DeleteComment([FromQuery] long id)
        {
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

                return Ok(new { deletedComment });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpPut]
        public IActionResult UpdateComment([FromBody] Comment comment)
        {
            var errorMessages = new List<string>();
            try
            {
                var commentToUpdate = _commentRepository.FindById(comment.Id);
                commentToUpdate.Text = comment.Text;
                var updatedComment = _commentRepository.Update(commentToUpdate);

                return Ok(new { updatedComment  });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }
    }
}