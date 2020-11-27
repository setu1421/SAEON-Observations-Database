namespace SAEON.Observations.QuerySite.Models
{
    public class HealthModel
    {
        public string AuthenticationServer { get; set; } = "Ok";
        public string WebApi { get; set; } = "Ok";
        public bool Healthy { get; set; } = true;
    }
}