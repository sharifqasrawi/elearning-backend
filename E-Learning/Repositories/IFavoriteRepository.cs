using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface IFavoriteRepository
    {
        Favorite Create(Favorite favorite);
        Favorite Delete(long id);
        IList<Favorite> GetFavoritesByUserId(string userId);
    }
}
