using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.EtmsManage.Dto.TenantManage.Request
{
    public class TenantEditImportantRequest : AgentRequestBase
    {
        public int Id { get; set; }

        public string NewTenantCode { get; set; }

        public override string Validate()
        {
            if (Id <= 0)
            {
                return "数据格式错误";
            }
            NewTenantCode = NewTenantCode.Trim();
            if (string.IsNullOrEmpty(NewTenantCode))
            {
                return "机构编码不能为空";
            }
            return base.Validate();
        }
    }
}
