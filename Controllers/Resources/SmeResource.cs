using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributionAPI.Controllers.Resources
{
    public class SmeResource : PersonResource
    {
        public int load { get; set; }
        public List<TeamResource> teams { get; set; } = new List<TeamResource>();

        public SmeResource() : base()
        {

        }
    }
}
