using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace ETMS.Entity.Database.Manage
{
    [Table("SysUpgradeMsg")]
    public class SysUpgradeMsg: EManageEntity<int>
    {
        public int AgentId { get; set; }

        public string VersionNo { get; set; }

        public string Title { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string UpContent { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EtmsManage.EmSysUpgradeMsgStatus"/>
        /// </ummary>
        public byte Status { get; set; }
    }
}
