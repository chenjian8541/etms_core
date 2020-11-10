using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.SysCom.Output
{
    public class SysUpgradeGetOutput
    {
        public bool IsHaveUpgrade { get; set; }

        public UpgradeInfo UpgradeInfo { get; set; }
    }

    public class UpgradeInfo
    {

        public int UpgradeId { get; set; }

        public string VersionNo { get; set; }

        public string Title { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string UpContent { get; set; }
    }
}
