using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net.Mime;

namespace SAEON.Observations.WebAPI.Controllers.Metadata
{
    public class MetadataController : BaseController
    {
        [Route("{id:int}")]
        public IActionResult DOI(int id)
        {
            var model = DbContext.DigitalObjectIdentifiers.SingleOrDefault(i => i.Id == id);
            if (model == null)
            {
                return NotFound(id);
            }
            return View(model);
        }

        [Route("{id:int}")]
        public IActionResult AsJson(int id)
        {
            var model = DbContext.DigitalObjectIdentifiers.SingleOrDefault(i => i.Id == id);
            if (model == null)
            {
                return NotFound(id);
            }
            return Content(model.MetadataJson, MediaTypeNames.Application.Json);
        }

        [Route("{id:int}")]
        public IActionResult AsHtml(int id)
        {
            var model = DbContext.DigitalObjectIdentifiers.SingleOrDefault(i => i.Id == id);
            if (model == null)
            {
                return NotFound(id);
            }
            return Content(model.MetadataHtml, MediaTypeNames.Text.Html);
        }

    }
}