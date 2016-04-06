using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.ObservationsDB.Data
{
    public enum ImportBatchStatus
    {
        DatalogWithErrors,
        NoLogErrors,
        MovedToLog,
        Deleted
    }
}
