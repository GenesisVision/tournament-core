using GenesisVision.Tournament.Core.Models;
using GenesisVision.Tournament.Core.ViewModels.Files;
using HeyRed.Mime;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GenesisVision.Tournament.Core.Controllers
{
	[Route("api/files")]
    public class FilesController : BaseController
    {
        private readonly string uploadPath;

        public FilesController(IHostingEnvironment environment)
        {
            uploadPath = Path.Combine(environment.WebRootPath, "uploads");
        }

        /// <summary>
        /// Upload file
        /// </summary>
        [HttpPost]
        [Route("upload")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(UploadResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public async Task<IActionResult> UploadFile(IFormFile uploadedFile)
        {
            if (uploadedFile == null || uploadedFile.Length == 0)
                return BadRequest(ErrorResult.GetResult(new List<string> {"File is empty"}));
            
            var fileName = Guid.NewGuid() + (uploadedFile.FileName.Contains(".")
                               ? uploadedFile.FileName.Substring(uploadedFile.FileName.LastIndexOf(".", StringComparison.Ordinal))
                               : "");
            var path = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await uploadedFile.CopyToAsync(stream);
            }

            return Ok(new UploadResult {FileName = fileName});
        }

        /// <summary>
        /// Download file
        /// </summary>
        [HttpGet]
        [Route("get")]
        public FileResult Get(string fileName)
        {
            var path = Path.Combine(uploadPath, fileName);
            if (!System.IO.File.Exists(path))
                return File(new byte[0], "");
            
            var type = MimeGuesser.GuessMimeType(path);
            return File(System.IO.File.ReadAllBytes(path), type);
        }
    }
}
