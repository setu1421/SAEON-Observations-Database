using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileHelpers
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class PhenomenonAttribute : Attribute
    {
        public Guid PhenomenonId;

        /// <summary>
        /// Unique Identifier of Phenomenon
        /// </summary>
        /// <param name="_val"></param>
        public PhenomenonAttribute(string phenomenonId)
        {
            if (phenomenonId.Length == 0 || !Guid.TryParse(phenomenonId,out PhenomenonId))
                throw new BadUsageException("The Phenomenon attribute must be a GUID");
        }
    }
}
