using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.SysCom.Request
{
    public class SysUpgradeSetReadRequest : RequestBase
    {
        public int UpgradeId { get; set; }

        public override string Validate()
        {
            if (UpgradeId <= 0)
            {
                return "请求数据不合法";
            }
            return string.Empty;
        }
    }
}
