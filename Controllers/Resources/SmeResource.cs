using System.Collections.Generic;


namespace DistributionAPI.Controllers.Resources
{
    public class SmeResource : PersonResource
    {
        public int Load { get; set; }
        public List<TeamResource> Teams { get; set; } = new List<TeamResource>();

        public SmeResource() : base()
        {

        }
    }
}
