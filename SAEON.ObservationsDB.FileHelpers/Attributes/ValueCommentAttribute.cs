using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileHelpers
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ValueCommentAttribute : Attribute
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="emptyValue"></param>
        public ValueCommentAttribute()
        {
        }
    }
}
