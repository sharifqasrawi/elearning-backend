using E_Learning.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public class SqlSectionRepository : ISectionRepository
    {
        private readonly ApplicationDBContext dBContext;

        public SqlSectionRepository(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public Section Create(Section section)
        {
            dBContext.Sections.Add(section);
            dBContext.SaveChanges();

            return section;
        }

        public Section Delete(long id)
        {
            var section = dBContext.Sections.Find(id);
            if(section != null)
            {
                dBContext.Sections.Remove(section);
                dBContext.SaveChanges();
            }
            return section;
        }

        public Section FindById(long id)
        {
            var section = dBContext.Sections
                                   .Include("Course")
                                   .Include("Sessions")
                                   .SingleOrDefault(s => s.Id == id);

            return section;
        }

        public IEnumerable<Section> GetSections()
        {
            var sections = dBContext.Sections
                                   .Include("Course")
                                   .Include("Sessions");

            return sections;
        }

        public Section Update(Section sectionChanges)
        {
            var section = dBContext.Sections.Attach(sectionChanges);
            section.State = EntityState.Modified;
            dBContext.SaveChanges();
            return sectionChanges;
        }
    }
}
