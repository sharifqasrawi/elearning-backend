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

        public Report FindByID(long id)
        {
            return dBContext.Reports.Find(id);
        }

        public IList<Report> GetReports()
        {
            var reports = dBContext.Reports
                .OrderByDescending(r => r.SeverityLevel)
                .ThenBy(r => r.IsSeen)
                .ThenByDescending(r => r.ReportDateTime)
                .ToList();

            return reports;
        }

        public IList<Report> GetReportsByType(string type)
        {
            var reports = dBContext.Reports
                .Where(r => r.Type.ToLower() == type.ToLower())
                .OrderByDescending(r => r.SeverityLevel)
                .ThenBy(r => r.IsSeen)
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

        public Report Update(Report reportChanges)
        {
            var report = dBContext.Reports.Attach(reportChanges);
            report.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            dBContext.SaveChanges();
            return reportChanges;
        }
    }
}
