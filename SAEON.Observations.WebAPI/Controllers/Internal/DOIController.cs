using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAEON.Logs;
using SAEON.Observations.Core;
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
                    var _doi = await DbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOI == doi);
                    if (_doi == null)
                    {
                        SAEONLogs.Error($"DOI {doi} not found");
                        return NotFound($"DOI {doi} not found");
                    }
                    return Content(_doi.MetadataJson, MediaTypeNames.Application.Json);
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
                    var _doi = await DbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOI == doi);
                    if (_doi == null)
                    {
                        SAEONLogs.Error($"DOI {doi} not found");
                        return NotFound($"DOI {doi} not found");
                    }
                    return Content(_doi.MetadataHtml, MediaTypeNames.Text.Html);
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
        public async Task<ActionResult<DataWizardDataInput>> AsQueryInput([FromForm] string doi)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var _doi = await DbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOI == doi);
                    if (_doi == null)
                    {
                        SAEONLogs.Error($"DOI {doi} not found");
                        return NotFound($"DOI {doi} not found");
                    }
                    var codes = _doi.Code.Split("~");
                    var station = await DbContext.Stations.SingleAsync(i => i.Code == codes[0]);
                    var phenomenon = await DbContext.Phenomena.SingleAsync(i => i.Code == codes[1]);
                    var offering = await DbContext.Offerings.SingleAsync(i => i.Code == codes[2]);
                    var unit = await DbContext.Units.SingleAsync(i => i.Code == codes[3]);
                    var input = new DataWizardDataInput();
                    input.Locations.Add(new Location { StationId = station.Id });
                    input.Variables.Add(new Variable { PhenomenonId = phenomenon.Id, OfferingId = offering.Id, UnitId = unit.Id });
                    var dataset = await DbContext.InventoryDatasets.SingleAsync(i => (i.StationId == station.Id) && (i.PhenomenonCode == phenomenon.Code) &&
                        (i.OfferingCode == offering.Code) && (i.UnitCode == unit.Code));
                    if (dataset.StartDate.HasValue) input.StartDate = dataset.StartDate.Value;
                    if (dataset.EndDate.HasValue) input.EndDate = dataset.EndDate.Value;
                    return Ok(input);
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
