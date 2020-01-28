using System.Collections.Generic;
using System.Linq;

namespace DistributionAPI.Model
{
    public class Distribution
    {
        private const int loadRange = 5;

        private const int step = 5;

        //tocheck
        private const int lowPriorityValue = 0;
        private readonly int averageLoad;
        public List<Team> ScheduleCellsList;
        public List<Sme> SmeList;

        public Distribution(List<Team> scheduleCellsList, List<Sme> sme)
        {
            ScheduleCellsList = scheduleCellsList;
            ScheduleCellsList.Sort();
            SmeList = sme;
            if (ScheduleCellsList.Where(x => x.Name == "OX").Any())
                ScheduleCellsList.Where(x => x.Name == "OX").First().Totalweight -= lowPriorityValue;
            averageLoad = GetListWeight(ScheduleCellsList) / SmeList.Count();
        }

        public int GetListWeight(List<Team> team)
        {
            return team.Sum(y => y.Totalweight);
        }

        public void Build()
        {
            for (var i = 0; i < SmeList.Count(); i++)
                SmeList[i].CurLoad = averageLoad;

            var extra = AssignSmeListTeam(ScheduleCellsList);
            ScheduleCellsList.Sort();

            extra = AssignByLocation(extra);
            var removed = new List<Team>();
            RemoveExtra(removed);
            extra.AddRange(removed);
            SmeList.Sort();
            AssignExtra(extra);
        }

        private List<Team> AssignSmeListTeam(List<Team> temp)
        {
            for (var i = 0; i < SmeList.Count(); i++)
            for (var j = 0; j < temp.Count(); j++)
                if (SmeList[i].Team == temp[j].Name)
                {
                    SmeList[i].AddTeam(ScheduleCellsList[j]);
                    temp.Remove(ScheduleCellsList[j]);
                }

            return temp;
        }

        private void AssignExtra(List<Team> extra)
        {
            var temp = extra.ToList();

            for (var i = 0; i < SmeList.Count(); i++)
            for (var j = 0; j < extra.Count(); j++)
            {
                var notAssigned = temp.Contains(extra[j]);
                if (SmeList[i].CurLoad >= extra[j].Totalweight && notAssigned)
                {
                    SmeList[i].AddTeam(extra[j]);
                    temp.Remove(extra[j]);
                }
            }

            if (temp.Any())
            {
                SmeList.ForEach(x => x.CurLoad += step);
                AssignExtra(temp);
            }
        }


        private List<Team> AssignByLocation(List<Team> temp)
        {
            for (var i = 0; i < SmeList.Count(); i++)
            for (var j = 0; j < ScheduleCellsList.Count(); j++)
            {
                var notAssigned = temp.Contains(ScheduleCellsList[j]);
                if (SmeList[i].SameLocation(ScheduleCellsList[j].Name) && notAssigned)
                {
                    SmeList[i].AddTeam(ScheduleCellsList[j]);
                    temp.Remove(ScheduleCellsList[j]);
                }
            }

            return temp;
        }

        private void RemoveExtra(List<Team> extras)
        {
            while (SmeList.Where(x => x.Load - loadRange > averageLoad).Any())
                for (var i = 0; i < SmeList.Count(); i++)
                    if (SmeList[i].Load - loadRange > averageLoad)
                    {
                        var min = SmeList[i].Teams.Last();
                        extras.Add(min);
                        SmeList[i].RemoveTeam(min);
                    }
        }
    }
}