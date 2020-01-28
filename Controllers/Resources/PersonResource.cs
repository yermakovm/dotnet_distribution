
namespace DistributionAPI.Controllers.Resources
{
    public class PersonResource
    {
        public int Z3kId { get; set; }
        public string Name { get; set; }
        public string Team { get; set; }
        public string Location { get; set; }
        public string Avatar { get; set; }
        public PersonResource()
        {
            Z3kId = 0;
            Name = "noname";
        }

    }
}
