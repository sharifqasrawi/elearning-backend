using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Learning.Models;
using E_Learning.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Learning.Controllers
{
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryRepository _countryRepository;
        public CountriesController(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetCountries()
        {
            var errorMessages = new List<string>();
            try
            {
                var countries = _countryRepository.GetCountries();
                return Ok(new { countries });

            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpPost]
        public IActionResult CreateCountry([FromBody] Country country)
        {
            var errorMessages = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(country.Name_EN))
                {
                    errorMessages.Add("Country name is required");
                    return BadRequest(new { errors = errorMessages });
                }

                var newCountry = new Country()
                {
                    Name_EN = country.Name_EN,
                    Name_FR = country.Name_FR ?? country.Name_EN,
                    FlagPath = country.FlagPath
                };

                var createdCountry = _countryRepository.Create(newCountry);
                
                
                return Ok(new { createdCountry });

            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpPut]
        public IActionResult UpdateCountry([FromBody] Country country)
        {
            var errorMessages = new List<string>();
            try
            {
                var c = _countryRepository.FindById(country.Id);

                if(c == null)
                {
                    return NotFound();
                }

                if (string.IsNullOrEmpty(country.Name_EN))
                {
                    errorMessages.Add("Country name is required");
                    return BadRequest(new { errors = errorMessages });
                }

                c.Name_EN = country.Name_EN;
                c.Name_FR = country.Name_FR ?? country.Name_EN;
                c.FlagPath = country.FlagPath;
                

                var updatedCountry = _countryRepository.Update(c);


                return Ok(new { updatedCountry });

            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpDelete]
        public IActionResult DeleteCountry([FromQuery] int countryId)
        {
            var errorMessages = new List<string>();
            try
            {
                var c = _countryRepository.FindById(countryId);

                if (c == null)
                {
                    return NotFound();
                }

                
                var deletedCountry = _countryRepository.Delete(c.Id);

                if (deletedCountry == null)
                {
                    errorMessages.Add("Error deleting country");
                    return BadRequest(new { errors = errorMessages });
                }

                return Ok(new { deletedCountryId = deletedCountry.Id });

            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }
    }
}