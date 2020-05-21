using E_Learning.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public class SqlAppRatingRepository : IAppRatingRepository
    {
        private readonly ApplicationDBContext dBContext;
        public SqlAppRatingRepository(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public AppRating Create(AppRating appRating)
        {
            dBContext.AppRatings.Add(appRating);
            dBContext.SaveChanges();
            return appRating;
        }

        public AppRating Delete(long id)
        {
            var appRating = dBContext.AppRatings.Find(id);
            if(appRating != null)
            {
                dBContext.AppRatings.Remove(appRating);
                dBContext.SaveChanges();
                return appRating;
            }
            return null;
        }

        public AppRating FindById(long id)
        {
            return dBContext.AppRatings.Find(id);
        }

        public AppRating FindByUserId(string userId)
        {
            return dBContext.AppRatings.SingleOrDefault(r => r.UserId == userId);
        }

        public IList<AppRating> GetAppRatings()
        {
            return dBContext.AppRatings.Include("User").ToList();
        }

        public AppRating Update(AppRating appRatingChanges)
        {
            var appRating = dBContext.AppRatings.Attach(appRatingChanges);
            appRating.State = EntityState.Modified;
            dBContext.SaveChanges();
            return appRatingChanges;
        }
    }
}
