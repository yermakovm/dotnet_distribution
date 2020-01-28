using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
namespace DistributionAPI.Model
{ 
    public abstract class Person : IEntity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int Z3kId { get; set; }
        public string Name { get; set; }
        public string Team { get; set; }
        public string Location { get; set; }
        public string Avatar { get; set; }
        protected Person()
        {
            Z3kId = 0;
            Name = "noname";
        }

        public Person(int id, string name, string team,string location, string avatar)
        {
            Z3kId = id;
            Name = name;
            Team = team;
            Location = location;
            Avatar = avatar;
        }
        public bool SameLocation(string teamname)
        {
            string teamlocation;
            if (teamname != null&&teamname != "OX" && teamname!="Overshifts")
            {
                teamlocation = teamname.Substring(0, teamname.ToString().IndexOf(" "));
                return this.Location == teamlocation;
            }
            else return false;
        }
    }
}