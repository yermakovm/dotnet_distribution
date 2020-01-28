using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DistributionAPI.Model;
using DistributionAPI.scheduleHelper;
using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using DistributionAPI.Controllers.Resources;
using DistributionAPI.ScheduleHelper;
using Microsoft.Extensions.Configuration;

namespace DistributionAPI.Controllers
{
    [Route("api/[controller]")]
    public class DistributionController : Controller
    {
        private readonly IRepository<DistributionData> dataRepository;
        private readonly IRepository<Department> _locationRepository;
        private readonly IMapper mapper;
        readonly IConfiguration config;
        readonly string username;
        readonly string password;
        Z3KParser parser = new Z3KParser();
        public DistributionController(IRepository<DistributionData> repo, IRepository<Department> locRepo, IMapper mapper, IConfiguration iconfig)
        {
            _locationRepository = locRepo;
            dataRepository = repo;
            this.mapper = mapper;
            config = iconfig;
            username = config.GetSection("z3k").GetSection("username").Value;
            password = config.GetSection("z3k").GetSection("password").Value;
        }
        //scheduled shift checks 
        [HttpGet("build")]
        public async Task<string> BuildDistribution([FromQuery] string dep)
        {
            if (dep == null)
                return "Error: no department name received";

            parser.LogIn(username, password);
            Department department;
            if (!_locationRepository.Filter().Where(x => x.Name.ToLower() == dep.ToLower()).Any())
            {
                DefaultValuesFiller filler = new DefaultValuesFiller(_locationRepository);
                await filler.SetDefaultDepartmentLocations();
                await filler.SetDefaultDepartmentPositions();
            }
            department = _locationRepository.Filter().Where(x => x.Name.ToLower() == dep.ToLower()).Last();
            parser.ReadSchedule(department);
            await parser.ParseCSs();
            Distribution dist = new Distribution(parser.GetTeams(), parser.SmeList);


            dist.Build();
            var time = parser.GetShiftTime();
            DistributionData data = new DistributionData
            {
                SmeList = dist.SmeList,
                Time = DateTime.Now,
                period=time.Item2,
                periodName = time.Item1,
                Department=department
            };
            dataRepository.Create(data);
            await dataRepository.SaveChanges();
            return "Distribution complete";
        }

        [HttpGet()]
        public ActionResult GetDistribution([FromQuery] string dep)
        {
            if (dep == null)
                return Ok("Error: no department name received");

            if (!dataRepository.Filter().Any())
                return Ok("No data available");
            var last = dataRepository.Filter().ToList().Where(x => x.Department.Name.ToLower() == dep.ToLower()).Last().SmeList;

            return Ok(mapper.Map<List<Sme>, List<SmeResource>>(last));
        }
        [HttpGet("info")]
        public ActionResult GetDistributionInfo([FromQuery] string dep)
        {
            if (dep == null)
                return Ok("Error: no department name received");

            if (!dataRepository.Filter().Any())
                return Ok("No data available");
            var last = dataRepository.Filter().ToList().Where(x => x.Department.Name.ToLower() == dep.ToLower()).Last();

            return Ok(mapper.Map<DistributionData, DistributionDataResource>(last));
        }
        [HttpGet("shift")]
        public ActionResult GetShiftInfo([FromQuery] string dep)
        {
            if (dep == null)
                return Ok("Error: no department name received");

            if (!dataRepository.Filter().Any())
                return Ok("No data available");
            var last = dataRepository.Filter().ToList().Where(x => x.Department.Name.ToLower() == dep.ToLower()).Last();
            return Ok(mapper.Map<DistributionData, DistributionResource>(last));
        }
        [HttpPost("location")]
        public async Task AddLocation([FromBody] Department newLocationStack)
        {
            _locationRepository.Create(newLocationStack);
            await _locationRepository.SaveChanges();

        }
        [HttpGet("positions")]
        public ActionResult GetPositions([FromQuery] string dep)
        {
            Dictionary<string,int> PositionCountList = new Dictionary<string, int>(new []
            {
                new KeyValuePair<string, int>("SME",0),
                new KeyValuePair<string, int>("CS",0),
                new KeyValuePair<string, int>("RR/TR",0),
                new KeyValuePair<string, int>("OX",0),
            });
            var last = dataRepository.Filter().ToList().Where(x => x.Department.Name.ToLower() == dep.ToLower()).Last();
            foreach (var sme in last.SmeList)
            {
                ++PositionCountList["SME"];
                foreach (var team in sme.Teams)
                {
                    foreach (var cs in team.Teammates)
                    {
                        if (cs.ShiftRole.Contains("Team"))
                            ++PositionCountList["CS"];
                        else if (cs.ShiftRole.Contains("Flock")|| cs.ShiftRole.Contains("Ticket"))
                            ++PositionCountList["RR/TR"];
                        else if(cs.ShiftRole.Contains("OX"))
                            ++PositionCountList["OX"];
                    }
                }
            }
            return Ok(PositionCountList);
        }
        [HttpGet("synctime")]
        public ActionResult GetSyncTime()
        {
            return Ok(DateTime.Now.Minute - dataRepository.Filter().Last().Time.Minute);
        }
        [HttpGet("department")]
        public async Task GetUserDepartment([FromQuery] string name)
        {
            Z3KParser parser = new Z3KParser();
            parser.LogIn(username, password);
            var json = await parser.GetPersonJson(name);
            int departmentId = (int)json["data"]["profile"]["legacydepartmentid"];
        }
        [HttpDelete("{id}")]
        public async Task RemoveSMEAndRebuild(int id)
        {
            var temp = dataRepository.Filter().Last();
            List<Sme> SmeListLeft = temp.SmeList.Where(x => x.Z3kId != id).ToList();
            List<Team> teams = new List<Team>();
            foreach (var sme in temp.SmeList)
                teams.AddRange(sme.Teams);
            Distribution dist = new Distribution(teams.Distinct().ToList(), SmeListLeft);
            dist.Build();
            DistributionData data = new DistributionData();
            data.SmeList = dist.SmeList;
            data.Time = DateTime.Now;
            dataRepository.Delete(temp);
            //await dataRepository.SaveChanges();
            dataRepository.Create(data);
            await dataRepository.SaveChanges();
        }

        [HttpPut()]
        public void UpdateDistribution([FromBody] List<Sme> sorted)
        {
            dataRepository.Filter().Last().SmeList = sorted;
            dataRepository.SaveChanges();
        }
    }
}
