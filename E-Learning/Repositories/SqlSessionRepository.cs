using E_Learning.Models;
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
            throw new NotImplementedException();
        }

        public Session Delete(long id)
        {
            throw new NotImplementedException();
        }

        public Session FindById(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Session> GetSessions()
        {
            throw new NotImplementedException();
        }

        public Session Update(Session sessionChanges)
        {
            throw new NotImplementedException();
        }
    }
}
