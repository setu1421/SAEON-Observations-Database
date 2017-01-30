using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.Core
{

    /// <summary>
    /// Instrument Data Transfer Object
    /// </summary>
    public class InstrumentDTO
    {
        /// <summary>
        /// Id of the Instrument
        /// </summary>
        [Required]
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// Code of the Instrument
        /// </summary>
        [Required, StringLength(50)]
        public string Code { get; set; }
        /// <summary>
        /// Name of the Instrument
        /// </summary>
        [Required, StringLength(150)]
        public string Name { get; set; }
        /// <summary>
        /// Description of the Instrument
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// Url of the Instrument
        /// </summary>
        [Url, StringLength(250)]
        public string Url { get; set; }
        /// <summary>
        /// StartDate of the Instrument, null means always
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// EndDate of the Instrument, null means always
        /// </summary>
        public DateTime? EndDate { get; set; }
    }

    /// <summary>
    /// Site Data Transfer Object
    /// </summary>
    public class SiteDTO
    {
        /// <summary>
        /// Id of the Site
        /// </summary>
        [Required]
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// Code of the Site
        /// </summary>
        [Required, StringLength(50)]
        public string Code { get; set; }
        /// <summary>
        /// Name of the Site
        /// </summary>
        [Required, StringLength(150)]
        public string Name { get; set; }
        /// <summary>
        /// Description of the Site
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// Url of the Site
        /// </summary>
        [Url, StringLength(250)]
        public string Url { get; set; }
        /// <summary>
        /// StartDate of the Site, null means always
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// EndDate of the Site, null means always
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// The Stations linked to this Site
        /// </summary>
        public List<StationDTO> Stations { get; set; }
    }

    /// <summary>
    /// Station Data Transfer Object
    /// </summary>
    public class StationDTO
    {
        /// <summary>
        /// Id of the Station
        /// </summary>
        [Required]
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// The SiteId of the Station
        /// </summary>
        [Required]
        public Guid SiteId { get; set; }
        /// <summary>
        /// The Site this Station is linked to
        /// </summary>
        public SiteDTO Site { get; set;}
        /// <summary>
        /// Code of the Station
        /// </summary>
        [Required, StringLength(50)]
        public string Code { get; set; }
        /// <summary>
        /// Name of the Station
        /// </summary>
        [Required, StringLength(150)]
        public string Name { get; set; }
        /// <summary>
        /// Description of the Station
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// Url of the Station
        /// </summary>
        [Url, StringLength(250)]
        public string Url { get; set; }
        /// <summary>
        /// StartDate of the site, null means always
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// EndDate of the Station, null means always
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// Latitude of the Station
        /// </summary>
        public double? Latitude { get; set; }
        /// <summary>
        /// Logitude of the Station
        /// </summary>
        public double? Longitude { get; set; }
        /// <summary>
        /// Elevation of the Station, positive above sea level, negative below sea level
        /// </summary>
        public double? Elevation { get; set; }
    }

    /// <summary>
    /// UserDownload Data Transfer Object
    /// </summary>
    public class UserDownloadDTO
    {
        /// <summary>
        /// Id of the user download
        /// </summary>
        [Required]
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// UserId of user query
        /// </summary>
        [Required, StringLength(128)]
        public string UserId { get; set; }
        /// <summary>
        /// Name of the user download
        /// </summary>
        [Required, StringLength(150)]
        public string Name { get; set; }
        /// <summary>
        /// Description of the user download
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }
        /// <summary>
        /// The URI of the original query that generated the download
        /// </summary>
        [StringLength(500)]
        public string QueryURI { get; set; }
        /// <summary>
        /// URI of the user download
        /// </summary>
        [Required, StringLength(500)]
        public string DownloadURI { get; set; }
    }

    /// <summary>
    /// User Query Data Transfer Object
    /// </summary>
    public class UserQueryDTO
    {
        /// <summary>
        /// Id of the user query
        /// </summary>
        [Required]
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// UserId of user query
        /// </summary>
        [Required, StringLength(128)]
        public string UserId { get; set; }
        /// <summary>
        /// Name of the user query
        /// </summary>
        [Required, StringLength(150)]
        public string Name { get; set; }
        /// <summary>
        /// Description of the user query
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }
        /// <summary>
        /// URI of the user query
        /// </summary>
        [Required, StringLength(500)]
        public string URI { get; set; }
    }

}
