using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface IAboutRepository
    {
        About Create(About about);
        About Update(About aboutChanges);
        About Get();
    }
}
