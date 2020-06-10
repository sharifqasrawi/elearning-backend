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
using Slugify;

namespace E_Learning.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly ISessionContentRepository _sessioncontentRepository;
        private readonly IDoneSessionRepository _doneSessionRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IClassRepository _classRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITranslator _translator;

        public SessionsController(ISessionRepository sessionRepository,
                                  ISessionContentRepository sessionContentRepository,
                                  IDoneSessionRepository doneSessionRepository,
                                  ISectionRepository sectionRepository,
                                  ICourseRepository courseRepository,
                                  IClassRepository classRepository,
                                  UserManager<ApplicationUser> userManager,
                                  ITranslator translator)
        {
            _sessionRepository = sessionRepository;
            _sessioncontentRepository = sessionContentRepository;
            _doneSessionRepository = doneSessionRepository;
            _sectionRepository = sectionRepository; 
            _courseRepository = courseRepository;
            _classRepository = classRepository;
            _userManager = userManager;
            _translator = translator;
        }


        [Authorize]
        [HttpGet]
        public IActionResult GetSessions([FromQuery] long sectionId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var sessions = _sessionRepository.GetSessionsBySectionId(sectionId)
                    .OrderBy(x => x.Order);

                return Ok(new { sessions });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create-session")]
        public IActionResult CreateSession([FromBody] Session session)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();

            var section = _sectionRepository.FindById(session.Section.Id);
            try
            {
                var newSession = new Session()
                {
                    Section = section,
                    Title_EN = session.Title_EN,
                    Title_FR = session.Title_FR ?? session.Title_EN,
                    Duration = session.Duration,
                    Slug_EN = new SlugHelper().GenerateSlug(session.Title_EN),
                    Slug_FR = session.Title_FR !=null ? new SlugHelper().GenerateSlug(session.Title_FR) : new SlugHelper().GenerateSlug(session.Title_EN),
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
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update-session")]
        public IActionResult UpdateSection([FromBody] Session session)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();

            try
            {
                var sec = _sessionRepository.FindById(session.Id);
                sec.Title_EN = session.Title_EN;
                sec.Title_FR = session.Title_FR ?? session.Title_EN;
                sec.Duration = session.Duration;
                sec.Slug_EN = new SlugHelper().GenerateSlug(session.Title_EN);
                sec.Slug_FR = session.Title_FR  != null ? new SlugHelper().GenerateSlug(session.Title_FR) : new SlugHelper().GenerateSlug(session.Title_EN);
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
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-session")]
        public IActionResult DeleteSession([FromQuery] long sessionId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();

            try
            {

                var deletedSession = _sessionRepository.Delete(sessionId);

                return Ok(new { deletedSessionId = deletedSession.Id });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize]
        [HttpGet("contents")]
        public IActionResult GetContents([FromQuery] long sessionId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var contents = _sessioncontentRepository.GetSessionContents(sessionId)
                    .OrderBy(x => x.Order);

                return Ok(new { contents });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create-content")]
        public IActionResult CreateContent([FromBody] SessionContent sessionContent)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();

            var session = _sessionRepository.FindById(sessionContent.SessionId);
            try
            {
                var newSessionContent = new SessionContent()
                {
                    Session = session,
                    Type = sessionContent.Type,
                    Content = sessionContent.Content,
                    Content_FR = sessionContent.Content_FR ?? sessionContent.Content,
                    Order = sessionContent.Order
                };

                if (!string.IsNullOrEmpty(sessionContent.Note))
                    newSessionContent.Note = sessionContent.Note;

                var createdContent = _sessioncontentRepository.Create(newSessionContent);

                return Ok(new { createdContent });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update-content")]
        public IActionResult UpdateContent([FromBody] SessionContent sessionContent)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();

            try
            {
                var content = _sessioncontentRepository.FindById(sessionContent.Id);
                content.Type = sessionContent.Type;
                content.Content = sessionContent.Content;
                content.Content_FR = sessionContent.Content_FR ?? sessionContent.Content;
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
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-content")]
        public IActionResult DeleteContent([FromQuery] long contentId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();

            try
            {

                var deletedSessionContent = _sessioncontentRepository.Delete(contentId);

                return Ok(new { deletedSessionContentId = deletedSessionContent.Id });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }



        [Authorize(Roles = "Admin")]
        [HttpGet("session-max-order")]
        public IActionResult GetMaxSessionOrder([FromQuery] long courseId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();

            try
            {
                var maxOrder = _sessionRepository.GetSessionsByCourseId(courseId)
                    .Max(s => s.Order);
                

                return Ok(new { maxOrder });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize]
        [HttpGet("done")]
        public IActionResult GetUserDoneSessions([FromQuery] string userId, long? courseId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            if (string.IsNullOrEmpty(userId) || courseId == null)
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }

            try
            {

                var doneSessions = _doneSessionRepository.GetDoneSessionsByUserAndCourse(userId, courseId.Value);

                var courseSessionsCount = _sessionRepository.GetSessionsByCourseId(courseId.Value).Count;

                var doneSessionsCount = doneSessions.Count;

                var donePercentage = ((doneSessionsCount * 100) / courseSessionsCount);


                return Ok(new { doneSessions, donePercentage });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }


        [Authorize]
        [HttpPost("mark-session")]
        public async Task<IActionResult> MarkSession([FromBody] DoneSession doneSession)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            if(string.IsNullOrEmpty(doneSession.UserID) || doneSession.SessionId == null)
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }

            try
            {

                var user = await _userManager.FindByIdAsync(doneSession.UserID);
                var session = _sessionRepository.FindById(doneSession.SessionId.Value);

                if (user == null || session == null)
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }

                if(session.Section.Course.Class == null || !_classRepository.IsUserInClass(session.Section.Course.Class.Id, user.Id))
                {
                    errorMessages.Add(_translator.GetTranslation("CLASSES.USER_NOT_ENROLLED", lang));
                    return BadRequest(new { errors = errorMessages });
                }

                var ds = _doneSessionRepository.FindBySessionAndUser(session.Id, user.Id);
                if (ds == null)
                {
                    var newDoneSession = new DoneSession()
                    {
                        User = user,
                        UserID = user.Id,
                        Session = session,
                        SessionId = session.Id,
                        DoneDateTime = DateTime.Now
                    };

                    var createdDoneSession = _doneSessionRepository.Create(newDoneSession);

                    if (createdDoneSession == null)
                    {
                        errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                        return BadRequest(new { errors = errorMessages });
                    }
                    var courseId = session.Section.Course.Id;
                    var doneSessions = _doneSessionRepository.GetDoneSessionsByUserAndCourse(doneSession.UserID, courseId);

                    var courseSessionsCount = _sessionRepository.GetSessionsByCourseId(courseId).Count;

                    var doneSessionsCount = doneSessions.Count;

                    var donePercentage = ((doneSessionsCount * 100) / courseSessionsCount);

                    return Ok(new { createdDoneSession, donePercentage });
                }
                else
                {
                    var courseId = session.Section.Course.Id;
                    var doneSessions = _doneSessionRepository.GetDoneSessionsByUserAndCourse(doneSession.UserID, courseId);

                    var courseSessionsCount = _sessionRepository.GetSessionsByCourseId(courseId).Count;

                    var doneSessionsCount = doneSessions.Count;

                    var donePercentage = ((doneSessionsCount * 100) / courseSessionsCount);

                    return Ok(new { createdDoneSession = ds, donePercentage });
                }
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize]
        [HttpDelete("unmark-session")]
        public IActionResult UnmarkSession([FromQuery] long? sessionId, string userId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            if (string.IsNullOrEmpty(userId) || sessionId == null)
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }

            try
            {
                var doneSession = _doneSessionRepository.FindBySessionAndUser(sessionId.Value, userId);

                if (doneSession == null || doneSession.UserID != userId)
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }

                if (!_classRepository.IsUserInClass(doneSession.Session.Section.Course.Class.Id, userId))
                {
                    errorMessages.Add(_translator.GetTranslation("CLASSES.USER_NOT_ENROLLED", lang));
                    return BadRequest(new { errors = errorMessages });
                }

               var deletedDoneSession = _doneSessionRepository.Delete(doneSession.Id);

                var courseId = doneSession.Session.Section.Course.Id;
                var doneSessions = _doneSessionRepository.GetDoneSessionsByUserAndCourse(doneSession.UserID, courseId);

                var courseSessionsCount = _sessionRepository.GetSessionsByCourseId(courseId).Count;

                var doneSessionsCount = doneSessions.Count;

                var donePercentage = ((doneSessionsCount * 100) / courseSessionsCount);

                return Ok(new { deletedDoneSessionId = deletedDoneSession.Id, donePercentage });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize]
        [HttpGet("courses-progress")]
        public IActionResult GetUserCoursesProgress([FromQuery] string userId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            if (string.IsNullOrEmpty(userId))
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }

            try
            {
                var courses = _courseRepository.GetEnrolledCoursesByUserId(userId);

                var memberCoursesProgress = new List<object>();

                foreach (var course in courses)
                {
                    var doneSessions = _doneSessionRepository.GetDoneSessionsByUserAndCourse(userId, course.Id);

                    var courseSessionsCount = _sessionRepository.GetSessionsByCourseId(course.Id).Count;

                    var doneSessionsCount = doneSessions.Count;

                    var donePercentage = ((doneSessionsCount * 100) / courseSessionsCount);


                    memberCoursesProgress.Add(new
                    {
                        courseId = course.Id,
                        donePercentage
                    });
                }

                return Ok(new { memberCoursesProgress });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }
    }
}