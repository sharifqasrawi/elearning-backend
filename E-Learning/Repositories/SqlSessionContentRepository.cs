using E_Learning.Models;
using Microsoft.EntityFrameworkCore;
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
            dBContext.SessionContents.Add(SessionContent);
            dBContext.SaveChanges();

            return SessionContent;
        }

        public SessionContent Delete(long id)
        {
            var sessionContent = dBContext.SessionContents.Find(id);
            if(sessionContent != null)
            {
                dBContext.SessionContents.Remove(sessionContent);
                dBContext.SaveChanges();
            }

            return sessionContent;
        }

        public SessionContent FindById(long id)
        {
            var sessionContent = dBContext.SessionContents
                                          .Include("Session")
                                          .SingleOrDefault(s => s.Id == id);

            return sessionContent;
        }

        public IEnumerable<SessionContent> GetSessionContents()
        {
            var sessionContents = dBContext.SessionContents
                                          .Include("Session");

            return sessionContents;
        }

        public SessionContent Update(SessionContent sessionContentChanges)
        {
            var sessionContent = dBContext.SessionContents.Attach(sessionContentChanges);
            sessionContent.State = EntityState.Modified;
            dBContext.SaveChanges();
            return sessionContentChanges;
        }
    }
}
