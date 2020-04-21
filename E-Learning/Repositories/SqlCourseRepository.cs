using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public class SqlCourseRepository : ICourseRepository
    {
        private readonly ApplicationDBContext dBContext;

        public SqlCourseRepository(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public Course Create(Course course)
        {
            throw new NotImplementedException();
        }

        public Course Delete(long id)
        {
            throw new NotImplementedException();
        }

        public Course FindById(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Course> GetCourses()
        {
            throw new NotImplementedException();
        }

        public Course Update(Course courseChanges)
        {
            throw new NotImplementedException();
        }
    }
}
