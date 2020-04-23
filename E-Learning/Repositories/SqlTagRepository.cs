using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public class SqlTagRepository : ITagRepository
    {
        private readonly ApplicationDBContext dBContext;

        public SqlTagRepository(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public Tag Create(Tag tag)
        {
            dBContext.Tags.Add(tag);
            dBContext.SaveChanges();

            return tag;
        }

        public Tag Delete(long id)
        {
            var tag = dBContext.Tags.Find(id);
            if(tag != null)
            {
                dBContext.Tags.Remove(tag);
                dBContext.SaveChanges();
            }

            return tag;
        }

        public Tag FindById(long id)
        {
            return dBContext.Tags.Find(id);
        }

        public IEnumerable<Tag> GetTags()
        {
            return dBContext.Tags.OrderBy(t => t.Name);
        }

        public Tag Update(Tag tagChanges)
        {
            var tag = dBContext.Tags.Attach(tagChanges);
            tag.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            dBContext.SaveChanges();

            return tagChanges;
        }
    }
}
