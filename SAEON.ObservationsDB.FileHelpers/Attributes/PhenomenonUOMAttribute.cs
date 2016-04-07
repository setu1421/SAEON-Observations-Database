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
    public sealed class PhenomenonUOMAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid phenomenonUOMId;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phenomenonofferingid"></param>
        public PhenomenonUOMAttribute(string phenomenonuomid)
        {
            if (phenomenonuomid.Length == 0 || !Guid.TryParse(phenomenonuomid, out phenomenonUOMId))
                throw new Exception("The Phenomenon Unit of Measure attribute must be a GUID");
        }
    }
}
