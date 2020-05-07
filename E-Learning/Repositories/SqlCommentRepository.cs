using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public class SqlCommentRepository : ICommentRepository
    {
        private readonly ApplicationDBContext dBContext;
        public SqlCommentRepository(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public Comment Create(Comment comment)
        {
            dBContext.Comments.Add(comment);
            dBContext.SaveChanges();
            return comment;
        }

        public Comment Delete(long id)
        {
            var comment = dBContext.Comments.Find(id);
            if(comment != null)
            {
                dBContext.Comments.Remove(comment);
                dBContext.SaveChanges();
            }
            return comment;
        }

        public Comment FindById(long id)
        {
            return dBContext.Comments.Find(id);
        }

        public IList<Comment> GetComments()
        {
            return dBContext.Comments.OrderBy(c => c.CommentDateTime).ToList();
        }

        public IList<Comment> GetCommentsByCourseId(long courseId)
        {
            return dBContext.Comments
                .Where(c => c.CourseId == courseId)
                .OrderBy(c => c.CommentDateTime).ToList();
        }

        public Comment Update(Comment commentChanges)
        {
            var comment = dBContext.Comments.Attach(commentChanges);
            comment.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            dBContext.SaveChanges();
            return commentChanges;

        }
    }
}
