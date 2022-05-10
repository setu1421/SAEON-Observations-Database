﻿using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Authorization;
using SAEON.AspNet.Auth;
using SAEON.Logs;
using SAEON.Observations.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    [ODataRoutePrefix("Stations")]
    public class StationsController : CodedNamedODataController<Station>
    {
        [ODataRoute("({id})/Site")]
        public Site GetSite(Guid id)
        {
            return GetSingle(id, s => s.Site);
        }

        [ODataRoute("({id})/Organisations")]
        public IQueryable<Organisation> GetOrganisations(Guid id)
        {
            var site = GetSingle(id, s => s.Site);
            var siteOrganiations = DbContext.Sites.Where(i => i.Id == site.Id).SelectMany(i => i.Organisations);
            return GetManyWithGuidId(id, s => s.Organisations).Union(siteOrganiations); ;
        }

        [ODataRoute("({id})/Projects")]
        public IQueryable<Project> GetProjects(Guid id)
        {
            return GetManyWithGuidId(id, s => s.Projects);
        }

        [ODataRoute("({id})/Instruments")]
        public IQueryable<Instrument> GetInstruments(Guid id)
        {
            return GetManyWithGuidId(id, s => s.Instruments);
        }

        //@@@@
        //[ODataRoute("({id})/Datasets")]
        //public IQueryable<Dataset> GetDatasets(Guid id)
        //{
        //    return GetManyWithLongId(id, s => s.Datasets);
        //}

        [ODataRoute("({id})/Observations")]
        [Authorize(Policy = ODPAuthenticationDefaults.IdTokenPolicy)]
        public async Task<IQueryable<Observation>> GetObservations(Guid id)
        {
            var result = GetManyWithIntId<Observation>(id, s => s.Observations).Take(100); //@@@
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
    }
}
