using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributionAPI.Controllers.Resources
{
    public class CSResource : PersonResource
    {
        public string shift_role { get; set; }
        public string level { get; set; }
        public string team { get; set; }
        public int weight { get; set; }

        public CSResource() : base()
        {

        }
    }
}
