﻿using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Repositories
{
    public interface ICourseRepository
    {
        Course Create(Course course);
        IList<Course> GetCourses();
        IList<Course> GetEnrolledCoursesByUserId(string userId);
        Course Update(Course courseChanges);
        Course Delete(long id);
        Course FindById(long id);
        Course FindByClassId(string id);
        Course FindBySlug(string slug);

    }
}
