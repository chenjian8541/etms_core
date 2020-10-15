using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.TenantManage.Request
{
    public class TenantChangeSmsRequest : AgentRequestBase
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///  <see cref="TenantChangeEnum"/>
        /// </summary>
        public int ChangeType { get; set; }

        public int ChangeCount { get; set; }

        public string Remark { get; set; }

        public decimal Sum { get; set; }

        public override string Validate()
        {
            if (Id <= 0)
            {
                return "机构Id不能为空";
            }
            if (ChangeCount <= 0)
            {
                return "数量必须大于0";
            }
            return base.Validate();
        }
    }
}
