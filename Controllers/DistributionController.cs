using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DistributionAPI.Model;
using DistributionAPI.Classes;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using DistributionAPI.Controllers.Resources;
using Microsoft.EntityFrameworkCore;

namespace DistributionAPI.Controllers
{
    [Route("api/[controller]")]
    public class DistributionController : Controller
    {
        private readonly IRepository<DistributionData> dataRepository;

        private readonly IMapper mapper;

        public DistributionController(IRepository<DistributionData> repo, IMapper mapper)
        {
            dataRepository = repo;
            this.mapper = mapper;
        }
        //scheduled shift checks 
        [HttpGet("build")]
        public async Task BuildDistribution()
        {
            int day = DateTime.Today.Day;
            int hour = DateTime.Now.Hour;
            int shift=0;
            if (hour < 8 && hour >= 0)
            {
                shift = 1;
            }
            else if (hour >= 23)
            {
                shift = 1;
                day = DateTime.Today.AddDays(1).Day;
            }
            else if (hour < 15 && hour >= 7)
                shift = 2;
            else if (hour >= 15&& hour < 23)
                shift = 3;
                Z3KParser parser = new Z3KParser(day, shift);
            await parser.ParseCSs();
            Distribution dist = new Distribution(parser.teams, parser.sme);
            dist.Build();
            DistributionData data = new DistributionData();
            data.smes = dist.smes;
            data.time = DateTime.Now;
            dataRepository.Create(data);
            await dataRepository.SaveChanges();
        }
        //return json from db
        [HttpGet()]
        public ActionResult GetDistribution()
        {
            if (!dataRepository.Filter().Any())
                return Ok("No data available");
            Guid last;
             last = dataRepository.Filter().Last().Id;
            var smeDistribution = dataRepository.Filter().Last().smes.ToList();

            return Ok(mapper.Map<List<Sme>, List<SmeResource>>(smeDistribution));
        }

        [HttpGet("synctime")]
        public ActionResult GetSyncTime()
        {
            return Ok(DateTime.Now.Minute - dataRepository.Filter().Last().time.Minute);
        }

        [HttpDelete("{id}")]
        public async Task RemoveSMEAndRebuild(int id)
        {
            var temp = dataRepository.Filter().Last();
            List<Sme> smesLeft = temp.smes.Where(x => x.z3kid != id).ToList();
            List<Team> teams = new List<Team>();
            foreach (var sme in temp.smes)
                teams.AddRange(sme.teams);
            Distribution dist = new Distribution(teams.Distinct().ToList(), smesLeft);
            dist.Build();
            DistributionData data = new DistributionData();
            data.smes = dist.smes;
            data.time = DateTime.Now;
            dataRepository.Delete(temp);
            //await dataRepository.SaveChanges();
            dataRepository.Create(data);
            await dataRepository.SaveChanges();
        }
        /*
        [HttpPost("{id}")]
        public async void AddSME(int id)
        {
            bool newSmeIsOnShift = csRepository.Filter(x => x.z3kid == id).Any();
            string newSmeName;
            int newSmeId;
            string newSmeTeam;
            //if new sme is cs then remove it from cslist and add to smelist
            if (newSmeIsOnShift)
            {
                var temp = csRepository.Filter(cs => cs.z3kid == id).First();
                csRepository.Delete(temp);
                newSmeName = temp.name;
                newSmeTeam = temp.team;
            }
            //if new sme is not present then find his name and add it
            else
            {
                Z3KParser parser = new Z3KParser();
                var json = await parser.GetPersonJson(id);
                newSmeName = json["data"]["full_name_eng"].ToString();
                newSmeTeam = json["data"]["positions"].Last()["org_unit_name"].ToString();
            }
            newSmeId = id;
            Sme newSme = new Sme(newSmeId,newSmeName, newSmeTeam);
            smeRepository.Create(newSme);
            await dataRepository.SaveChanges();
        }
        */
        //on list sort
        [HttpPut()]
        public void UpdateDistribution([FromBody] List<Sme> sorted)
        {
            dataRepository.Filter().Last().smes = sorted;
            dataRepository.SaveChanges();
        }
    }
}
