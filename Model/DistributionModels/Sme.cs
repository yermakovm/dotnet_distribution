using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;
namespace DistributionAPI.Model
{
    [Table("SmeList")]
    public class Sme : Person, IComparable<Sme>
    {
        public int Load { get; set; }
        public virtual List<Team> Teams { get; set; } = new List<Team>();
        public int CurLoad { get; set; }
        public virtual Guid DistributionDataId { get; set; }
        public virtual DistributionData DistributionData { get; set; }
        public Sme() : base(0, "","","","")
        {

        }
        public Sme(int id, string name, string team, string location, string avatar) : base(id, name, team, location, avatar)
        {
            
        }
        public void GetLoad()
        {
            foreach (var team in Teams)
            {
                Load+=team.Totalweight;
            }
        }
        public int CompareTo(Sme compared)
        {
            int result = (Load >= compared.Load) ?  1 : -1; ;
            
            return result;
        }
        public void AddTeam(Team t)
        {
            this.Teams.Add(t);
            this.CurLoad -= t.Totalweight;
            this.Load += t.Totalweight;
        }
        public void RemoveTeam(Team t)
        {
            this.CurLoad += t.Totalweight;
            this.Load -= t.Totalweight;
            this.Teams.Remove(t);
        }
    }
}