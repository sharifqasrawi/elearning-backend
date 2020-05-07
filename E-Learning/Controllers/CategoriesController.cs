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

    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetCategories()
        {
            var errorMessages = new List<string>();
            try
            {
                var categories = _categoryRepository.GetCategories()
                                                    .Where(c => c.DeletedAt == null);

                return Ok(new { categories });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpGet("deleted")]
        public IActionResult GetDeletedCategories()
        {
            var errorMessages = new List<string>();
            try
            {
                var categories = _categoryRepository.GetCategories()
                                                    .Where(c => c.DeletedAt != null);

                return Ok(new { categories = categories });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpGet("category")]
        public IActionResult GetCategory([FromQuery] int id)
        {
            var errorMessages = new List<string>();
            try
            {
                var category = _categoryRepository.GetCategory(id);

                if (category == null)
                    return NotFound();

                return Ok(new { category });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpPost("new")]
        public IActionResult CreateCategory([FromBody] Category category)
        {
            var errorMessages = new List<string>();

            if (string.IsNullOrEmpty(category.Title_EN))
                errorMessages.Add("Category title is required");

            if (errorMessages.Count > 0)
                return BadRequest(new { errors = errorMessages });

            try
            {
                SlugHelper slugHelper = new SlugHelper();
                var newCategory = new Category()
                {
                    Title_EN = category.Title_EN,
                    ImagePath = category.ImagePath,
                    CreatedAt = DateTime.Now,
                    CreatedBy = category.CreatedBy,
                    Slug = slugHelper.GenerateSlug(category.Title_EN)
                };

                newCategory.UpdatedAt = newCategory.CreatedAt;
                newCategory.UpdatedBy = newCategory.CreatedBy;

                var createdCategory = _categoryRepository.Create(newCategory);

                return Ok(new { category = createdCategory });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpPut("update")]
        public IActionResult UpdateCategory([FromBody] Category category)
        {
            var errorMessages = new List<string>();

            try
            {
                var cat = _categoryRepository.GetCategory(category.Id);

                if (cat == null)
                    return NotFound();

                cat.Title_EN = category.Title_EN;
                cat.ImagePath = category.ImagePath;
                cat.Slug = new SlugHelper().GenerateSlug(category.Title_EN);
                cat.UpdatedAt = DateTime.Now;
                cat.UpdatedBy = category.UpdatedBy;

               var updatedCategory = _categoryRepository.Update(cat);

                return Ok(new { updatedCategory });

            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpDelete("delete")]
        public IActionResult deleteCategory([FromQuery] int id)
        {
            var errorMessages = new List<string>();
            
            try
            {
                var cat = _categoryRepository.GetCategory(id);

                if (cat == null)
                    return NotFound();

                _categoryRepository.Delete(cat.Id);
              
                return Ok(new { categoryId = cat.Id });

            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpPut("trash")]
        public IActionResult TrashCategory([FromBody] Category category)
        {
            var errorMessages = new List<string>();

            try
            {
                var cat = _categoryRepository.GetCategory(category.Id);

                if (cat == null)
                    return NotFound();

                cat.DeletedAt = DateTime.Now;
                cat.DeletedBy = category.DeletedBy;
                

                var updatedCategory = _categoryRepository.Update(cat);

                return Ok(new { category = updatedCategory });

            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpPut("restore")]
        public IActionResult RestoreCategory([FromBody] Category category)
        {
            var errorMessages = new List<string>();

            try
            {
                var cat = _categoryRepository.GetCategory(category.Id);

                if (cat == null)
                    return NotFound();

                cat.DeletedAt = null;
                cat.DeletedBy = null;


                var updatedCategory = _categoryRepository.Update(cat);

                return Ok(new { category = updatedCategory });

            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
                return BadRequest(new { errors = errorMessages });
            }
        }
    }
}