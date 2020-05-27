using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Learning.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Learning.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class QuizzesClientController : ControllerBase
    {
        private readonly IQuizRepository _quizRepository;

        public QuizzesClientController(IQuizRepository quizRepository)
        {
            _quizRepository = quizRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetQuizzes()
        {
            var errorMessages = new List<string>();
            try
            {
                var quizzes = _quizRepository.GetQuizzesOnly()
                                             .Where(q => q.DeletedAt == null && q.IsPublished == true)
                                             .OrderBy(q => q.title_EN)
                                             .ToList();


                return Ok(new { quizzes });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpGet("questions")]
        public IActionResult GetQuizQuestions([FromQuery] long? quizId)
        {
            var errorMessages = new List<string>();
            try
            {
                if(quizId == null)
                {
                    return NotFound();
                }

                var allQuestions = _quizRepository.GetQuestions(quizId.Value)
                    .OrderBy(r => Guid.NewGuid()).Take(10);

                if (allQuestions == null)
                {
                    return NotFound();
                }

                var questions = new List<object>();
                foreach(var question in allQuestions)
                {
                    var answers = new List<object>();

                    foreach(var answer in question.Answers)
                    {
                        answers.Add(new
                        {
                            answer.Id,
                            answer.QuestionId,
                            answer.Text_EN,
                            answer.ImagePath
                        });
                    }

                    questions.Add(new
                    {
                        question.Id,
                        question.Slug_EN,
                        question.QuizId,
                        question.Text_EN,
                        question.ImagePath,
                        question.Duration,
                        answers
                    });
                }

                return Ok(new { questions  });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }
    }
}