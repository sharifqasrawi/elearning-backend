using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Learning.Helpers;
using E_Learning.Models;
using E_Learning.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_Learning.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public FavoritesController(IFavoriteRepository favoriteRepository,
                                   ICourseRepository courseRepository,
                                   UserManager<ApplicationUser> userManager)
        {
            _favoriteRepository = favoriteRepository;
            _courseRepository = courseRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult GetUserFavorites([FromQuery] string userId)
        {
            var errorMessages = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    errorMessages.Add("Error fetching favorites");
                    return BadRequest(new { errors = errorMessages });
                }

                var favorites = _favoriteRepository.GetFavoritesByUserId(userId);

                return Ok(new { favorites });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpPost("add-course")]
        public async Task<IActionResult> AddCourseToFavorites([FromBody] Favorite favorite)
        {
            var errorMessages = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(favorite.UserId) || favorite.CourseId == null)
                {
                    errorMessages.Add("Error adding course to favorites");
                    return BadRequest(new { errors = errorMessages });
                }


                var user = await _userManager.FindByIdAsync(favorite.UserId);
                var course = _courseRepository.FindById(favorite.CourseId.Value);

                if (user == null || course== null)
                {
                    errorMessages.Add("Error adding course to favorites");
                    return BadRequest(new { errors = errorMessages });
                }


                var newFavorite = new Favorite()
                {
                    Course = course,
                    CourseId = course.Id,
                    User = user,
                    UserId = user.Id
                };

                var createdFavorite = _favoriteRepository.Create(newFavorite);
                return Ok(new { createdFavorite });

            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpDelete("remove-course")]
        public IActionResult RemoveCourseFromFavorite([FromQuery] long? id)
        {
            var errorMessages = new List<string>();
            try
            {
                if (id == null)
                {
                    errorMessages.Add("Error removing course from favorites");
                    return BadRequest(new { errors = errorMessages });
                }

                var deletedFavorite = _favoriteRepository.Delete(id.Value);

                if(deletedFavorite  == null)
                {
                    errorMessages.Add("Error removing course from favorites");
                    return BadRequest(new { errors = errorMessages });
                }

                return Ok(new { deletedFavoriteId = deletedFavorite.Id });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpGet("courses")]
        public IActionResult GetUserFavoriteCourses([FromQuery] string userId)
        {
            var errorMessages = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    errorMessages.Add("Error fetching favorite courses");
                    return BadRequest(new { errors = errorMessages });
                }

                var favorites = _favoriteRepository.GetFavoritesByUserId(userId);
                var courses = new List<object>();

                foreach(var fav in favorites)
                {
                    foreach(var course in _courseRepository.GetCourses())
                    {
                        if(fav.CourseId == course.Id)
                        {
                            courses.Add(ResponseGenerator.GenerateCourseResponse(course,false));
                        }
                    }
                }

                return Ok(new { courses });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }
    }
}