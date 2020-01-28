using System.ComponentModel.DataAnnotations.Schema;

namespace DistributionAPI.Model
{
    [Table("CSs")]
    public class CSRepresentative : Person
    {
        public string ShiftRole { get; set; }
        public string Level { get; set; }
        public int Weight { get; set; }

        public CSRepresentative():base()
        {
            
        }

        public CSRepresentative(int id, string name, string shiftrole, string level, string team, string location, string avatar) : base(id, name, team, location, avatar)
        {
            ShiftRole = shiftrole;
            Level = level;
            Team = team;
            Location = location;
            Weight = GetWeight();
        }

        int GetWeight()
        {
            int w = 0;
            switch (Level)
            {
                case "Newbee":
                    w = 10;
                    break;
                case "Skilled Padawan":
                    w = 8;
                    break;
                case "Google Guru":
                    w = 6;
                    break;
                case "Jedi Master":
                    w = 3;
                    break;
            }
            return w;
        }
    }
}
