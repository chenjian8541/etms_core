using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Request
{
    public class StatisticsTenantGetRequest : RequestBase, IDataLimit
    {
        public string GetDataLimitFilterWhere()
        {
            return "1=1";
        }
    }
}
