using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Learning.Models;
using E_Learning.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Slugify;

namespace E_Learning.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseRepository _courseRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICategoryRepository _categoryRepository;
        public CoursesController(ICourseRepository courseRepository,
                                 UserManager<ApplicationUser> userManager,
                                 ICategoryRepository categoryRepository)
        {
            _courseRepository = courseRepository;
            _userManager = userManager;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public IActionResult GetCourses()
        {
            var errorMessages = new List<string>();
            try
            {
                var courses = _courseRepository.GetCourses()
                                                .Where(c => c.DeletedAt == null);

                return Ok(new {  courses });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpGet("deleted")]
        public IActionResult GetDeletedCourses()
        {
            var errorMessages = new List<string>();
            try
            {
                var courses = _courseRepository.GetCourses()
                                              .Where(c => c.DeletedAt != null);

                return Ok(new { courses });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpPost("create-course")]
        public async Task<IActionResult> CreateCourse([FromBody] Course course)
        {
            var errorMessages = new List<string>();

            try
            {
                var author = await _userManager.FindByIdAsync(course.Author.Id);
                var category = _categoryRepository.GetCategory(course.Category.Id);

                var slugHelper = new SlugHelper();
                var newCourse = new Course()
                {
                    Title_EN = course.Title_EN,
                    Slug_EN = slugHelper.GenerateSlug(course.Title_EN),
                    Description_EN = course.Description_EN,
                    Prerequisites_EN = course.Prerequisites_EN,
                    Duration= course.Duration,
                    ImagePath = course.ImagePath,
                    IsFree = course.IsFree == null ? false : course.IsFree.Value,
                    Price = course.Price,
                    IsPublished = course.IsPublished == null ? false : course.IsPublished.Value,
                    CreatedBy = course.CreatedBy,
                    UpdatedBy = course.UpdatedBy,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    PublishedAt = null,
                    DeletedAt = null,
                    DeletedBy = null,
                    Author = author,
                    Category = category,
                    Languages = course.Languages,
                    Level = course.Level
                };

                if (newCourse.IsPublished.Value == true)
                    newCourse.PublishedAt = DateTime.Now;

                var createdCourse = _courseRepository.Create(newCourse);

                if (createdCourse != null)
                {
                    var response =
                    new
                    {
                        createdCourse.Title_EN,
                        createdCourse.Slug_EN,
                        createdCourse.Description_EN,
                        createdCourse.Prerequisites_EN,
                        createdCourse.Duration,
                        createdCourse.ImagePath,
                        createdCourse.IsFree,
                        createdCourse.Languages,
                        createdCourse.Level,
                        createdCourse.Price,
                        createdCourse.IsPublished,
                        createdCourse.CreatedBy,
                        createdCourse.UpdatedBy,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        createdCourse.PublishedAt,
                        createdCourse.DeletedAt,
                        createdCourse.DeletedBy,
                        Author = new {
                            createdCourse.Author.Id,
                            createdCourse.Author.FirstName,
                            createdCourse.Author.LastName,
                        },
                        Category = new
                        {
                            createdCourse.Category.Id,
                            createdCourse.Category.Title_EN,
                            createdCourse.Category.Slug,
                        }
                    };

                    return Ok(new { createdCourse = response });
                }

                errorMessages.Add("Error creating course");
                return BadRequest(new { errors = errorMessages });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

       [HttpPut("trash-restore-course")]
       public IActionResult TrashCourse([FromBody] Course course, [FromQuery] string action)
        {
            var errorMessages = new List<string>();
            try
            {
                var crs = _courseRepository.FindById(course.Id);

                if (action == "trash")
                {
                    crs.DeletedAt = DateTime.Now;
                    crs.DeletedBy = course.DeletedBy;
                }
                else if(action == "restore")
                {
                    crs.DeletedAt = null;
                    crs.DeletedBy = null;
                }
                var updatedCourse = _courseRepository.Update(crs);

                return Ok(new { updatedCourse });
            }
            catch(Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        

    }
}