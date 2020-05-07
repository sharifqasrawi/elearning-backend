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
    public class SectionsController : ControllerBase
    {
        private readonly ISectionRepository _sectionRepository;
        private readonly ICourseRepository _courseRepository;

        public SectionsController(ISectionRepository sectionRepository,
                                  ICourseRepository courseRepository)
        {
            _sectionRepository = sectionRepository;
            _courseRepository = courseRepository;
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetSections([FromQuery] long courseId)
        {
            var errorMessages = new List<string>();
            try
            {
                var sections = _sectionRepository.GetSectionsByCourseId(courseId)
                    .OrderBy(x => x.Order);

                return Ok(new { sections });
            }
            catch(Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpPost("create-section")]
        public IActionResult CreateSection([FromBody] Section section)
        {
            var errorMessages = new List<string>();

            var course = _courseRepository.FindById(section.Course.Id);
            try
            {
                var newSection = new Section()
                {
                    Course = course,
                    Name_EN = section.Name_EN,
                    Slug_EN = new SlugHelper().GenerateSlug(section.Name_EN),
                    Order = section.Order,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = section.CreatedBy,
                    UpdatedBy = section.UpdatedBy,
                    DeletedAt = null,
                    DeletedBy = null
                };

                var createdSection = _sectionRepository.Create(newSection);

                return Ok(new { createdSection });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpPut("update-section")]
        public IActionResult UpdateSection([FromBody] Section section)
        {
            var errorMessages = new List<string>();

            try
            {
                var sec = _sectionRepository.FindById(section.Id);
                sec.Name_EN = section.Name_EN;
                sec.Slug_EN = new SlugHelper().GenerateSlug(section.Name_EN);
                sec.UpdatedAt = DateTime.Now;
                sec.UpdatedBy = section.UpdatedBy;

                Section updatedOldSection = null;

                if (sec.Order != section.Order)
                {
                    var oldOrder = sec.Order;

                    // Previous
                    var oldSec = _sectionRepository.GetSectionsByCourseId(sec.Course.Id)
                        .SingleOrDefault(s => s.Order == section.Order);

                    if (oldSec != null)
                    {
                        oldSec.Order = oldOrder;
                        updatedOldSection = _sectionRepository.Update(oldSec);
                    }

                    // New
                    sec.Order = section.Order;

                }

                var updatedSection = _sectionRepository.Update(sec);

                if(updatedOldSection != null)
                {
                    return Ok(new { updatedSection, updatedOldSection });
                }else
                {
                    return Ok(new { updatedSection });
                }

            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpDelete("delete-section")]
        public IActionResult Deleteection([FromQuery] long sectionId)
        {
            var errorMessages = new List<string>();

            try
            {

                var deletedSection = _sectionRepository.Delete(sectionId);

                return Ok(new { deletedSectionId = deletedSection.Id });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }
    }
}