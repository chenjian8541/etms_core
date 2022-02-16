using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ETMS.Entity.Enum.EtmsManage;

namespace ETMS.Entity.Database.Manage
{
    /// <summary>
    /// 系统公告
    /// </summary>
    [Table("SysNoticeBulletin")]
    public class SysNoticeBulletin : EManageEntity<int>
    {
        public int AgentId { get; set; }

        public long UserId { get; set; }

        public string Title { get; set; }

        public string LinkUrl { get; set; }

        public DateTime? EndTime { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsAdvertise { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmNoticeBulletinStatus"/>
        /// </summary>
        public byte Status { get; set; }
    }
}
