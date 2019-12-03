using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributionAPI.Model
{
    class TeamComparer : IEqualityComparer<Team>
    {
        public bool Equals(Team a, Team b)
        {
            bool e = a.name.Equals(b.name);
            return e;
        }
        public int GetHashCode(Team t)
        {
            return t.name.GetHashCode();
        }
    }
}
