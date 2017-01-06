using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileHelpers.Dynamic
{
    partial class FieldBuilder
    {
        private bool mValueDate;
        public bool ValueDate
        {
            get { return mValueDate; }
            set { mValueDate = value; }
        }

        private string mValueDateformat;
        public string ValueDateformat
        {
            get { return mValueDateformat; }
            set { mValueDateformat = value; }
        }


        private bool mValueTime;
        public bool ValueTime
        {
            get { return mValueTime; }
            set { mValueTime = value; }
        }

        private string mValueTimeformat;
        public string ValueTimeformat
        {
            get { return mValueTimeformat; }
            set { mValueTimeformat = value; }
        }

        private bool mValueEmpty;
        public bool ValueEmpty
        {
            get { return mValueEmpty; }
            set { mValueEmpty = value; }
        }

        private string mEmptyValue;
        public string EmptyValue
        {
            get { return mEmptyValue; }
            set { mEmptyValue = value; }
        }


        private Guid? mphenomenonOfferingID;
        public Guid? PhenomenonOfferingID
        {
            get { return mphenomenonOfferingID; }
            set { mphenomenonOfferingID = value; }
        }

        private Guid? mphenomenonUOMID;
        public Guid? PhenomenonUOMID
        {
            get { return mphenomenonUOMID; }
            set { mphenomenonUOMID = value; }
        }


        private string mFixedTime;
        public string FixedTime
        {
            get { return mFixedTime; }
            set { mFixedTime = value; }
        }

        private bool mValueComment;
        public bool ValueComment
        {
            get { return mValueComment; }
            set { mValueComment = value; }
        }


    }
}
