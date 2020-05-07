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

        [AllowAnonymous]
        [HttpPost("create-comment")]
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
                    Course = course,
                    CourseId = course.Id,
                    Text = comment.Text,
                    CommentDateTime = DateTime.Now
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
    }
}