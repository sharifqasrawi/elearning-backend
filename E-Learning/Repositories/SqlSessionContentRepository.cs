using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public class SqlSessionContentRepository : ISessionContentRepository
    {
        private readonly ApplicationDBContext dBContext;

        public SqlSessionContentRepository(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public SessionContent Create(SessionContent SessionContent)
        {
            throw new NotImplementedException();
        }

        public SessionContent Delete(long id)
        {
            throw new NotImplementedException();
        }

        public SessionContent FindById(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SessionContent> GetSessionContents()
        {
            throw new NotImplementedException();
        }

        public SessionContent Update(SessionContent sessionContentChanges)
        {
            throw new NotImplementedException();
        }
    }
}
