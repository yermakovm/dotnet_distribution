using System;
using System.Collections.Generic;
using System.Linq;

namespace DistributionAPI.Model
{


    public class Distribution
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

        private List<List<Team>> SplitOption(List<List<Team>> options)
        {
            var initial = options.Where(x => x.Count > 1).ToList();
            //mutate options
            if (initial.Count > 0)
            {
                var splitted_teamlist = initial.First();
                options.Remove(splitted_teamlist);
                var splitted_team = splitted_teamlist.First();
                splitted_teamlist.Remove(splitted_team);
                var separated = new List<List<Team>>();
                var new_teamlist = new List<Team>();
                new_teamlist.Add(splitted_team);
                separated.Add(new_teamlist);
                options.Add(splitted_teamlist);
                options.Add(new_teamlist);
            }
            return options;
        }

        private bool ProcessSimilarOptions(List<List<Team>> same, List<List<Team>> teamlist)
        {
            bool foundsame = false;
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
                TeamListComparer teamsComparer = new TeamListComparer();
                same.RemoveAll(x => teamsComparer.Equals(min, x));
                foreach (var same_team in same)
                    teamlist.RemoveAll(x => new TeamListComparer().Equals(same_team, x));
            }
            return foundsame;
        }

        private void FilterOptions(List<List<Team>> source)
        {
            List<List<Team>> temp = source;
            distributed_teamlist = temp;
            bool foundsame = false;
            //iterating through the team arrays
            for (int i = 0; i < source.Count && !foundsame; i++)
            {
                List<List<Team>> same = new List<List<Team>>();
                //iterating through the rest of team arrays after i
                for (int j = i; j < source.Count; j++)
                {
                    //finding team arrays having common names
                    bool hasSameTeams = source[i].Intersect(source[j], new TeamComparer()).Any();
                    if (hasSameTeams)
                        same.Add((source[j]));
                }
                //finding the lightest team array among similar
                foundsame = ProcessSimilarOptions(same, temp);
            }
            //do the same again if the list is too big
            if (temp.Count > smes.Count)
            {
                FilterOptions(temp);
            }
            //or too small
            else if (temp.Count < smes.Count)
            {
                temp = SplitOption(temp);
                FilterOptions(temp);
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

        public void Build(int[] id = null)
        {
            GetPossibleOptions(raw_schedule, new List<Team>());
            FilterOptions(distributed_teamlist);
            Distribute();
        }
    }
}