using SubSonic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observations.Data
{
    public partial class TransformationType : ActiveRecord<TransformationType>, IActiveRecord, IRecordBase
    {
        public const string RatingTable = "538d1697-c817-4644-8b0d-5100a4acc451";
        public const string QualityControlValues = "9ca36c10-cbad-4862-9f28-591acab31237";
        public const string CorrectionValues = "4b850c72-d15f-4bf4-a99a-a5bd3de13349";
    }
}
