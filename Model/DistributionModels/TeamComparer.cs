using System.Collections.Generic;

namespace DistributionAPI.Model
{
    class TeamComparer : IEqualityComparer<Team>
    {
        public bool Equals(Team a, Team b)
        {
            bool e = a.Name.Equals(b.Name);
            return e;
        }
        public int GetHashCode(Team t)
        {
            return t.Name.GetHashCode();
        }
    }
}
