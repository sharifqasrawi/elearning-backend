using E_Learning.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public class SqlCategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDBContext dBContext;

        public SqlCategoryRepository(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public Category Create(Category category)
        {
            dBContext.Add(category);
            dBContext.SaveChanges();
            return category;
        }

        public Category Delete(int id)
        {
            var category = dBContext.Categories.Find(id);
            if(category != null)
            {
                dBContext.Categories.Remove(category);
                dBContext.SaveChanges();
            }
            return category;
        }

        public IList<Category> GetCategories()
        {
            return dBContext.Categories
                .Include("Courses")
                .OrderBy(c => c.Title_EN)
                .ToList();
        }

        public Category GetCategory(int id)
        {
            return dBContext.Categories.Find(id);
        }

        public Category Update(Category categoryChanges)
        {
            var category = dBContext.Categories.Attach(categoryChanges);
            category.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            dBContext.SaveChanges();

            return categoryChanges;
        }
    }
}
