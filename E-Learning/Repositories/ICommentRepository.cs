using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface ICommentRepository
    {
        Comment Create(Comment comment);
        Comment Update(Comment commentChanges);
        Comment Delete(long id);
        Comment FindById(long id);
        IList<Comment> GetComments();
        IList<Comment> GetCommentsByCourseId(long courseId);
    }
}
