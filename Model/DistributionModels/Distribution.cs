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
        private const int load_range = 5;
        private const int step = 5;
        private const int lowPriorityValue = 10;
        public Distribution(List<Team> _raw_schedule, List<Sme> _sme)
        {
            raw_schedule = _raw_schedule;
            raw_schedule.Sort();
            smes = _sme;
            if (raw_schedule.Where(x => x.name == "OX").Any())
                raw_schedule.Where(x => x.name == "OX").First().total_weight -= lowPriorityValue;
            average_load = GetListWeight(raw_schedule) / smes.Count();
        }

        public int GetListWeight(List<Team> team)
        {
            return team.Sum(y => y.total_weight);
        }


        public void Build(int[] id = null)
        {
            for (int i = 0; i < smes.Count(); i++)
                smes[i].curLoad = average_load;
            raw_schedule.Sort();

            List<Team> extra = AssignByLocation();
            List<Team> removed = new List<Team>();
            RemoveExtra(removed);
            extra.AddRange(removed);
            smes.Sort();
            AssignExtra(extra);
        }

        private void AssignExtra(List<Team> extra)
        {
            List<Team> temp = extra.ToList();

            for (int i = 0; i < smes.Count(); i++)
                for (int j = 0; j < extra.Count(); j++)
                {
                    bool notAssigned = temp.Contains(extra[j]);
                    if (smes[i].curLoad >= extra[j].total_weight && notAssigned)
                    {
                        smes[i].teams.Add(extra[j]);
                        smes[i].curLoad -= extra[j].total_weight;
                        smes[i].load += extra[j].total_weight;
                        temp.Remove(extra[j]);
                    }
                }
            if (temp.Any())
            {
                smes.ForEach(x => x.curLoad += step);
                AssignExtra(temp);
            }
        }


        private List<Team> AssignByLocation()
        {
            List<Team> temp = raw_schedule.ToList();

            for (int i = 0; i < smes.Count(); i++)
                for (int j = 0; j < raw_schedule.Count(); j++)
                {
                    bool notAssigned = temp.Contains(raw_schedule[j]);
                    if ((smes[i].SameLocation(raw_schedule[j].name)) && notAssigned)
                    {
                        smes[i].teams.Add(raw_schedule[j]);
                        smes[i].curLoad -= raw_schedule[j].total_weight;
                        smes[i].load += raw_schedule[j].total_weight;
                        temp.Remove(raw_schedule[j]);
                    }
                }
            return temp;
        }

        private void RemoveExtra(List<Team> extras)
        {
            while (smes.Where(x=>x.load - load_range > average_load).Any())
            {
                for (int i = 0; i < smes.Count(); i++)
                {
                    if (smes[i].load - load_range > average_load)
                    {
                        var min = smes[i].teams.Last();
                        extras.Add(min);
                        smes[i].curLoad += min.total_weight;
                        smes[i].load -= min.total_weight;
                        smes[i].teams.Remove(min);
                    }
                }
            }
        }
    }
}