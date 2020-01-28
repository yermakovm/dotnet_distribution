using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DistributionAPI.Model
{
    [Table("Locations")]
    public class Location
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public virtual int Z3kId { get; set; }
        public string City { get; set; }
        public virtual List<ShiftPeriod> XPaths { get; set; }

        public virtual Department Department { get; set; }

        public Location() { }
        public Location(string city, List<ShiftPeriod> shifts, int id)
        {
            City = city;
            XPaths = shifts;
            Z3kId = id;
        }
    }
}
