using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public int DepartmentId { get; set; }
        public string city { get; set; }
        public virtual List<xPath> xPaths { get; set; }

        public virtual Guid LocationStackId { get; set; }
        public virtual LocationStack LocationStack { get; set; }

        public Location() { }
        public Location(string _city, List<xPath> _shifts, int _id)
        {
            city = _city;
            xPaths = _shifts;
            DepartmentId = _id;
        }
    }
}
