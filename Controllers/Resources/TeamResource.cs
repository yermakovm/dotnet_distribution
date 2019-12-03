using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributionAPI.Controllers.Resources
{
    public class TeamResource
    {
        public string name { get; set; }
        public List<CSResource> teammates { get; set; }
        public int total_weight { get; set; }

        public TeamResource()
        {
            name = "undefined";
            teammates = new List<CSResource>();
        }

    }
}
