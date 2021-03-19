using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.TenantManage.Request
{
    public class TenantSetExDateRequest : AgentRequestBase
    {
        public int Id { get; set; }

        public DateTime? NewExDate { get; set; }

        public override string Validate()
        {
            if (Id <= 0)
            {
                return "数据格式错误";
            }
            if (NewExDate == null)
            {
                return "请设置到期时间";
            }
            return base.Validate();
        }
    }
}
