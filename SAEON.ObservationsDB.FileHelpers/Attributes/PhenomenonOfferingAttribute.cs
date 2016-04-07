using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileHelpers
{

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class PhenomenonOfferingAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid PhenomenonOfferingId;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phenomenonofferingid"></param>
        public PhenomenonOfferingAttribute(string phenomenonofferingid)
        {
            if (phenomenonofferingid.Length == 0 || !Guid.TryParse(phenomenonofferingid, out PhenomenonOfferingId))
                throw new Exception("The Phenomenon Offering attribute must be a GUID");
        }
    }
}
