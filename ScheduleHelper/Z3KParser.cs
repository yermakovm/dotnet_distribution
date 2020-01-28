using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DistributionAPI.Model;
using DistributionAPI.ScheduleHelper;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DistributionAPI.scheduleHelper
{ //storing information about schedule from specific location

    public class Z3KParser : ScheduleParser
    {
        private readonly List<CSRepresentative> csList = new List<CSRepresentative>();

        // /schedule URL request data
        private readonly Dictionary<int, string> idRolePairs = new Dictionary<int, string>();

        //raw html data
        private string RawSchedule;
        public List<Sme> SmeList = new List<Sme>();

        public void GetRawSchedule(string url)
        {
            string pageSource;
            var getRequest = (HttpWebRequest) WebRequest.Create(url);
            getRequest.CookieContainer = cookies;
            var getResponse = getRequest.GetResponse();
            using (var sr = new StreamReader(getResponse.GetResponseStream()))
            {
                pageSource = sr.ReadToEnd();
            }

            RawSchedule = pageSource;
        }

        public void ReadSchedule(Department department)
        {
            (var shiftNumber, var day) = GetShiftNumber();
            foreach (var location in department.locations)
            {
                var url = "https://staff.zone3000.net/schedule?utf8=✓&department_id={0}&date[year]=" +
                          DateTime.Today.Year + "&date[month]=" + DateTime.Today.Month + "&date[day]=1";
                url = string.Format(url, location.Z3kId.ToString());
                try
                {
                    GetRawSchedule(url);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Request to {url} failed!");
                    Console.WriteLine("Error: " + e.Message);
                }

                var shift = location.XPaths[shiftNumber - 1].XPath;
                shift = string.Format(shift, day.ToString());
                var doc = new HtmlDocument();
                doc.LoadHtml(RawSchedule);    
                var check = doc.DocumentNode.SelectNodes(shift);
                foreach (var cell in doc.DocumentNode.SelectNodes(shift))
                {
                    var test = cell.Attributes["title"].Value;
                    var values = test.Split('[');
                    values[1] = values[1].Replace("]", "");
                    var id = int.Parse(cell.InnerText.Replace("\n", ""));
                    foreach (var allowedPosition in department.allowedRolesList)
                    foreach (var disallowedPosition in department.disallowedRolesList)
                        if (!idRolePairs.ContainsKey(id)&&values[1].Contains(allowedPosition.Name) && !values[1].Contains(disallowedPosition.Name))
                            idRolePairs.Add(id, values[1]);
                }
            }
        }

        public override List<Team> GetTeams()
        {
             List<Team> teamsList = new List<Team>();
            var teamnames = new List<string>();
            foreach (var person in csList) teamnames.Add(person.Team);
            teamnames = teamnames.Distinct().ToList();
            foreach (var temp in teamnames) teamsList.Add(new Team(temp, csList.Where(x => x.Team == temp).ToList()));
            return teamsList;
        }

        public async Task ParseCSs()
        {
            var listOfTasks = new List<Task>();
            foreach (var cs in idRolePairs)
                listOfTasks.Add(RequestInfo(cs.Key, cs.Value));
            try
            {
                await Task.WhenAll(listOfTasks);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error!!!: "+e.Message);
            }
        }

        public async Task<int> GetActualId(int id)
        {
            var actual = 0;
            var url = "https://core.zone3000.net/v1/users?q%5Bprofile_identifier_eq%5D={0}";
            url = string.Format(url, id.ToString());
            try
            {
                string pageSource;
                var getRequest = (HttpWebRequest) WebRequest.Create(url);
                getRequest.CookieContainer = cookies;
                var getResponse = await getRequest.GetResponseAsync();
                using (var sr = new StreamReader(getResponse.GetResponseStream()))
                {
                    pageSource = sr.ReadToEnd();
                }
                var joResponse = JObject.Parse(pageSource);
                actual = (int) joResponse["data"][0]["id"];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return actual;
        }

        public async Task<int> GetActualId(string name)
        {
            var actual = 0;
            var url =
                "https://core.zone3000.net/v1/users?q[employment_status_eq]=2&q[full_name_or_full_name_eng_cont]={0}";
            url = string.Format(url, name);
            try
            {
                string pageSource;
                var getRequest = (HttpWebRequest) WebRequest.Create(url);
                getRequest.CookieContainer = cookies;
                var getResponse = await getRequest.GetResponseAsync();
                using (var sr = new StreamReader(getResponse.GetResponseStream()))
                {
                    pageSource = sr.ReadToEnd();
                }
                var joResponse = JObject.Parse(pageSource);
                actual = (int) joResponse["data"][0]["id"];
            }
            catch (Exception e)
            {
                Console.WriteLine("Error requesting API: "+e.Message);
            }
            return actual;
        }

        public async Task<JObject> GetPersonJson(object id)
        {
            int actualId;
            if (id.GetType() == typeof(int))
                actualId = await GetActualId((int) id);
            else if (id.GetType() == typeof(string))
                actualId = await GetActualId((string) id);
            else throw new ArgumentException();
            var joResponse = new JObject();
            if (actualId > 0)
                try
                {
                    var url = string.Format("https://core.zone3000.net/v1/users/{0}", actualId.ToString());
                    var getRequest = (HttpWebRequest) WebRequest.Create(url);
                    getRequest = (HttpWebRequest) WebRequest.Create(url);
                    getRequest.CookieContainer = cookies;
                    var getResponse = await getRequest.GetResponseAsync();
                    var pageSource = "";
                    using (var sr = new StreamReader(getResponse.GetResponseStream()))
                    {
                        pageSource = sr.ReadToEnd();
                    }

                    joResponse = JObject.Parse(pageSource);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error getting JSON: " + e.Message);
                }
            return joResponse;
        }
        private async Task<Task> RequestInfo(int id, string role)
        {
            //after we get url - get other data by direct userID api call
            var joResponse = await GetPersonJson(id);
            if (!joResponse.HasValues)
                return Task.CompletedTask;
            //var position = joResponse["data"]["positions"];
            var position = joResponse["data"]["positions"].Where(x =>
                x["position_profile_name"].ToString().Contains("Specialist")  ||
                x["position_profile_name"].ToString().Contains("Expert"));
                
            JToken max;
            string level;
            string team;
            var location = "";

                max = position.Last();

                foreach (var pos in position)
                    if ((int) pos["id"] > (int) max["id"])
                        max = pos;
                if (role == "OX")
                    team = "OX";
                else if (role == "Overshifts")
                    team = "Overshifts";
                else team = max["org_unit_name"].ToString();
                level = max["level_name"].ToString();
                location = max["org_unit_name"].ToString().Substring(0, max["org_unit_name"].ToString().IndexOf(" "));
                if(max==null)
            {
                level = "undefined";
                team = "undefined";
            }

            var avatar = joResponse["data"]["small_avatar_url"].ToString();
            var name = joResponse["data"]["full_name_eng"].ToString();
            if (role == "Subject Matter Expert")
                SmeList.Add(new Sme(id, name, team, location, avatar));
            else
                csList.Add(new CSRepresentative(id, name, role, level, team, location, avatar));

            return Task.CompletedTask;
        }
    }
}