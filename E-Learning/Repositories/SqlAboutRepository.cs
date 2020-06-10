using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public class SqlAboutRepository : IAboutRepository
    {
        private readonly ApplicationDBContext dBContext;
        public SqlAboutRepository(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public About Create(About about)
        {
            dBContext.Abouts.Add(about);
            dBContext.SaveChanges();
            return about;
        }

        public About Get()
        {
            return dBContext.Abouts.FirstOrDefault();
        }

        public About Update(About aboutChanges)
        {
            var about = dBContext.Abouts.Attach(aboutChanges);
            about.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            dBContext.SaveChanges();
            return aboutChanges;
        }
    }
}
