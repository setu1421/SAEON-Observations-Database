namespace SAEON.Observations.WebAPI.Models
{
    public class HealthModel
    {
        public string IdentityService { get; set; } = "Ok";
        public string Database { get; set; } = "Ok";
        public bool Healthy { get; set; } = true;
    }
}