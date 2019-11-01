using System;
using System.Collections.Generic;
using System.Linq;
namespace DistributionAPI.Model
{
    public class Team : IComparable<Team>
    {
        public string name;
        public List<Person> teammates;
        public int total_weight;

        public Team(string _name, List<Person> guys)
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

        public bool SameLocation(Team t)
        {
            if ((name.Contains("Lviv") && t.name.Contains("Lviv")) ||
            (name.Contains("Dnipro") && t.name.Contains("Dnipro")) ||
            (name.Contains("Kharkiv") && t.name.Contains("Kharkiv")))
                return true;
            else return false;
        }
    }
}