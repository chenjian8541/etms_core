using ETMS.Entity.Alien.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.TenantStatistics.Request
{
    public class AlienTenantStatisticsClassAttendanceGetRequest : AlienTenantRequestBase
    {
        public string Ot { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(Ot))
            {
                return "请选择时间";
            }
            return base.Validate();
        }
    }
}
