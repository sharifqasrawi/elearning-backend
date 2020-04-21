using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface IDirectoryRepository
    {
        IEnumerable<Directory> GetDirectories();

        Directory Create(Directory directory);
        Directory FindByPath(string path);
        Directory FindById(int id);
        Directory Delete(int id);
    }
}
