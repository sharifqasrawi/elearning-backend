using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface ISessionRepository
    {
        Session Create(Session session);
        IList<Session> GetSessions();
        Session Update(Session sessionChanges);
        Session Delete(long id);
        Session FindById(long id);

        IList<Session> GetSessionsByCourseId(long courseId);
        IList<Session> GetSessionsBySectionId(long sectionId);

    }
}
