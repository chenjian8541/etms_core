using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.SysCom.Output
{
    public class SysNotifyGetOutput
    {
        public SysUpgradeGetOutput SysUpgradeInfo { get; set; }

        public SystemExpiredInfo SystemExpiredInfo { get; set; }
    }

    public class SystemExpiredInfo
    {
        public bool IsRemind { get; set; }

        public string ExpireDateDesc { get; set; }
    }
}
