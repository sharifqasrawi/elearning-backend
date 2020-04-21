using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Learning.Emails;
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

        public MessagesController(IMessageRepository messageRepository, IEmailMessageRepository emailMessageRepository)
        {
            _messageRepository = messageRepository;
            _emailMessageRepository = emailMessageRepository;
        }

        [HttpGet]
        public IActionResult GetMessages()
        {
            var errorMessages = new List<string>();
            try
            {
                var messages = _messageRepository.GetMessages();

                return Ok(new { messages = messages });
            }
            catch(Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpGet("message")]
        public IActionResult GetMessage([FromQuery] long id)
        {
            var errorMessages = new List<string>();
            try
            {
                var message = _messageRepository.GetMessage(id);
                if (message == null)
                    return NotFound();

                return Ok(new { message = message });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpPost("send")]
        public IActionResult Send([FromBody] Message message)
        {
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
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpDelete("delete")]
        public IActionResult DeleteMessage([FromQuery] long msgId)
        {
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
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpPut("change-seen")]
        public IActionResult ChangeSeen([FromQuery] long msgId)
        {
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

                return Ok(new { updatedMessage = updatedMessage });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpGet("sent-emails")]
        public IActionResult GetEmailMessages()
        {
            var errorMessages = new List<string>();
            try
            {
                var messages = _emailMessageRepository.GetEmailMessages();

                return Ok(new { emailMessages = messages });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpPost("send-email")]
        public IActionResult SendEmail([FromBody] EmailMessage message)
        {
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
                catch (Exception ex)
                {
                    errorMessages.Add(ex.Message);
                    return BadRequest(new { errors = errorMessages });
                }

            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

    }
}