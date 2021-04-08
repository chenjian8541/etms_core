using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.ClientUpgrade.Request
{
    public class SysClientUpgradeDelRequest : AgentRequestBase
    {
        public int Id { get; set; }

        public override string Validate()
        {
            if (Id <= 0)
            {
                return "Id不能为空";
            }
            return base.Validate();
        }
    }
}