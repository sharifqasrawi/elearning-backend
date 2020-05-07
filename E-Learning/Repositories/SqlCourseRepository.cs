using E_Learning.Models;
using Microsoft.EntityFrameworkCore;
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
            dBContext.Courses.Add(course);
            dBContext.SaveChanges();

            return course;
        }

        public Course Delete(long id)
        {
            var course = dBContext.Courses.Find(id);
            if(course != null)
            {
                dBContext.Courses.Remove(course);
                dBContext.SaveChanges();
            }
            return course;
        }

        public Course FindById(long id)
        {
            var course = dBContext.Courses
                                  .Include("Category")
                                  .Include("Sections")
                                  .Include("Sections.Sessions")
                                  //   .Include("Sections.Course")
                                  .Include("CourseTags")
                                  .Include("CourseTags.Tag")
                                  .Include("Likes")
                                  .SingleOrDefault(c => c.Id == id);


            return course;
        }

        public Course FindBySlug(string slug)
        {
            var course = dBContext.Courses
                                 .Include("Category")
                                 .Include("Sections")
                                 .Include("Sections.Sessions")
                                 .Include("CourseTags")
                                 .Include("CourseTags.Tag")
                                 .Include("Likes")
                                 .SingleOrDefault(c => c.Slug_EN == slug);


            return course;
        }

        public IEnumerable<Course> GetCourses()
        {
            var courses = dBContext.Courses
                                  .Include("Category")
                                  .Include("Sections")
                                  .Include("Sections.Sessions")
                                  .Include("CourseTags")
                                  .Include("CourseTags.Tag")
                                  .Include("Likes");

            return courses;
        }

        public Course Update(Course courseChanges)
        {
            var course = dBContext.Courses.Attach(courseChanges);
            course.State = EntityState.Modified;
            dBContext.SaveChanges();
            return courseChanges;
        }
    }
}
