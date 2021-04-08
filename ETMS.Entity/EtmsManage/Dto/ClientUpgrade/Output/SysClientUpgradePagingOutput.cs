using System;
using System.Collections.Generic;
using System.Text;
using ETMS.Entity.Enum.EtmsManage;

namespace ETMS.Entity.EtmsManage.Dto.ClientUpgrade.Output
{
    public class SysClientUpgradePagingOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public int AgentId { get; set; }

        /// <summary>
        ///  <see cref="EmSysClientUpgradeClientType"/>
        /// </summary>
        public int ClientType { get; set; }

        public string ClientTypeDesc { get; set; }

        /// <summary>
        ///  <see cref="EmSysClientUpgradeUpgradeType"/>
        /// </summary>
        public byte UpgradeType { get; set; }

        public string UpgradeTypeDesc { get; set; }

        public string VersionNo { get; set; }

        public string UpgradeContent { get; set; }

        public string FileUrl { get; set; }

        public DateTime Ot { get; set; }
    }
}
