
namespace DistributionAPI.Model
{
    public class Person
    {
        public int id;
        public string name;
        public string shift_role;
        public string level;
        public string team;
        public int weight;

        public Person(int _id, string _name, string _shift_role, string _level, string _team)
        {
            id = _id;
            name = _name;
            shift_role = _shift_role;
            level = _level;
            team = _team;
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