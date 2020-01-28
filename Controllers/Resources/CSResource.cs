
namespace DistributionAPI.Controllers.Resources
{
    public class CSRepresentativeResource : PersonResource
    {
        public string Shiftrole { get; set; }
        public string Level { get; set; }
        public int Weight { get; set; }

        public CSRepresentativeResource() : base()
        {

        }
    }
}
