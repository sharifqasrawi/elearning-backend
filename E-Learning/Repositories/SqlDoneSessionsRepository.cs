using E_Learning.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public class SqlDoneSessionsRepository : IDoneSessionRepository
    {
        private readonly ApplicationDBContext dBContext;
        public SqlDoneSessionsRepository(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public DoneSession Create(DoneSession doneSession)
        {
            dBContext.DoneSessions.Add(doneSession);
            dBContext.SaveChanges();
            return doneSession;
        }

        public DoneSession Delete(long id)
        {
            var doneSession = dBContext.DoneSessions.Find(id);
            if(doneSession != null)
            {
                dBContext.DoneSessions.Remove(doneSession);
                dBContext.SaveChanges();
                return doneSession;
            }
            return null;
        }

        public DoneSession Delete(long sessionId, string userId)
        {
            var doneSession = dBContext.DoneSessions.SingleOrDefault(s => s.SessionId == sessionId && s.UserID == userId);
            if (doneSession != null)
            {
                dBContext.DoneSessions.Remove(doneSession);
                dBContext.SaveChanges();
                return doneSession;
            }
            return null;
        }

        public bool DeleteAllByCourseUser(long courseId, string userId)
        {
            var doneSessions = dBContext.DoneSessions
                                         .Include("User")
                                         .Include("Session.Section.Course")
                                         .Where(s => s.UserID == userId && s.Session.Section.Course.Id == courseId)
                                         .ToList();

            dBContext.DoneSessions.RemoveRange(doneSessions);
            dBContext.SaveChanges();
            return true;
        }


        public DoneSession FindById(long id)
        {
            return dBContext.DoneSessions
                .Include("User")
                .Include("Session.Section.Course")
                .SingleOrDefault(d => d.Id == id);
        }

        public DoneSession FindBySessionAndUser(long sessionId, string userId)
        {
            return dBContext.DoneSessions
                .Include("User")
                .Include("Session.Section.Course")
                .Include("Session.Section.Course.Class")
                .Include("Session.Section.Course.Class.ClassUsers")
                .SingleOrDefault(s => s.SessionId == sessionId && s.UserID == userId);
        }

        public IList<DoneSession> GetDoneSessions()
        {
            return dBContext.DoneSessions.ToList();
        }

        public IList<DoneSession> GetDoneSessionsByUserAndCourse(string userId, long courseId)
        {
            var doneSessions = dBContext.DoneSessions
                                        .Include("User")
                                        .Include("Session.Section.Course")
                                        .Where(s => s.UserID == userId && s.Session.Section.Course.Id == courseId)
                                        .ToList();

            return doneSessions;
        }

        public DoneSession Update(DoneSession doneSessionChanges)
        {
            var doneSession = dBContext.DoneSessions.Attach(doneSessionChanges);
            doneSession.State = EntityState.Modified;
            dBContext.SaveChanges();
            return doneSessionChanges;
        }
    }
}
