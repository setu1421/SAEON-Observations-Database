using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAEON.Logs;
using System;
using System.Net.Mime;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    public class DOIController : InternalApiController
    {
        [HttpPost("[action]")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AsJson([FromForm] string doi)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var model = await DbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOI == doi);
                    if (model == null)
                    {
                        SAEONLogs.Error($"DOI {doi} not found");
                        return NotFound($"DOI {doi} not found");
                    }
                    return Content(model.MetadataJson, MediaTypeNames.Application.Json);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpPost("[action]")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AsHtml([FromForm] string doi)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var model = await DbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOI == doi);
                    if (model == null)
                    {
                        SAEONLogs.Error($"DOI {doi} not found");
                        return NotFound($"DOI {doi} not found");
                    }
                    return Content(model.MetadataHtml, MediaTypeNames.Text.Html);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }
    }
}
