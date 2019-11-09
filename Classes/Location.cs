using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributionAPI.Classes
{
    public class Location
    {
        public string city;
        public List<string> shifts;
        public string url;

        public Location(string _city, List<string> _shifts, string _url)
        {
            city = _city;
            shifts = _shifts;
            url = _url;
        }
    }
}
