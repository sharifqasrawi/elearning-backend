using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface IAppRatingRepository
    {
        AppRating Create(AppRating appRating);
        AppRating Update(AppRating appRatingChanges);
        AppRating Delete(long id);
        AppRating FindById(long id);
        AppRating FindByUserId(string userId);
        IList<AppRating> GetAppRatings();

    }
}
