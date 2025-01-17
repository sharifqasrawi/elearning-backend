﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using E_Learning.Helpers;
using E_Learning.Models;
using E_Learning.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace E_Learning.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUploadedFileRepository _uploadedFileRepository;
        private readonly IDirectoryRepository _directoryRepository;
        private readonly ITranslator _translator;
        public FilesController(IWebHostEnvironment webHostEnvironment,
                                IDirectoryRepository directoryRepository,
                                 IUploadedFileRepository uploadedFileRepository,
                                 ITranslator translator)
        {
            _webHostEnvironment = webHostEnvironment;
            _directoryRepository = directoryRepository;
            _uploadedFileRepository = uploadedFileRepository;
            _translator = translator;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetUploadedFiles()
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var uploadedFiles = _uploadedFileRepository.GetUploadedFiles();
                    
                return Ok(new { uploadedFiles });
            }
            catch 
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("upload"), DisableRequestSizeLimit]
        public IActionResult UploadFile([FromQuery] string directory)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            try
            {
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "appData", directory);
                var dir = _directoryRepository.FindByPath(directory);

                foreach (var file in Request.Form.Files)
                {
                    if (file.Length > 0)
                    {
                        string originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                        string fileName =  $"_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}"
                               + Path.GetExtension(originalFileName);

                        string fullPath = Path.Combine(path, fileName);
                       
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        //var serverUrl = "https://localhost:44383";
                        var serverUrl = "https://qasrawi.fr";


                        var newFile = new UploadedFile()
                        {
                            UploadDirectory = dir,
                            DownloadPath = Path.Combine(serverUrl, "appData", dir.Path, fileName).Replace("\\", "/"),
                            UploadDateTime = DateTime.Now,
                            FileType = Path.GetExtension(originalFileName),
                            OriginalFileName = originalFileName,
                            ModifiedFileName = fileName,
                            UploadPath = Path.Combine(dir.Path, fileName)

                        };
                        var createdFile = _uploadedFileRepository.Create(newFile);
                    }
                }
               

                return Ok(new { status = "File Uploaded" });
            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete")]
        public IActionResult DeleteFile([FromQuery] int fileId)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();

            try
            {
                var file = _uploadedFileRepository.FindById(fileId);
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "appData");
                var uploadPath = file.UploadPath;

                var fullPath = Path.Combine(path, uploadPath);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);

                    var deletedFile = _uploadedFileRepository.Delete(fileId);

                    return Ok(new { deletedFileId = deletedFile.Id });
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