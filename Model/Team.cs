using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace DistributionAPI.Model
{
    public class Team : IComparable<Team>, IEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string name { get; set; }
        public virtual List<CS> teammates { get; set; }
        public int total_weight { get; set; }

        public Guid SmeId { get; set; }
        public virtual Sme Sme { get; set; }
        public Team()
        {
            name = "";
            teammates = new List<CS>();
        }

        public Team(string _name, List<CS> guys)
        {
            name = _name;
            teammates = guys;
            total_weight = teammates.Sum(mate => mate.weight);
        }

        public int CompareTo(Team t)
        {
            if (t != null) return t.total_weight.CompareTo(this.total_weight);
            else throw new Exception("whoops...");
        }

    }
}