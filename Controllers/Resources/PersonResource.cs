using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributionAPI.Controllers.Resources
{
    public class PersonResource
    {
        public int z3kid { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public PersonResource()
        {
            z3kid = 0;
            name = "noname";
        }

    }
}
