using E_Learning.Models;
using Microsoft.EntityFrameworkCore;
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
            return dBContext.Comments
                .Include("User")
                .Include("Course")
                .Include("Replies")
                .Include("Likes")
                .SingleOrDefault(c => c.Id == id);
        }

        public IList<Comment> GetComments()
        {
            return dBContext.Comments
                .Include("User")
                .Include("Course")
                .Include("Replies")
                .Include("Likes")
                .OrderBy(c => c.CommentDateTime).ToList();
        }

        public IList<Comment> GetCommentsByCourseId(long courseId)
        {
            return dBContext.Comments
               .Include("User")
                .Include("Course")
                .Include("Replies")
                .Include("Replies.Likes")
                .Include("Likes")
                .Where(c => c.CourseId == courseId && c.CommentId == null)
                .OrderBy(c => c.CommentDateTime).ToList();
        }

        public Comment Update(Comment commentChanges)
        {
            var comment = dBContext.Comments.Attach(commentChanges);
            comment.State = EntityState.Modified;
            dBContext.SaveChanges();
            return commentChanges;

        }
    }
}
