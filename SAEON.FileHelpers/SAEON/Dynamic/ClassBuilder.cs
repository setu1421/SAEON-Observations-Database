using FileHelpers.Dynamic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileHelpers.Dynamic
{
    partial class ClassBuilder
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mSplitSelector;

        /// <summary>
        /// SplitSelector
        /// </summary>
        public string SplitSelector
        {
            get { return mSplitSelector; }
            set { mSplitSelector = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mSplitIndex;
        /// <summary>
        /// SlitIndex
        /// </summary>
        public int SplitIndex
        {
            get { return mSplitIndex; }
            set { mSplitIndex = value; }
        }

    }
}
