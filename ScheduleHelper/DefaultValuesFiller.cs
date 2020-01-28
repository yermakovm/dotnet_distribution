using System.Collections.Generic;
using System.Threading.Tasks;
using DistributionAPI.Model;
using DistributionAPI.Model.LocationModels;

namespace DistributionAPI.ScheduleHelper
{
    public class DefaultValuesFiller
    {
        private readonly IRepository<Department> _locationRepository;

        public DefaultValuesFiller(IRepository<Department> locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task<bool> SetDefaultDepartmentPositions()
        {
            foreach (var dep in _locationRepository.Filter())
            {
                dep.disallowedRolesList = new List<ShiftRoles>(new[] { new ShiftRoles("Manager"), new ShiftRoles("QA"), });
                dep.allowedRolesList =
                    new List<ShiftRoles>(new[]
                    {
                        new ShiftRoles("Team"), 
                        new ShiftRoles("OX"), 
                        new ShiftRoles("Subject"), 
                        new ShiftRoles("Flock"), 
                        new ShiftRoles("Tickets"), 
                        new ShiftRoles("Overshifts")
                    });
            }

            await _locationRepository.SaveChanges();
            return true;
        }

        public async Task<bool> SetDefaultDepartmentLocations()
        {
            var locations = new List<Location>();
            var XPathNight =
                "//li[@class='cell' and substring(@id, string-length(@id) - string-length('_{0}') +1) = '_{0}'][count(preceding-sibling::li[@class='left_part first'])=1][text()>2]";
            var XPathMorning1 =
                "//li[@class='cell' and substring(@id, string-length(@id) - string-length('_{0}') +1) = '_{0}'][count(preceding-sibling::li[@class='left_part first'])=2][text()>2]";
            var XPathMorning2 =
                "//li[@class='cell' and substring(@id, string-length(@id) - string-length('_{0}') +1) = '_{0}'][count(preceding-sibling::li[@class='left_part first'])=3][text()>2]";
            var XPathEvening1 =
                "//li[@class='cell' and substring(@id, string-length(@id) - string-length('_{0}') +1) = '_{0}'][count(preceding-sibling::li[@class='left_part first'])=4][text()>2]";
            var XPathEvening2 =
                "//li[@class='cell' and substring(@id, string-length(@id) - string-length('_{0}') +1) = '_{0}'][count(preceding-sibling::li[@class='left_part first'])=5][text()>2]";
            var shifts = new List<ShiftPeriod>
            {
                new ShiftPeriod(XPathNight, "00:00", "08:00"),
                new ShiftPeriod(XPathMorning1, "08:00", "12:00"),
                new ShiftPeriod(XPathMorning2, "12:00", "16:00"),
                new ShiftPeriod(XPathEvening1, "16:00", "20:00"),
                new ShiftPeriod(XPathEvening2, "20:00", "00:00")
            };
            var loc = new Location("Kharkiv/Lviv", shifts, 4);
            locations.Add(loc);
            shifts = new List<ShiftPeriod>
            {
                new ShiftPeriod(XPathNight, "00:00", "08:00"),
                new ShiftPeriod(XPathMorning1, "08:00", "12:00"),
                new ShiftPeriod(XPathMorning2, "12:00", "16:00"),
                new ShiftPeriod(XPathEvening1, "16:00", "20:00"),
                new ShiftPeriod(XPathEvening2, "20:00", "00:00")
            };
            loc = new Location("Dnipro", shifts, 53);
            locations.Add(loc);
            var department = new Department("Hosting", locations);
            _locationRepository.Create(department);
            //
            shifts = new List<ShiftPeriod>
            {
                new ShiftPeriod(XPathNight, "00:00", "08:00"),
                new ShiftPeriod(XPathMorning1, "08:00", "12:00"),
                new ShiftPeriod(XPathMorning2, "12:00", "16:00"),
                new ShiftPeriod(XPathEvening1, "16:00", "20:00"),
                new ShiftPeriod(XPathEvening2, "20:00", "00:00")
            };
            locations = new List<Location>
            {
                new Location("Default", shifts, 6)
            };
            var domains = new Department("Domains", locations);
            //
            _locationRepository.Create(domains);
            await _locationRepository.SaveChanges();
            //await dataRepository.SaveChanges();
            return true;
        }
    }
}