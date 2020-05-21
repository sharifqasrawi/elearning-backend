using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public class SqlReportRepository : IReportRepository
    {
        private readonly ApplicationDBContext dBContext;
        public SqlReportRepository(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public Report Create(Report report)
        {
            dBContext.Reports.Add(report);
            dBContext.SaveChanges();
            return report;
        }

        public Report Delete(long id)
        {
            var report = dBContext.Reports.Find(id);
            if(report != null)
            {
                dBContext.Reports.Remove(report);
                dBContext.SaveChanges();
                return report;
            }
            return null;
        }

        public IList<Report> GetReports()
        {
            var reports = dBContext.Reports
                .OrderByDescending(r => r.SeverityLevel)
                .ThenByDescending(r => r.ReportDateTime)
                .ToList();

            return reports;
        }

        public IList<Report> GetReportsByType(string type)
        {
            var reports = dBContext.Reports
                .Where(r => r.Type.ToLower() == type.ToLower())
                .OrderByDescending(r => r.SeverityLevel)
                .ThenByDescending(r => r.ReportDateTime)
                .ToList();

            return reports;
        }

        public IList<Report> GetReportsByUserId(string userId)
        {
            var reports = dBContext.Reports
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.SeverityLevel)
                .ThenByDescending(r => r.ReportDateTime)
                .ToList();

            return reports;
        }
    }
}
