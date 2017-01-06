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
    public sealed class RecordSensorAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid SensorProcedureId;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stationId"></param>
        public RecordSensorAttribute(string sensorProcedureId)
        {
            if (sensorProcedureId.Length == 0 || !Guid.TryParse(sensorProcedureId, out SensorProcedureId))
                throw new BadUsageException("The Sensor Procedure Id attribute must be a GUID");
        }
    }
}
