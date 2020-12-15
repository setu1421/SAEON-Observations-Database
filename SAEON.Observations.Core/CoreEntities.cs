#if NET472
using Newtonsoft.Json;
#endif
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
#if NET5_0
using System.Text.Json.Serialization;
#endif

namespace SAEON.Observations.Core
{
    public static class EntityConfig
    {
        public static string BaseUrl { get; set; }
    }

    /// <summary>
    /// Absolute base class
    /// </summary>
    public abstract class BaseEntity
    {
        [NotMapped, JsonIgnore]
#if NET5_0
        [SwaggerIgnore]
#endif
        public string EntitySetName { get; protected set; } = null;
        [NotMapped, JsonIgnore]
#if NET5_0
        [SwaggerIgnore]
#endif
        public List<string> Links { get; } = new List<string>();
    }

    public abstract class GuidIdEntity : BaseEntity
    {
        /// <summary>
        /// Id of the Entity
        /// </summary>
        //[Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        //[HiddenInput]
        public Guid Id { get; set; }

        /// <summary>
        /// Navigation links of this Entity
        /// </summary>
        [NotMapped]
        public Dictionary<string, string> NavigationLinks
        {
            get
            {
                if (string.IsNullOrWhiteSpace(EntitySetName))
                    return null;
                else
                {
                    var result = new Dictionary<string, string>
                    {
                        { "Self", $"{EntityConfig.BaseUrl}/{EntitySetName}/{Id}" }
                    };
                    foreach (var link in Links.OrderBy(i => i))
                    {
                        result.Add(link, $"{EntityConfig.BaseUrl}/{EntitySetName}/{Id}/{link}");
                    }
                    return result;
                };
            }
        }
    }

    public abstract class IntIdEntity : BaseEntity
    {
        /// <summary>
        /// Id of the Entity
        /// </summary>
        //[Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        //[HiddenInput]
        public int Id { get; set; }

        /// <summary>
        /// Navigation links of this Entity
        /// </summary>
        [NotMapped]
        public Dictionary<string, string> NavigationLinks
        {
            get
            {
                if (string.IsNullOrWhiteSpace(EntitySetName))
                    return null;
                else
                {
                    var result = new Dictionary<string, string>
                    {
                        { "Self", $"{EntityConfig.BaseUrl}/{EntitySetName}/{Id}" }
                    };
                    foreach (var link in Links.OrderBy(i => i))
                    {
                        result.Add(link, $"{EntityConfig.BaseUrl}/{EntitySetName}/{Id}/{link}");
                    }
                    return result;
                };
            }
        }
    }

    public abstract class LongIdEntity : BaseEntity
    {
        /// <summary>
        /// Id of the Entity
        /// </summary>
        //[Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        //[HiddenInput]
        public long Id { get; set; }

        /// <summary>
        /// Navigation links of this Entity
        /// </summary>
        [NotMapped]
        public Dictionary<string, string> NavigationLinks
        {
            get
            {
                if (string.IsNullOrWhiteSpace(EntitySetName))
                    return null;
                else
                {
                    var result = new Dictionary<string, string>
                    {
                        { "Self", $"{EntityConfig.BaseUrl}/{EntitySetName}/{Id}" }
                    };
                    foreach (var link in Links.OrderBy(i => i))
                    {
                        result.Add(link, $"{EntityConfig.BaseUrl}/{EntitySetName}/{Id}/{link}");
                    }
                    return result;
                };
            }
        }
    }

    /// <summary>
    /// Base for entities
    /// </summary>
    public abstract class IdedEntity : GuidIdEntity
    {
        [JsonIgnore, Timestamp, Column(Order = 10000), ConcurrencyCheck, ScaffoldColumn(false)]
        //[HiddenInput]
        [IgnoreDataMember]
        public byte[] RowVersion { get; set; }
        [JsonIgnore, Required]
        [IgnoreDataMember]
        public Guid UserId { get; set; }
    }

    public abstract class CodedEntity : IdedEntity
    {
        /// <summary>
        /// Code of the Entity
        /// </summary>
        [Required, StringLength(50)]
        public virtual string Code { get; set; }
    }

    public abstract class NamedEntity : CodedEntity
    {
        /// <summary>
        /// Name of the Entity
        /// </summary>
        [Required, StringLength(150)]
        public string Name { get; set; }
    }

    /// <summary>
    /// UserDownload entity
    /// </summary>
    public class UserDownload : NamedEntity
    {
        /// <summary>
        /// Description of the UserDownload
        /// DataCite Abstracts, should include citation format
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// Title of the DownLoad
        /// DataCite Titles
        /// </summary>
        [Required, StringLength(5000)]
        public string Title { get; set; }
        /// <summary>
        /// How this DownLoad should be cited
        /// </summary>
        [Required, StringLength(5000)]
        public string Citation { get; set; }
        /// <summary>
        /// Keywords of the Download, semi-colon separated
        /// </summary>
        [Required, StringLength(1000)]
        public string Keywords { get; set; }
        /// <summary>
        /// When the query for the download was executed
        /// </summary>
        [Required]
        public DateTime Date { get; set; }
        /// <summary>
        /// The input of the query that generated the download
        /// </summary>
        [Required, StringLength(5000), Display(Name = "Input")]
        public string Input { get; set; }
        /// <summary>
        /// Requery Url for download
        /// </summary>
        [Required, StringLength(2000), Display(Name = "Requery Url")]
        public string RequeryUrl { get; set; }
        ///// <summary>
        ///// DigitalObjectIdentifierID of the download
        ///// </summary>
        //[Required, Display(Name = "Digital Object Identifier (DOI)")]
        //public int DigitalObjectIdentifierId { get; set; }
        /// <summary>
        /// Json sent to metadata service
        /// </summary>
        [Required, Display(Name = "Metadata Json")]
        public string MetadataJson { get; set; }
        /// <summary>
        /// Metadata Url for download
        /// </summary>
        [Required, StringLength(2000), Display(Name = "Metadata Url")]
        public string MetadataUrl { get; set; }
        /// <summary>
        /// ODP Id for the download
        /// </summary>
        public Guid OpenDataPlatformId { get; set; }
        /// <summary>
        /// Url to view the download
        /// </summary>
        [Required, StringLength(2000), Display(Name = "Download Url")]
        public string DownloadUrl { get; set; }
        /// <summary>
        /// Full file name of Zip on server
        /// </summary>
        [Required, StringLength(2000), Display(Name = "Zip Full Name")]
        public string ZipFullName { get; set; }
        /// <summary>
        /// SHA256 checksum of Zip
        /// </summary>
        [Required, StringLength(64), Display(Name = "Zip SHA256 Checksum")]
        public string ZipCheckSum { get; set; }
        /// <summary>
        /// Url to Zip of the download
        /// </summary>
        [Required, StringLength(2000), Display(Name = "Zip Url")]
        public string ZipUrl { get; set; }
        /// <summary>
        /// Places of the DownLoad
        /// Lookup on GeoNames in format Name:Country:Lat:Lon, semi-colon separated
        /// </summary>
        [Required, StringLength(5000)]
        public string Places { get; set; }
        /// <summary>
        /// North-most Latitude of the download
        /// </summary>
        public double? LatitudeNorth { get; set; } // +N to -S
        /// <summary>
        /// South-most Latitude of the download
        /// </summary>
        public double? LatitudeSouth { get; set; } // +N to -S
        /// <summary>
        /// West-morthmost Longitude of the download
        /// </summary>
        public double? LongitudeWest { get; set; } // -W to +E
        /// <summary>
        /// East-morthmost Longitude of the download
        /// </summary>
        public double? LongitudeEast { get; set; } // -W to +E
        /// <summary>
        /// Minimum elevation of the download
        /// </summary>
        public double? ElevationMinimum { get; set; }
        /// <summary>
        /// Maximum elevation of the download
        /// </summary>
        public double? ElevationMaximum { get; set; }
        /// <summary>
        /// Start date of the download
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// End date of the download
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// UserId of the UserDownload
        /// </summary>
        [ScaffoldColumn(false)]
        //[HiddenInput]
        public new string UserId { get; set; }
        /// <summary>
        /// UserId of user who added the UserDownload
        /// </summary>
        [Required, StringLength(128), ScaffoldColumn(false)]
        public string AddedBy { get; set; }
        /// <summary>
        /// Time the UserDownload was added
        /// </summary>
        [ScaffoldColumn(false)]
        public DateTime? AddedAt { get; set; }
        ///// <summary>
        ///// UserId of user who last updated the UserDownload
        ///// </summary>
        [Required, StringLength(128), ScaffoldColumn(false)]
        public string UpdatedBy { get; set; }
        /// <summary>
        /// Time the UserDownload was updated
        /// </summary>
        [ScaffoldColumn(false)]
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        //public DigitalObjectIdentifier DigitalObjectIdentifier { get; set; }
    }

    /// <summary>
    /// UserQuery entity
    /// </summary>
    public class UserQuery : NamedEntity
    {
        /// <summary>
        /// Description of the UserQuery
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// URI of the user query
        /// </summary>
        [Required, StringLength(5000), Display(Name = "Input")]
        public string QueryInput { get; set; }
        /// <summary>
        /// UserId of UserQuery
        /// </summary>
        [ScaffoldColumn(false)]
        //[HiddenInput]
        public new string UserId { get; set; }
        /// <summary>
        /// UserId of user who added the UserQuery
        /// </summary>
        [StringLength(128), ScaffoldColumn(false)]
        public string AddedBy { get; set; }
        /// <summary>
        /// Time the UserDownload was added
        /// </summary>
        [ScaffoldColumn(false)]
        public DateTime? AddedAt { get; set; }
        /// <summary>
        /// UserId of user who last updated the UserQuery
        /// </summary>
        [StringLength(128), ScaffoldColumn(false)]
        public string UpdatedBy { get; set; }
        /// <summary>
        /// Time the UserDownload was updated
        /// </summary>
        [ScaffoldColumn(false)]
        public DateTime? UpdatedAt { get; set; }
    }
}
