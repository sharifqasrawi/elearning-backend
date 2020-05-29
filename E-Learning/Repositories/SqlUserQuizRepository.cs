using E_Learning.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public class SqlUserQuizRepository : IUserQuizRepository
    {
        private readonly ApplicationDBContext dBContext;
        public SqlUserQuizRepository(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public UserQuiz Create(UserQuiz userQuiz)
        {
            dBContext.UserQuizzes.Add(userQuiz);
            dBContext.SaveChanges();
            return userQuiz;
        }

        public UserQuiz FindByUserIdAndQuizId(string userId, long quizId)
        {
            var lastUserQuiz = dBContext.UserQuizzes
                                    .Where(x => x.UserId == userId
                                           && x.QuizId == quizId)
                                      .OrderBy(x => x.TakeDateTime)
                                      .LastOrDefault();

            if(lastUserQuiz.IsSubmitted.Value)
            {
                var userQuiz = dBContext.UserQuizzes
                                     .Include("User")
                                     .Include("Quiz")
                                     .Include("Quiz.Questions")
                                     .Where(x => x.UserId == userId
                                           && x.QuizId == quizId
                                           && x.IsSubmitted.Value)
                                      .OrderBy(x => x.TakeDateTime)
                                      .LastOrDefault();

                return userQuiz;
            }
            else
            {
                var userQuiz = dBContext.UserQuizzes
                                     .Include("User")
                                     .Include("Quiz")
                                     .Include("Quiz.Questions")
                                     .SingleOrDefault(x => x.UserId == userId
                                                       && x.QuizId == quizId
                                                       && x.IsOngoing.Value);

                return userQuiz;
            }
           
        }

        public UserQuiz FindLastByUserIdAndQuizId(string userId, long quizId)
        {
            var userQuiz = dBContext.UserQuizzes
                                      .Include("User")
                                      .Include("Quiz")
                                      .Include("Quiz.Questions")
                                      .Where(x => x.UserId == userId
                                                        && x.QuizId == quizId)
                                      .OrderBy(x => x.TakeDateTime)
                                      .LastOrDefault();

            return userQuiz;
        }


        public IList<UserQuiz> GetUserQuizzes()
        {
            var userQuizzes = dBContext.UserQuizzes
                                       .Include("User")
                                       .Include("Quiz")
                                       .ToList();

            return userQuizzes;
        }

        public IList<UserQuiz> GetUserQuizzesByQuizId(long quizId)
        {
            var userQuizzes = dBContext.UserQuizzes
                                      .Include("User")
                                      .Include("Quiz")
                                      .Where(x => x.QuizId == quizId)
                                      .ToList();

            return userQuizzes;
        }

        public IList<UserQuiz> GetUserQuizzesByQuizIdAndUserId(long quizId, string userId)
        {
            var userQuizzes = dBContext.UserQuizzes
                                     .Include("User")
                                     .Include("Quiz")
                                     .Where(x => x.QuizId == quizId && x.UserId == userId)
                                     .ToList();

            return userQuizzes;
        }

        public IList<UserQuiz> GetUserQuizzesByUserId(string userId)
        {
            var userQuizzes = dBContext.UserQuizzes
                                    .Include("User")
                                    .Include("Quiz")
                                    .Where(x => x.UserId == userId)
                                    .ToList();

            return userQuizzes;
        }

        public UserQuiz Update(UserQuiz userQuizChanges)
        {
            var userQuiz = dBContext.UserQuizzes.Attach(userQuizChanges);
            userQuiz.State = EntityState.Modified;
            dBContext.SaveChanges();
            return userQuizChanges;
        }
        public UserQuiz FindById(long id)
        {
            var userQuiz = dBContext.UserQuizzes
                                        .Include("User")
                                        .Include("Quiz")
                                        .SingleOrDefault(x => x.Id == id);

            return userQuiz;
        }

        ///////////////////////

        public UserQuizAnswer CreateAnswer(UserQuizAnswer userQuizAnswer)
        {
            dBContext.UserQuizAnswers.Add(userQuizAnswer);
            dBContext.SaveChanges();
            return userQuizAnswer;
        }


        public UserQuizAnswer UpdateAnswer(UserQuizAnswer userQuizAnswerChanges)
        {
            var userQuizAnswer = dBContext.UserQuizAnswers.Attach(userQuizAnswerChanges);
            userQuizAnswer.State = EntityState.Modified;
            dBContext.SaveChanges();
            return userQuizAnswerChanges;
        }

        public UserQuizAnswer Find(long userQuizId, long questionId)
        {
            var userQuizAnswer = dBContext.UserQuizAnswers
                .SingleOrDefault(x => x.UserQuizId == userQuizId
                                    && x.QuestionId == questionId);

            return userQuizAnswer;
        }

        public IList<UserQuizAnswer> GetUserQuizAnswers(long id)
        {
            var userQuizAnswers = dBContext.UserQuizAnswers
                .Where(x => x.UserQuizId == id)
                .ToList();

            return userQuizAnswers;
        }
    }
}
