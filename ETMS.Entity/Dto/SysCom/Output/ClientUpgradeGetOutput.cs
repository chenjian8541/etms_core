using System;
using System.Collections.Generic;
using System.Text;
using ETMS.Entity.Enum.EtmsManage;

namespace ETMS.Entity.Dto.SysCom.Output
{
    public class ClientUpgradeGetOutput
    {
        /// <summary>
        ///  <see cref="EmSysClientUpgradeUpgradeType"/>
        /// </summary>
        public byte UpgradeType { get; set; }

        public string VersionNo { get; set; }

        public string UpgradeContent { get; set; }

        public string FileUrl { get; set; }
    }
}
