using E_Learning.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public class SqlFavoriteRepository : IFavoriteRepository
    {
        private readonly ApplicationDBContext dBContext;
        public SqlFavoriteRepository(ApplicationDBContext dBContext)    
        {
            this.dBContext = dBContext;
        }
        public Favorite Create(Favorite favorite)
        {
            dBContext.Favorites.Add(favorite);
            dBContext.SaveChanges();
            return favorite;
        }

        public Favorite Delete(long id)
        {
            var favorite = dBContext.Favorites.Find(id);
            if (favorite != null)
            {
                dBContext.Favorites.Remove(favorite);
                dBContext.SaveChanges();
                return favorite;
            }
            return null;
        }

        public IList<Favorite> GetFavoritesByUserId(string userId)
        {
            var favorites = dBContext.Favorites
                                     .Include("Course")
                                     .Where(f => f.UserId == userId)
                                     .OrderBy(f => f.Course.Title_EN)
                                     .ToList();

            return favorites;
        }
    }
}
