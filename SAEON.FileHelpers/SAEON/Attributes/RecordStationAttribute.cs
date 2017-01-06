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
    public sealed class RecordStationAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid StationId;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stationId"></param>
        public RecordStationAttribute(string stationId)
        {
            if (stationId.Length == 0 || !Guid.TryParse(stationId, out StationId))
                throw new BadUsageException("The Station Id attribute must be a GUID");
        }
    }
}
