﻿using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Request
{
    public class StatisticsLcsPayMonthGetLineRequest : RequestBase
    {
        public int? Year { get; set; }
    }
}
