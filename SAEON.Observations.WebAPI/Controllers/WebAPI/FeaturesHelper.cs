using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    public static class FeaturesHelper
    {
        public static IQueryable<Feature> GetFeatures(ObservationsDbContext db)
        {
            using (Logging.MethodCall<Feature>(typeof(FeaturesHelper)))
            {
                try
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    var result = new List<Feature>();
                    foreach (var phenomenon in db.Phenomena
                        .Include(i => i.PhenomenonOfferings.Select(po => po.Offering))
                        .Where(i => i.PhenomenonOfferings.Any())
                        .OrderBy(i => i.Name))
                    {
                        var phenomenonNode = new Feature
                        {
                            Id = phenomenon.Id,
                            Key = $"PHE~{phenomenon.Id}~",
                            Text = phenomenon.Name,
                            HasChildren = phenomenon.PhenomenonOfferings.Any()
                        };
                        result.Add(phenomenonNode);
                        foreach (var phenomeononOffering in phenomenon.PhenomenonOfferings.OrderBy(po => po.Offering.Name))
                        {
                            var offeringNode = new Feature
                            {
                                Id = phenomeononOffering.Id,
                                ParentId = phenomenonNode.Id,
                                Key = $"OFF~{phenomeononOffering.Id}~{phenomenonNode.Id}",
                                ParentKey = phenomenonNode.Key,
                                Text = phenomeononOffering.Offering.Name,
                                Name = $"{phenomenon.Name} - {phenomeononOffering.Offering.Name}",
                                HasChildren = false
                            };
                            result.Add(offeringNode);
                        }
                    }
                    return result.AsQueryable();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get Features");
                    throw;
                }
            }
        }

    }
}