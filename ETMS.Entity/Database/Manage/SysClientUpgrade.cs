using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ETMS.Entity.Enum.EtmsManage;

namespace ETMS.Entity.Database.Manage
{
    /// <summary>
    /// 客户端升级
    /// </summary>
    [Table("SysClientUpgrade")]
    public class SysClientUpgrade : EManageEntity<int>
    {
        public int AgentId { get; set; }

        /// <summary>
        ///  <see cref="EmSysClientUpgradeClientType"/>
        /// </summary>
        public int ClientType { get; set; }

        /// <summary>
        ///  <see cref="EmSysClientUpgradeUpgradeType"/>
        /// </summary>
        public byte UpgradeType { get; set; }

        public string VersionNo { get; set; }

        public string UpgradeContent { get; set; }

        public string FileUrl { get; set; }

        public DateTime Ot { get; set; }
    }
}
