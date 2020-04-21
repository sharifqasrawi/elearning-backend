using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface ISectionRepository
    {
        Section Create(Section section);
        IEnumerable<Section> GetSections();
        Section Update(Section sectionChanges);
        Section Delete(long id);
        Section FindById(long id);
    }
}
