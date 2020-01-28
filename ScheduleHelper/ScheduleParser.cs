using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DistributionAPI.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DistributionAPI.ScheduleHelper
{
    public abstract class ScheduleParser
    {
        protected CookieContainer cookies;
        public abstract List<Team> GetTeams();

        public WebResponse LogIn(string user, string pass)
        {
            var formUrl = "https://core.zone3000.net/staff_sign_in";
            var formParams = new
            {
                email = user,
                password = pass
            };
            var json = JsonConvert.SerializeObject(formParams);
            var jsonFormatted = JToken.Parse(json).ToString(Formatting.Indented);
            var req = (HttpWebRequest)WebRequest.Create(formUrl);
            cookies = new CookieContainer();
            req.CookieContainer = cookies;
            req.ContentType = "application/json;charset=UTF-8";
            req.Method = "POST";
            var bytes = Encoding.ASCII.GetBytes(jsonFormatted);
            req.ContentLength = bytes.Length;
            using (var os = req.GetRequestStream())
            {
                os.Write(bytes, 0, bytes.Length);
            }

            var resp = req.GetResponse();
            return resp;
        }

        protected (int, int) GetShiftNumber()
        {
            var day = DateTime.Today.Day;
            var hour = DateTime.Now.Hour;
            var shift = 0;
            if (hour < 8 && hour >= 0)
            {
                shift = 1;
            }

            else if (hour >= 23)
            {
                shift = 1;
                day = DateTime.Today.AddDays(1).Day;
            }
            else if (hour < 11 && hour >= 7)
            {
                shift = 2;
            }
            else if (hour < 15 && hour >= 11)
            {
                shift = 3;
            }
            else if (hour >= 15 && hour < 20)
            {
                shift = 4;
            }
            else if (hour >= 20 && hour < 23)
            {
                shift = 5;
            }

            return (shift, day);
        }
        public (string, string) GetShiftTime()
        {
            string shiftName = "";
            string currentPeriod = "";
            int hour = DateTime.Now.Hour;
            if (hour < 8 || hour >= 23)
            {
                shiftName = "night";
                currentPeriod = "00:00-08:00";
            }
            if (hour >= 8 && hour < 16)
            {
                shiftName = "morning";
                currentPeriod = "08:00-16:00";
            }
            if (hour >= 16 && hour < 23)
            {
                shiftName = "evening";
                currentPeriod = "16:00-00:00";
            }
            return (shiftName, currentPeriod);
        }
    }
}
