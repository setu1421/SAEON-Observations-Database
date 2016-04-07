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
    public sealed class FixedTimeAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string timeSpan; 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timespan"></param>
        public FixedTimeAttribute(string timespan)
        {
            try
            {
                TimeSpan tm;
                if (!TimeSpan.TryParse(timespan,out tm))
                    throw new Exception("The timespan is not valid for the ValueTime timespan");

                timeSpan = timespan;
            }
            catch
            {
                throw new Exception("The timespan: '" + timespan + " is invalid for the FixedTime Attribute.");
            }
        }
    }
}
