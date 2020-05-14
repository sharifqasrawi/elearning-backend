using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface IClassRepository
    {
        Class Create(Class cls);
        Class Update(Class clsChanges);
        Class FindById(string id);

        IList<Class> GetClasses();

        bool IsExistsInCourse(long courseId);
    }
}
