﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributionAPI.Controllers.Resources
{
    public class DistributionResource
    {
        public List<SmeResource> smes = new List<SmeResource>();
        public List<TeamResource> raw_schedule = new List<TeamResource>();
        public int average_load;
        public List<List<TeamResource>> distributed_teamlist = new List<List<TeamResource>>();
    }
    }
