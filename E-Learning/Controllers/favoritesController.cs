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
        private readonly ITranslator _translator;

        public FavoritesController(IFavoriteRepository favoriteRepository,
                                   ICourseRepository courseRepository,
                                   UserManager<ApplicationUser> userManager,
                                   ITranslator translator)
        {
            _favoriteRepository = favoriteRepository;
            _courseRepository = courseRepository;
            _userManager = userManager;
            _translator = translator;
        }

        [Authorize(Roles = "Admin, Author, User")]
        [HttpGet]
        public IActionResult GetUserFavorites([FromQuery] string userId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }

                var favorites = _favoriteRepository.GetFavoritesByUserId(userId);

                return Ok(new { favorites });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin, Author, User")]
        [HttpPost("add-course")]
        public async Task<IActionResult> AddCourseToFavorites([FromBody] Favorite favorite)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(favorite.UserId) || favorite.CourseId == null)
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }


                var user = await _userManager.FindByIdAsync(favorite.UserId);
                var course = _courseRepository.FindById(favorite.CourseId.Value);

                if (user == null || course== null)
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
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
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin, Author, User")]
        [HttpDelete("remove-course")]
        public IActionResult RemoveCourseFromFavorite([FromQuery] long? id)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                if (id == null)
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }

                var deletedFavorite = _favoriteRepository.Delete(id.Value);

                if(deletedFavorite  == null)
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }

                return Ok(new { deletedFavoriteId = deletedFavorite.Id });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin, Author, User")]
        [HttpGet("courses")]
        public IActionResult GetUserFavoriteCourses([FromQuery] string userId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
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
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }
    }
}