using SAEON.Observations.Core;
using System.Collections.Generic;
using System.Linq;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    public class VariablesController : InternalApiListController<VariableNode>
    {
        protected override List<VariableNode> GetList()
        {
            var result = base.GetList();
            VariableNode phenomenon = null;
            VariableNode offering = null;
            VariableNode unit = null;
            foreach (var variable in DbContext.VFeatures.OrderBy(i => i.PhenomenonName).ThenBy(i => i.OfferingName).ThenBy(i => i.UnitName))
            {
                if (phenomenon?.Id != variable.PhenomenonID)
                {
                    offering = null;
                    unit = null;
                    phenomenon = new VariableNode
                    {
                        Id = variable.PhenomenonID,
                        Key = $"PHE~{variable.PhenomenonID}",
                        Text = variable.PhenomenonName,
                        Name = $"{variable.PhenomenonName}",
                        ToolTip = new LinkAttribute("Phenomenon"),
                        HasChildren = true
                    };
                    result.Add(phenomenon);
                }
                if (offering?.Id != variable.PhenomenonOfferingID)
                {
                    unit = null;
                    offering = new VariableNode
                    {
                        Id = variable.OfferingID,
                        ParentId = phenomenon.Id,
                        Key = $"OFF~{variable.OfferingID}|{phenomenon.Key}",
                        ParentKey = phenomenon.Key,
                        Text = variable.OfferingName,
                        Name = $"{phenomenon.Text} | {variable.OfferingName}",
                        ToolTip = new LinkAttribute("Offering"),
                        HasChildren = true
                    };
                    result.Add(offering);
                }
                if (unit?.Id != variable.PhenomenonUnitID)
                {
                    unit = new VariableNode
                    {
                        Id = variable.UnitID,
                        ParentId = offering.Id,
                        Key = $"UNI~{variable.UnitID}|{offering.Key}",
                        ParentKey = offering.Key,
                        Text = variable.UnitName,
                        Name = $"{phenomenon.Text} | {offering.Text} | {variable.UnitName}",
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
