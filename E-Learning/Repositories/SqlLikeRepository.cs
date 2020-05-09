using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public class SqlLikeRepository : ILikeRepository
    {
        private readonly ApplicationDBContext dBContext;
        public SqlLikeRepository(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public Like Create(Like like)
        {
            dBContext.Likes.Add(like);
            dBContext.SaveChanges();
            return like;
        }

        public Like Delete(long typeId, string userId, string type)
        {
            Like like = null;

            if (type == "course")
            {
                like = dBContext.Likes.SingleOrDefault(l => l.CourseId == typeId && l.UserId == userId);
            }
            else if (type == "comment")
            {
                like = dBContext.Likes.SingleOrDefault(l => l.CommentId == typeId && l.UserId == userId);
            }

            if (like != null)
            {
                dBContext.Likes.Remove(like);
                dBContext.SaveChanges();
            }
            return like;

        }

        public Like FindById(long id)
        {
            return dBContext.Likes.Find(id);
        }

        public IList<Like> GetLikes()
        {
            return dBContext.Likes.ToList();
        }

        public IList<Like> GetLikesByCourseId(long courseId)
        {
            return dBContext.Likes.Where(l => l.Course.Id == courseId).ToList();
        }

        public Like Update(Like likeChanges)
        {
            var like = dBContext.Likes.Attach(likeChanges);
            like.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            dBContext.SaveChanges();
            return likeChanges;
        }
    }
}
