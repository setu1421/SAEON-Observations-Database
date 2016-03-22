using Newtonsoft.Json;
using SubSonic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SAEON.ObservationsDB.Data
{
    public partial class DataSourceTransformation : ActiveRecord<DataSourceTransformation>, IActiveRecord
    {
        private IEnumerable<double> ratingTableValues;
        private Dictionary<string, double> qualityValues;
        private Dictionary<string, string> correctionValues;
        private DataLogCollection colDataLogRecords;

        public IEnumerable<double> RatingTableValues
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Definition) && this.TransformationTypeID.ToString() == "538d1697-c817-4644-8b0d-5100a4acc451")
                {
                    string str = "{" + this.Definition + "}";
                    this.ratingTableValues = (IEnumerable<double>)new List<double>();
                    Dictionary<double, double> dic = JsonConvert.DeserializeObject<Dictionary<double, double>>(str);
                    this.ratingTableValues = Enumerable.Select<double, double>((IEnumerable<double>)Enumerable.OrderBy<double, double>((IEnumerable<double>)dic.Keys, (Func<double, double>)(v => dic[v])), (Func<double, double>)(v => dic[v]));
                }
                return this.ratingTableValues;
            }
        }

        public Dictionary<string, double> QualityValues
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Definition) && this.TransformationTypeID.ToString() == "9ca36c10-cbad-4862-9f28-591acab31237")
                {
                    string input = "{" + this.Definition + "}";
                    this.qualityValues = new Dictionary<string, double>();
                    this.qualityValues = new JavaScriptSerializer().Deserialize<Dictionary<string, double>>(input);
                }
                return this.qualityValues;
            }
        }

        public Dictionary<string, string> CorrectionValues
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Definition) && this.TransformationTypeID.ToString() == "4b850c72-d15f-4bf4-a99a-a5bd3de13349")
                {
                    string input = "{" + this.Definition + "}";
                    this.correctionValues = new Dictionary<string, string>();
                    this.correctionValues = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(input);
                }
                return this.correctionValues;
            }
        }

        public double GetRatingValue(double RawValue)
        {
            double num = RawValue;
            if (!string.IsNullOrEmpty(this.Definition) && this.TransformationTypeID.ToString() == "538d1697-c817-4644-8b0d-5100a4acc451")
            {
                Dictionary<double, double> dictionary = JsonConvert.DeserializeObject<Dictionary<double, double>>("{" + this.Definition + "}");
                List<double> list = Enumerable.ToList<double>((IEnumerable<double>)Enumerable.OrderBy<double, double>((IEnumerable<double>)dictionary.Keys, (Func<double, double>)(v => v)));
                int index = list.BinarySearch(RawValue);
                if (index < 0)
                    index = ~index;
                if (index > list.Count - 1)
                    index = list.Count - 1;
                num = dictionary[list[index]];
            }
            return num;
        }

    }
}
