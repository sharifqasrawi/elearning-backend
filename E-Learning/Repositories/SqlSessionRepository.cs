
using E_Learning.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public class SqlSessionRepository : ISessionRepository
    {
        private readonly ApplicationDBContext dBContext;

        public SqlSessionRepository(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public Session Create(Session session)
        {
            dBContext.Sessions.Add(session);
            dBContext.SaveChanges();

            return session;
        }

        public Session Delete(long id)
        {
            var session = dBContext.Sessions.Find(id);
            if (session != null)
            {
                dBContext.Sessions.Remove(session);
                dBContext.SaveChanges();
            }

            return session;
        }

        public Session FindById(long id)
        {
            var session = dBContext.Sessions
                                   .Include("Section")
                                   .Include("Contents")
                                   .SingleOrDefault(s => s.Id == id);

            return session;
        }

        public IEnumerable<Session> GetSessions()
        {
            var sessions = dBContext.Sessions
                                   .Include("Section")
                                   .Include("Contents");

            return sessions;
        }

        public Session Update(Session sessionChanges)
        {
            var session = dBContext.Attach(sessionChanges);
            session.State = EntityState.Modified;
            dBContext.SaveChanges();
            return sessionChanges;
        }
    }
}
