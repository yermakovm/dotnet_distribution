using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DistributionAPI.Model;
namespace DistributionAPI.Controllers.Resources
{
    public class DistributionDataResource {
        public DateTime Time { get; set; }
        public virtual DepartmentResource Department { get; set; }
        public string periodName { get; set; }
        public string period { get; set; }
    }
}
