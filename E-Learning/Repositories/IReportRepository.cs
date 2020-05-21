using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface IReportRepository
    {
        Report Create(Report report);
        Report Delete(long id);
        IList<Report> GetReports();
        IList<Report> GetReportsByType(string type);
        IList<Report> GetReportsByUserId(string userId);


    }
}
