using da = SAEON.Observations.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace SAEON.Observations.Data
{
    public partial class DataSourceTransformation
    {

        static double Search(List<KeyValuePair<double, double>> list, double key)
        {
            int index = list.BinarySearch(
                new KeyValuePair<double, double>(key, 0),
                new Comparer());

            // Case 1 
            if (index >= 0)
                return list[index].Value;

            // NOTE: if the search fails, List<T>.BinarySearch returns the  
            // bitwise complement of the insertion index that would be used 
            // to keep the list sorted. 
            index = ~index;

            // Case 2 
            if (index == 0 || index == list.Count)
                return 0;

            // Case 3 
            return (list[index - 1].Value + list[index].Value) / 2;
        }

        class Comparer : IComparer<KeyValuePair<double, double>>
        {
            public int Compare(
                KeyValuePair<double, double> x,
                KeyValuePair<double, double> y)
            {
                if (Math.Abs(x.Key - y.Key) < 0)
                    return 0;

                return x.Key.CompareTo(y.Key);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private IEnumerable<double> ratingTableValues;

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<double> RatingTableValues 
        {
            get
            {

                if (!String.IsNullOrEmpty(Definition) && TransformationType.Code == TransformationType.RatingTable)
                {
                    string json = String.Concat("{", Definition, "}");

                    ratingTableValues = new List<double>();


                    Dictionary<double, double> dic = JsonConvert.DeserializeObject<Dictionary<double, double>>(json);

                    ratingTableValues = from v in dic.Keys
                                             orderby dic[v] ascending
                                             select dic[v];

                }

                return ratingTableValues;
            }
        }



        public double GetRatingValue(double RawValue)
        {

            double result = RawValue;

            if (!String.IsNullOrEmpty(Definition) && TransformationType.Code == TransformationType.RatingTable)
            {
                string json = String.Concat("{", Definition, "}");

                Dictionary<double, double> dic = JsonConvert.DeserializeObject<Dictionary<double, double>>(json);


                //List<KeyValuePair<double, double>> vals = dic.ToList<KeyValuePair<double, double>>();

                //result = Search(vals, RawValue);

                //string s = "TEst";

                List<double> vals = (from v in dic.Keys
                                     orderby v ascending
                                     select v).ToList<double>();



                var index = vals.BinarySearch(RawValue);

                if (index < 0)
                    index = ~index;

                if (index > vals.Count - 1)
                    index = vals.Count - 1;

                result = dic[vals[index]];

                //var ceiling = dic[vals[vals.Count - 1]];
                //var floor = dic[vals[0]];


                //if (RawValue > ceiling)
                //    result = ceiling;
                //else if (RawValue < floor)
                //    result = floor;


                //double closest = vals.Aggregate((x, y) => Math.Abs(x - RawValue) < Math.Abs(y - RawValue) ? x : y);

                //result = dic[closest];
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, double> qualityValues;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, double> QualityValues
        {
            get
            {
                if (!String.IsNullOrEmpty(Definition) && TransformationType.Code == TransformationType.QualityControlValues)
                {
                    string json = String.Concat("{", Definition, "}");

                    qualityValues = new Dictionary<string, double>();

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    qualityValues = js.Deserialize<Dictionary<string, double>>(json);
                }

                return qualityValues;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, string> correctionValues;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> CorrectionValues
        {
            get
            {

                if (!String.IsNullOrEmpty(Definition) && TransformationType.Code == TransformationType.CorrectionValues)
                {
                    string json = String.Concat("{", Definition, "}");

                    correctionValues = new Dictionary<string, string>();

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    correctionValues = js.Deserialize<Dictionary<string, string>>(json);
                }

                return correctionValues;
            }
        }

        private Dictionary<string, double> lookupValues;

        public Dictionary<string, double> LookupValues
        {
            get
            {
                if (!String.IsNullOrEmpty(Definition) && TransformationType.Code == TransformationType.Lookup)
                {
                    string json = String.Concat("{", Definition, "}");

                    lookupValues = new Dictionary<string, double>(StringComparer.InvariantCultureIgnoreCase);

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    lookupValues = new Dictionary<string, double>(js.Deserialize<Dictionary<string, double>>(json), StringComparer.InvariantCultureIgnoreCase);
                }

                return lookupValues;
            }
        }


    }


}
