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
    public class SavedSessionsController : ControllerBase
    {
        private readonly ISavedSessionRepository _savedSessionRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITranslator _translator;

        public SavedSessionsController(ISavedSessionRepository savedSessionRepository,
                                       ISessionRepository sessionRepository,
                                       UserManager<ApplicationUser> userManager,
                                       ITranslator translator)
        {
            _savedSessionRepository = savedSessionRepository;
            _sessionRepository = sessionRepository;
            _userManager = userManager;
            _translator = translator;
        }


        [Authorize(Roles = "Admin, Author, User")]
        [HttpGet]
        public IActionResult GetSavedSessions([FromQuery] string userId)
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

                var savedSessions = _savedSessionRepository.GetSavedSessionsByUserId(userId);

                return Ok(new { savedSessions });
            }
            catch 
            {
               errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin, Author, User")]
        [HttpGet("sessions")]
        public IActionResult GetSavedSessions2([FromQuery] string userId)
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

                var savedSessions = _savedSessionRepository.GetSavedSessionsByUserId(userId);
                var allSessions = _sessionRepository.GetSessions();

                var sessions = new List<object>();


                foreach(var ss in savedSessions)
                {
                    foreach(var s in allSessions)
                    {
                        if(s.Id == ss.SessionId.Value)
                        {
                            sessions.Add(new {
                                s.Id,
                                s.Order,
                                s.Title_EN,
                                s.Title_FR,
                                s.Duration,
                                ss.SaveDateTime,
                                ss.SessionUrl,
                                courseTitle_EN = s.Section.Course.Title_EN,
                                courseTitle_FR = s.Section.Course.Title_FR
                            });
                        }
                    }
                }


                return Ok(new { sessions });
            }
            catch 
            {
               errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin, Author, User")]
        [HttpPost]
        public async Task<IActionResult> SaveSession([FromBody] SavedSession savedSession)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                if(savedSession.SessionId  == null || string.IsNullOrEmpty(savedSession.UserId))
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }

                var user = await _userManager.FindByIdAsync(savedSession.UserId);
                var session = _sessionRepository.FindById(savedSession.SessionId.Value);

                if (user == null || session == null)
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }

                var newSavedSession = new SavedSession()
                {
                    Session = session,
                    SessionId = session.Id,
                    User = user,
                    UserId = user.Id,
                    SaveDateTime = DateTime.Now,
                    SessionUrl = savedSession.SessionUrl
                };

                var createdSavedSession = _savedSessionRepository.Create(newSavedSession);


                return Ok(new { createdSavedSession });
            }
            catch 
            {
               errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }
        [Authorize(Roles = "Admin, Author, User")]
        [HttpDelete]
        public IActionResult RemoveSession([FromQuery] long? id)
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

                var deletedSession = _savedSessionRepository.Delete(id.Value);
                if(deletedSession == null)
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }

                return Ok(new { deletedSessionId = deletedSession.Id });
            }
            catch 
            {
               errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }
    }
}