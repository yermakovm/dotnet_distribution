using System;
using System.Collections.Generic;
using System.Linq;

namespace DistributionAPI.Model
{
    interface IDistributionBuilder
    {
        void Build();
    }

    public class Distribution : IDistributionBuilder
    {

        public List<Sme> smes;
        public List<Team> raw_schedule;
        public int average_load;
        public List<List<Team>> distributed_teamlist = new List<List<Team>>();

        public Distribution(List<Team> _raw_schedule, List<Sme> _sme)
        {
            raw_schedule = _raw_schedule;
            raw_schedule.Sort();
            smes = _sme;
            average_load = GetListWeight(raw_schedule) / smes.Count();
        }

        private void GetPossibleOptions(List<Team> cs_teams, List<Team> part)
        {
            int s = GetListWeight(part);

            if (Math.Abs(s - average_load) < average_load)
            {
                distributed_teamlist.Add(part);
            }

            for (int i = 0; i < cs_teams.Count; i++)
            {
                List<Team> remaining = new List<Team>();
                Team n = cs_teams[i];
                for (int j = i + 1; j < cs_teams.Count; j++) remaining.Add(cs_teams[j]);

                List<Team> partial_rec = new List<Team>(part);
                partial_rec.Add(n);
                GetPossibleOptions(remaining, partial_rec);
            }
        }

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

        public int GetListWeight(List<Team> team)
        {
            return team.Sum(y => y.total_weight);
        }

        private void PrintDistribution()
        {
            foreach (var sme in smes)
            {
                Console.WriteLine();
                foreach (var team in sme.teams)
                {
                    Console.Write(team.name + " ");
                }
                Console.WriteLine(GetListWeight(sme.teams));
            }
        }

        private bool SingleLocation(List<Team> list)
        {
            bool res = true;
            for (int i = 0; i < list.Count; i++)
                for (int j = i; j < list.Count; j++)
                {
                    if (!list[i].SameLocation(list[j]))
                        res = false;
                }
            return res;
        }
        private void FilterOptions(List<List<Team>> source)
        {
            List<List<Team>> temp = source;
            distributed_teamlist = temp;
            var test = distributed_teamlist.Where(x => x.Where(y => y.name == "Dnipro Team A").Any()).ToList();
            //iterating through the team arrays
            for (int i = 0; i < source.Count; i++)
            {
                bool foundsame = false;
                List<List<Team>> same = new List<List<Team>>();
                //iterating through the rest of team arrays after i
                for (int j = i; j < source.Count; j++)
                {
                    //finding team arrays having common names
                    bool hasSameTeams = source[i].Intersect(source[j], new TeamComparer()).Any();
                    if (hasSameTeams)
                    {
                        same.Add((source[j]));
                    }
                }

                //finding the lightest team array among similar
                if (same.Count > 1)
                {
                    foundsame = true;
                    List<Team> min = same.First();
                    foreach (var same_team in same)
                    {
                        if (Math.Abs(GetListWeight(min) - average_load) > Math.Abs(GetListWeight(same_team) - average_load))
                        {
                            min = same_team;
                        }
                    }
                    //removing all but the lightest from source
                    TeamListComparer tlc = new TeamListComparer();
                    same.RemoveAll(x => tlc.Equals(min, x));
                    foreach (var same_team in same)
                        temp.RemoveAll(x => new TeamListComparer().Equals(same_team, x));
                }
                if (foundsame)
                    break;
            }
            //do the same again if the list is too big
            if (temp.Count > smes.Count)
            {
                FilterOptions(temp);
            }
            //
            else if (temp.Count < smes.Count)
            {
                var longlist = temp.Where(x => x.Count > 1).ToList();
                if (longlist.Count > 0)
                {
                    var to_split = longlist.First();
                    temp.Remove(to_split);
                    var new_cell = to_split.First();
                    to_split.Remove(new_cell);
                    var separated = new List<List<Team>>();
                    var new_list = new List<Team>();
                    new_list.Add(new_cell);
                    separated.Add(new_list);
                    temp.Add(to_split);
                    temp.Add(new_list);
                }
            }
        }

        private void Distribute()
        {
            for (int i = 0; i < smes.Count; i++)
            {
                smes[i].teams = distributed_teamlist[i];
                smes[i].GetLoad();
            }
        }

        public void Build()
        {
            GetPossibleOptions(raw_schedule, new List<Team>());
            FilterOptions(distributed_teamlist);
            Distribute();
        }
    }
}