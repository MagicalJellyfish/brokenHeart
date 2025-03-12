using brokenHeart.Models;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Services.DataTransfer.Projection;
using brokenHeart.Services.DataTransfer.Save.Entities;
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
        private readonly IElementSubmissionService _elementSubmissionService;

        public ElementController(
            IElementRetrievalService elementProjectionService,
            IElementSubmissionService elementSubmissionService
        )
        {
            _elementProjectionService = elementProjectionService;
            _elementSubmissionService = elementSubmissionService;
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

        [HttpPost("{type}")]
        public ActionResult<int> CreateElement(ElementType type, ElementCreate elementCreate)
        {
            ExecutionResult<int> result = _elementSubmissionService.CreateElement(
                type,
                elementCreate.ParentType,
                elementCreate.ParentId
            );

            if (!result.Succeeded)
            {
                return StatusCode((int)result.StatusCode, result.Message);
            }

            return Ok(result.Value);
        }

        [HttpPut("reorder/{type}")]
        public ActionResult ReorderElements(ElementType type, List<ElementReorder> elementReorder)
        {
            _elementSubmissionService.ReorderElements(type, elementReorder);

            return Ok();
        }

        [HttpPut("{type}/{id}")]
        public ActionResult UpdateElement(
            ElementType type,
            int id,
            List<ElementUpdate> elementUpdate
        )
        {
            _elementSubmissionService.UpdateElement(type, id, elementUpdate);

            return Ok();
        }

        [HttpDelete("{type}/{id}")]
        public ActionResult DeleteElement(ElementType type, int id)
        {
            if (type == ElementType.InjuryEffect)
            {
                return BadRequest("Cannot delete injury effects");
            }

            _elementSubmissionService.DeleteElement(type, id);

            return Ok();
        }
    }
}
