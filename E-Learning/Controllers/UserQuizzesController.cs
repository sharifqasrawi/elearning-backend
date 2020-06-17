using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Learning.Helpers;
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
    public class UserQuizzesController : ControllerBase
    {
        private readonly IUserQuizRepository _userQuizRepository;
        private readonly IQuizRepository _quizRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITranslator _translator;

        public UserQuizzesController(IUserQuizRepository userQuizRepository,
                                     IQuizRepository quizRepository,
                                     UserManager<ApplicationUser> userManager,
                                     ITranslator translator)
        {
            _userQuizRepository = userQuizRepository;
            _quizRepository = quizRepository;
            _userManager = userManager;
            _translator = translator;
        }

        [Authorize(Roles = "Admin, Author, User")]
        [HttpPost("start-quiz")]
        public async Task<IActionResult> StartQuiz([FromBody] UserQuiz userQuiz)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var user = await _userManager.FindByIdAsync(userQuiz.UserId);
                var quiz = _quizRepository.FindQuizById(userQuiz.QuizId);

                if (user == null || quiz == null)
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }


                var newUserQuiz = new UserQuiz()
                {
                    User = user,
                    UserId = user.Id,
                    Quiz = quiz,
                    QuizId = quiz.Id,
                    IsStarted = true,
                    IsOngoing = true,
                    IsSubmitted = false,
                    TakeDateTime = DateTime.Now,
                };

                var lastUserQuiz = _userQuizRepository.FindLastByUserIdAndQuizId(userQuiz.UserId, userQuiz.QuizId);
                if (lastUserQuiz != null && lastUserQuiz.IsOngoing.Value == true)
                {
                    lastUserQuiz.IsOngoing = false;
                    var updatedLastUserQuiz = _userQuizRepository.Update(lastUserQuiz);
                }

                var startedQuiz = _userQuizRepository.Create(newUserQuiz);


                return Ok(new { startedQuiz });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin, Author, User")]
        [HttpPost("choose-answer")]
        public IActionResult ChooseAnswer([FromBody] UserQuizAnswer userQuizAnswer)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var userQuiz =  _userQuizRepository.FindById(userQuizAnswer.UserQuizId);
                var question = _quizRepository.FindQuestionById(userQuizAnswer.QuestionId);
                var answer = _quizRepository.FindAnswerById(userQuizAnswer.AnswerId);

                if (userQuiz == null || question == null || answer == null)
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }

                var curUserQuizAnswer = _userQuizRepository.Find(userQuiz.Id, question.Id);

                UserQuizAnswer createdUserQuizAnswer = null;

                if (curUserQuizAnswer == null)
                {
                    var newUserQuizAnswer = new UserQuizAnswer()
                    {
                        UserQuiz = userQuiz,
                        UserQuizId = userQuiz.Id,
                        Question = question,
                        QuestionId = question.Id,
                        Answer = answer,
                        AnswerId = answer.Id,
                        ChooseDateTime = DateTime.Now
                    };

                     createdUserQuizAnswer = _userQuizRepository.CreateAnswer(newUserQuizAnswer);
                }else
                {
                    curUserQuizAnswer.Answer = answer;
                    curUserQuizAnswer.AnswerId = answer.Id;
                    curUserQuizAnswer.ChooseDateTime = DateTime.Now;

                    createdUserQuizAnswer = _userQuizRepository.UpdateAnswer(curUserQuizAnswer);
                }

                return Ok(new { createdUserQuizAnswer });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin, Author, User")]
        [HttpGet("get-user-quiz")]
        public IActionResult GetUserQuiz([FromQuery] string userId, [FromQuery] long? quizId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(userId) || quizId == null)
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }

                var userQuiz = _userQuizRepository.FindByUserIdAndQuizId(userId, quizId.Value);
                var userQuizAnswers = _userQuizRepository.GetUserQuizAnswers(userQuiz.Id);


                return Ok(new { userQuiz, userQuizAnswers });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin, Author, User")]
        [HttpPost("submit-quiz")]
        public IActionResult SubmitQuiz([FromBody] UserQuiz userQuiz)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var curUserQuiz = _userQuizRepository.FindByUserIdAndQuizId(userQuiz.UserId, userQuiz.QuizId);
                
                if (curUserQuiz == null)
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }

                var userQuizAnswers = _userQuizRepository.GetUserQuizAnswers(curUserQuiz.Id);

                var result = 0;
                var questionsCount = curUserQuiz.Quiz.Questions.Count;
                var questionMark = 100 / questionsCount;

                foreach(var userAnswer in userQuizAnswers)
                {
                    var correctAnswer = _quizRepository.FindQuestionById(userAnswer.QuestionId)
                                                         .Answers
                                                         .SingleOrDefault(a => a.IsCorrect.Value == true);

                    if(correctAnswer != null && userAnswer.AnswerId == correctAnswer.Id)
                    {
                        result += questionMark;
                    }
                }

                curUserQuiz.Result = result;
                curUserQuiz.IsOngoing = false;
                curUserQuiz.IsSubmitted = true;

                var updatedUserQuiz = _userQuizRepository.Update(curUserQuiz);

                return Ok(new { updatedUserQuiz, userQuizAnswers });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }
    }
}