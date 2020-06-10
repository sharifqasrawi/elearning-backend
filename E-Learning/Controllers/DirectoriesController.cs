using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using E_Learning.Dtos.Users;
using E_Learning.Helpers;
using E_Learning.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Learning.Controllers
{

    [Route("[controller]")]

    [ApiController]
    public class DirectoriesController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDirectoryRepository _directoryRepository;
        private readonly ITranslator _translator;

        public DirectoriesController(IWebHostEnvironment webHostEnvironment,
                                     IDirectoryRepository directoryRepository,
                                     ITranslator translator)
        {
            _webHostEnvironment = webHostEnvironment;
            _directoryRepository = directoryRepository;
            _translator = translator;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public IActionResult CreateDirectory([FromBody] DirectoryDto directoryDto)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();

            try
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                var dirPath = $"{webRootPath}\\appData\\{directoryDto.Path}\\{directoryDto.Name.Replace(" ", "_").Replace(".", "_").Replace("+","_")}";

                if (!Directory.Exists(dirPath))
                {
                    DirectoryInfo createDir = Directory.CreateDirectory(dirPath);

                    if (createDir != null)
                    {
                        var dir = new E_Learning.Models.Directory()
                        {
                            Name = directoryDto.Name.Replace(" ", "_"),
                            Path = dirPath.Replace($"{webRootPath}\\appData\\", "").Replace(" ", "_"),
                            CreatedAt = DateTime.Now
                        };

                        var createdDir = _directoryRepository.Create(dir);
                        return Ok(new { directory = createdDir });
                    }
                    else
                    {
                        errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                        return BadRequest(new { errors = errorMessages });
                    }

                }
                else
                {
                    errorMessages.Add(_translator.GetTranslation("DIRECTORIES.DIRECTORY_EXISTS", lang));
                    return BadRequest(new { errors = errorMessages });
                }

            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetAllDirectories()
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();

            try
            {
                var dirs = _directoryRepository.GetDirectories();
                return Ok(new { directories = dirs });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("physical")]
        public IActionResult GetPhysicalDirectoriesIn([FromQuery] string path)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                var dirPath = $"{webRootPath}\\appData\\{path}";

                var dirs = Directory.GetDirectories(dirPath);

                var dirsPaths = new List<DirectoryDto>();
                foreach(var dir in dirs)
                {
                    dirsPaths.Add(
                        new DirectoryDto()
                        {
                            Name = dir.Replace($"{dirPath}\\", ""),
                            Path = dir.Replace($"{webRootPath}\\appData\\", "")
                        });
                }


                return Ok(new { physical_directories = dirsPaths });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("directory")]
        public IActionResult GetDirectoryByPath([FromQuery] string path)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var dir = _directoryRepository.FindByPath(path);
                if (dir == null)
                    return NotFound();

                return Ok(new { directory = dir });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete")]
        public IActionResult DeleteDirectory([FromQuery] int dirId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var dir = _directoryRepository.FindById(dirId);
                if (dir == null)
                    return NotFound();

                var path = Path.Combine(_webHostEnvironment.WebRootPath, "appData");
                var fullPath = Path.Combine(path, dir.Path);

                if(Directory.Exists(fullPath))
                {
                    Directory.Delete(fullPath);

                    var deletedDir = _directoryRepository.Delete(dirId);

                    return Ok(new { deletedDirId = deletedDir.Id });
                }


                errorMessages.Add(_translator.GetTranslation("NOT_FOUND", lang));
                return BadRequest(new { errors = errorMessages });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }
    }
}