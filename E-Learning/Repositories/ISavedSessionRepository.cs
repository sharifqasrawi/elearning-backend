using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface ISavedSessionRepository
    {
        SavedSession Create(SavedSession savedSession);
        SavedSession Delete(long id);
        IList<SavedSession> GetSavedSessionsByUserId(string userId);
    }
}
