﻿using ETMS.Entity.Alien.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.TenantStatistics.Request
{
    public class AlTenantStatisticsSalesProductMonthGetRequest: AlienTenantRequestBase
    {
        public int? Year { get; set; }
    }
}
