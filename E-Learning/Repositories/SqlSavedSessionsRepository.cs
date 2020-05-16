using E_Learning.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public class SqlSavedSessionsRepository : ISavedSessionRepository
    {
        private readonly ApplicationDBContext dBContext;
        public SqlSavedSessionsRepository(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public SavedSession Create(SavedSession savedSession)
        {
            dBContext.SavedSessions.Add(savedSession);
            dBContext.SaveChanges();
            return savedSession;
        }

        public SavedSession Delete(long id)
        {
            var savedSession = dBContext.SavedSessions.Find(id);
            if(savedSession != null)
            {
                dBContext.SavedSessions.Remove(savedSession);
                dBContext.SaveChanges();
                return savedSession;
            }
            return null;
        }

        public IList<SavedSession> GetSavedSessionsByUserId(string userId)
        {
            var savedSessions = dBContext.SavedSessions
                                        .Where(s => s.UserId == userId)
                                        .ToList();

            return savedSessions;
        }
    }
}
