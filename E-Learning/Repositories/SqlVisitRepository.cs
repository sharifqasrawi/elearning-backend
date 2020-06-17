using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public class SqlVisitRepository : IVisitRepository
    {
        private readonly ApplicationDBContext dBContext;
        public SqlVisitRepository(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public Visit Create(Visit visit)
        {
            dBContext.Visits.Add(visit);
            dBContext.SaveChanges();
            return visit;
        }

        public Visit FindByIP(string IP)
        {
            var visit = dBContext.Visits.Where(x => x.IPAddress == IP)
                                        .OrderBy(x => x.DateTime)
                                        .LastOrDefault();

            return visit;
        }

        public IList<Visit> GetVisits()
        {
            var visits = dBContext.Visits.ToList();
            return visits;
        }

        public Visit Update(Visit visitChanges)
        {
            var visit = dBContext.Visits.Attach(visitChanges);
            visit.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            dBContext.SaveChanges();
            return visitChanges;
        }
    }
}
