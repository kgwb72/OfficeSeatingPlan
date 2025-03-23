using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OfficeSeatingPlan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // Require authentication for all endpoints
    public class FileUploadController : ControllerBase
    {
        private readonly ILogger<FileUploadController> _logger;
        private readonly string _uploadsFolder;

        public FileUploadController(ILogger<FileUploadController> logger)
        {
            _logger = logger;

            // Create uploads directory if it doesn't exist
            _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (!Directory.Exists(_uploadsFolder))
            {
                Directory.CreateDirectory(_uploadsFolder);
            }
        }

        [HttpPost("svg")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UploadSvg([FromForm] FileUploadModel model)
        {
            try
            {
                if (model.File == null || model.File.Length == 0)
                {
                    return BadRequest(new { message = "No file was uploaded" });
                }

                // Validate the file is an SVG
                if (!model.File.ContentType.Equals("image/svg+xml") &&
                    !Path.GetExtension(model.File.FileName).Equals(".svg", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { message = "The file must be an SVG" });
                }

                // Create layout directory if needed
                var layoutFolder = Path.Combine(_uploadsFolder, "layouts");
                if (!Directory.Exists(layoutFolder))
                {
                    Directory.CreateDirectory(layoutFolder);
                }

                // Generate file name based on layout/building ID and timestamp
                var layoutId = model.LayoutId > 0 ? model.LayoutId.ToString() : "default";
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var fileName = $"layout_{layoutId}_{timestamp}.svg";
                var filePath = Path.Combine(layoutFolder, fileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }

                // Return the file path and URL
                var fileUrl = $"/api/FileUpload/layouts/{fileName}";
                return Ok(new { fileName, filePath, url = fileUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading SVG file");
                return StatusCode(500, new { message = "An error occurred while uploading the file" });
            }
        }

        [HttpPost("panorama")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UploadPanorama([FromForm] FileUploadModel model)
        {
            try
            {
                if (model.File == null || model.File.Length == 0)
                {
                    return BadRequest(new { message = "No file was uploaded" });
                }

                // Validate the file is an image
                if (!model.File.ContentType.StartsWith("image/"))
                {
                    return BadRequest(new { message = "The file must be an image" });
                }

                // Create panorama directory if needed
                var panoramaFolder = Path.Combine(_uploadsFolder, "panoramas");
                if (!Directory.Exists(panoramaFolder))
                {
                    Directory.CreateDirectory(panoramaFolder);
                }

                // Generate file name based on layout/building ID and timestamp
                var layoutId = model.LayoutId > 0 ? model.LayoutId.ToString() : "default";
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var extension = Path.GetExtension(model.File.FileName);
                var fileName = $"panorama_{layoutId}_{timestamp}{extension}";
                var filePath = Path.Combine(panoramaFolder, fileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }

                // Return the file path and URL
                var fileUrl = $"/api/FileUpload/panoramas/{fileName}";
                return Ok(new { fileName, filePath, url = fileUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading panorama file");
                return StatusCode(500, new { message = "An error occurred while uploading the file" });
            }
        }

        [HttpGet("layouts/{fileName}")]
        [AllowAnonymous]  // Allow anonymous access for file retrieval
        public IActionResult GetLayoutFile(string fileName)
        {
            var filePath = Path.Combine(_uploadsFolder, "layouts", fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            // Return the SVG file with appropriate content type
            return PhysicalFile(filePath, "image/svg+xml");
        }

        [HttpGet("panoramas/{fileName}")]
        [AllowAnonymous]  // Allow anonymous access for file retrieval
        public IActionResult GetPanoramaFile(string fileName)
        {
            var filePath = Path.Combine(_uploadsFolder, "panoramas", fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            // Determine content type based on file extension
            var contentType = "image/jpeg";  // Default
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    contentType = "image/jpeg";
                    break;
                case ".png":
                    contentType = "image/png";
                    break;
                case ".gif":
                    contentType = "image/gif";
                    break;
                case ".webp":
                    contentType = "image/webp";
                    break;
            }

            return PhysicalFile(filePath, contentType);
        }
    }

    public class FileUploadModel
    {
        public Microsoft.AspNetCore.Http.IFormFile File { get; set; }
        public int LayoutId { get; set; }
        public string Description { get; set; }
    }
}