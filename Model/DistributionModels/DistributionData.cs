using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DistributionAPI.Model
{
    public class DistributionData : IEntity
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTime Time { get; set; }
        public string periodName { get; set; }
        public string period { get; set; }
        public virtual List<Sme> SmeList { get; set; }
        public virtual Department Department { get; set; }
    }
}
