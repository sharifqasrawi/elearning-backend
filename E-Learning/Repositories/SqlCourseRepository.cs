﻿using E_Learning.Models;
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
                                  .Include("CourseTags")
                                  .Include("CourseTags.Tag")
                                  .Include("Likes")
                                  .Include("Comments")
                                  .Include("Comments.Replies")
                                  .Include("Comments.Likes")
                                  .Include("Class")
                                  .Include("Class.ClassUsers")
                                  .Include("Class.ClassUsers.User")
                                  .Include("Ratings")
                                  .Include("Ratings.User")
                                  .SingleOrDefault(c => c.Id == id);


            return course;
        }

        public Course FindByClassId(string id)
        {
            var course = dBContext.Courses
                                  .Include("Category")
                                  .Include("Sections")
                                  .Include("Sections.Sessions")
                                  .Include("Sections.Sessions.Contents")
                                  .Include("CourseTags")
                                  .Include("CourseTags.Tag")
                                  .Include("Likes")
                                  .Include("Comments")
                                  .Include("Comments.Replies")
                                  .Include("Comments.Likes")
                                  .Include("Class")
                                  .Include("Class.ClassUsers")
                                  .Include("Class.ClassUsers.User")
                                  .Include("Ratings")
                                  .Include("Ratings.User")
                                  .SingleOrDefault(c => c.Class.Id == id);


            return course;
        }


        public Course FindBySlug(string slug)
        {
            var course = dBContext.Courses
                                 .Include("Category")
                                 .Include("Sections")
                                 .Include("Sections.Sessions")
                                 .Include("Sections.Sessions.Contents")
                                 .Include("CourseTags")
                                 .Include("CourseTags.Tag")
                                 .Include("Likes")
                                 .Include("Comments")
                                 .Include("Comments.Replies")
                                 .Include("Comments.Likes")
                                 .Include("Class")
                                 .Include("Class.ClassUsers")
                                 .Include("Class.ClassUsers.User")
                                 .Include("Ratings")
                                 .Include("Ratings.User")
                                 .SingleOrDefault(c => c.Slug_EN == slug);


            return course;
        }

        public IList<Course> GetCourses()
        {
            var courses = dBContext.Courses
                                  .Include("Category")
                                  .Include("Sections")
                                  .Include("Sections.Sessions")
                                  .Include("Sections.Sessions.Contents")
                                  .Include("CourseTags")
                                  .Include("CourseTags.Tag")
                                  .Include("Likes")
                                  .Include("Comments")
                                  .Include("Comments.Replies")
                                  .Include("Comments.Likes")
                                  .Include("Class")
                                  .Include("Class.ClassUsers")
                                  .Include("Class.ClassUsers.User")
                                  .Include("Ratings")
                                  .Include("Ratings.User")
                                  .ToList();

            return courses;
        }

        public Course Update(Course courseChanges)
        {
            var course = dBContext.Courses.Attach(courseChanges);
            course.State = EntityState.Modified;
            dBContext.SaveChanges();
            return courseChanges;
        }

        public IList<Course> GetEnrolledCoursesByUserId(string userId)
        {
            var courses = dBContext.Courses
                                  .Include("Category")
                                  .Include("Sections")
                                  .Include("Sections.Sessions")
                                  .Include("Sections.Sessions.Contents")
                                  .Include("CourseTags")
                                  .Include("CourseTags.Tag")
                                  .Include("Likes")
                                  .Include("Comments")
                                  .Include("Comments.Replies")
                                  .Include("Comments.Likes")
                                  .Include("Class")
                                  .Include("Class.ClassUsers")
                                  .Include("Class.ClassUsers.User")
                                  .Include("Ratings")
                                  .Include("Ratings.User")
                                  .Where(c => c.Class.ClassUsers.SingleOrDefault(u => u.UserId == userId).UserId == userId)
                                  .ToList();

            return courses;
        }
    }
}
