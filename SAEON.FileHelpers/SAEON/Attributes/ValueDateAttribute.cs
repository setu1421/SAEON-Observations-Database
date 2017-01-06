using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileHelpers
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ValueDateAttribute : Attribute
    {
        public string Format; 

        /// <summary>
        /// 
        /// </summary>
        public ValueDateAttribute(string format)
        {
            try
            {
                if(string.IsNullOrEmpty(format))
                    throw new BadUsageException("The format cannot may not be empty for the ValueDate Attribute");

                DateTime.Now.ToString(format);

                Format = format;
            }
            catch
            {
                throw new BadUsageException("The format: '" + format + " is invalid for the ValueDate Attribute.");
            }
        }
    }
}
