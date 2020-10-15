using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    /// <summary>
    /// 代理商账户
    /// </summary>
    [Table("SysAgentEtmsAccount")]
    public class SysAgentEtmsAccount : EManageEntity<int>
    {
        /// <summary>
        /// 代理商ID
        /// </summary>
        public int AgentId { get; set; }

        /// <summary>
        /// 系统版本ID
        /// </summary>
        public int VersionId { get; set; }

        /// <summary>
        /// 系统数量
        /// </summary>
        public int EtmsCount { get; set; }
    }
}
