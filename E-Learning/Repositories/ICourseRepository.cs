using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface ICourseRepository
    {
        Course Create(Course course);
        IEnumerable<Course> GetCourses();
        Course Update(Course courseChanges);
        Course Delete(long id);
        Course FindById(long id);
    }
}
