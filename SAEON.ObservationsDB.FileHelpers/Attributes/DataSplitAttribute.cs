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
    public sealed class DataSplitAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public String SplitSelector;

        /// <summary>
        /// 
        /// </summary>
        public int  SplitIndex;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="splitSelector"></param>
        /// <param name="splitIndex"></param>
        public DataSplitAttribute(string splitSelector, int splitIndex)
        {
            if (String.IsNullOrEmpty(splitSelector))
                throw new Exception("The split Selector value cannot be empty");
            else
                this.SplitSelector = splitSelector;


            if (SplitIndex <= 0)
                throw new Exception("The split Index of the DataSplit Attribute must be greater than 0");
            else
                this.SplitIndex = splitIndex;
        }
    }
}
