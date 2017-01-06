using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileHelpers
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RecordPhenomenonAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid PhenomenonId;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phenomenonId"></param>
        public RecordPhenomenonAttribute(string phenomenonId)
        {
            if (phenomenonId.Length == 0 || !Guid.TryParse(phenomenonId,out PhenomenonId))
                throw new BadUsageException("The Phenomenon attribute must be a GUID");
        }
    }
}
