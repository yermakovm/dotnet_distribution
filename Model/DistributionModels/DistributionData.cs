using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DistributionAPI.Model
{
    public class DistributionData : IEntity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTime time { get; set; }
        public int departmentId { get; set; }
        public virtual List<Sme> smes { get; set; }
    }
}
