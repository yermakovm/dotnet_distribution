using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DistributionAPI.Model.LocationModels;

namespace DistributionAPI.Model.LocationModels
{
    public class ShiftRoles
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ShiftRoles() { }

        public ShiftRoles(string name)
        {
            Name = name;
        }
    }
}
