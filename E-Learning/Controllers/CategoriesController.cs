using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Learning.Helpers;
using E_Learning.Models;
using E_Learning.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Slugify;

namespace E_Learning.Controllers
{
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
    [ApiController]

    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITranslator _translator;

        public CategoriesController(ICategoryRepository categoryRepository, ITranslator translator)
        {
            _categoryRepository = categoryRepository;
            _translator = translator;
        }

        [HttpGet]
        
        public IActionResult GetCategories()
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var categories = _categoryRepository.GetCategories()
                                                    .Where(c => c.DeletedAt == null);

                return Ok(new { categories });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpGet("deleted")]
        public IActionResult GetDeletedCategories()
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var categories = _categoryRepository.GetCategories()
                                                    .Where(c => c.DeletedAt != null);

                return Ok(new { categories });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpGet("category")]
        public IActionResult GetCategory([FromQuery] int id)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var category = _categoryRepository.GetCategory(id);

                if (category == null)
                    return NotFound();

                return Ok(new { category });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpPost("new")]
        public IActionResult CreateCategory([FromBody] Category category)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();

            if (string.IsNullOrEmpty(category.Title_EN))
                errorMessages.Add(_translator.GetTranslation("VALIDATION.CATEGORY_TITLE_REQUIRED", lang));

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
                    Slug = slugHelper.GenerateSlug(category.Title_EN),
                    Title_FR = category.Title_FR != null ? category.Title_FR : category.Title_EN,
                    Slug_FR = category.Title_FR != null ? slugHelper.GenerateSlug(category.Title_FR) : slugHelper.GenerateSlug(category.Title_EN),
                };

                newCategory.UpdatedAt = newCategory.CreatedAt;
                newCategory.UpdatedBy = newCategory.CreatedBy;

                var createdCategory = _categoryRepository.Create(newCategory);

                return Ok(new { category = createdCategory });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpPut("update")]
        public IActionResult UpdateCategory([FromBody] Category category)
        {
            var lang = Request.Headers["language"].ToString();
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
                cat.Title_FR = category.Title_FR != null ? category.Title_FR : category.Title_EN;
                cat.Slug_FR = category.Title_FR != null ? new SlugHelper().GenerateSlug(category.Title_FR) : new SlugHelper().GenerateSlug(category.Title_EN);

                var updatedCategory = _categoryRepository.Update(cat);

                return Ok(new { updatedCategory });

            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpDelete("delete")]
        public IActionResult deleteCategory([FromQuery] int id)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            
            try
            {
                var cat = _categoryRepository.GetCategory(id);

                if (cat == null)
                    return NotFound();

                _categoryRepository.Delete(cat.Id);
              
                return Ok(new { categoryId = cat.Id });

            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpPut("trash")]
        public IActionResult TrashCategory([FromBody] Category category)
        {
            var lang = Request.Headers["language"].ToString();
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
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [HttpPut("restore")]
        public IActionResult RestoreCategory([FromBody] Category category)
        {
            var lang = Request.Headers["language"].ToString();
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
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }
    }
}