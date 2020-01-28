using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
namespace DistributionAPI.Model
{
    public class Team : IComparable<Team>, IEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual List<CSRepresentative> Teammates { get; set; }
        public int Totalweight { get; set; }

        public Guid SmeId { get; set; }
        public virtual Sme Sme { get; set; }
        public Team()
        {
            Name = "";
            Teammates = new List<CSRepresentative>();
        }

        public Team(string name, List<CSRepresentative> guys)
        {
            Name = name;
            Teammates = guys;
            Totalweight = Teammates.Sum(mate => mate.Weight);
        }

        public int CompareTo(Team t)
        {
            if (t != null) return t.Totalweight.CompareTo(this.Totalweight);
            else throw new Exception("whoops...");
        }

    }
}