using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace brokenHeart.Endpoints.brokenEye
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ImageController : ControllerBase
    {
        public ImageController() { }

        [AllowAnonymous]
        [HttpGet("character/{id}")]
        public ActionResult GetImage(int id)
        {
            FileName? targetFile = GetFilePath(id);

            if (targetFile == null)
            {
                return NotFound("Could not find requested image.");
            }

            return File(
                System.IO.File.OpenRead($"Images/{targetFile.Name}.{targetFile.Extension}"),
                $"image/{targetFile.Extension}"
            );
        }

        [HttpPut("character/{id}")]
        public async Task<ActionResult> PutImage(int id, IFormFile image)
        {
            string fileExtension = Path.GetExtension(image.FileName);

            using (var fileStream = System.IO.File.OpenWrite($"Images/{id}{fileExtension}"))
            {
                await image.CopyToAsync(fileStream);
            }

            return Ok();
        }

        [HttpDelete("character/{id}")]
        public async Task<ActionResult> DeleteImage(int id)
        {
            FileName? targetFile = GetFilePath(id);

            if (targetFile == null)
            {
                return NotFound("Could not find requested image.");
            }

            System.IO.File.Delete($"Images/{targetFile.Name}.{targetFile.Extension}");

            return Ok();
        }

        private FileName? GetFilePath(int id)
        {
            List<string> images = Directory.GetFiles("Images").ToList();

            foreach (string image in images)
            {
                string[] targetFile = image.Split(Path.DirectorySeparatorChar).Last().Split('.');
                if (targetFile.First() == id.ToString())
                {
                    string targetFileName = targetFile[0];
                    string targetFileExtension = targetFile.Last();

                    return new FileName()
                    {
                        Name = targetFileName,
                        Extension = targetFileExtension
                    };
                }
            }

            return null;
        }

        private class FileName
        {
            public string Name { get; set; }
            public string Extension { get; set; }
        }
    }
}
