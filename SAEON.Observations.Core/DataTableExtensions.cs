using SAEON.Core;
using SAEON.OpenXML;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace SAEON.Observations.Core
{
    public static class DataTableExtensions
    {
        public static string ToCSV(this DataTable dataTable, string separator)
        {
            var sb = new StringBuilder();
            IEnumerable<String> headerValues = dataTable
                .Columns
                .OfType<DataColumn>()
                .Select(column => column.Caption);
            sb.AppendLine(String.Join(separator, headerValues));
            foreach (DataRow row in dataTable.Rows)
            {
                var values = new List<string>();
                foreach (DataColumn col in dataTable.Columns)
                {
                    if (row.IsNull(col))
                    {
                        values.Add(string.Empty);
                    }
                    else
                    {
                        var v = row[col];
                        if ((v is string) || (v is Guid))
                        {
                            values.Add(v.ToString().DoubleQuoted());
                        }
                        //else if (v is DateTime date)
                        else if (v is DateTime time)
                        {
                            //DateTime date = ((DateTime)v).ToUniversalTime().ToLocalTime();
                            DateTime date = DateTime.SpecifyKind(time, DateTimeKind.Local);
                            //values.Add(date.ToString("o"));
                            values.Add(date.ToString("yyyy-MM-ddTHH:mm:ss.fffK"));
                        }
                        else
                        {
                            values.Add(v.ToString());
                        }
                    }
                }
                sb.AppendLine(string.Join(separator, values));
            }
            return sb.ToString();
        }

        public static byte[] ToCSV(this DataTable dataTable, bool utf16 = false)
        {
            var separator = utf16 ? "/t" : ",";
            if (!utf16)
                return Encoding.UTF8.GetBytes(dataTable.ToCSV(separator));
            else
                return Encoding.Unicode.GetPreamble().Concat(Encoding.Unicode.GetBytes(dataTable.ToCSV(separator))).ToArray();
        }

        public static byte[] ToExcel(this DataTable dataTable)
        {
            using (var ms = new MemoryStream())
            {
                using (var doc = ExcelSaxHelper.CreateSpreadsheet(ms, dataTable))
                {
                    doc.Save();
                }
                return ms.ToArray();
            }
        }
    }
}
