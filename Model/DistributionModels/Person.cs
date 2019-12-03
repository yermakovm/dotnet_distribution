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
        public int z3kid { get; set; }
        public string name { get; set; }
        public string team { get; set; }
        public string location { get; set; }
        public string avatar { get; set; }
        protected Person()
        {
            z3kid = 0;
            name = "noname";
        }

        public Person(int _id, string _name, string _team,string _location, string _avatar)
        {
            z3kid = _id;
            name = _name;
            team = _team;
            location = _location;
            avatar = _avatar;
        }
        public bool SameLocation(string teamname)
        {
            string teamlocation;
            if (teamname != "OX" && teamname!="Overshifts")
            {
                teamlocation = teamname.Substring(0, teamname.ToString().IndexOf(" "));
                return this.location == teamlocation;
            }
            else return false;
        }
    }
}