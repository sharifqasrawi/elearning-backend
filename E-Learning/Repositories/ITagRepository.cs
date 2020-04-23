using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface ITagRepository
    {
        IEnumerable<Tag> GetTags();
        Tag Create(Tag tag);
        Tag Update(Tag tagChanges);
        Tag FindById(long id);
        Tag Delete(long id);

    }
}
