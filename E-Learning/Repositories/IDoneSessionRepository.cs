using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface IDoneSessionRepository
    {
        DoneSession Create(DoneSession doneSession);
        DoneSession Update(DoneSession doneSessionChanges);
        DoneSession Delete(long id);
        DoneSession Delete(long sessionId, string userId);

        bool DeleteAllByCourseUser(long courseId, string userId);
        DoneSession FindBySessionAndUser(long sessionId, string userId);
        DoneSession FindById(long id);

        IList<DoneSession> GetDoneSessions();
        IList<DoneSession> GetDoneSessionsByUserAndCourse(string userId, long courseId);

    }
}
