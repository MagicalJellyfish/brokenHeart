using Microsoft.AspNetCore.Mvc;

namespace brokenHeart.Endpoints.brokenEye
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]
    public class ImageController : ControllerBase
    {
        public ImageController() { }

        [HttpGet("character/{id}")]
        public ActionResult GetImage(int id)
        {
            List<string> images = Directory.GetFiles("Images").ToList();

            string? targetFileName = null;
            string? targetFileExtension = null;
            foreach (string image in images)
            {
                string[] targetFile = image.Split('\\').Last().Split('.');
                if (targetFile.First() == id.ToString())
                {
                    targetFileName = targetFile[0];
                    targetFileExtension = targetFile.Last();
                }
            }

            if (targetFileName == null)
            {
                return NotFound("Could not find requested image.");
            }

            return File(
                System.IO.File.OpenRead($"Images/{targetFileName}.{targetFileExtension}"),
                $"image/{targetFileExtension}"
            );
        }
    }
}
