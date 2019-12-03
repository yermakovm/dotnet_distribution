using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                    if (a[i].name != b[i].name)
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
