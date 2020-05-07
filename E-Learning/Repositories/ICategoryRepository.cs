using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface ICategoryRepository
    {
        IList<Category> GetCategories();
        Category GetCategory(int id);

        Category Create(Category category);
        Category Update(Category categoryChanges);
        Category Delete(int id);

    }
}
