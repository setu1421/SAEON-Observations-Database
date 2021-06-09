using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAEON.Observations.Core
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
            if (!string.IsNullOrWhiteSpace(Identifier))
            {
                jObj.Add(new JProperty("affiliationIdentifier", Identifier));
            }
            if (!string.IsNullOrWhiteSpace(Scheme))
            {
                jObj.Add(new JProperty("affiliationIdentifierScheme", Scheme));
            }
            if (!string.IsNullOrWhiteSpace(SchemeUri))
            {
                jObj.Add(new JProperty("schemeURI", SchemeUri));
            }
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
                jObj.Add(new JProperty("affiliation", new JArray(Affiliations.Select(i => i.AsJson()))));
                // Pre Schema 4.3
                //jObj.Add(new JProperty("affiliations", new JArray(Affiliations.Select(i => i.AsJson()))));
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

    public class MetadataCore
    {
        public MetadataCore Parent { get; set; }
        //public DigitalObjectIdentifier DOI { get; set; }
        //public MetadataIdentifier Identifier => DOI == null ? null : new MetadataIdentifier { Name = DOI.DOI, Type = "DOI" };
        public List<MetadataAlternateIdentifier> AlternateIdentifiers { get; } = new List<MetadataAlternateIdentifier>();
        public MetadataCreator Creator = new MetadataCreator
        {
            Name = "SAEON Observations Database",
            Type = "Organizational",
            Identifiers = new List<MetadataNameIdentifier> { new MetadataNameIdentifier {
                Name = "https://ror.org/041j42q70", Scheme = "ROR", SchemeUri = "https://ror.org" } },
            Affiliations = new List<MetadataAffiliation> { new MetadataAffiliation { Name = "South African Environmental Observation Network (SAEON), PO Box 2600, Pretoria, 0001, South Africa" } }
        };
        public string Language { get; set; } = "en-za";
        public MetadataResourceType ResourceType { get; } = new MetadataResourceType { Name = "Dataset", Type = "Observations" };
        public string Publisher { get; set; } = "South African Environmental Observation Network (SAEON)";
        public DateTime? PublicationDate { get; set; }
        public int? PublicationYear => PublicationDate?.Year ?? EndDate?.Year ?? StartDate?.Year;
        public string Title { get; set; }
        public string Description { get; set; }
        public string DescriptionHtml { get; set; }
        public string Citation { get; set; }
        public string CitationHtml { get; set; }

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
                Name = "South African Environmental Observation Network (SAEON)",
                Type = "Organizational",
                ContributorType = "Distributor",
                Affiliations = new List<MetadataAffiliation>
                {
                    new MetadataAffiliation
                    {
                        Name = "PO Box 2600, Pretoria, 0001, South Africa",
                        Identifier = "https://ror.org/041j42q70",
                        Scheme = "ROR",
                        SchemeUri = "https://ror.org"
                    }
                }
            },
            new MetadataContributor
            {
                Name = "uLwazi Node",
                Type = "Organizational",
                ContributorType = "DataCurator",
                Affiliations = new List<MetadataAffiliation>
                {
                    new MetadataAffiliation
                    {
                        Name = "South African Environmental Observation Network (SAEON), PO Box 2600, Pretoria, 0001, South Africa",
                        Identifier = "https://ror.org/041j42q70",
                        Scheme = "ROR",
                        SchemeUri = "https://ror.org"
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
                        Name = "Elwandle Coastal Node, South African Environmental Observation Network (SAEON), PO Box 2600, Pretoria, 0001, South Africa",
                        Identifier = "https://ror.org/041j42q70",
                        Scheme = "ROR",
                        SchemeUri = "https://ror.org"
                    }
                }
            }
        };
        public DateTimeOffset? Accessed { get; set; }
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
    }
}
