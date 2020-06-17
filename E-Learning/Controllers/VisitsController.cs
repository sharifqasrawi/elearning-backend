using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using E_Learning.Models;
using E_Learning.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;

namespace E_Learning.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VisitsController : ControllerBase
    {
        private readonly IActionContextAccessor _accessor;
        private readonly IVisitRepository _visitRepository;
        public VisitsController(IActionContextAccessor accessor, IVisitRepository visitRepository)
        {
            _accessor = accessor;
            _visitRepository = visitRepository;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Visit()
        {
            var errorMessages = new List<string>();
            //var lang = Request.Headers["language"].ToString();
            try
            {
                var ip = _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString();
              //  ip = "77.204.246.110";

                var visit = _visitRepository.FindByIP(ip);
                
                if (visit != null)
                {
                    var isNewDay = ( DateTime.Now - visit.DateTime.Value).TotalHours > 24;
                    if (!isNewDay)
                    {
                        visit.DayVisitsCount++;
                        visit.BrowserInfo = _accessor.ActionContext.HttpContext.Request.Headers["User-Agent"][0];
                        var createdVisit = _visitRepository.Update(visit);

                        return Ok(new { visitsCount = _visitRepository.GetVisits().Count });
                    }
                    else
                    {
                        var createdVisit = CreateVisit(ip);
                        return Ok(new { visitsCount = _visitRepository.GetVisits().Count });
                    }
                }
                else
                {
                    var createdVisit = CreateVisit(ip);

                    return Ok(new { visitsCount = _visitRepository.GetVisits().Count });
                }
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult GetVisitsForAdmin()
        {
            var errorMessages = new List<string>();
            try
            {
                var visits = _visitRepository.GetVisits();

                return Ok(new { visits });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpGet("client")]
        public IActionResult GetVisitsForClient()
        {
            var errorMessages = new List<string>();
            try
            {
                var dbVisits = _visitRepository.GetVisits();

                var visitsCount = dbVisits.Count;



                return Ok(new { visitsCount });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }


        private Visit CreateVisit(string ip)
        {
            var browserInfo = _accessor.ActionContext.HttpContext.Request.Headers["User-Agent"][0];

            var api = $"http://api.ipstack.com";
            var security_key = "access_key=b89423a8b7b1b4e526c59060916ee1d4";
            var language = $"language=en";
            var options = $"fields=ip,continent_code,continent_name,country_code,country_name,region_code,region_name,city,zip,latitude,longitude";

            var countryInfo = new WebClient().DownloadString($"{api}/{ip}?{security_key}&{language}&{options}");
            var ipInfo = JsonConvert.DeserializeObject<IPInfo>(countryInfo);

            var newVisit = new Visit
            {
                BrowserInfo = browserInfo,
                IPAddress = ipInfo.IPAddress,
                Continent_Code = ipInfo.Continent_Code,
                Continent_Name = ipInfo.Continent_Name,
                Country_Code = ipInfo.Country_Code,
                Country_Name = ipInfo.Country_Name,
                Region_Code = ipInfo.Region_Code,
                Region_Name = ipInfo.Region_Name,
                City = ipInfo.City,
                Zip = ipInfo.Zip,
                Latitude = ipInfo.Latitude,
                Longitude = ipInfo.Longitude,
                DateTime = DateTime.Now,
                DayVisitsCount = 1
            };

            return _visitRepository.Create(newVisit);
        }
    }
}