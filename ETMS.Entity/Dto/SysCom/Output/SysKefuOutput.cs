using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.SysCom.Output
{
    public class SysKefuOutput
    {
        public List<KefuHelpCenter> HelpCenterInfos { get; set; }

        public KefuInfo KefuInfo { get; set; }

        public List<UpgradeIngo> UpgradeIngos { get; set; }
    }

    public class KefuHelpCenter
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string RelationUrl { get; set; }
    }

    public class KefuInfo
    {
        public string qq { get; set; }

        public string Phone { get; set; }
    }

    public class UpgradeIngo
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string RelationUrl { get; set; }
    }
}
