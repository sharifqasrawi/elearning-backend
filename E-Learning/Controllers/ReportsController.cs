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

namespace E_Learning.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportRepository _reportRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportsController(IReportRepository reportRepository,
                                 UserManager<ApplicationUser> userManager)
        {
            _reportRepository = reportRepository;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost("report-bug")]
        public async Task<IActionResult> ReportBug([FromBody] Report report)
        {
            var errorMessages = new List<string>();
            try
            {
                if(string.IsNullOrEmpty(report.UserFullName)
                    || string.IsNullOrEmpty(report.Severity)
                    || string.IsNullOrEmpty(report.Type)
                    || string.IsNullOrEmpty(report.Description)
                    || (report.SeverityLevel != 0 ||report.SeverityLevel != 1 || report.SeverityLevel != 2))
                {
                    errorMessages.Add("Error reporting bug. Please try again.");
                    return BadRequest(new { errors = errorMessages });
                }

                ApplicationUser user = null;
                if (!string.IsNullOrEmpty(report.UserId))
                {
                    user = await _userManager.FindByIdAsync(report.UserId);
                }

                var newReport = new Report()
                {
                    User = user,
                    UserId = user?.Id,
                    UserFullName = report.UserFullName,
                    SeverityLevel = report.SeverityLevel,
                    Severity = report.Severity,
                    Type = report.Type,
                    Description = report.Description,
                    ReportDateTime = DateTime.Now,
                    IsSeen = false
                };

                var createdReport = _reportRepository.Create(newReport);

                if(createdReport == null)
                {
                    errorMessages.Add("Error reporting bug. Please try again.");
                    return BadRequest(new { errors = errorMessages });
                }


                return Ok(new { createdReport });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetReports([FromQuery] string type)
        {
            var errorMessages = new List<string>();
            try
            {
                var reports = _reportRepository.GetReportsByType(type);

                return Ok(new { reports });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }

        }
    }
}