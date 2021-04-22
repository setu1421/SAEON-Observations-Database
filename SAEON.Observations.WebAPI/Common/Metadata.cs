using Humanizer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SAEON.Core;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.WebAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAEON.Observations.WebAPI
{
    public class Metadata : MetadataCore
    {
        public DigitalObjectIdentifier DOI { get; set; }
        public MetadataIdentifier Identifier => DOI == null ? null : new MetadataIdentifier { Name = DOI.DOI, Type = "DOI" };
        public string GenerateTitle() => GenerateTitle(DOI.Name);
        public string GenerateTitle(string name) => $"Observations in the SAEON Observations Database for {DOI.DOIType.Humanize(LetterCasing.LowerCase)} {MetadataHelper.CleanPrefixes(name)}";
        public string GenerateDescription() => GenerateDescription(DOI.Name);
        public string GenerateDescription(string name) => $"The observations in the SAEON Observations Database for {DOI.DOIType.Humanize(LetterCasing.LowerCase)} {MetadataHelper.CleanPrefixes(name)}";
        public string CitationHtml => $"{Creator.Name} ({PublicationYear}): {Title}. {Publisher}. (dataset). <a href='{DOI.DOIUrl}'>{DOI.DOIUrl}</a>" +
            (!Accessed.HasValue ? "" : $". Accessed {Accessed:yyyy-MM-dd HH:mm}");
        public string CitationText => $"{Creator.Name} ({PublicationYear}): {Title}. {Publisher}. (dataset). {DOI.DOIUrl}" +
            (!Accessed.HasValue ? "" : $". Accessed {Accessed:yyyy-MM-dd HH:mm}");

        public Metadata() { }

        public Metadata(MetadataCore metadata) : this()
        {
            PublicationDate = metadata.PublicationDate;
            Title = metadata.Title;
            Description = metadata.Description;
            Accessed = metadata.Accessed;
            StartDate = metadata.StartDate;
            EndDate = metadata.EndDate;
            Subjects.AddRange(metadata.Subjects);
            LatitudeNorth = metadata.LatitudeNorth;
            LatitudeSouth = metadata.LatitudeSouth;
            LongitudeEast = metadata.LongitudeEast;
            LongitudeWest = metadata.LongitudeWest;
            ElevationMaximum = metadata.ElevationMaximum;
            ElevationMinimum = metadata.ElevationMinimum;
        }

        public void Generate(string title = "", string description = "")
        {
            if (DOI == null) throw new InvalidOperationException($"{nameof(DOI)} cannot be null");
            if (string.IsNullOrWhiteSpace(title))
            {
                title = GenerateTitle();
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                description = GenerateDescription();
            }
            if (StartDate.HasValue && EndDate.HasValue)
            {
                if (StartDate.Value == EndDate.Value)
                {
                    title += $" on {StartDate.Value:yyyy-MM-dd}";
                    description += $" on {StartDate.Value.ToJsonDate()}";
                }
                else
                {
                    title += $" from {StartDate.Value:yyyy-MM-dd} to {EndDate.Value:yyyy-MM-dd}";
                    description += $" from {StartDate.Value.ToJsonDate()} to {EndDate.Value.ToJsonDate()}";
                }
            }
            Title = title;
            if (LatitudeNorth.HasValue && LatitudeSouth.HasValue && LongitudeWest.HasValue && LongitudeEast.HasValue)
            {
                if ((LatitudeNorth == LatitudeSouth) && (LongitudeWest == LongitudeEast))
                {
                    description += $" at {LatitudeNorth:f5},{LongitudeWest:f5} (+N-S,-W+E)";
                }
                else
                {
                    description += $" in area {LatitudeNorth:f5},{LongitudeWest:f5} (N,W) {LatitudeSouth:f5},{LongitudeEast:f5} (S,E) (+N-S,-W+E)";
                }
            }
            if (ElevationMinimum.HasValue && ElevationMaximum.HasValue)
            {
                if (ElevationMinimum == ElevationMaximum)
                {
                    description += $" at {ElevationMinimum:f2}m above mean sea level";
                }
                else
                {
                    description += $" between {ElevationMinimum:f2}m and {ElevationMaximum:f2}m above mean sea level";
                }
            }
            // Build Description as Text and Html
            var sbText = new StringBuilder();
            var sbHtml = new StringBuilder();
            sbText.AppendLine(description.AddTrailing("."));
            sbHtml.AppendDLStart();
            sbHtml.AppendDTDD("Title", title);
            sbHtml.AppendDT("Description");
            sbHtml.AppendDDStart().AppendHtmlP(description);
            if (DOI.Parent != null)
            {
                sbText.AppendLine($"This collection is part of the {(DOI.Parent.DOIType == DOIType.ObservationsDb ? "" : DOI.Parent.DOIType.Humanize(LetterCasing.LowerCase))} {MetadataHelper.CleanPrefixes(DOI.Parent.Name)} {DOI.Parent.DOIUrl}.");
                sbHtml.AppendHtmlP($"This collection is part of the {(DOI.Parent.DOIType == DOIType.ObservationsDb ? "" : DOI.Parent.DOIType.Humanize(LetterCasing.LowerCase))} {MetadataHelper.CleanPrefixes(DOI.Parent.Name)} <a href='{DOI.Parent.DOIUrl}'>{DOI.Parent.DOI}</a>");
                RelatedIdentifiers.Add(new MetadataRelatedIdentifier { Name = "IsPartOf", Identifier = DOI.Parent.DOI, Type = "DOI" });
                foreach (var subject in Subjects.Distinct())
                {
                    Parent.Subjects.AddIfNotExists(subject);
                }
            }
            // Dynamic children
            if (MetadataHelper.DynamicDOITypes.Contains(DOI.DOIType))
            {
                var children = DOI.Children.Where(i => MetadataHelper.DynamicDOITypes.Contains(i.DOIType)).OrderBy(i => i.Name).ToList();
                if (children.Any())
                {
                    sbText.AppendLine($"This collection includes observations from the following {((DOI.DOIType + 1).Humanize(LetterCasing.LowerCase).ToQuantity(children.Count, ShowQuantityAs.None))}: " +
                        string.Join(", ", children.Select(i => $"{MetadataHelper.CleanPrefixes(i.Name)} {i.DOIUrl}")));
                    sbHtml.AppendHtmlP($"This collection includes observations from the following {((DOI.DOIType + 1).Humanize(LetterCasing.LowerCase).ToQuantity(children.Count, ShowQuantityAs.None))}:");
                    sbHtml.AppendHtmlUL(children.Select(i => $"{MetadataHelper.CleanPrefixes(i.Name)} <a href='{i.DOIUrl}'>{i.DOI}</a>"));
                    foreach (var child in children)
                    {
                        RelatedIdentifiers.Add(new MetadataRelatedIdentifier { Name = "HasPart", Identifier = child.DOI, Type = "DOI" });
                    }
                }
            }
            // Periodic children
            // Ad-Hoc children
            if (DOI.DOIType == DOIType.Collection && DOI.Code == DOIHelper.AdHocDOIsCode)
            {
                var children = DOI.Children.Where(i => i.DOIType == DOIType.AdHoc).OrderBy(i => i.Name).ToList();
                foreach (var child in children)
                {
                    RelatedIdentifiers.Add(new MetadataRelatedIdentifier { Name = "HasPart", Identifier = child.DOI, Type = "DOI" });
                }
            }
            sbHtml.AppendDDEnd();

            sbHtml.AppendDTDD("Publisher", $"{Publisher} {PublicationYear}");
            if (StartDate.HasValue || EndDate.HasValue)
            {
                var dates = string.Empty;
                if (StartDate.HasValue)
                {
                    dates += $" Created: {StartDate:yyyy-MM-dd}";
                }
                if (StartDate.HasValue && EndDate.HasValue)
                {
                    dates += $" Collected: {StartDate.ToJsonDate()}/{EndDate.ToJsonDate()}";
                }
                else if (StartDate.HasValue)
                {
                    dates += $" Collected:  {StartDate.ToJsonDate()}";
                }
                sbHtml.AppendDTDD("Dates", dates.Trim());
            }
            if (Rights.Any())
            {
                sbHtml.AppendDTDD("License", Rights[0].Name);
            }
            // Keyword cleanup
            var cleanSubjects = Subjects.Where(i => !i.Name.StartsWith("http")).Distinct().ToList();
            var excludes = new List<string> { "South African Environmental Observation Network", "Observations Database" };
            cleanSubjects.RemoveAll(i => excludes.Contains(i.Name));
            cleanSubjects.ForEach(i => i.Name = i.Name/*.Replace(",", "")*/.Replace("  ", " "));
            sbHtml.AppendDTDD("Keywords", $"{string.Join("; ", cleanSubjects.OrderBy(i => i.Name).Select(i => i.Name))}");
            if (!string.IsNullOrWhiteSpace(DOI.MetadataUrl))
            {
                sbHtml.AppendDTDD("Metadata URL", $"<a href='{DOI.MetadataUrl}'>{DOI.MetadataUrl}</a>");
            }
            if (!string.IsNullOrWhiteSpace(DOI.QueryUrl))
            {
                sbHtml.AppendDTDD("Query URL", $"<a href='{DOI.QueryUrl}'>{DOI.QueryUrl}</a>");
            }
            sbHtml.AppendDTDD("Citation", CitationHtml);
            sbHtml.AppendDLEnd();
            var desc = sbText.ToString().TrimEnd(Environment.NewLine).Replace(Environment.NewLine, " ").Replace("  ", " ").TrimEnd("."); //.Replace(Environment.NewLine,"<br>");
            SAEONLogs.Verbose("Desc: {Desc}", desc);
            Description = desc;
            DescriptionHtml = sbHtml.ToString();
        }

        public string ToJson()
        {
            var jDates = new JArray();
            if (StartDate.HasValue)
            {
                jDates.Add(
                    new JObject(
                        new JProperty("date", StartDate.Value.ToString("yyyy-MM-dd")),
                        new JProperty("dateType", "Created")));
                jDates.Add(
                    new JObject(
                        new JProperty("date", StartDate.Value.ToString("yyyy-MM-dd")),
                        new JProperty("dateType", "Submitted")));
                jDates.Add(
                    new JObject(
                        new JProperty("date", StartDate.Value.ToString("yyyy-MM-dd")),
                        new JProperty("dateType", "Accepted")));
            }
            if (StartDate.HasValue && EndDate.HasValue)
            {
                jDates.Add(
                    new JObject(
                        new JProperty("date", $"{StartDate.Value.ToJsonDate()}/{EndDate.Value.ToJsonDate()}"),
                        new JProperty("dateType", "Valid")));
            }
            else if (StartDate.HasValue)
            {
                jDates.Add(
                    new JObject(
                        new JProperty("date", $"{StartDate.Value.ToJsonDate()}"),
                        new JProperty("dateType", "Valid")));
            }
            var jGeoLocations = new JArray();
            if (LatitudeNorth.HasValue && LatitudeSouth.HasValue && LongitudeWest.HasValue && LongitudeEast.HasValue)
            {
                if ((LatitudeNorth == LatitudeSouth) && (LongitudeWest == LongitudeEast))
                {
                    jGeoLocations.Add(
                        new JObject(
                            //new JProperty("geoLocationPlace", $"{splits[0]}, {splits[1]}"),
                            new JProperty("geoLocationPoint",
                                new JObject(
                                    new JProperty("pointLatitude", LatitudeNorth),
                                    new JProperty("pointLongitude", LongitudeWest)
                                // Pre Schema 4.3
                                //new JProperty("pointLatitude", LatitudeNorth.ToString()),
                                //new JProperty("pointLongitude", LongitudeWest.ToString())
                                )
                            )
                        ));
                }
                else
                {
                    jGeoLocations.Add(
                        new JObject(
                            new JProperty("geoLocationBox",
                                new JObject(
                                    new JProperty("westBoundLongitude", LongitudeWest),
                                    new JProperty("eastBoundLongitude", LongitudeEast),
                                    new JProperty("northBoundLatitude", LatitudeNorth),
                                    new JProperty("southBoundLatitude", LatitudeSouth)
                                // Pre Schema 4.3
                                //new JProperty("westBoundLongitude", LongitudeWest.ToString()),
                                //new JProperty("eastBoundLongitude", LongitudeEast.ToString()),
                                //new JProperty("northBoundLatitude", LatitudeNorth.ToString()),
                                //new JProperty("southBoundLatitude", LatitudeSouth.ToString())
                                )
                            )
                        )
                    );
                }
            }
            var jObj =
                new JObject(
                    new JProperty("doi", DOI.DOI),
                    // Pre Schema 4.3
                    //new JProperty("identifier", Identifier?.AsJson()),
                    new JProperty("language", Language),
                    new JProperty("types", ResourceType.AsJson()),
                    // Pre Schema 4.3
                    //new JProperty("resourceType", ResourceType.AsJson()),
                    new JProperty("publisher", Publisher),
                    new JProperty("publicationYear", PublicationYear),
                    // Pre Schema 4.3
                    //new JProperty("publicationYear", PublicationYear.ToString()),
                    new JProperty("creators", new JArray(Creator.AsJson())),
                    new JProperty("dates", jDates),
                    new JProperty("titles",
                        new JArray(
                            new JObject(
                                new JProperty("title", Title)
                            )
                        )
                    ),
                    new JProperty("descriptions",
                        new JArray(
                            new JObject(
                                new JProperty("descriptionType", "Abstract"),
                                new JProperty("description", Description)
                            )
                        )
                    ),
                    new JProperty("rightsList", new JArray(Rights.Select(i => i.AsJson()))),
                    new JProperty("contributors", new JArray(Contributors.Select(i => i.AsJson()))),
                    new JProperty("subjects", new JArray(Subjects.Distinct().OrderBy(i => i.Name).Select(i => i.AsJson()))),
                    new JProperty("geoLocations", jGeoLocations),
                    new JProperty("identifiers", new JArray(AlternateIdentifiers.Select(i => i.AsJson()))),
                    // Pre Schema 4.3
                    //new JProperty("alternateIdentifiers", new JArray(AlternateIdentifiers.Select(i => i.AsJson()))),
                    new JProperty("relatedIdentifiers", new JArray(RelatedIdentifiers.Select(i => i.AsJson()))),
                    // Pre Schema 4.3
                    //new JProperty("schemaVersion", "https://datacite.org/schema/kernel-4"),
                    new JProperty("immutableResource", new JObject(
                        new JProperty("resourceDescription", Title),
                        new JProperty("resourceData", new JObject(
                            new JProperty("downloadURL", DOI.QueryUrl)))))
               );
            return jObj.ToString(Formatting.Indented);
        }

        public string ToHtml()
        {
            return DescriptionHtml;
        }
    }
}
