using System.Collections.Generic;

namespace DistributionAPI.Controllers.Resources
{
    public class TeamResource
    {
        public string Name { get; set; }
        public List<CSRepresentativeResource> Teammates { get; set; }
        public int Totalweight { get; set; }

        public TeamResource()
        {
            Name = "undefined";
            Teammates = new List<CSRepresentativeResource>();
        }

    }
}
