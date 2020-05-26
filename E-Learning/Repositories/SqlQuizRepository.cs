using E_Learning.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public class SqlQuizRepository : IQuizRepository
    {
        private readonly ApplicationDBContext dBContext;
        public SqlQuizRepository(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public Quiz CreateQuiz(Quiz quiz)
        {
            dBContext.Quizzes.Add(quiz);
            dBContext.SaveChanges();
            return quiz;
        }
        public Quiz UpdateQuiz(Quiz quizChanges)
        {
            var quiz = dBContext.Quizzes.Attach(quizChanges);
            quiz.State = EntityState.Modified;
            dBContext.SaveChanges();
            return quizChanges;
        }

        public Quiz DeleteQuiz(long id)
        {
            var quiz = dBContext.Quizzes.Find(id);
            if(quiz != null)
            {
                
                dBContext.Quizzes.Remove(quiz);
                dBContext.SaveChanges();
                return quiz;
            }
            return null;
        }

        public Quiz FindQuizById(long id)
        {
            var quiz = dBContext.Quizzes
                                .Include("Questions")
                                .Include("Questions.Answers")
                                .SingleOrDefault(q => q.Id == id);

            return quiz;
        }

        public IList<Quiz> GetQuizzes()
        {
            var quizzes = dBContext.Quizzes
                                   .Include("Questions")
                                   .Include("Questions.Answers")
                                   .ToList();

            return quizzes;
        }


        //////////////////////////

        public Question CreateQuestion(Question question)
        {
            dBContext.Questions.Add(question);
            dBContext.SaveChanges();
            return question;
        }
        public Question UpdateQuestion(Question questionChanges)
        {
            var question = dBContext.Questions.Attach(questionChanges);
            question.State = EntityState.Modified;
            dBContext.SaveChanges();
            return questionChanges;
        }
        public Question DeleteQuestion(long id)
        {
            var question = dBContext.Questions.Find(id);
            if (question != null)
            {
                dBContext.Questions.Remove(question);
                dBContext.SaveChanges();
                return question;
            }
            return null;
        }

        public Question FindQuestionById(long id)
        {
            var question = dBContext.Questions
                                 .Include("Quiz")
                                 .Include("Answers")
                                .SingleOrDefault(q => q.Id == id);

            return question;
        }

        public IList<Question> GetQuestions(long quizId)
        {
            var questions = dBContext.Questions
                                 .Include("Quiz")
                                 .Include("Answers")
                                 .Where(q => q.QuizId == quizId)
                                 .ToList();

            return questions;
        }

        //////////////////////////

        public Answer CreateAnswer(Answer answer)
        {
            dBContext.Answers.Add(answer);
            dBContext.SaveChanges();
            return answer;
        }
        public Answer UpdateAnswer(Answer answerChanges)
        {
            var answer = dBContext.Answers.Attach(answerChanges);
            answer.State = EntityState.Modified;
            dBContext.SaveChanges();
            return answerChanges;
        }

        public Answer DeleteAnswer(long id)
        {
            var answer = dBContext.Answers.Find(id);
            if (answer != null)
            {
                dBContext.Answers.Remove(answer);
                dBContext.SaveChanges();
                return answer;
            }
            return null;
        }

        public Answer FindAnswerById(long id)
        {
            var answer = dBContext.Answers
                                 .Include("Question")
                                 .Include("Question.Answers")
                                .SingleOrDefault(a => a.Id == id);

            return answer;
        }

        public IList<Answer> GetAnswers(long questionId)
        {
            var answers = dBContext.Answers
                               .Include("Question")
                               .Where(a => a.QuestionId == questionId)
                               .ToList();

            return answers;
        }
       
    }
}
