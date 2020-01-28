using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributionAPI.Controllers.Resources
{
    public class LocationResource
    {
        public virtual int Z3kId { get; set; }
        public string city { get; set; }
        public virtual List<ShiftPeriodResource> XPaths { get; set; }

        public virtual DepartmentResource Department { get; set; }

        public LocationResource() { }
    }
}
