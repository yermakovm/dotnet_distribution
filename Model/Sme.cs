using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
namespace DistributionAPI.Model
{
    [Table("SMEs")]
    public class Sme : Person
    {
        public int load { get; set; }
        public virtual List<Team> teams { get; set; }

        public virtual Guid DistributionDataId { get; set; }
        public virtual DistributionData DistributionData { get; set; }
        public Sme() : base(0, "")
        {

        }
        public Sme(int _id, string _name) : base(_id, _name)
        {
            
        }
        public void GetLoad()
        {
            foreach (var team in teams)
            {
                load+=team.total_weight;
            }
        }
    }
}