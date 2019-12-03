using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
namespace DistributionAPI.Model
{
    [Table("SMEs")]
    public class Sme : Person, IComparable<Sme>
    {
        public int load { get; set; }
        public virtual List<Team> teams { get; set; } = new List<Team>();
        public int curLoad { get; set; }
        public virtual Guid DistributionDataId { get; set; }
        public virtual DistributionData DistributionData { get; set; }
        public Sme() : base(0, "","","")
        {

        }
        public Sme(int _id, string _name, string team, string _location) : base(_id, _name, team, _location)
        {
            
        }
        public void GetLoad()
        {
            foreach (var team in teams)
            {
                load+=team.total_weight;
            }
        }
        public int CompareTo(Sme compared)
        {
            int result = (load >= compared.load) ?  1 : -1; ;
            
            return result;
        }
    }
}