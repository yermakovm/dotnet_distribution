using System.Collections.Generic;

namespace DistributionAPI.Model
{
    public class Sme : Person
    {
        public int load;
        public List<Team> teams = new List<Team>();
        public Sme(int _id, string _name, string _shift_role, string _level, string _team) : base(_id, _name, _shift_role, _level, _team)
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