using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface IVisitRepository
    {
        Visit Create(Visit visit);
        Visit Update(Visit visitChanges);
        IList<Visit> GetVisits();
        Visit FindByIP(string IP);
    }
}
