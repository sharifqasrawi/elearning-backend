using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface IUserQuizRepository
    {
        UserQuiz Create(UserQuiz userQuiz);
        UserQuiz Update(UserQuiz userQuizChanges);
        UserQuiz FindById(long id);
        UserQuiz FindByUserIdAndQuizId(string userId, long quizId);
        UserQuiz FindLastByUserIdAndQuizId(string userId, long quizId);
        IList<UserQuiz> GetUserQuizzes();
        IList<UserQuiz> GetUserQuizzesByUserId(string userId);
        IList<UserQuiz> GetUserQuizzesByQuizId(long quizId);
        IList<UserQuiz> GetUserQuizzesByQuizIdAndUserId(long quizId, string userId);


        UserQuizAnswer CreateAnswer(UserQuizAnswer userQuizAnswer);
        UserQuizAnswer UpdateAnswer(UserQuizAnswer userQuizAnswerChanges);
        UserQuizAnswer Find(long userQuizId, long questionId);
        IList<UserQuizAnswer> GetUserQuizAnswers(long id);

    }
}
