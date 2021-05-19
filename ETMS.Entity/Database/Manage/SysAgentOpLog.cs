using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysAgentOpLog")]
    public class SysAgentOpLog : EManageEntity<int>
    {
        /// <summary>
        /// 代理商
        /// </summary>
        public int AgentId { get; set; }

        /// <summary>
        /// 操作用户
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        ///  操作类型 <see cref="ETMS.Entity.Enum.EtmsManage.EmSysAgentOpLogType"/>
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 操作内容
        /// </summary>
        public string OpContent { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Ot { get; set; }
    }
}
