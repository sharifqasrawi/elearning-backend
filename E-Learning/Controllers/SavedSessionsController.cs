using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Learning.Models;
using E_Learning.Repositories;
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

        public SavedSessionsController(ISavedSessionRepository savedSessionRepository,
                                       ISessionRepository sessionRepository,
                                       UserManager<ApplicationUser> userManager)
        {
            _savedSessionRepository = savedSessionRepository;
            _sessionRepository = sessionRepository;
            _userManager = userManager;
        }


        [HttpGet]
        public IActionResult GetSavedSessions([FromQuery] string userId)
        {
            var errorMessages = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    errorMessages.Add("Error fetching saved sessions");
                    return BadRequest(new { errors = errorMessages });
                }

                var savedSessions = _savedSessionRepository.GetSavedSessionsByUserId(userId);

                return Ok(new { savedSessions });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveSession([FromBody] SavedSession savedSession)
        {
            var errorMessages = new List<string>();
            try
            {
                if(savedSession.SessionId  == null || string.IsNullOrEmpty(savedSession.UserId))
                {
                    errorMessages.Add("Error saving session");
                    return BadRequest(new { errors = errorMessages });
                }

                var user = await _userManager.FindByIdAsync(savedSession.UserId);
                var session = _sessionRepository.FindById(savedSession.SessionId.Value);

                if (user == null || session == null)
                {
                    errorMessages.Add("Error saving session");
                    return BadRequest(new { errors = errorMessages });
                }

                var newSavedSession = new SavedSession()
                {
                    Session = session,
                    SessionId = session.Id,
                    User = user,
                    UserId = user.Id,
                    SaveDateTime = DateTime.Now
                };

                var createdSavedSession = _savedSessionRepository.Create(newSavedSession);


                return Ok(new { createdSavedSession });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpDelete]
        public IActionResult RemoveSession([FromQuery] long? id)
        {
            var errorMessages = new List<string>();
            try
            {
                if (id == null)
                {
                    errorMessages.Add("Error removing session from saved sessions");
                    return BadRequest(new { errors = errorMessages });
                }

                var deletedSession = _savedSessionRepository.Delete(id.Value);
                if(deletedSession == null)
                {
                    errorMessages.Add("Error removing session from saved sessions");
                    return BadRequest(new { errors = errorMessages });
                }

                return Ok(new { deletedSessionId = deletedSession.Id });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }
    }
}