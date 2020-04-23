using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Learning.Models;
using E_Learning.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Learning.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ITagRepository _tagRepository;

        public TagsController(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        [HttpGet]
        public IActionResult GetTags()
        {
            var errorMessages = new List<string>();
            try
            {
                var tags = _tagRepository.GetTags();

                return Ok(new { tags });
            }
            catch(Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpGet("tag")]
        public IActionResult GetTagById([FromQuery] long id)
        {
            var errorMessages = new List<string>();
            try
            {
                var tag = _tagRepository.FindById(id);

                return Ok(new { tag });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] Tag tag)
        {
            var errorMessages = new List<string>();
            try
            {
                var newTag = new Tag()
                {
                    Name = tag.Name
                };

                var createdTag = _tagRepository.Create(newTag);

                if(createdTag == null)
                {
                    errorMessages.Add("Error creating tag");
                    return BadRequest(new { errors = errorMessages });
                }

                return Ok(new { createdTag });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpPut("update")]
        public IActionResult Update([FromBody] Tag tag)
        {
            var errorMessages = new List<string>();
            try
            {
                var t = _tagRepository.FindById(tag.Id);
                if (t == null)
                    return NotFound();

                t.Name = tag.Name;

                var updatedTag = _tagRepository.Update(t);

                return Ok(new { updatedTag });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpDelete("delete")]
        public IActionResult Delete([FromQuery] long id)
        {
            var errorMessages = new List<string>();
            try
            {
                var tag = _tagRepository.FindById(id);

                if (tag == null)
                    return NotFound();

                var deletedTag = _tagRepository.Delete(id);

                return Ok(new { deletedTagId = deletedTag.Id });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }
    }
}