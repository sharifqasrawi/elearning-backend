using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface ICourseRatingRepository
    {
        CourseRating Create(CourseRating courseRating);
        CourseRating Update(CourseRating courseRatingChanges);
        CourseRating Delete(long id);
        CourseRating FindById(long id);
        CourseRating FindByUserCourse(string userId, long courseId);
        IList<CourseRating> GetCourseRatingsByCourseId(long courseId);
        IList<CourseRating> GetCourseRatings();
       
    }
}
