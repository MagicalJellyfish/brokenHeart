using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Services.DataTransfer.Projection;
using brokenHeart.Services.DataTransfer.Projection.Templates;
using brokenHeart.Services.DataTransfer.Save;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace brokenHeart.Endpoints.brokenEye.Entities
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TemplateController : ControllerBase
    {
        private readonly ITemplateListProjectionService _templateListProjectionService;
        private readonly IElementRetrievalService _elementRetrievalService;
        private readonly IElementSubmissionService _elementSubmissionService;

        public TemplateController(
            ITemplateListProjectionService templateListProjectionService,
            IElementRetrievalService elementRetrievalService,
            IElementSubmissionService elementSubmissionService
        )
        {
            _templateListProjectionService = templateListProjectionService;
            _elementRetrievalService = elementRetrievalService;
            _elementSubmissionService = elementSubmissionService;
        }

        [HttpGet("view")]
        public ActionResult<List<ElementList>> GetTemplateView()
        {
            return Ok(_templateListProjectionService.GetTemplateView());
        }

        [HttpGet("selection/{type}")]
        public ActionResult<List<ElementList>> GetTemplateSelection(ElementType type)
        {
            return Ok(_elementRetrievalService.GetTemplateSelection(type));
        }

        [HttpPatch("{type}/{id}/relate/{parentType}/{parentId}")]
        public ActionResult<int> RelateTemplate(
            ElementType type,
            int id,
            ElementType parentType,
            int parentId
        )
        {
            _elementSubmissionService.RelateTemplate(type, id, parentType, parentId);

            return Ok();
        }

        [HttpPatch("{type}/{id}/unrelate/{parentType}/{parentId}")]
        public ActionResult<int> UnrelateTemplate(
            ElementType type,
            int id,
            ElementType parentType,
            int parentId
        )
        {
            _elementSubmissionService.UnrelateTemplate(type, id, parentType, parentId);

            return Ok();
        }
    }
}
