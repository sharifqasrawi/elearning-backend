using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface ILikeRepository
    {
        Like Create(Like like);
        Like Update(Like likeChanges);
        Like Delete(long typeId, string userId, string type);
        Like FindById(long id);
        IList<Like> GetLikesByCourseId(long courseId);
        IList<Like> GetLikes();
    }
}
