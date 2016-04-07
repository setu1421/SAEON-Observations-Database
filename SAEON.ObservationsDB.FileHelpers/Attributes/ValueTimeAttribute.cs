using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileHelpers
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ValueTimeAttribute : Attribute
    {
        public string Format; 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        public ValueTimeAttribute(string format)
        {
            try
            {
                if (string.IsNullOrEmpty(format))
                    throw new Exception("The format cannot may not be empty for the ValueTime Attribute");

                DateTime.Now.ToString(format);

                Format = format;
            }
            catch
            {
                throw new Exception("The format: '" + format + " is invalid for the ValueTime Attribute.");
            }
        }
    }
}
