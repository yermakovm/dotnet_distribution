using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace DistributionAPI.Model
{
    [Table("CSs")]
    public class CS : Person
    {
        public string shift_role { get; set; }
        public string level { get; set; }
        public int weight { get; set; }

        public CS():base()
        {
            
        }

        public CS(int _id, string _name, string _shift_role, string _level, string _team, string _location) : base(_id, _name, _team, _location)
        {
            shift_role = _shift_role;
            level = _level;
            team = _team;
            location = _location;
            weight = GetWeight();
        }

        int GetWeight()
        {
            int w = 0;
            if (level == "Newbee")
                w = 10;
            if (level == "Skilled Padawan")
                w = 8;
            if (level == "Google Guru")
                w = 5;
            if (level == "Jedi Master")
                w = 2;
            return w;
        }
    }
}
