using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface ICountryRepository
    {
        Country Create(Country country);
        Country Update(Country countryChanges);
        Country Delete(int id);
        Country FindById(int id);
        IList<Country> GetCountries();

    }
}
