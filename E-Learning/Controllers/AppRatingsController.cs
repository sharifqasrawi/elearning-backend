﻿using System;
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

namespace E_Learning.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AppRatingsController : ControllerBase
    {
        private readonly IAppRatingRepository _appRatingRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<SignalHub> _hubContext;

        public AppRatingsController(IAppRatingRepository appRatingRepository,
                                    UserManager<ApplicationUser> userManager,
                                    IHubContext<SignalHub> hubContext)
        {
            _appRatingRepository = appRatingRepository;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> RateApp([FromBody] AppRating   appRating)
        {
            var errorMessages = new List<string>();
            try
            {
                var user = await _userManager.FindByIdAsync(appRating.UserId);

                if (user == null)
                {
                    errorMessages.Add("Error rating application.");
                    return BadRequest(new { errors = errorMessages });
                }


                var allowedValues = new List<float>() { 1, 2, 3, 4, 5 };

                if (!allowedValues.Contains(appRating.Value))
                {
                    errorMessages.Add("Error rating application.");
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
                    rating.Value = appRating.Value;
                    rating.RateDateTimeUpdated = DateTime.Now;

                    createdAppRating = _appRatingRepository.Update(rating);
                }

                if (createdAppRating == null)
                {
                    errorMessages.Add("Error rating Application.");
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
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAppRatings()
        {
            var errorMessages = new List<string>();
            try
            {
                var appRatings = _appRatingRepository.GetAppRatings();

                var ratings = new
                {
                    total = appRatings.Count > 0 ? appRatings.Sum(r => r.Value) / appRatings.Count : 0,
                    ratings = appRatings
                };

                return Ok(new { ratings });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }
    }
}