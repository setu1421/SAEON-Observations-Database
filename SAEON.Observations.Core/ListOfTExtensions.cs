using CsvHelper;
using CsvHelper.Configuration;
using SAEON.Logs;
using SAEON.OpenXML;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SAEON.Observations.Core
{
    public static class ListOfTExtensions
    {
        public static string ToCSV<T>(this List<T> list, string delimiter = ", ", string culture = "en-za")
        {
            using (SAEONLogs.MethodCall(typeof(ListOfTExtensions)))
            {
                try
                {
                    var config = new CsvConfiguration(CultureInfo.CreateSpecificCulture(culture)) { NewLine = Environment.NewLine, Delimiter = delimiter };
                    using var writer = new StringWriter();
                    using var csv = new CsvWriter(writer, config);
                    csv.WriteRecords(list);
                    writer.Flush();
                    return writer.ToString();

                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public static byte[] ToExcel<T>(this List<T> list) where T : class
        {
            using (SAEONLogs.MethodCall(typeof(ListOfTExtensions)))
            {
                try
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var doc = ExcelSaxHelper.CreateSpreadsheet(ms, list))
                        {
                            doc.Save();
                        }
                        return ms.ToArray();
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }
    }
}
