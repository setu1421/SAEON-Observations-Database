using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Diagnostics;
using SAEON.AspNet.Auth;
using SAEON.Logs;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    public class DatasetsController : CodedNamedApiController<VDatasetExpansion>
    {
        /// <summary>
        /// All Datasets
        /// </summary>
        /// <returns></returns>
        public override Task<IEnumerable<VDatasetExpansion>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Dataset by Id
        /// </summary>
        /// <param name="id">The Id of the Dataset</param>
        /// <returns>Dataset</returns>
        public override Task<ActionResult<VDatasetExpansion>> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }

        /// <summary>
        /// Dataset by Code
        /// </summary>
        /// <param name="code">The Code of the Dataset</param>
        /// <returns>Dataset</returns>
        public override Task<ActionResult<VDatasetExpansion>> GetByCodeAsync(string code)
        {
            return base.GetByCodeAsync(code);
        }

        /// <summary>
        /// Dataset by Name
        /// </summary>
        /// <param name="name">The Name of the Dataset</param>
        /// <returns>Dataset</returns>
        public override Task<ActionResult<VDatasetExpansion>> GetByNameAsync(string name)
        {
            return base.GetByNameAsync(name);
        }

        /// <summary>
        /// Station of the Dataset
        /// </summary>
        /// <param name="id">The Id of the Dataset</param>
        /// <returns>Site</returns>
        [HttpGet("{id:guid}/Station")]
        public async Task<ActionResult<Station>> GetStationAsync(Guid id)
        {
            return await GetSingleAsync<Station>(id, s => s.Station);
        }

        /// <summary>
        /// Phenomenon of the Dataset
        /// </summary>
        /// <param name="id">The Id of the Dataset</param>
        /// <returns>Site</returns>
        [HttpGet("{id:guid}/Phenomenon")]
        public async Task<ActionResult<Phenomenon>> GetPhenomenonAsync(Guid id)
        {
            return await GetSingleAsync<Phenomenon>(id, s => s.Phenomenon);
        }

        /// <summary>
        /// Offering of the Dataset
        /// </summary>
        /// <param name="id">The Id of the Dataset</param>
        /// <returns>Site</returns>
        [HttpGet("{id:guid}/Offering")]
        public async Task<ActionResult<Offering>> GetOfferingAsync(Guid id)
        {
            return await GetSingleAsync<Offering>(id, s => s.Offering);
        }

        /// <summary>
        /// Unit of the Dataset
        /// </summary>
        /// <param name="id">The Id of the Dataset</param>
        /// <returns>Site</returns>
        [HttpGet("{id:guid}/Unit")]
        public async Task<ActionResult<Unit>> GetUnitAsync(Guid id)
        {
            return await GetSingleAsync<Unit>(id, s => s.Unit);
        }

        /// <summary>
        /// Observations of the Dataset
        /// </summary>
        /// <param name="id">Id of the Dataset</param>
        /// <returns>ListOf(DataStream)</returns>
        [HttpGet("{id:guid}/Observations")]
        [Authorize(Policy = ODPAuthenticationDefaults.IdTokenPolicy)]
        public async Task<List<ObservationDTO>> GetObservations(Guid id)
        {
            using (SAEONLogs.MethodCall<VDatasetExpansion, ObservationDTO>(GetType()))
            {
                try
                {
                    UpdateRequest();
                    if (!await GetQuery(i => (i.Id == id)).AnyAsync())
                    {
                        SAEONLogs.Error("{id} not found", id);
                        throw new ArgumentException($"{id} not found");
                    }
                    var result = new List<ObservationDTO>();
                    var fileType = DatasetFileTypes.CSV;
                    //if (Request.Headers.TryGetValue("Accept", out var acceptHeaders))
                    //{
                    //    var value = acceptHeaders.FirstOrDefault().ToLowerInvariant();
                    //    if (value == AspNetConstants.ContentTypeExcel.ToLowerInvariant())
                    //    {
                    //        fileType = DatasetFileTypes.NetCDF;
                    //    }
                    //    else if (value == AspNetConstants.ContentTypeNetCDF.ToLowerInvariant())
                    //    {
                    //        fileType = DatasetFileTypes.NetCDF;
                    //    }
                    //}
                    result.AddRange(await DatasetHelper.LoadAsync(DbContext, Config, id, fileType));
                    //SAEONLogs.Information("Obs: {@Observation}", result.FirstOrDefault());
                    try
                    {
                        await RequestLogger.LogAsync(DbContext, Request);
                    }
                    catch (Exception ex)
                    {
                        SAEONLogs.Exception(ex);
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }

        /// <summary>
        /// Observations of the Datatset over ranges
        /// </summary>
        /// <param name="id">Id of the Dataset</param>
        /// <param name="input">Input of the range of observations</param>
        /// <returns>ListOf(Observation)</returns>
        [HttpPost("{id:guid}/Observations")]
        [Authorize(Policy = ODPAuthenticationDefaults.IdTokenPolicy)]
        public async Task<List<ObservationDTO>> GetObservationsRanges(Guid id, [FromBody] ObservationInput input)
        {
            using (SAEONLogs.MethodCall<VDatasetExpansion, ObservationDTO>(GetType()))
            {
                try
                {
                    UpdateRequest();
                    if (!await GetQuery(i => (i.Id == id)).AnyAsync())
                    {
                        SAEONLogs.Error("{id} not found", id);
                        throw new ArgumentException($"{id} not found");
                    }
                    Guard.IsNotNull(input, nameof(input));

                    var result = new List<ObservationDTO>();
                    var fileType = DatasetFileTypes.CSV;
                    //if (Request.Headers.TryGetValue("Accept", out var acceptHeaders))
                    //{
                    //    var value = acceptHeaders.FirstOrDefault().ToLowerInvariant();
                    //    if (value == AspNetConstants.ContentTypeExcel.ToLowerInvariant())
                    //    {
                    //        fileType = DatasetFileTypes.NetCDF;
                    //    }
                    //    else if (value == AspNetConstants.ContentTypeNetCDF.ToLowerInvariant())
                    //    {
                    //        fileType = DatasetFileTypes.NetCDF;
                    //    }
                    //}
                    result.AddRange(await DatasetHelper.LoadAsync(DbContext, Config, id, fileType));
                    if (input.StartDate.HasValue)
                    {
                        result.RemoveAll(i => i.Date < input.StartDate);
                    }
                    if (input.EndDate.HasValue)
                    {
                        result.RemoveAll(i => i.Date > input.EndDate);
                    }
                    if (input.ElevationMinimum.HasValue)
                    {
                        result.RemoveAll(i => i.Elevation < input.ElevationMinimum);
                    }
                    if (input.ElevationMaximum.HasValue)
                    {
                        result.RemoveAll(i => i.Elevation > input.ElevationMaximum);
                    }
                    try
                    {
                        await RequestLogger.LogAsync(DbContext, Request);
                    }
                    catch (Exception ex)
                    {
                        SAEONLogs.Exception(ex);
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }
    }
}
