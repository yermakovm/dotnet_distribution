using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DistributionAPI.Model.LocationModels;

namespace DistributionAPI.Model
{
    [Table("Departments")]
    public class Department: IEntity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("AllowedRoles")]
        public virtual List<ShiftRoles> allowedRolesList { get; set; }
        [ForeignKey("DisallowedRoles")]
        public virtual List<ShiftRoles> disallowedRolesList { get; set; }

    public virtual List<Location> locations { get; set; }


        public Department()
        {

        }
        public Department(string name, List<Location> locs)
        {
            Name = name;
            locations = locs;
        }
    }
}
