using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Learning.Models;
using E_Learning.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Slugify;

namespace E_Learning.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly ISessionContentRepository _sessioncontentRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly ICourseRepository _courseRepository;

        public SessionsController(ISessionRepository sessionRepository,
                                  ISessionContentRepository sessionContentRepository,
                                  ISectionRepository sectionRepository,
                                  ICourseRepository courseRepository)
        {
            _sessionRepository = sessionRepository;
            _sessioncontentRepository = sessionContentRepository;
            _sectionRepository = sectionRepository; 
            _courseRepository = courseRepository;
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetSessions([FromQuery] long sectionId)
        {
            var errorMessages = new List<string>();
            try
            {
                var sessions = _sessionRepository.GetSessionsBySectionId(sectionId)
                    .OrderBy(x => x.Order);

                return Ok(new { sessions });
            }
            catch(Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpPost("create-session")]
        public IActionResult CreateSession([FromBody] Session session)
        {
            var errorMessages = new List<string>();

            var section = _sectionRepository.FindById(session.Section.Id);
            try
            {
                var newSession = new Session()
                {
                    Section = section,
                    Title_EN = session.Title_EN,
                    Duration = session.Duration,
                    Slug_EN = new SlugHelper().GenerateSlug(session.Title_EN),
                    Order = session.Order,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = session.CreatedBy,
                    UpdatedBy = session.UpdatedBy,
                    DeletedAt = null,
                    DeletedBy = null
                };

                var createdSession = _sessionRepository.Create(newSession);

                return Ok(new { createdSession });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpPut("update-session")]
        public IActionResult UpdateSection([FromBody] Session session)
        {
            var errorMessages = new List<string>();

            try
            {
                var sec = _sessionRepository.FindById(session.Id);
                sec.Title_EN = session.Title_EN;
                sec.Duration = session.Duration;
                sec.Slug_EN = new SlugHelper().GenerateSlug(session.Title_EN);
                sec.UpdatedAt = DateTime.Now;
                sec.UpdatedBy = session.UpdatedBy;

                Session updatedOldSession = null;

                if (sec.Order != session.Order)
                {
                    var oldOrder = sec.Order;

                    // Previous
                    var oldSec = _sessionRepository.GetSessionsByCourseId(session.Section.Course.Id)
                        .SingleOrDefault(s => s.Order == session.Order);

                    if (oldSec != null)
                    {
                        oldSec.Order = oldOrder;
                        updatedOldSession = _sessionRepository.Update(oldSec);
                    }

                    // New
                    sec.Order = session.Order;

                }

                var updatedSession = _sessionRepository.Update(sec);

                if(updatedOldSession != null)
                {
                    return Ok(new { updatedSession, updatedOldSession });
                }else
                {
                    return Ok(new { updatedSession });
                }

            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpDelete("delete-session")]
        public IActionResult DeleteSession([FromQuery] long sessionId)
        {
            var errorMessages = new List<string>();

            try
            {

                var deletedSession = _sessionRepository.Delete(sessionId);

                return Ok(new { deletedSessionId = deletedSession.Id });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }


        [HttpGet("contents")]
        public IActionResult GetContents([FromQuery] long sessionId)
        {
            var errorMessages = new List<string>();
            try
            {
                var contents = _sessioncontentRepository.GetSessionContents(sessionId)
                    .OrderBy(x => x.Order);

                return Ok(new { contents });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpPost("create-content")]
        public IActionResult CreateContent([FromBody] SessionContent sessionContent)
        {
            var errorMessages = new List<string>();

            var session = _sessionRepository.FindById(sessionContent.SessionId);
            try
            {
                var newSessionContent = new SessionContent()
                {
                    Session = session,
                    Type = sessionContent.Type,
                    Content = sessionContent.Content,
                    Order = sessionContent.Order
                };

                if (!string.IsNullOrEmpty(sessionContent.Note))
                    newSessionContent.Note = sessionContent.Note;

                var createdContent = _sessioncontentRepository.Create(newSessionContent);

                return Ok(new { createdContent });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpPut("update-content")]
        public IActionResult UpdateContent([FromBody] SessionContent sessionContent)
        {
            var errorMessages = new List<string>();

            try
            {
                var content = _sessioncontentRepository.FindById(sessionContent.Id);
                content.Type = sessionContent.Type;
                content.Content = sessionContent.Content;
                if (!string.IsNullOrEmpty(sessionContent.Note))
                    content.Note = sessionContent.Note;

                SessionContent updatedOldSessionContent = null;

                if (content.Order != sessionContent.Order)
                {
                    var oldOrder = content.Order;

                    // Previous
                    var oldSesContent = _sessioncontentRepository.GetSessionContents(content.SessionId)
                        .SingleOrDefault(s => s.Order == sessionContent.Order);

                    if (oldSesContent != null)
                    {
                        oldSesContent.Order = oldOrder;
                        updatedOldSessionContent = _sessioncontentRepository.Update(oldSesContent);
                    }

                    // New
                    content.Order = sessionContent.Order;

                }

                var updatedSessionContent = _sessioncontentRepository.Update(content);

                if (updatedOldSessionContent != null)
                {
                    return Ok(new { updatedSessionContent, updatedOldSessionContent });
                }
                else
                {
                    return Ok(new { updatedSessionContent });
                }

            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpDelete("delete-content")]
        public IActionResult DeleteContent([FromQuery] long contentId)
        {
            var errorMessages = new List<string>();

            try
            {

                var deletedSessionContent = _sessioncontentRepository.Delete(contentId);

                return Ok(new { deletedSessionContentId = deletedSessionContent.Id });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }



        [AllowAnonymous]
        [HttpGet("session-max-order")]
        public IActionResult GetMaxSessionOrder([FromQuery] long courseId)
        {
            var errorMessages = new List<string>();

            try
            {
                var maxOrder = _sessionRepository.GetSessionsByCourseId(courseId)
                    .Max(s => s.Order);
                

                return Ok(new { maxOrder });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }
    }
}