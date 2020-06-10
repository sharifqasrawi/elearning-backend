using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using E_Learning.Models;
using E_Learning.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using E_Learning.Hubs;
using E_Learning.Helpers;

namespace E_Learning.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AppRatingsController : ControllerBase
    {
        private readonly IAppRatingRepository _appRatingRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<SignalHub> _hubContext;
        private readonly ITranslator _translator;

        public AppRatingsController(IAppRatingRepository appRatingRepository,
                                    UserManager<ApplicationUser> userManager,
                                    IHubContext<SignalHub> hubContext,
                                    ITranslator translator)
        {
            _appRatingRepository = appRatingRepository;
            _userManager = userManager;
            _hubContext = hubContext;
            _translator = translator;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RateApp([FromBody] AppRating   appRating)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var user = await _userManager.FindByIdAsync(appRating.UserId);

                if (user == null)
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }


                var allowedValues = new List<float>() { 1, 2, 3, 4, 5 };

                if (!allowedValues.Contains(appRating.Value))
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }

                var rating = _appRatingRepository.FindByUserId(user.Id);
                AppRating createdAppRating;

                if (rating == null)
                {

                    var newRating = new AppRating()
                    {
                        User = user,
                        UserId = user.Id,
                        Value = appRating.Value,
                        RateDateTime = DateTime.Now
                    };

                    createdAppRating = _appRatingRepository.Create(newRating);

                }
                else
                {
                    rating.OldValue = rating.Value;
                    rating.Value = appRating.Value;
                    rating.RateDateTimeUpdated = DateTime.Now;

                    createdAppRating = _appRatingRepository.Update(rating);
                }

                if (createdAppRating == null)
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }


                var appRatings = _appRatingRepository.GetAppRatings();


                var ratings = new
                {
                    total = appRatings.Count > 0 ? appRatings.Sum(r => r.Value) / appRatings.Count : 0,
                    ratings = appRatings
                };

                await _hubContext.Clients.All.SendAsync("SignalAppRateReceived", new { ratings });
              
                return Ok(new { ratings });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAppRatings([FromQuery] string userId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                var appRatings = _appRatingRepository.GetAppRatings();

                var ratingsList = new List<object>();

                if (user != null && user.IsAdmin && (await _userManager.IsInRoleAsync(user, "Admin")))
                {
                    foreach (var ar in appRatings)
                    {
                        ratingsList.Add(new
                        {
                            ar.Id,
                            ar.UserId,
                            userName = $"{ar.User.FirstName } {ar.User.LastName }",
                            userGender = ar.User.Gender,
                            userCountry = ar.User.Country,
                            ar.Value,
                            ar.OldValue,
                            ar.RateDateTime,
                            ar.RateDateTimeUpdated
                        });
                    }
                }
                else
                {

                    foreach (var ar in appRatings)
                    {
                        ratingsList.Add(new
                        {
                            ar.Id,
                            ar.UserId,
                            ar.Value,
                        });
                    }
                }
                var ratings = new
                {
                    total = appRatings.Count > 0 ? appRatings.Sum(r => r.Value) / appRatings.Count : 0,
                    ratings = ratingsList
                };

                return Ok(new { ratings });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }
    }
}