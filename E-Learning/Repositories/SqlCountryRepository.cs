using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public class SqlCountryRepository : ICountryRepository
    {
        private readonly ApplicationDBContext dBContext;
        public SqlCountryRepository(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public Country Create(Country country)
        {
            dBContext.Countries.Add(country);
            dBContext.SaveChanges();
            return country;
        }

        public Country Delete(int id)
        {
            var country = dBContext.Countries.Find(id);
            if(country != null)
            {
                dBContext.Countries.Remove(country);
                dBContext.SaveChanges();
                return country;
            }
            return null;
        }

        public Country FindById(int id)
        {
            return dBContext.Countries.Find(id);
        }

        public IList<Country> GetCountries()
        {
            return dBContext.Countries.ToList();
        }

        public Country Update(Country countryChanges)
        {
            var country = dBContext.Countries.Attach(countryChanges);
            country.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            dBContext.SaveChanges();
            return countryChanges;
        }
    }
}
