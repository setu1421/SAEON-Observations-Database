using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SAEON.Logs;
using SAEON.Observations.Core.Entities;
using Serilog;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SAEON.Observations.Core.ConsoleTests
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Logging
                 .CreateConfiguration(@"Logs\SAEON.Observations.Core.ConsoleTests {Date}.txt")
                 .WriteTo.Console()
                 .Create();
            using (Logging.MethodCall(typeof(Program)))
            {
                try
                {
                    var db = new ObservationsDbContext("SAEON");
                    Logging.Information("Schemas: {Schemas}", db.DataSchemas.ToList().Count);
                    //var col = db.Offerings.Take(5).Include(i => i.Phenomena.Select(j => j.Units)).OrderBy(i => i.Name);
                    ////var col = db.Units.Include(i => i.Phenomena).OrderBy(i => i.Name);
                    //Logging.Information("Count: {count} Phenomena: {phenomena}", col.Count(), col.SelectMany(i => i.Phenomena).Count());
                    //var json = JsonConvert.SerializeObject(col, Formatting.Indented, new JsonSerializerSettings
                    //{
                    //    MaxDepth = 1,
                    //    NullValueHandling = NullValueHandling.Ignore,
                    //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    //    PreserveReferencesHandling = PreserveReferencesHandling.None
                    //});
                    //Logging.Information("Length: {length}", json.Length);
                    //Logging.Information("Length: {length} json: {json}", json.Length, json);

                    //var dm = new DataMatrix();
                    //dm.AddColumn("Name", "Name", MaxtixDataType.String);
                    //dm.AddColumn("Value", "Value", MaxtixDataType.Double);
                    //dm.AddRow("Test", 1);
                    //dm.AddRow("Wow", 2);
                    //var json = JsonConvert.SerializeObject(dm, Formatting.Indented, new JsonSerializerSettings
                    //{
                    //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    //});
                    //Logging.Information("json: {json}", json);
                    //Logging.Information("Cols: {cols} Rows: {rows}", dm.Columns.Count, dm.Rows.Count);
                    //Logging.Information("Data: {data}", dm.Rows[0]["Name"]);

                    CreateJohansAPIJson();
                    CreateDataCiteXml();
                    CreateDataCiteJson();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Exception");
                    throw;
                }
            }

        }

        private static void CreateJohansAPIJson()
        {
            var db = new ObservationsDbContext("Fynbos");
            var result = db.UserDownloads.Include(i => i.DigitalObjectIdentifier).First();
            var jSubjects =
                new JArray(
                    new JObject(
                        new JProperty("subject", "Observations")
                    ),
                    new JObject(
                        new JProperty("subject", "South African Environmental Observation Network (SAEON)")
                    ),
                    new JObject(
                        new JProperty("subjectScheme", "SOFTWARE_APP"),
                        new JProperty("schemeURI", "http://www.saeon.ac.za/"),
                        new JProperty("subject", "Observations Database")
                    ),
                    new JObject(
                        new JProperty("subjectScheme", "SOFTWARE_URL"),
                        new JProperty("schemeURI", "http://www.saeon.ac.za/"),
                        new JProperty("subject", "https://observations-test.saeon.ac.za")
                    )
                );
            var keywords = result.Keywords.Split(new string[] { "; " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var subject in keywords)
            {
                jSubjects.Add(
                    new JObject(
                        new JProperty("subject", subject)
                    )
                );
            }
            var places = result.Places.Split(new string[] { "; " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var place in places)
            {
                jSubjects.Add(
                    new JObject(
                        new JProperty("subjectScheme", "name"),
                        new JProperty("schemeURI", "http://www.geonames.org/"),
                        new JProperty("subject", place)
                    )
                );
            }

            var jGeoLocations = new JArray();
            foreach (var place in places)
            {
                var splits = place.Split(new char[] { ':' });
                jGeoLocations.Add(
                    new JObject(
                        new JProperty("geoLocationPlace", $"{splits[0]}, {splits[1]}")
                    )
                );
                jGeoLocations.Add(
                    new JObject(
                        new JProperty("geoLocationPoint", $"{splits[3]} {splits[2]}")
                    )
                );
            }
            jGeoLocations.Add(
                new JObject(
                    new JProperty("geoLocationBox", $"{result.LongitudeWest:f5} {result.LatitudeSouth:f5} {result.LongitudeEast:f5} {result.LatitudeNorth:f5}")
                )
            );

            var jMetadata =
                new JObject(
                    new JProperty("json",
                        new JObject(
                            new JProperty("dataSchema", "DataCite"),
                            new JProperty("language", "eng"),
                            new JProperty("identifier",
                                new JObject(
                                    new JProperty("identifierType", "DOI"),
                                    new JProperty("identifier", result.DigitalObjectIdentifier.DOI)
                                )
                            ),
                            new JProperty("resourceTypeGeneral", "Dataset"),
                            new JProperty("resourceType", "Tabular Data in Text File(s)"),
                            new JProperty("publisher", "South African Environmental Observation Network (SAEON)"),
                            new JProperty("publicationYear", $"{result.Date.Year}"),
                            new JProperty("dates",
                                new JArray(
                                    new JObject(
                                        new JProperty("date", result.Date.ToString("yyyy-MM-dd")),
                                        new JProperty("dateType", "accepted")
                                    ),
                                    new JObject(
                                        new JProperty("date", result.Date.ToString("yyyy-MM-dd")),
                                        new JProperty("dateType", "issued")
                                    ),
                                    new JObject(
                                        new JProperty("date", result.StartDate.ToString("yyyy-MM-dd")),
                                        new JProperty("dateType", "collected")
                                    ),
                                    new JObject(
                                        new JProperty("date", result.EndDate.ToString("yyyy-MM-dd")),
                                        new JProperty("dateType", "collected")
                                    )
                                )
                            ),
                            new JProperty("rights",
                                new JArray(
                                    new JObject(
                                        new JProperty("rights", "Attribution-ShareAlike 4.0 International (CC BY-SA 4.0)"),
                                        new JProperty("rightsURI", "https://creativecommons.org/licenses/by-sa/4.0/")
                                    )
                                )
                            ),
                            new JProperty("creators",
                                new JArray(
                                    new JObject(
                                        new JProperty("creatorName", "South African Environmental Observation Network (SAEON)"),
                                        new JProperty("nameType", "Organizational")
                                    ),
                                    new JObject(
                                        new JProperty("creatorName", "Observations Database Administrator"),
                                        new JProperty("affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:timpn@saeon.ac.za"),
                                        new JProperty("nameType", "Personal"),
                                        new JProperty("givenName", "Tim"),
                                        new JProperty("familyName", "Parker-Nance"),
                                        new JProperty("nameIdentifier", "0000-0001-7040-7736"),
                                        new JProperty("nameIdentifierScheme", "ORCID")
                                    )
                                )
                            ),
                            new JProperty("titles",
                                new JArray(
                                    new JObject(
                                        //new JProperty("titleType", ""),
                                        new JProperty("title", result.Title)
                                    )
                                )
                            ),
                            new JProperty("description",
                                new JArray(
                                    new JObject(
                                        new JProperty("descriptionType", "Abstract"),
                                        new JProperty("description", result.Description)
                                    )
                                )
                            ),
                            new JProperty("contributors",
                                new JArray(
                                    new JObject(
                                        new JProperty("contributorType", "DataManager"),
                                        new JProperty("contributorName", "SAEON uLwazi Node"),
                                        new JProperty("affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:wim@saeon.ac.za")
                                    ),
                                    new JObject(
                                        new JProperty("contributorType", "DataCurator"),
                                        new JProperty("contributorName", "SAEON uLwazi Node"),
                                        new JProperty("affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:wim@saeon.ac.za")
                                    )
                                )
                            ),
                            new JProperty("alternateIdentifiers",
                                new JArray(
                                    new JObject(
                                        new JProperty("alternateIdentifierType", "Internal"),
                                        new JProperty("alternateIdentifier", result.Id)
                                    )
                                )
                            ),
                            new JProperty("subjects", jSubjects),
                            new JProperty("additionalFields",
                                new JObject(
                                   new JProperty("onlineResources",
                                        new JArray(
                                            new JObject(
                                                new JProperty("func", "download"),
                                                new JProperty("desc", $"Observations Database Data Download {result.Id}"),
                                                new JProperty("href", result.DownloadUrl)
                                            ),
                                            new JObject(
                                                new JProperty("func", "download"),
                                                new JProperty("desc", $"Observations Database Data Download {result.Id} as Zip"),
                                                new JProperty("href", result.ZipUrl),
                                                new JProperty("format", "zip")
                                            )
                                        )
                                    ),
                                    new JProperty("coverageBegin", result.StartDate.ToString("yyyy-MM-dd")),
                                    new JProperty("coverageEnd", result.EndDate.ToString("yyyy-MM-dd"))
                                )
                            ),
                            new JProperty("geoLocations", jGeoLocations),
                            new JProperty("bounds",
                                new JArray(
                                    new JValue($"{result.LatitudeSouth:f5}"), new JValue($"{result.LongitudeWest:f5}"),
                                    new JValue($"{result.LatitudeNorth:f5}"), new JValue($"{result.LongitudeEast:f5}")
                                )
                            )
                        )
                    ),
                    new JProperty("schema", "datacite"),
                    new JProperty("mode", "manual"),
                    new JProperty("repository",
                        new JObject(
                            new JProperty("URL", "http://test.sasdi.net/"),
                            new JProperty("username", "testfour"),
                            new JProperty("password", "testfour"),
                            new JProperty("institution", "south-african-environmental-observation-network")
                        )
                    ),
                    new JProperty("target", "http://odp.org.za/test_sodp.aspx")
                );
            Console.WriteLine(jMetadata.ToString());
            File.WriteAllText("JohansAPI.json", jMetadata.ToString());
        }

        private static void CreateDataCiteXml()
        {
            var db = new ObservationsDbContext("Fynbos");
            var result = db.UserDownloads.Include(i => i.DigitalObjectIdentifier).First();
            XNamespace ns = "http://datacite.org/schema/kernel-4";
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";

            var subjects =
                new XElement(ns + "subjects",
                    new XElement(ns + "subject", "Observations"),
                    new XElement(ns + "subject", "South African Environmental Observation Network (SAEON)"),
                    new XElement(ns + "subject", new XAttribute("subjectScheme", "SOFTWARE_APP"), new XAttribute("schemeURI", "http://www.saeon.ac.za/"), "Observations Database"),
                    new XElement(ns + "subject", new XAttribute("subjectScheme", "SOFTWARE_URL"), new XAttribute("schemeURI", "http://www.saeon.ac.za/"), "https://observations-test.saeon.ac.za")
                );
            var keywords = result.Keywords.Split(new string[] { "; " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var subject in keywords)
            {
                subjects.Add(new XElement(ns + "subject", subject));
            }
            var places = result.Places.Split(new string[] { "; " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var place in places)
            {
                subjects.Add(new XElement(ns + "subject", new XAttribute("subjectScheme", "name"), new XAttribute("schemeURI", "http://www.geonames.org/"), place));
            }

            var geoLocations = new XElement(ns + "geoLocations");
            foreach (var place in places)
            {
                var splits = place.Split(new char[] { ':' });
                geoLocations.Add(
                    new XElement(ns + "geoLocation",
                        new XElement(ns + "geoLocationPlace", $"{splits[0]}, {splits[1]}"),
                        new XElement(ns + "geoLocationPoint",
                            new XElement(ns + "pointLatitude", splits[2]),
                            new XElement(ns + "pointLongitude", splits[3])
                        )
                    ));
            }
            geoLocations.Add(
                new XElement(ns + "geoLocation",
                    new XElement(ns + "geoLocationBox",
                        new XElement(ns + "westBoundLongitude", result.LongitudeWest),
                        new XElement(ns + "eastBoundLongitude", result.LongitudeEast),
                        new XElement(ns + "nothBoundLatitude", result.LatitudeNorth),
                        new XElement(ns + "southBoundLatitude", result.LatitudeSouth)
                    )
                )
            );

            var root = new XElement(ns + "resource",
                new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                new XAttribute(xsi + "schemaLocation", "http://datacite.org/schema/kernel-4 http://schema.datacite.org/meta/kernel-4.2/metadata.xsd"),
                new XElement(ns + "identifier", new XAttribute("identifierType", "DOI"), result.DigitalObjectIdentifier.DOI),
                new XElement(ns + "language", "en-UK"),
                new XElement(ns + "resourceType", new XAttribute("resourceTypeGeneral", "Dataset"), "Tabular Data in Text File(s)"),
                new XElement(ns + "alternateIdentifiers",
                    new XElement(ns + "alternateIdentifier", new XAttribute("alternateIdentifierType", "Internal"), result.Id)
                ),
                new XElement(ns + "publisher", "South African Environmental Observation Network (SAEON)"),
                new XElement(ns + "publicationYear", result.Date.Year),
                new XElement(ns + "dates",
                    new XElement(ns + "date", new XAttribute("dateType", "accepted"), result.Date.Year),
                    new XElement(ns + "date", new XAttribute("dateType", "issued"), result.Date.Year),
                    new XElement(ns + "date", new XAttribute("dateType", "collected"), result.StartDate.Year),
                    new XElement(ns + "date", new XAttribute("dateType", "collected"), result.EndDate.Year)
                ),
                new XElement(ns + "rightsList",
                    new XElement(ns + "rights",
                        new XAttribute("schemeURI", "https://spdx.org/licenses/"),
                        new XAttribute("rightsIdentifierScheme", "SPDX"),
                        new XAttribute("rightsIdentifier", "CC-BY-SA-4.0"),
                        new XAttribute("rightsURI", "https://creativecommons.org/licenses/by-sa/4.0/"),
                        "Attribution-ShareAlike 4.0 International (CC BY-SA 4.0)")
                ),
                new XElement(ns + "creators",
                    new XElement(ns + "creator",
                        new XElement(ns + "creatorName", new XAttribute("nameType", "Organizational"), "South African Environmental Observation Network (SAEON)")
                    ),
                    new XElement(ns + "creator",
                        new XElement(ns + "creatorName", new XAttribute("nameType", "Personal"), "Observations Database Administrator"),
                        new XElement(ns + "givenName", "Tim"),
                        new XElement(ns + "familyName", "Parker-Nance"),
                        new XElement(ns + "nameIdentifier", new XAttribute("schemeURI", "http://orcid.org/"), new XAttribute("nameIdentifierScheme", "ORCID"), "0000-0001-7040-7736"),
                        new XElement(ns + "affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:timpn@saeon.ac.za")
                    )
                ),
                new XElement(ns + "titles",
                    new XElement(ns + "title", result.Title)
                ),
                new XElement(ns + "descriptions",
                    new XElement(ns + "description", new XAttribute("descriptionType", "Abstract"), result.Description)
                ),
                new XElement(ns + "contributors",
                    new XElement(ns + "contributor", new XAttribute("contributorType", "ContactPerson"),
                        new XElement(ns + "contributorName", "Parker-Nance, Tim"),
                        new XElement(ns + "givenName", "Tim"),
                        new XElement(ns + "familyName", "Parker-Nance"),
                        new XElement(ns + "nameIdentifier", new XAttribute("schemeURI", "http://orcid.org/"), new XAttribute("nameIdentifierScheme", "ORCID"), "0000-0001-7040-7736"),
                        new XElement(ns + "affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:timpn@saeon.ac.za")
                    ),
                    new XElement(ns + "contributor", new XAttribute("contributorType", "DataManager"),
                        new XElement(ns + "contributorName", "SAEON uLwazi Node"),
                        new XElement(ns + "affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:wim@saeon.ac.za")
                    ),
                    new XElement(ns + "contributor", new XAttribute("contributorType", "DataCurator"),
                        new XElement(ns + "contributorName", "SAEON uLwazi Node"),
                        new XElement(ns + "affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:wim@saeon.ac.za")
                    )
                ),
                new XElement(ns + "subjects", subjects),
                new XElement(ns + "formats",
                    new XElement(ns + "format", "application/zip"),
                    new XElement(ns + "format", "text/csv")
                ),
                new XElement(ns + "geoLocations", geoLocations)
            );
            Console.WriteLine(root);
            File.WriteAllText("DataCite.xml", root.ToString());
        }

        private static void CreateDataCiteJson()
        {
            var db = new ObservationsDbContext("Fynbos");
            var result = db.UserDownloads.Include(i => i.DigitalObjectIdentifier).First();
            var jSubjects =
                new JArray(
                    new JObject(
                        new JProperty("subject", "Observations")
                    ),
                    new JObject(
                        new JProperty("subject", "South African Environmental Observation Network (SAEON)")
                    ),
                    new JObject(
                        new JProperty("subjectScheme", "SOFTWARE_APP"),
                        new JProperty("schemeURI", "http://www.saeon.ac.za/"),
                        new JProperty("subject", "Observations Database")
                    ),
                    new JObject(
                        new JProperty("subjectScheme", "SOFTWARE_URL"),
                        new JProperty("schemeURI", "http://www.saeon.ac.za/"),
                        new JProperty("subject", "https://observations-test.saeon.ac.za")
                    )
                );
            var keywords = result.Keywords.Split(new string[] { "; " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var keyword in keywords)
            {
                jSubjects.Add(
                    new JObject(
                        new JProperty("subject", keyword)
                    )
                );
            }
            var places = result.Places.Split(new string[] { "; " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var place in places)
            {
                jSubjects.Add(
                    new JObject(
                        new JProperty("subjectScheme", "name"),
                        new JProperty("schemeURI", "http://www.geonames.org/"),
                        new JProperty("subject", place)
                    )
                );
            }

            var jGeoLocations = new JArray();
            foreach (var place in places)
            {
                var splits = place.Split(new char[] { ':' });
                jGeoLocations.Add(
                    new JObject(
                        new JProperty("geoLocationPlace", $"{splits[0]}, {splits[1]}"),
                        new JProperty("geoLocationPoint",
                            new JObject(
                                new JProperty("pointLatitude",splits[2]),
                                new JProperty("pointLongitude", splits[3])
                            )
                        )
                    )
                );
            }
            jGeoLocations.Add(
                new JObject(
                    new JProperty("geoLocationBox", 
                        new JObject(
                            new JProperty("westBoundLongitude",result.LongitudeWest),
                            new JProperty("eastBoundLongitude", result.LongitudeEast),
                            new JProperty("northBoundLatitude", result.LatitudeNorth),
                            new JProperty("southBoundLatitude", result.LatitudeSouth)
                        )
                    )
                )
            );

            var jDataCite =
                new JObject(
                    new JProperty("id", result.DigitalObjectIdentifier.DOI),
                    new JProperty("type", "dois"),
                    new JProperty("attributes",
                        new JObject(
                            new JProperty("doi", result.DigitalObjectIdentifier.DOI),
                            new JProperty("identifiers",
                                new JArray(
                                    new JObject(
                                        new JProperty("identifier", result.DigitalObjectIdentifier.DOIUrl),
                                        new JProperty("identifierType", "DOI")
                                    ),
                                    new JObject(
                                        new JProperty("identifier", result.Id),
                                        new JProperty("identifierType", "Internal")
                                    )
                                )
                            ),
                        new JProperty("language", "en-uk"),
                        new JProperty("types",
                            new JObject(
                                new JProperty("resourceTypeGeneral", "Dataset"),
                                new JProperty("resourceType", "Tabular Data in Text File(s)")
                            )
                        ),
                        new JProperty("publisher", "South African Environmental Observation Network (SAEON)"),
                        new JProperty("publicationYear", $"{result.Date.Year}"),
                        new JProperty("dates",
                            new JArray(
                                new JObject(
                                    new JProperty("date", result.Date.ToString("yyyy-MM-dd")),
                                    new JProperty("dateType", "accepted")
                                ),
                                new JObject(
                                    new JProperty("date", result.Date.ToString("yyyy-MM-dd")),
                                    new JProperty("dateType", "issued")
                                ),
                                new JObject(
                                    new JProperty("date", result.StartDate.ToString("yyyy-MM-dd")),
                                    new JProperty("dateType", "collected")
                                ),
                                new JObject(
                                    new JProperty("date", result.EndDate.ToString("yyyy-MM-dd")),
                                    new JProperty("dateType", "collected")
                                )
                            )
                        ),
                        new JProperty("rights",
                            new JArray(
                                new JObject(
                                    new JProperty("rights", "Attribution-ShareAlike 4.0 International (CC BY-SA 4.0)"),
                                    new JProperty("rightsURI", "https://creativecommons.org/licenses/by-sa/4.0/")
                                )
                            )
                        ),
                        new JProperty("creators",
                            new JArray(
                                new JObject(
                                    new JProperty("name", "South African Environmental Observation Network (SAEON)"),
                                    new JProperty("nameType", "Organizational")
                                ),
                                new JObject(
                                    new JProperty("name", "Observations Database Administrator"),
                                    new JProperty("nameType", "Personal"),
                                    new JProperty("givenName", "Tim"),
                                    new JProperty("familyName", "Parker-Nance"),
                                    new JProperty("nameIdentifier", "0000-0001-7040-7736"),
                                    new JProperty("nameIdentifierScheme", "ORCID"),
                                    new JProperty("affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:timpn@saeon.ac.za")
                                )
                            )
                        ),
                        new JProperty("titles",
                            new JArray(
                                new JObject(
                                    new JProperty("title", result.Title)
                                )
                            )
                        ),
                        new JProperty("descriptions",
                            new JArray(
                                new JObject(
                                    new JProperty("descriptionType", "Abstract"),
                                    new JProperty("description", result.Description)
                                )
                            )
                        ),
                        new JProperty("contributors",
                            new JArray(
                                new JObject(
                                    new JProperty("contributorType", "ContactPerson"),
                                    new JProperty("contributorName", "Parker-Nance, Tim"),
                                    new JProperty("givenName", "Tim"),
                                    new JProperty("familyName", "Parker-Nance"),
                                    new JProperty("nameIdentifier", "0000-0001-7040-7736"),
                                    new JProperty("nameIdentifierScheme", "ORCID"),
                                    new JProperty("affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:timpn@saeon.ac.za")
                                ),
                                new JObject(
                                    new JProperty("contributorType", "DataManager"),
                                    new JProperty("contributorName", "SAEON uLwazi Node"),
                                    new JProperty("affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:wim@saeon.ac.za")
                                ),
                                new JObject(
                                    new JProperty("contributorType", "DataCurator"),
                                    new JProperty("contributorName", "SAEON uLwazi Node"),
                                    new JProperty("affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:wim@saeon.ac.za")
                                )
                            )
                        ),
                        new JProperty("subjects", jSubjects),
                        new JProperty("geoLocations", jGeoLocations)
                    )
                )
            );
            Console.WriteLine(jDataCite.ToString());
            File.WriteAllText("DataCite.json", jDataCite.ToString());
        }

    }
}
