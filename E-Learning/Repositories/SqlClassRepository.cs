using E_Learning.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public class SqlClassRepository : IClassRepository
    {
        private readonly ApplicationDBContext dBContext;
        public SqlClassRepository(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public Class Create(Class cls)
        {
            dBContext.Classes.Add(cls);
            dBContext.SaveChanges();
            return cls;
        }

        public Class FindById(string id)
        {
            return dBContext.Classes
                .Include("ClassUsers")
                .Include("Course")
                .Include("Course.Category")
                .Include("Course.Sections")
                .Include("Course.Sections.Sessions")
                .Include("Course.Sections.Sessions.Contents")
                .Include("Course.CourseTags")
                .Include("Course.CourseTags.Tag")
                .Include("Course.Likes")
                .Include("Course.Comments")
                .SingleOrDefault(c => c.Id == id);
        }

        public IList<Class> GetClasses()
        {
            return dBContext.Classes
                .Include("Course")
                .Include("Course.Category")
                .Include("Course.Sections")
                .Include("Course.Sections.Sessions")
                .Include("Course.Sections.Sessions.Contents")
                .Include("Course.CourseTags")
                .Include("Course.CourseTags.Tag")
                .Include("Course.Likes")
                .Include("Course.Comments")
                .ToList();
        }

        public bool IsExistsInCourse(long courseId)
        {
            var cls = dBContext.Classes.Where(c => c.CourseId == courseId).ToList();
            if(cls.Count == 0)
            {
                return false;
            }
            return true;
        }

        public Class Update(Class clsChanges)
        {
            var cls = dBContext.Classes.Attach(clsChanges);
            cls.State = EntityState.Modified;
            dBContext.SaveChanges();
            return clsChanges;
        }
    }
}
