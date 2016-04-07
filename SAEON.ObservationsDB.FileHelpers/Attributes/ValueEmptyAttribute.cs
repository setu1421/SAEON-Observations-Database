using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileHelpers
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ValueEmptyAttribute : Attribute
    {
        public string EmptyValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emptyValue"></param>
        public ValueEmptyAttribute(string emptyValue)
        {
            if (string.IsNullOrEmpty(emptyValue))
                throw new Exception("The Empty value for the ValueEmpty Attribute cannot be null");

            EmptyValue = emptyValue;
        }
    }
}
