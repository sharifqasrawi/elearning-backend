using E_Learning.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public class SqlDirectoryRepository : IDirectoryRepository
    {
        private readonly ApplicationDBContext dBContext;

        public SqlDirectoryRepository(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public Directory Create(Directory directory)
        {
            dBContext.Directories.Add(directory);
            dBContext.SaveChanges();

            return directory;
        }

        public IEnumerable<Directory> GetDirectories()
        {
            return dBContext.Directories.OrderBy(d => d.Path);
        }

        public Directory FindByPath(string path)
        {
            return dBContext.Directories.Include("UploadedFiles")
                .SingleOrDefault(d => d.Path == path);
        }

        public Directory FindById(int id)
        {
            return dBContext.Directories.Find(id);
        }

        public Directory Delete(int id)
        {
            var dir = dBContext.Directories.Find(id);
            if (dir != null)
            {
                dBContext.Directories.Remove(dir);
                dBContext.SaveChanges();
            }
            return dir;
        }
    }
}
