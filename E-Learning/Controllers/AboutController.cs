using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Learning.Helpers;
using E_Learning.Models;
using E_Learning.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Learning.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AboutController : ControllerBase
    {
        private readonly IAboutRepository _aboutRepository;
        private readonly ITranslator _translator;

        public AboutController(IAboutRepository aboutRepository, ITranslator translator)
        {
            _aboutRepository = aboutRepository;
            _translator = translator;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();

            try
            {
                var about = _aboutRepository.Get();
                if (about == null)
                {
                    var newAbout = new About();

                    about = _aboutRepository.Create(newAbout);
                }
                return Ok(new { about });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public IActionResult Update([FromBody] About aboutData)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();

            try
            {
                var dbAbout = _aboutRepository.Get();

                dbAbout.Name = aboutData.Name;
                dbAbout.Title = aboutData.Title;
                dbAbout.Title_FR = aboutData.Title_FR ?? aboutData.Title;
                dbAbout.ImagePath = aboutData.ImagePath;
                dbAbout.Info = aboutData.Info;
                dbAbout.Info_FR = aboutData.Info_FR ?? aboutData.Info;
                dbAbout.Email1 = aboutData.Email1;
                dbAbout.Email2 = aboutData.Email2;
                dbAbout.Website = aboutData.Website;
                dbAbout.PhoneNumber = aboutData.PhoneNumber;

                var about = _aboutRepository.Update(dbAbout);


                return Ok(new { about });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }
    }
}