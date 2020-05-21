using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Learning.Emails;
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
                var severityLevels = new List<int>() { 0, 1, 2 };

                if (string.IsNullOrEmpty(report.UserFullName)
                    || string.IsNullOrEmpty(report.Severity)
                    || string.IsNullOrEmpty(report.Type)
                    || string.IsNullOrEmpty(report.Description)
                    || !severityLevels.Contains(report.SeverityLevel.Value))
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
                    UserEmail = report.UserEmail,
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

        [HttpGet]
        public IActionResult GetReports([FromQuery] string type)
        {
            var errorMessages = new List<string>();
            try
            {
                IList<Report> reports = null;

                if(string.IsNullOrEmpty(type))
                {
                    reports = _reportRepository.GetReports();
                }
                else
                {
                    reports = _reportRepository.GetReportsByType(type);
                }

                return Ok(new { reports });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }

        }

        [HttpGet("by-user")]
        public IActionResult GetReportsByUserId([FromQuery] string userId)
        {
            var errorMessages = new List<string>();
            try
            {
                IList<Report> reports = null;

                if (string.IsNullOrEmpty(userId))
                {
                    errorMessages.Add("Error fetching reports.");
                    return BadRequest(new { errors = errorMessages });
                }


                reports = _reportRepository.GetReportsByUserId(userId);
                

                return Ok(new { reports });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }

        }

        [HttpPut("mark-report-seen")]
        public IActionResult MarkReportSeen([FromBody] Report report)
        {
            var errorMessages = new List<string>();
            try
            {
                var rpt = _reportRepository.FindByID(report.Id);

                rpt.IsSeen = report.IsSeen;

                var updatedReport = _reportRepository.Update(rpt);

                return Ok(new { updatedReport });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }

        }


        [HttpPut("mark-reply-seen")]
        public IActionResult MarkReplySeen([FromBody] Report report)
        {
            var errorMessages = new List<string>();
            try
            {
                var rpt = _reportRepository.FindByID(report.Id);
                if(report.UserId != rpt.UserId)
                {
                    errorMessages.Add("Error. Cannot mark this reply as seen");
                    return BadRequest(new { errors = errorMessages });
                }

                rpt.IsReplySeen = report.IsReplySeen;

                var updatedReport = _reportRepository.Update(rpt);

                return Ok(new { updatedReport });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }

        }

        [HttpPut("reply-report")]
        public IActionResult ReplyToReport([FromBody] Report report)
        {
            var errorMessages = new List<string>();
            try
            {
                var rpt = _reportRepository.FindByID(report.Id);

                rpt.ReplyMessage = report.ReplyMessage;
                rpt.ReplyDateTime = DateTime.Now;
                rpt.IsReplySeen = false;

                var updatedReport = _reportRepository.Update(rpt);

                try
                {
                    string To = rpt.UserEmail;
                    string Subject = "Report follow up";
                    string Body = rpt.ReplyMessage;
                    var email = new Email(To, Subject, Body);
                    email.Send();
                }
                catch
                {
                }

                return Ok(new { updatedReport });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }

        }
    }
}