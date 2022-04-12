using ETMS.Entity.Alien.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.TenantStatistics.Request
{
    public class AlienTenantClassRecordGetRequest: AlienTenantRequestBase
    {
        public long ClassRecordId { get; set; }

        public override string Validate()
        {
            if (ClassRecordId <= 0)
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}
