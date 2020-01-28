using System.Collections.Generic;
using DistributionAPI.Model;

namespace DistributionAPI.Controllers.Resources
{
    public class DistributionResource
    {
        public List<SmeResource> smes = new List<SmeResource>();
        public List<TeamResource> raw__schedule = new List<TeamResource>();
        public int average_load;
        public List<List<TeamResource>> distributed_teamlist = new List<List<TeamResource>>();
        public Department Department { get; set; }
    }
    }
