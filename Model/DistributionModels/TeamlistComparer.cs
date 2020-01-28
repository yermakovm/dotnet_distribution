using System.Collections.Generic;

namespace DistributionAPI.Model
{

    class TeamListComparer : IEqualityComparer<List<Team>>
    {
        public bool Equals(List<Team> a, List<Team> b)
        {
            bool e = false;
            if (a.Count == b.Count)
            {
                for (int i = 0; i < a.Count; i++)
                {
                    if (a[i].Name != b[i].Name)
                    {
                        e = false;
                        break;
                    }
                    else e = true;
                }
            }
            return e;
        }

        public int GetHashCode(List<Team> t)
        {
            return t.GetHashCode();
        }
    }

}
