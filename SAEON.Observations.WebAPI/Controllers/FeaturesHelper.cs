using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SAEON.Observations.WebAPI.Controllers
{
    public static class FeaturesHelper
    {
        public static IQueryable<Feature> GetFeatures(ObservationsDbContext db)
        {
            using (Logging.MethodCall<Feature>(typeof(FeaturesHelper)))
            {
                try
                {
                    var result = new List<Feature>();
                    foreach (var phenomenon in db.Phenomena.Include(i => i.Offerings)/*.Where(i => i.HasOfferings)*/.OrderBy(i => i.Name))
                    {
                        var phenomenonNode = new Feature
                        {
                            Id = phenomenon.Id,
                            Key = $"PHE~{phenomenon.Id}~",
                            Text = phenomenon.Name,
                            HasChildren = phenomenon.HasOfferings
                        };
                        result.Add(phenomenonNode);
                        foreach (var offering in phenomenon.Offerings.OrderBy(i => i.Name))
                        {
                            var offeringNode = new Feature
                            {
                                Id = offering.Id,
                                ParentId = phenomenonNode.Id,
                                Key = $"OFF~{offering.Id}~{phenomenonNode.Id}",
                                ParentKey = phenomenonNode.Key,
                                Text = offering.Name,
                                Name = $"{phenomenon.Name} - {offering.Name}",
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