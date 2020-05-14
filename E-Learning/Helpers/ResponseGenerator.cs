using E_Learning.Dtos.Classes;
using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Helpers
{
    public static class ResponseGenerator
    {
        public static object GenerateCourseResponse(Course course)
        {
            var tags = new List<Tag>();

            if (course.CourseTags != null)
            {
                foreach (var courseTag in course.CourseTags)
                {
                    tags.Add(new Tag() { Id = courseTag.TagId, Name = courseTag.Tag.Name });
                }
            }

            var sections = new List<Section>();
            if (course.Sections != null)
            {
                foreach (var courseSection in course.Sections)
                {
                    var sessions = new List<Session>();

                    if (courseSection.Sessions != null)
                    {
                        foreach (var sectionSession in courseSection.Sessions)
                        {
                            sessions.Add(new Session()
                            {
                                Id = sectionSession.Id,
                                Title_EN = sectionSession.Title_EN,
                                Slug_EN = sectionSession.Slug_EN,
                                Duration = sectionSession.Duration,
                                Order = sectionSession.Order,
                                CreatedAt = sectionSession.CreatedAt,
                                CreatedBy = sectionSession.CreatedBy,
                                UpdatedAt = sectionSession.UpdatedAt,
                                UpdatedBy = sectionSession.UpdatedBy
                            });
                        }
                    }
                    sections.Add(new Section()
                    {
                        Id = courseSection.Id,
                        Name_EN = courseSection.Name_EN,
                        Slug_EN = courseSection.Slug_EN,
                        Order = courseSection.Order,
                        CreatedAt = courseSection.CreatedAt,
                        CreatedBy = courseSection.CreatedBy,
                        UpdatedAt = courseSection.UpdatedAt,
                        UpdatedBy = courseSection.UpdatedBy,
                        DeletedAt = courseSection.DeletedAt,
                        DeletedBy = courseSection.DeletedBy,
                        Sessions = sessions.OrderBy(s => s.Order).ToList()
                    });
                }
            }

            var classDto = new ClassDto();
            if(course.Class != null)
            {
                var classUsers = new List<Member>();
                if (course.Class.ClassUsers != null)
                {
                    foreach (var classUser in course.Class.ClassUsers)
                    {
                        classUsers.Add(new Member()
                        {
                            Id = classUser.User.Id,
                            FullName = $"{classUser.User.FirstName} {classUser.User.LastName}",
                            Email = classUser.User.Email,
                            Gender = classUser.User.Gender,
                            Country = classUser.User.Country,
                            EnrollDateTime = classUser.EnrollDateTime.Value,
                            CurrentSessionId = classUser.CurrentSessionId,
                            CurrentSessionSlug = classUser.CurrentSessionSlug
                        });
                    }
                }
                classDto = new ClassDto()
                {
                    Id = course.Class.Id,
                    Name_EN = course.Class.Name_EN,
                    CourseId = course.Id,
                    Members = classUsers
                };
            }

            var response = new
            {
                course.Id,
                course.Title_EN,
                course.Slug_EN,
                course.Description_EN,
                course.Prerequisites_EN,
                course.Duration,
                course.ImagePath,
                course.IsFree,
                course.Price,
                course.IsPublished,
                course.Languages,
                course.Level,
                course.Category,
                course.CreatedAt,
                course.CreatedBy,
                course.DeletedAt,
                course.DeletedBy,
                course.PublishedAt,
                course.UpdatedAt,
                course.UpdatedBy,
                tags,
                sections = sections.OrderBy(s => s.Order),
                course.Likes,
                course.Comments,
                cls = classDto
            };

            return response;
        }

        public static object GenerateSessionResponse(Session session)
        {
            var response = new
            {
                session.Id,
                session.Order,
                session.Title_EN,
                session.Slug_EN,
                session.Duration,
                session.CreatedAt,
                session.CreatedBy,
                session.UpdatedAt,
                session.UpdatedBy,
                sectionId = session.Section.Id,
                courseId = session.Section.Course.Id,
                session.Contents
            };

            return response;
        }

        public static object GenerateCommentResponse(Comment comment)
        {
            var response = new
            {
                id = comment.Id,
                courseId = comment.CourseId,
                userId = comment.UserId,
                userFullName = comment.UserFullName,
                userGender = comment.UserGender,
                text = comment.Text,
                commentDateTime = comment.CommentDateTime,
                commentId = comment.CommentId,
                replies = comment.Replies,
                likes = comment.Likes
            };

            return response;
        }
    }
}
