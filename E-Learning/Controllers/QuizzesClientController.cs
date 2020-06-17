using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Learning.Helpers;
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
        private readonly ITranslator _translator;

        public QuizzesClientController(IQuizRepository quizRepository, ITranslator translator)
        {
            _quizRepository = quizRepository;
            _translator = translator;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetQuizzes()
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var quizzes = _quizRepository.GetQuizzesOnly()
                                             .Where(q => q.DeletedAt == null && q.IsPublished == true)
                                             .OrderBy(q => q.title_EN)
                                             .ToList();


                return Ok(new { quizzes });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin, Author, User")]
        [HttpGet("questions")]
        public IActionResult GetQuizQuestions([FromQuery] long? quizId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                if(quizId == null)
                {
                    return NotFound();
                }

                var allQuestions = _quizRepository.GetQuestions(quizId.Value)
                     .Where(x => x.DeletedAt == null)
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
                            answer.Text_FR,
                            answer.ImagePath
                        });
                    }

                    questions.Add(new
                    {
                        question.Id,
                        question.Slug_EN,
                        question.Slug_FR,
                        question.QuizId,
                        question.Text_EN,
                        question.Text_FR,
                        question.ImagePath,
                        question.Duration,
                        answers
                    });
                }

                return Ok(new { questions  });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }
    }
}