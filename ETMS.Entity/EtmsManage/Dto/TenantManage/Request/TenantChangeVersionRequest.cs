using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.TenantManage.Request
{
    public class TenantChangeVersionRequest : AgentRequestBase
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        public int Id { get; set; }

        public int NewVersionId { get; set; }

        public override string Validate()
        {
            if (Id <= 0)
            {
                return "机构Id不能为空";
            }
            if (NewVersionId <= 0)
            {
                return "请选择系统版本";
            }
            return base.Validate();
        }
    }
}
