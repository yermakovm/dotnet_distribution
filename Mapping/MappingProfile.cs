using AutoMapper;
using DistributionAPI.Controllers.Resources;
using DistributionAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributionAPI.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Person, PersonResource>();
            CreateMap<CSRepresentative, CSRepresentativeResource>().IncludeBase<Person, PersonResource>();
            CreateMap<Sme, SmeResource>().IncludeBase<Person, PersonResource>();
            CreateMap<Distribution, DistributionResource>();
            CreateMap<Team, TeamResource>();
            CreateMap<Department, DepartmentResource>();
            CreateMap<Location, LocationResource>();
            CreateMap<ShiftPeriod, ShiftPeriodResource>(); ;
            CreateMap<DistributionData, DistributionDataResource>();

        }
    }
}
