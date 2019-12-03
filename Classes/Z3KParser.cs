using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using DistributionAPI.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;

namespace DistributionAPI.Classes
{   //storing information about schedule from specific location

    public class Z3KParser
    {
        //raw html data
        string schedule;
        CookieContainer cookies;

        // /schedule URL request data
        Dictionary<int, string> idRolePairs = new Dictionary<int, string>();
        List<CS> cslist = new List<CS>();
        public List<Sme> sme = new List<Sme>();
        public List<Team> teams = new List<Team>();

        public void LogIn(string user, string pass)
        {
            string formUrl = "https://core.zone3000.net/staff_sign_in";
            var formParams = new
            {
                email = user,
                password = pass
            };
            string json = JsonConvert.SerializeObject(formParams);
            string jsonFormatted = JValue.Parse(json).ToString(Formatting.Indented);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(formUrl);
            cookies = new CookieContainer();
            req.CookieContainer = cookies;
            req.ContentType = "application/json;charset=UTF-8";
            req.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes(jsonFormatted);
            req.ContentLength = bytes.Length;
            using (Stream os = req.GetRequestStream())
            {
                os.Write(bytes, 0, bytes.Length);
            }
            WebResponse resp = req.GetResponse();
        }

        public void GetSchedule(string url)
        {
            string pageSource;
            HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(url);
            getRequest.CookieContainer = cookies;
            WebResponse getResponse = getRequest.GetResponse();
            using (StreamReader sr = new StreamReader(getResponse.GetResponseStream()))
            {
                pageSource = sr.ReadToEnd();
            }
            schedule = pageSource;
        }

        public List<Location> SetLocations()
        {
            List<Location> locations = new List<Location>();
            List<xPath> shifts = new List<xPath>();

            shifts.Add(new xPath("//li[@class='cell' and substring(@id, string-length(@id) - string-length('_{0}') +1) = '_{0}'][count(preceding-sibling::li[@class='left_part first'])=1][text()>2]"));
            shifts.Add(new xPath("//li[@class='cell' and substring(@id, string-length(@id) - string-length('_{0}') +1) = '_{0}'][count(preceding-sibling::li[@class='left_part first'])=3][text()>2]"));
            shifts.Add(new xPath("//li[@class='cell' and substring(@id, string-length(@id) - string-length('_{0}') +1) = '_{0}'][count(preceding-sibling::li[@class='left_part first'])=5][text()>2]"));
            Location loc = new Location("Kharkiv/Lviv", shifts, 4);
            locations.Add(loc);
            shifts = new List<xPath>();
            shifts.Add(new xPath("//li[@class='cell' and substring(@id, string-length(@id) - string-length('_{0}') +1) = '_{0}'][count(preceding-sibling::li[@class='left_part first'])=1][text()>2]"));
            shifts.Add(new xPath("//li[@class='cell' and substring(@id, string-length(@id) - string-length('_{0}') +1) = '_{0}'][count(preceding-sibling::li[@class='left_part first'])=3][text()>2]"));
            shifts.Add(new xPath("//li[@class='cell' and substring(@id, string-length(@id) - string-length('_{0}') +1) = '_{0}'][count(preceding-sibling::li[@class='left_part first'])=5][text()>2]"));
            
            loc = new Location("Dnipro", shifts, 53);
            locations.Add(loc);
            return locations;
        }

        public void ReadSchedule(int day, int shiftNumber)
        {
            List<Location> locs = SetLocations();

            for (int i = 0; i < locs.Count; i++)
            {
                string url = "https://staff.zone3000.net/schedule?utf8=✓&department_id={0}&date[year]=" + DateTime.Today.Year + "&date[month]=" + DateTime.Today.Month + "&date[day]=1";
                url = string.Format(url, locs[i].DepartmentId.ToString());
                GetSchedule(url);
                string shift = locs[i].xPaths[shiftNumber - 1].xpath;
                shift = string.Format(shift, day.ToString());
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(schedule);
                foreach (HtmlNode cell in doc.DocumentNode.SelectNodes(shift))
                {
                    string test = cell.Attributes["title"].Value;
                    string[] values = test.Split('[');
                    values[1] = values[1].Replace("]", "");
                    int id = Convert.ToInt32(cell.InnerText.Replace("\n", ""));
                    if ((values[1].Contains("Team") || values[1].Contains("OX") || values[1].Contains("Subject") || values[1].Contains("Flock") || values[1].Contains("Tickets") || values[1].Contains("Overshifts")) && !values[1].Contains("Manager"))
                        idRolePairs.Add(id, values[1]);
                }
            }
        }


        public void SplitTeams()
        {
            List<string> teamnames = new List<string>();
            foreach (var person in cslist)
            {
                teamnames.Add(person.team);
            }
            teamnames = teamnames.Distinct().ToList();

            foreach (var temp in teamnames)
            {
                teams.Add(new Team(temp, cslist.Where(x => x.team == temp).ToList()));
            }
        }

        public async Task ParseCSs()
        {
            List<Task> listOfTasks = new List<Task>();

            foreach (var cs in idRolePairs)
            {
                listOfTasks.Add(RequestInfo(cs.Key, cs.Value)); ;
            }
            await Task.WhenAll(listOfTasks);
            SplitTeams();
        }

        public async Task<int> GetActualID(int id)
        {
            int actual = 0;

            string url = "https://core.zone3000.net/v1/users?q%5Bactive_eq%5D=true&q%5Bprofile_identifier_eq={0}";
            url = string.Format(url, id.ToString());
            string pageSource;
            try
            {
                HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(url);
                getRequest.CookieContainer = cookies;
                WebResponse getResponse = await getRequest.GetResponseAsync();
                using (StreamReader sr = new StreamReader(getResponse.GetResponseStream()))
                {
                    pageSource = sr.ReadToEnd();
                }
                JObject joResponse = JObject.Parse(pageSource);
                actual = (int)joResponse["data"][0]["id"];

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return actual;
        }
        public async Task<int> GetActualID(string name)
        {
            int actual = 0;

            string url = "https://core.zone3000.net/v1/users?q[employment_status_eq]=2&q[full_name_or_full_name_eng_cont]={0}";
            url = string.Format(url, name);
            string pageSource;
            try
            {
                HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(url);
                getRequest.CookieContainer = cookies;
                WebResponse getResponse = await getRequest.GetResponseAsync();
                using (StreamReader sr = new StreamReader(getResponse.GetResponseStream()))
                {
                    pageSource = sr.ReadToEnd();
                }
                JObject joResponse = JObject.Parse(pageSource);
                actual = (int)joResponse["data"][0]["id"];

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return actual;
        }
        public async Task<JObject> GetPersonJson(object id)
        {
            int actual_id = 0;
            if (id.GetType()==typeof(int))
                actual_id = await GetActualID((int)id);
            else if (id.GetType() == typeof(string))
                actual_id = await GetActualID((string)id);
            else throw new ArgumentException();
            JObject joResponse = new JObject();
            if (actual_id > 0)
                try
                {
                    string url = string.Format("https://core.zone3000.net/v1/users/{0}", actual_id.ToString());
                    HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(url);
                    getRequest = (HttpWebRequest)WebRequest.Create(url);
                    getRequest.CookieContainer = cookies;
                    WebResponse getResponse = await getRequest.GetResponseAsync();
                    string pageSource = "";
                    using (StreamReader sr = new StreamReader(getResponse.GetResponseStream()))
                    {
                        pageSource = sr.ReadToEnd();
                    }
                    joResponse = JObject.Parse(pageSource);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            return joResponse;
        }

        async Task<Task> RequestInfo(int id, string role)
        {
            //after we get url - get other data by direct userID api call
            JObject joResponse = await GetPersonJson(id);
            if (!joResponse.HasValues)
                return Task.CompletedTask;
            var position = joResponse["data"]["positions"].Where(x => x["position_profile_name"].ToString() == "Customer Support Specialist" || x["position_profile_name"].ToString().Contains("Expert"));
            JToken max;
            string level;
            string team;
            string location = "";
            if (position.Count() > 0)
            {
                max = position.Last();

                foreach (var pos in position)
                {
                    if ((int)pos["id"] > (int)max["id"])
                        max = pos;
                }
                if (role == "OX")
                    team = "OX";
                else if (role == "Overshifts")
                    team = "Overshifts";
                else team = max["org_unit_name"].ToString();
                level = max["level_name"].ToString();
                location = max["org_unit_name"].ToString().Substring(0, max["org_unit_name"].ToString().IndexOf(" "));
            }
            else
            {
                level = "undefined";
                team = "undefined";
            }
            string avatar = joResponse["data"]["small_avatar_url"].ToString();
            string name = joResponse["data"]["full_name_eng"].ToString();
            if (role == "Subject Matter Expert")
                sme.Add(new Sme(id, name, team, location, avatar));
            else
                cslist.Add(new CS(id, name, role, level, team, location, avatar));

            return Task.CompletedTask;
        }
    }
}
