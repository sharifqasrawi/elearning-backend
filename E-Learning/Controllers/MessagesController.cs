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
using Microsoft.AspNetCore.Mvc;

namespace E_Learning.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IEmailMessageRepository _emailMessageRepository;
        private readonly ITranslator _translator;

        public MessagesController(IMessageRepository messageRepository, 
                                  IEmailMessageRepository emailMessageRepository,
                                   ITranslator translator)
        {
            _messageRepository = messageRepository;
            _emailMessageRepository = emailMessageRepository;
            _translator = translator;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetMessages()
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var messages = _messageRepository.GetMessages();

                return Ok(new { messages });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("message")]
        public IActionResult GetMessage([FromQuery] long id)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var message = _messageRepository.GetMessage(id);
                if (message == null)
                    return NotFound();

                return Ok(new {  message });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpPost("send")]
        public IActionResult Send([FromBody] Message message)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var newMessage = new Message()
                {
                    Name = message.Name,
                    Email = message.Email,
                    Subject = message.Subject,
                    Text = message.Text,
                    DateTime = DateTime.Now,
                    IsSeen = false
                };

                var createdMessage = _messageRepository.Send(newMessage);

                return Ok(new { message = createdMessage });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete")]
        public IActionResult DeleteMessage([FromQuery] long msgId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var message = _messageRepository.GetMessage(msgId);

                if(message == null)
                {
                    return NotFound();
                }

                message = _messageRepository.Delete(msgId);

                return Ok(new { deletedMsgId = message.Id });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("change-seen")]
        public IActionResult ChangeSeen([FromQuery] long msgId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var message = _messageRepository.GetMessage(msgId);

                if (message == null)
                {
                    return NotFound();
                }
                var oldState = message.IsSeen;
                message.IsSeen = !oldState;

                if (message.IsSeen)
                    message.SeenDateTime = DateTime.Now;
               

                var updatedMessage = _messageRepository.Update(message);

                return Ok(new {  updatedMessage });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("sent-emails")]
        public IActionResult GetEmailMessages()
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var messages = _emailMessageRepository.GetEmailMessages();

                return Ok(new { emailMessages = messages });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("send-email")]
        public IActionResult SendEmail([FromBody] EmailMessage message)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                try
                {
                    string To = message.Emails;
                    string Subject = message.Subject;
                    string Body = message.Message;

                    Email email = new Email(To, Subject, Body);
                    email.Send();

                    var createdMessage = _emailMessageRepository.Create(message);

                    return Ok(new { emailMessage = createdMessage });
                }
                catch 
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }

            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

    }
}