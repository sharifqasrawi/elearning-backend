using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface IQuizRepository
    {
        Quiz CreateQuiz(Quiz quiz);
        Quiz UpdateQuiz(Quiz quizChanges);
        Quiz DeleteQuiz(long id);
        Quiz FindQuizById(long id);
        IList<Quiz> GetQuizzes();
        IList<Quiz> GetQuizzesOnly();

        Question CreateQuestion(Question question);
        Question UpdateQuestion(Question questionChanges);
        Question DeleteQuestion(long id);
        Question FindQuestionById(long id);
        IList<Question> GetQuestions(long quizId);
        IList<Question> GetQuestionsOnly(long quizId);

        Answer CreateAnswer(Answer answer);
        Answer UpdateAnswer(Answer answerChanges);
        Answer DeleteAnswer(long id);
        Answer FindAnswerById(long id);
        IList<Answer> GetAnswers(long questionId);

    }
}
