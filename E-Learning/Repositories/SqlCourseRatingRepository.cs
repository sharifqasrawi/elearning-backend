using E_Learning.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public class SqlCourseRatingRepository : ICourseRatingRepository
    {
        private readonly ApplicationDBContext dBContext;
        public SqlCourseRatingRepository(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public CourseRating Create(CourseRating courseRating)
        {
            dBContext.CourseRatings.Add(courseRating);
            dBContext.SaveChanges();
            return courseRating;
        }

        public CourseRating Delete(long id)
        {
            var courseRating = dBContext.CourseRatings.Find(id);
            if(courseRating != null)
            {
                dBContext.CourseRatings.Remove(courseRating);
                dBContext.SaveChanges();
                return courseRating;
            }
            return null;
        }

        public CourseRating FindById(long id)
        {
            return dBContext.CourseRatings.Include("Course").SingleOrDefault(r => r.Id == id);
        }

        public CourseRating FindByUserCourse(string userId, long courseId)
        {
            return dBContext.CourseRatings
                .Include("Course")
                .SingleOrDefault(r => r.UserId == userId && r.CourseId == courseId);
        }

        public IList<CourseRating> GetCourseRatings()
        {
            return dBContext.CourseRatings.Include("Course").ToList();
        }

        public IList<CourseRating> GetCourseRatingsByCourseId(long courseId)
        {
            var courseRatings = dBContext.CourseRatings
                                .Include("Course")
                                .Where(r => r.CourseId == courseId)
                                .ToList();

            return courseRatings;
        }

        public CourseRating Update(CourseRating courseRatingChanges)
        {
            var courseRating = dBContext.CourseRatings.Attach(courseRatingChanges);
            courseRating.State = EntityState.Modified;
            dBContext.SaveChanges();
            return courseRatingChanges;
        }
    }
}
