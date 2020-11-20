#define Schema43
using Humanizer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SAEON.Observations.WebAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAEON.Observations.WebAPI
{
    public class MetadataAffiliation
    {
        public string Name { get; set; }
        public string Identifier { get; set; }
        public string Scheme { get; set; }
        public string SchemeUri { get; set; }

        public JObject AsJson()
        {
            var jObj = new JObject(new JProperty("affiliation", Name));
#if Schema43
            if (!string.IsNullOrWhiteSpace(Identifier))
            {
                new JProperty("affiliationIdentifier", Identifier);
            }
            if (!string.IsNullOrWhiteSpace(Scheme))
            {
                jObj.Add(new JProperty("affiliationIdentifierScheme", Scheme));
            }
            if (!string.IsNullOrWhiteSpace(SchemeUri))
            {
                jObj.Add(new JProperty("schemeURI", SchemeUri));
            }
#endif
            return jObj;
        }
    }

    public class MetadataIdentifier
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public JObject AsJson()
        {
            var jObj = new JObject(new JProperty("identifier", Name));
            if (!string.IsNullOrWhiteSpace(Type))
            {
                jObj.Add(new JProperty("identifierType", Type));
            }
            return jObj;
        }
    }

    public class MetadataAlternateIdentifier
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public JObject AsJson()
        {
            var jObj = new JObject(new JProperty("alternateIdentifier", Name));
            if (!string.IsNullOrWhiteSpace(Type))
            {
                jObj.Add(new JProperty("alternateIdentifierType", Type));
            }
            return jObj;
        }
    }

    public class MetadataNameIdentifier
    {
        public string Name { get; set; }
        public string Scheme { get; set; }
        public string SchemeUri { get; set; }

        public JObject AsJson()
        {
            var jObj = new JObject(new JProperty("nameIdentifier", Name));
            if (!string.IsNullOrWhiteSpace(Scheme))
            {
                jObj.Add(new JProperty("nameIdentifierScheme", Scheme));
            }
            if (!string.IsNullOrWhiteSpace(SchemeUri))
            {
                jObj.Add(new JProperty("schemeURI", SchemeUri));
            }
            return jObj;
        }
    }

    public class MetadataCreator
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string FirstNames { get; set; }
        public string Surname { get; set; }
        public List<MetadataNameIdentifier> Identifiers { get; set; } = new List<MetadataNameIdentifier>();
        public List<MetadataAffiliation> Affiliations { get; set; } = new List<MetadataAffiliation>();

        public virtual JObject AsJson()
        {
            var jObj = new JObject(
                new JProperty("name", Name),
                new JProperty("nameType", Type)
                );
            if (Identifiers?.Any() ?? false)
            {
                jObj.Add(new JProperty("nameIdentifiers", new JArray(Identifiers.Select(i => i.AsJson()))));
            }
            if (Affiliations?.Any() ?? false)
            {
#if Schema43
                jObj.Add(new JProperty("affiliation", new JArray(Affiliations.Select(i => i.AsJson()))));
#else
                jObj.Add(new JProperty("affiliations", new JArray(Affiliations.Select(i => i.AsJson()))));
#endif
            }
            return jObj;
        }
    }

    public class MetadataContributor : MetadataCreator
    {
        public string ContributorType { get; set; }

        public override JObject AsJson()
        {
            var jObj = base.AsJson();
            jObj.Add(new JProperty("contributorType", ContributorType));
            return jObj;
        }
    }

    public class MetadataRelatedIdentifier
    {
        public string Name { get; set; }
        public string Identifier { get; set; }
        public string Type { get; set; }

        public JObject AsJson()
        {
            var jObj = new JObject(
                new JProperty("relationType", Name),
                new JProperty("relatedIdentifier", Identifier),
                new JProperty("relatedIdentifierType", Type)
                );
            return jObj;
        }
    }

    public class MetadataResourceType
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public JObject AsJson()
        {
            var jObj = new JObject(
                new JProperty("resourceTypeGeneral", Name),
                new JProperty("resourceType", Type));
            return jObj;
        }
    }

    public class MetadataRights
    {
        public string Name { get; set; }
        public string URI { get; set; }
        public string Identifier { get; set; }
        public string Scheme { get; set; }
        public string SchemeUri { get; set; }

        public JObject AsJson()
        {
            var jObj = new JObject(new JProperty("rights", Name));
            if (!string.IsNullOrWhiteSpace(URI))
            {
                jObj.Add(new JProperty("rightsURI", URI));
            }
            if (!string.IsNullOrWhiteSpace(Identifier))
            {
                jObj.Add(new JProperty("rightsIdentifier", Identifier));
            }
            if (!string.IsNullOrWhiteSpace(Scheme))
            {
                jObj.Add(new JProperty("rightsIdentifierScheme", Scheme));
            }
            if (!string.IsNullOrWhiteSpace(SchemeUri))
            {
                jObj.Add(new JProperty("schemeURI", SchemeUri));
            }
            return jObj;
        }
    }

    public class MetadataSubject
    {
        public string Name { get; set; }
        public string Scheme { get; set; }
        public string SchemeUri { get; set; }
        public string ValueUri { get; set; }

        public JObject AsJson()
        {
            var jObj = new JObject(new JProperty("subject", Name));
            if (!string.IsNullOrWhiteSpace(Scheme))
            {
                jObj.Add(new JProperty("subjectScheme", Scheme));
            }
            if (!string.IsNullOrWhiteSpace(SchemeUri))
            {
                jObj.Add(new JProperty("schemeURI", SchemeUri));
            }
            if (!string.IsNullOrWhiteSpace(ValueUri))
            {
                jObj.Add(new JProperty("valueURI", ValueUri));
            }
            return jObj;
        }

        public override bool Equals(object obj)
        {
            return obj is MetadataSubject subject &&
                   Name == subject.Name &&
                   Scheme == subject.Scheme &&
                   SchemeUri == subject.SchemeUri &&
                   ValueUri == subject.ValueUri;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Scheme, SchemeUri, ValueUri);
        }
    }

    public class Metadata
    {
        public Metadata Parent { get; set; }
        public DigitalObjectIdentifier DOI { get; set; }
        public MetadataIdentifier Identifier => DOI == null ? null : new MetadataIdentifier { Name = DOI.DOI, Type = "DOI" };
        public List<MetadataAlternateIdentifier> AlternateIdentifiers { get; } = new List<MetadataAlternateIdentifier>();
        public MetadataCreator Creator = new MetadataCreator
        {
            Name = "SAEON Observations Database",
            Type = "Organizational",
            Identifiers = new List<MetadataNameIdentifier> { new MetadataNameIdentifier {
                Name = "https://ror.org/041j42q70", Scheme = "ROR", SchemeUri = "https://ror.org" } }
        };
        public string Language { get; set; } = "en-za";
        public MetadataResourceType ResourceType { get; } = new MetadataResourceType { Name = "Dataset", Type = "Observations" };
        public string Publisher { get; set; } = "South African Environmental Observation Network (SAEON)";
        public int? PublicationYear => StartDate?.Year ?? EndDate?.Year;
        public string Title { get; set; }
        public string Description { get; set; }
        public string DescriptionHtml { get; set; }
        public string ItemDescription { get; set; }
        public string ItemUrl { get; set; }
        public List<MetadataRights> Rights { get; } = new List<MetadataRights> {
            new MetadataRights {
                Name = "Attribution-ShareAlike 4.0 International (CC BY-SA 4.0)",
                URI = "https://creativecommons.org/licenses/by-sa/4.0",
                Identifier = "CC-BY-SA-4.0",
                Scheme = "SPDX",
                SchemeUri = "https://spdx.org/licenses"
            }
        };
        public List<MetadataSubject> Subjects { get; } = new List<MetadataSubject> {
            new MetadataSubject
            {
                Name = "Observations Database",
                Scheme = "SOFTWARE_APP",
                SchemeUri = "https://observations.saeon.ac.za"
            },
            new MetadataSubject
            {
                Name = "https://observations.saeon.ac.za",
                Scheme = "SOFTWARE_URL",
                SchemeUri = "https://observations.saeon.ac.za"
            }
        };
        public List<MetadataContributor> Contributors { get; set; } = new List<MetadataContributor>() {
            new MetadataContributor
            {
                Name = "South African Environmental Observation Network (SAEON), uLwazi node",
                Type = "Organizational",
                ContributorType = "DataCurator",
                Affiliations = new List<MetadataAffiliation>
                {
                    new MetadataAffiliation
                    {
                        Name = "South African Environmental Observation Network (SAEON)",
                        Scheme = "ROR",
                        SchemeUri = "https://ror.org/041j42q70"
                    }
                }
            },
            new MetadataContributor
            {
                Name="Parker-Nance, Tim",
                Type="Personal",
                ContributorType = "ContactPerson",
                FirstNames = "Tim",
                Surname = "Parker-Nance",
                Identifiers = new List<MetadataNameIdentifier>
                {
                    new MetadataNameIdentifier
                    {
                        Name="https://orcid.org/0000-0001-7040-7736",
                        Scheme="ORCID",
                        SchemeUri="https://orcid.org"
                    }
                },
                Affiliations = new List<MetadataAffiliation>
                {
                    new MetadataAffiliation
                    {
                        Name = "South African Environmental Observation Network (SAEON)",
                        Scheme = "ROR",
                        SchemeUri = "https://ror.org/041j42q70"
                    }
                }
            }
        };
        private DateTimeOffset? startDate;
        public DateTimeOffset? StartDate
        {
            get { return startDate; }
            set
            {
                if (startDate != value)
                {
                    startDate = value;
                    if (value.HasValue && (Parent != null) && (!Parent.StartDate.HasValue || (Parent.StartDate > value)))
                    {
                        Parent.StartDate = value;
                    }
                }
            }
        }
        private DateTimeOffset? endDate;
        public DateTimeOffset? EndDate
        {
            get => endDate;
            set
            {
                if (endDate != value)
                {
                    endDate = value;
                    if (value.HasValue && (Parent != null) && (!Parent.EndDate.HasValue || (Parent.EndDate < value)))
                    {
                        Parent.EndDate = value;
                    }
                }
            }
        }

        private double? latitudeNorth;
        public double? LatitudeNorth
        {
            get => latitudeNorth;
            set
            {
                if (latitudeNorth != value)
                {
                    latitudeNorth = value;
                    if (value.HasValue && (Parent != null) && (!Parent.LatitudeNorth.HasValue || (Parent.LatitudeNorth > value)))
                    {
                        Parent.LatitudeNorth = value;
                    }
                }
            }
        }
        private double? latitudeSouth;
        public double? LatitudeSouth
        {
            get => latitudeSouth;
            set
            {
                if (latitudeSouth != value)
                {
                    latitudeSouth = value;
                    if (value.HasValue && (Parent != null) && (!Parent.LatitudeSouth.HasValue || (Parent.LatitudeSouth < value)))
                    {
                        Parent.LatitudeSouth = value;
                    }
                }
            }
        }
        private double? longitudeWest;
        public double? LongitudeWest
        {
            get => longitudeWest;
            set
            {
                if (longitudeWest != value)
                {
                    longitudeWest = value;
                    if (value.HasValue && (Parent != null) && (!Parent.LongitudeWest.HasValue || (Parent.LongitudeWest > value)))
                    {
                        Parent.LongitudeWest = value;
                    }
                }
            }
        }
        private double? longitudeEast;
        public double? LongitudeEast
        {
            get => longitudeEast;
            set
            {
                if (longitudeEast != value)
                {
                    longitudeEast = value;
                    if (value.HasValue && (Parent != null) && (!Parent.LongitudeEast.HasValue || (Parent.LongitudeEast < value)))
                    {
                        Parent.LongitudeEast = value;
                    }
                }
            }
        }
        private double? elevationMinimum;
        public double? ElevationMinimum
        {
            get => elevationMinimum;
            set
            {
                if (elevationMinimum != value)
                {
                    elevationMinimum = value;
                    if (value.HasValue && (Parent != null) && (!Parent.ElevationMinimum.HasValue || (Parent.ElevationMinimum > value)))
                    {
                        Parent.ElevationMinimum = value;
                    }
                }
            }
        }
        private double? elevationMaximum;
        public double? ElevationMaximum
        {
            get => elevationMaximum;
            set
            {
                if (elevationMaximum != value)
                {
                    elevationMaximum = value;
                    if (value.HasValue && (Parent != null) && (!Parent.ElevationMaximum.HasValue || (Parent.ElevationMaximum < value)))
                    {
                        Parent.ElevationMaximum = value;
                    }
                }
            }
        }

        public List<MetadataRelatedIdentifier> RelatedIdentifiers { get; } = new List<MetadataRelatedIdentifier>();

        public string GenerateTitle() => GenerateTitle(DOI.Name);
        public string GenerateTitle(string name) => $"Observations in the SAEON Observations Database for {DOI.DOIType.Humanize(LetterCasing.LowerCase)} {name}";
        public string GenerateDescription() => GenerateDescription(DOI.Name);
        public string GenerateDescription(string name) => $"The observations in the SAEON Observations Database for {DOI.DOIType.Humanize(LetterCasing.LowerCase)} {name}";

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
                    description += $" on {startDate.Value.ToJsonDate()}";
                }
                else
                {
                    title += $" from {startDate.Value:yyyy-MM-dd} to {endDate.Value:yyyy-MM-dd}";
                    description += $" from {startDate.Value.ToJsonDate()} to {endDate.Value.ToJsonDate()}";
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
            var sbText = new StringBuilder();
            var sbHtml = new StringBuilder();
            sbHtml.AppendHtmlH3(title);
            sbText.AppendLine(description);
            sbHtml.AppendHtmlP(description);
            if (!string.IsNullOrWhiteSpace(ItemDescription))
            {
                var itemDescription = ItemDescription;
                if (!string.IsNullOrWhiteSpace(ItemUrl))
                {
                    if (ItemUrl.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                    {
                        itemDescription += $" <a href='{ItemUrl}'>{ItemUrl}</a>";
                    }
                    else
                    {
                        itemDescription += $" {ItemUrl}";
                    }
                }
                sbHtml.AppendHtmlP(itemDescription);
            }
            if (DOI.Parent != null)
            {
                sbText.AppendLine($"This collection is part of the {(DOI.Parent.DOIType == DOIType.ObservationsDb ? "" : DOI.Parent.DOIType.Humanize(LetterCasing.LowerCase))} {DOI.Parent.Name} doi:{DOI.Parent.DOI}");
                sbHtml.AppendHtmlP($"This collection is part of the {(DOI.Parent.DOIType == DOIType.ObservationsDb ? "" : DOI.Parent.DOIType.Humanize(LetterCasing.LowerCase))} {DOI.Parent.Name} <a href='{DOI.Parent.DOIUrl}'>{DOI.Parent.DOI}</a>");
                RelatedIdentifiers.Add(new MetadataRelatedIdentifier { Name = "IsPartOf", Identifier = DOI.Parent.DOI, Type = "DOI" });
                foreach (var subject in Subjects.Distinct())
                {
                    Parent.Subjects.AddIfNotExists(subject);
                }
            }
            // Dynamic children
            var dynamicChildren = DOI.Children.Where(i => MetadataHelper.DynamicDOITypes.Contains(i.DOIType)).OrderBy(i => i.Name).ToList();
            if (dynamicChildren.Count > 0)
            {
                sbText.AppendLine($"This collection includes observations from the following {((DOI.DOIType + 1).Humanize(LetterCasing.LowerCase).ToQuantity(dynamicChildren.Count, ShowQuantityAs.None))}: " +
                    string.Join(", ", dynamicChildren.Select(i => $"{i.Name} doi:{i.DOI}")));
                sbHtml.AppendHtmlP($"This collection includes observations from the following {((DOI.DOIType + 1).Humanize(LetterCasing.LowerCase).ToQuantity(dynamicChildren.Count, ShowQuantityAs.None))}:");
                sbHtml.AppendHtmlUL(dynamicChildren.Select(i => $"{i.Name} <a href='{i.DOIUrl}'>{i.DOI}</a>"));
                foreach (var child in dynamicChildren)
                {
                    RelatedIdentifiers.Add(new MetadataRelatedIdentifier { Name = "HasPart", Identifier = child.DOI, Type = "DOI" });
                }
            }
            // Periodic children
            // Ad-Hoc children
            sbHtml.AppendHtmlP($"{"Publisher:".HtmlB()} {Publisher} {PublicationYear}");
            if (StartDate.HasValue || EndDate.HasValue)
            {
                var dates = "Dates: ".HtmlB();
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
                sbHtml.AppendHtmlP(dates);
            }
            if (Rights.Any())
            {
                sbHtml.AppendHtmlP($"{"License:".HtmlB()} {Rights[0].Name}");
            }
            // Keyword cleanup
            var cleanSubjects = Subjects.Where(i => !i.Name.StartsWith("http")).Distinct().ToList();
            var excludes = new List<string> { "South African Environmental Observation Network", "Observations Database" };
            cleanSubjects.RemoveAll(i => excludes.Contains(i.Name));
            cleanSubjects.ForEach(i => i.Name = i.Name.Replace(",", "").Replace("  ", " "));
            sbHtml.AppendHtmlP($"{"Keywords:".HtmlB()} {string.Join(", ", cleanSubjects.OrderBy(i => i.Name).Select(i => i.Name))}");
            if (!string.IsNullOrWhiteSpace(DOI.MetadataUrl))
            {
                sbHtml.AppendHtmlP($"{"Metadata URL:".HtmlB()} <a href='{DOI.MetadataUrl}'>{DOI.MetadataUrl}</a>".Trim());
            }
            if (!string.IsNullOrWhiteSpace(DOI.QueryUrl))
            {
                sbHtml.AppendHtmlP($"{"Query URL:".HtmlB()} <a href='{DOI.QueryUrl}'>{DOI.QueryUrl}</a>".Trim());
            }
            sbHtml.AppendHtmlP($"{"Citation:".HtmlB()} {Creator.Name} ({PublicationYear}): {Title}. {Publisher}. (dataset). <a href='{DOI.DOIUrl}'>{DOI.DOIUrl}</a>");
            Description = sbText.ToString().Replace(Environment.NewLine, " ").Replace("  ", " ").Trim(); //.Replace(Environment.NewLine,"<br>");
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
                if ((LatitudeNorth == latitudeSouth) && (LongitudeWest == LongitudeEast))
                {
                    jGeoLocations.Add(
                        new JObject(
                            //new JProperty("geoLocationPlace", $"{splits[0]}, {splits[1]}"),
                            new JProperty("geoLocationPoint",
                                new JObject(
#if Schema43
                                    new JProperty("pointLatitude", LatitudeNorth),
                                    new JProperty("pointLongitude", LongitudeWest)
#else
                                    new JProperty("pointLatitude", LatitudeNorth.ToString()),
                                    new JProperty("pointLongitude", LongitudeWest.ToString())
#endif
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
#if Schema43
                                    new JProperty("westBoundLongitude", LongitudeWest),
                                    new JProperty("eastBoundLongitude", LongitudeEast),
                                    new JProperty("northBoundLatitude", LatitudeNorth),
                                    new JProperty("southBoundLatitude", LatitudeSouth)
#else
                                    new JProperty("westBoundLongitude", LongitudeWest.ToString()),
                                    new JProperty("eastBoundLongitude", LongitudeEast.ToString()),
                                    new JProperty("northBoundLatitude", LatitudeNorth.ToString()),
                                    new JProperty("southBoundLatitude", LatitudeSouth.ToString())
#endif
                                )
                            )
                        )
                    );
                }
            }
            var jObj =
                new JObject(
#if Schema43
                    new JProperty("doi", DOI.DOI),
#else
                    new JProperty("identifier", Identifier?.AsJson()),
#endif
                    new JProperty("language", Language),
#if Schema43
                    new JProperty("types", ResourceType.AsJson()),
#else
                    new JProperty("resourceType", ResourceType.AsJson()),
#endif
                    new JProperty("publisher", Publisher),
#if Schema43
                    new JProperty("publicationYear", PublicationYear),
#else
                    new JProperty("publicationYear", PublicationYear.ToString()),
#endif
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
#if Schema43
                    new JProperty("identifiers", new JArray(AlternateIdentifiers.Select(i => i.AsJson()))),
#else
                    new JProperty("alternateIdentifiers", new JArray(AlternateIdentifiers.Select(i => i.AsJson()))),
#endif
                    new JProperty("relatedIdentifiers", new JArray(RelatedIdentifiers.Select(i => i.AsJson()))),
#if Schema43
                    new JProperty("schemaVersion", "http://datacite.org/schema/kernel-4"),
#endif
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
