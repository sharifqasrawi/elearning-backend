using E_Learning.Helpers;
using E_Learning.Models;
using E_Learning.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace E_Learning.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Route("[controller]")]
    [ApiController]
    public class QuizzesAdminController : ControllerBase
    {
        private readonly IQuizRepository _quizRepository;
        private readonly ITranslator _translator;

        public QuizzesAdminController(IQuizRepository quizRepository, ITranslator translator)
        {
            _quizRepository = quizRepository;
            _translator = translator;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create-quiz")]
        public IActionResult CreateQuiz([FromBody] Quiz quiz)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var newQuiz = new Quiz()
                {
                    title_EN = quiz.title_EN,
                    title_FR = quiz.title_FR ?? quiz.title_EN,
                    Slug_EN = new Slugify.SlugHelper().GenerateSlug(quiz.title_EN),
                    Slug_FR = quiz.title_FR != null ? new Slugify.SlugHelper().GenerateSlug(quiz.title_FR) : new Slugify.SlugHelper().GenerateSlug(quiz.title_EN),
                    Description_EN = quiz.Description_EN,
                    Description_FR = quiz.Description_FR ?? quiz.Description_EN,
                    ImagePath = quiz.ImagePath,
                    Languages = quiz.Languages,
                    Duration = quiz.Duration,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = quiz.CreatedBy,
                    UpdatedBy = quiz.CreatedBy,
                    DeletedAt = null,
                    DeletedBy = null,
                    IsPublished = false,
                    PublishDateTime = null,
                    Questions = new List<Question>()
                };

                var createdQuiz = _quizRepository.CreateQuiz(newQuiz);

                return Ok(new { createdQuiz });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update-quiz")]
        public IActionResult UpdateQuiz([FromBody] Quiz quiz)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var qz = _quizRepository.FindQuizById(quiz.Id);
                if (qz == null)
                {
                    return NotFound();
                }

                qz.title_EN = quiz.title_EN;
                qz.title_FR = quiz.title_FR ?? quiz.title_EN;
                qz.Slug_EN = new Slugify.SlugHelper().GenerateSlug(quiz.title_EN);
                qz.Slug_FR = quiz.title_FR != null ? new Slugify.SlugHelper().GenerateSlug(quiz.title_FR) : new Slugify.SlugHelper().GenerateSlug(quiz.title_EN);
                qz.Description_EN = quiz.Description_EN;
                qz.Description_FR = quiz.Description_FR ?? quiz.Description_EN;
                qz.ImagePath = quiz.ImagePath;
                qz.Languages = quiz.Languages;
                qz.Duration = quiz.Duration;
                qz.UpdatedAt = DateTime.Now;
                qz.UpdatedBy = quiz.UpdatedBy;

                var updatedQuiz = _quizRepository.UpdateQuiz(qz);

                return Ok(new { updatedQuiz });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("trash-restore-quiz")]
        public IActionResult TrashRestoreQuiz([FromBody] Quiz quiz, [FromQuery] string action)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var qz = _quizRepository.FindQuizById(quiz.Id);
                if (qz == null)
                {
                    return NotFound();
                }

                if (action == "trash")
                {
                    qz.DeletedAt = DateTime.Now;
                    qz.DeletedBy = quiz.DeletedBy;
                }
                else if (action == "restore")
                {
                    qz.DeletedAt = null;
                    qz.DeletedBy = null;
                }
                var updatedQuiz = _quizRepository.UpdateQuiz(qz);

                return Ok(new { updatedQuiz });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-quiz")]
        public IActionResult DeleteQuiz([FromQuery] long quizId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var qz = _quizRepository.FindQuizById(quizId);
                if (qz == null)
                {
                    return NotFound();
                }

                //foreach(var question in qz.Questions)
                //{
                //    foreach(var answer in question.Answers)
                //    {
                //        _quizRepository.DeleteAnswer(answer.Id);
                //    }
                //    _quizRepository.DeleteQuestion(question.Id);
                //}

                var deletedQuiz = _quizRepository.DeleteQuiz(quizId);

                return Ok(new { deletedQuizId = deletedQuiz.Id });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new
                {
                    errors = errorMessages
                });
            }
        }

       [Authorize(Roles = "Admin")]
        [HttpPut("publish-quiz")]
        public IActionResult PublishQuiz([FromBody] Quiz quiz, [FromQuery] string action)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var qz = _quizRepository.FindQuizById(quiz.Id);
                if (qz == null)
                {
                    return NotFound();
                }

                if (action == "publish")
                {
                    qz.IsPublished = true;
                    qz.PublishDateTime = DateTime.Now;
                }
                else if (action == "unpublish")
                {
                    qz.IsPublished = false;
                    qz.PublishDateTime = null;
                }
                var updatedQuiz = _quizRepository.UpdateQuiz(qz);

                return Ok(new { updatedQuiz });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetQuizzes()
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var quizzes = _quizRepository.GetQuizzesOnly()
                                             .Where(q => q.DeletedAt == null)
                                             .ToList();

                return Ok(new { quizzes });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("trashed")]
        public IActionResult GetTrashedQuizzes()
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var quizzes = _quizRepository.GetQuizzes()
                                             .Where(q => q.DeletedAt != null)
                                             .ToList();

                return Ok(new { quizzes });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }



        /////////////////////////////////

        [Authorize(Roles = "Admin")]
        [HttpGet("questions")]
        public IActionResult GetQuestions([FromQuery] long? quizId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                if (quizId == null)
                {
                    return NotFound();
                }

                var questions = _quizRepository.GetQuestionsOnly(quizId.Value);

                if (questions == null)
                {
                    return NotFound();
                }

                return Ok(new { questions });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create-question")]
        public IActionResult CreateQuestion([FromBody] Question question)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var quiz = _quizRepository.FindQuizById(question.QuizId);
                if (quiz == null)
                {
                    return NotFound();
                }

                var newQuestion = new Question()
                {
                    Quiz = quiz,
                    QuizId = quiz.Id,
                    Text_EN = question.Text_EN,
                    Slug_EN = new Slugify.SlugHelper().GenerateSlug(question.Text_EN),
                    Text_FR = question.Text_FR ?? question.Text_EN,
                    Slug_FR = question.Text_FR != null ? new Slugify.SlugHelper().GenerateSlug(question.Text_FR) : new Slugify.SlugHelper().GenerateSlug(question.Text_EN),
                    ImagePath = question.ImagePath,
                    Duration = question.Duration,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = question.CreatedBy,
                    UpdatedBy = question.CreatedBy,
                    DeletedAt = null,
                    DeletedBy = null,
                    Answers = new List<Answer>()
                };

                var createdQuestion = _quizRepository.CreateQuestion(newQuestion);

                return Ok(new { createdQuestion });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("update-question")]
        public IActionResult UpdateQuestion([FromBody] Question question)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var q = _quizRepository.FindQuestionById(question.Id);
                if (q == null)
                {
                    return NotFound();
                }

                //var quiz = _quizRepository.FindQuizById(question.QuizId);
                //if(quiz == null)
                //{
                //    errorMessages.Add("Quiz not found.");
                //    return BadRequest(new { errors = errorMessages });
                //}

                //    q.Quiz = quiz;
                //   q.QuizId = quiz.Id;
                q.Text_EN = question.Text_EN;
                q.Text_FR = question.Text_FR ?? question.Text_EN;
                q.Slug_EN = new Slugify.SlugHelper().GenerateSlug(question.Text_EN);
                q.Slug_FR = question.Text_FR != null ? new Slugify.SlugHelper().GenerateSlug(question.Text_FR) : new Slugify.SlugHelper().GenerateSlug(question.Text_EN);
                q.ImagePath = question.ImagePath;
                q.Duration = question.Duration;
                q.UpdatedAt = DateTime.Now;
                q.UpdatedBy = question.UpdatedBy;


                var updatedQuestion = _quizRepository.UpdateQuestion(q);

                return Ok(new { updatedQuestion });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("trash-restore-question")]
        public IActionResult TrashRestoreQuestion([FromBody] Question question, [FromQuery] string action)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var q = _quizRepository.FindQuestionById(question.Id);
                if (q == null)
                {
                    return NotFound();
                }

                if (action == "trash")
                {
                    q.DeletedAt = DateTime.Now;
                    q.DeletedBy = question.DeletedBy;
                }
                else if (action == "restore")
                {
                    q.DeletedAt = null;
                    q.DeletedBy = null;
                }

                var updatedQuestion = _quizRepository.UpdateQuestion(q);

                return Ok(new { updatedQuestion });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-question")]
        public IActionResult DeleteQuestion([FromQuery] long questionId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var question = _quizRepository.FindQuestionById(questionId);
                if (question == null)
                {
                    return NotFound();
                }

                var deletedQuestion = _quizRepository.DeleteQuestion(question.Id);

                return Ok(new { deletedQuestionId = deletedQuestion.Id });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        /////////////////////////////////


        [Authorize(Roles = "Admin")]
        [HttpGet("answers")]
        public IActionResult GetAnswers([FromQuery] long? questionId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                if (questionId == null)
                {
                    return NotFound();
                }

                var answers = _quizRepository.GetAnswers(questionId.Value);

                if (answers == null)
                {
                    return NotFound();
                }

                return Ok(new { answers });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create-answer")]
        public IActionResult CreateAnswer([FromBody] Answer answer)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var question = _quizRepository.FindQuestionById(answer.QuestionId);

                if (question == null)
                {
                    return NotFound();
                }

                var newAnswer = new Answer()
                {
                    Question = question,
                    QuestionId = question.Id,
                    Text_EN = answer.Text_EN,
                    Text_FR = answer.Text_FR ?? answer.Text_EN,
                    ImagePath = answer.ImagePath,
                    IsCorrect = answer.IsCorrect,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = answer.CreatedBy,
                    UpdatedBy = answer.CreatedBy,
                    DeletedAt = null,
                    DeletedBy = null
                };

                var createdAnswer = _quizRepository.CreateAnswer(newAnswer);

                return Ok(new { createdAnswer });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update-answer")]
        public IActionResult UpdateAnswer([FromBody] Answer answer)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {

                var a = _quizRepository.FindAnswerById(answer.Id);

                if (a == null)
                {
                    return NotFound();
                }

                var question = _quizRepository.FindQuestionById(answer.QuestionId);

                if (question == null)
                {
                    return NotFound();
                }


                a.Question = question;
                a.QuestionId = question.Id;
                a.Text_EN = answer.Text_EN;
                a.Text_FR = answer.Text_FR ?? answer.Text_EN;
                a.ImagePath = answer.ImagePath;
                a.IsCorrect = answer.IsCorrect;
                a.UpdatedAt = DateTime.Now;
                a.UpdatedBy = answer.UpdatedBy;


                var updatedAnswer = _quizRepository.UpdateAnswer(a);

                return Ok(new { updatedAnswer });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("trash-restore-answer")]
        public IActionResult TrashRestoreAnswer([FromBody] Answer answer, [FromQuery] string action)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {

                var a = _quizRepository.FindAnswerById(answer.Id);

                if (a == null)
                {
                    return NotFound();
                }

                if (action == "trash")
                {
                    a.DeletedAt = DateTime.Now;
                    a.DeletedBy = answer.DeletedBy;
                }
                else if (action == "restore")
                {
                    a.DeletedAt = null;
                    a.DeletedBy = null;
                }


                var updatedAnswer = _quizRepository.UpdateAnswer(a);

                return Ok(new { updatedAnswer });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-answer")]
        public IActionResult DeleteAnswer([FromQuery] long answerId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var answer = _quizRepository.FindAnswerById(answerId);
                if (answer == null)
                {
                    return NotFound();
                }

                var deletedAnswer = _quizRepository.DeleteAnswer(answer.Id);

                return Ok(new { deletedAnswerId = deletedAnswer.Id });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }
    }
}