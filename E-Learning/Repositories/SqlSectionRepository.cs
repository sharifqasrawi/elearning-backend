using E_Learning.Models;
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
            throw new NotImplementedException();
        }

        public Section Delete(long id)
        {
            throw new NotImplementedException();
        }

        public Section FindById(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Section> GetSections()
        {
            throw new NotImplementedException();
        }

        public Section Update(Section sectionChanges)
        {
            throw new NotImplementedException();
        }
    }
}
