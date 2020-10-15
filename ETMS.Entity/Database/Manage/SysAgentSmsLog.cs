using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysAgentSmsLog")]
    public class SysAgentSmsLog : EManageEntity<int>
    {
        /// <summary>
        /// 代理商
        /// </summary>
        public int AgentId { get; set; }

        /// <summary>
        /// 变动类型<see cref="ETMS.Entity.Enum.EtmsManage.EmSysAgentSmsLogChangeType"/>
        /// </summary>
        public int ChangeType { get; set; }

        /// <summary>
        /// 变动数量
        /// </summary>
        public int ChangeCount { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Sum { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Ot { get; set; }
    }
}
