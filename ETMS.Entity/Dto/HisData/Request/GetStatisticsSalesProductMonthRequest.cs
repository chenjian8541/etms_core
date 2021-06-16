using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Request
{
    public class GetStatisticsSalesProductMonthRequest : RequestBase
    {
        public int? Year { get; set; }
    }
}
