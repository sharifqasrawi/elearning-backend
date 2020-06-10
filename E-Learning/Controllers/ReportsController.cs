using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Learning.Emails;
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
    public class ReportsController : ControllerBase
    {
        private readonly IReportRepository _reportRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITranslator _translator;

        public ReportsController(IReportRepository reportRepository,
                                 UserManager<ApplicationUser> userManager,
                                  ITranslator translator)
        {
            _reportRepository = reportRepository;
            _userManager = userManager;
            _translator = translator;
        }

        [AllowAnonymous]
        [HttpPost("report-bug")]
        public async Task<IActionResult> ReportBug([FromBody] Report report)
        {
            var lang = Request.Headers["language"].ToString();
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
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
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
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }


                return Ok(new { createdReport });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetReports([FromQuery] string type)
        {
            var lang = Request.Headers["language"].ToString();
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
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }

        }

        [Authorize]
        [HttpGet("by-user")]
        public IActionResult GetReportsByUserId([FromQuery] string userId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                IList<Report> reports = null;

                if (string.IsNullOrEmpty(userId))
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }


                reports = _reportRepository.GetReportsByUserId(userId);
                

                return Ok(new { reports });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpPut("mark-report-seen")]
        public IActionResult MarkReportSeen([FromBody] Report report)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var rpt = _reportRepository.FindByID(report.Id);

                rpt.IsSeen = report.IsSeen;

                var updatedReport = _reportRepository.Update(rpt);

                return Ok(new { updatedReport });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }

        }

        [Authorize]
        [HttpPut("mark-reply-seen")]
        public IActionResult MarkReplySeen([FromBody] Report report)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var rpt = _reportRepository.FindByID(report.Id);
                if(report.UserId != rpt.UserId)
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }

                rpt.IsReplySeen = report.IsReplySeen;

                var updatedReport = _reportRepository.Update(rpt);

                return Ok(new { updatedReport });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }

        }
        [Authorize(Roles = "Admin")]
        [HttpPut("reply-report")]
        public IActionResult ReplyToReport([FromBody] Report report)
        {
            var lang = Request.Headers["language"].ToString();
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
                    string Subject = _translator.GetTranslation("REPORTS.REPLY_EMAIL_SUBJECT", lang);
                    string Body = rpt.ReplyMessage;
                    var email = new Email(To, Subject, Body);
                    email.Send();
                }
                catch
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }

                return Ok(new { updatedReport });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }

        }
    }
}