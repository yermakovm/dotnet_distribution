using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DistributionAPI.Model;
using DistributionAPI.Classes;

namespace DistributionAPI.Controllers
{
    [Route("api/[controller]")]
    public class DistributionController : Controller
    {
        [HttpGet("{day}/{shift}")]
        public async Task<ActionResult> GetSchedule(int day, int shift)
        {
            Z3KParser parser = new Z3KParser(day, shift);
            await parser.ParsePersons();
            parser.SplitTeams();
            Distribution dist = new Distribution(parser.teams, parser.sme);
            dist.Build();
            return Ok(dist.smes);
        }
    }
}
