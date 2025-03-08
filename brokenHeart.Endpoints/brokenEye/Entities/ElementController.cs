using brokenHeart.Models.DataTransfer;
using brokenHeart.Services.DataTransfer.Projection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace brokenHeart.Endpoints.brokenEye.Entities
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ElementController : ControllerBase
    {
        private readonly IElementRetrievalService _elementProjectionService;

        public ElementController(IElementRetrievalService elementProjectionService)
        {
            _elementProjectionService = elementProjectionService;
        }

        [HttpGet("{type}/{id}")]
        public ActionResult<dynamic> GetElement(ElementType type, int id)
        {
            dynamic? element = _elementProjectionService.GetElement(type, id);

            if (element == null)
            {
                return NotFound($"Could not find a element of type {type.ToString()} with ID {id}");
            }

            return Ok(element);
        }
    }
}
