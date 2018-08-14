using SAEON.Observations.Core;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    [RoutePrefix("Internal/Features")]
    public class FeaturesController : BaseController<FeatureNode>
    {
        protected override List<FeatureNode> GetList()
        {
            var result = base.GetList();
            FeatureNode phenomenon = null;
            FeatureNode offering = null;
            FeatureNode unit = null;
            foreach (var feature in db.Features.OrderBy(i => i.PhenomenonName).ThenBy(i => i.OfferingName).ThenBy(i => i.UnitName))
            {
                if (phenomenon?.Id != feature.PhenomenonID)
                {
                    offering = null;
                    unit = null;
                    phenomenon = new FeatureNode
                    {
                        Id = feature.PhenomenonID,
                        Key = $"PHE~{feature.PhenomenonID}~",
                        Text = feature.PhenomenonName,
                        Name = $"{feature.PhenomenonName}",
                        ToolTip = new LinkAttribute("Phenomenon"),
                        HasChildren = true
                    };
                    result.Add(phenomenon);
                }
                if (offering?.Id != feature.PhenomenonOfferingID)
                {
                    unit = null;
                    offering = new FeatureNode
                    {
                        Id = feature.PhenomenonOfferingID,
                        ParentId = phenomenon.Id,
                        Key = $"OFF~{feature.PhenomenonOfferingID}~{phenomenon.Key}",
                        ParentKey = phenomenon.Key,
                        Text = feature.OfferingName,
                        Name = $"{phenomenon.Name} - {feature.OfferingName}",
                        ToolTip = new LinkAttribute("Offering"),
                        HasChildren = true
                    };
                    result.Add(offering);
                }
                if (unit?.Id != feature.PhenomenonUnitID)
                {
                    unit = new FeatureNode
                    {
                        Id = feature.PhenomenonUnitID,
                        ParentId = offering.Id,
                        Key = $"UNI~{feature.PhenomenonUnitID}~{offering.Key}",
                        ParentKey = offering.Key,
                        Text = feature.UnitName,
                        Name = $"{phenomenon.Name} - {offering.Name} - {feature.UnitName}",
                        ToolTip = new LinkAttribute("Unit"),
                        HasChildren = false
                    };
                    result.Add(unit);
                }
            }
            return result;
        }
    }
}
