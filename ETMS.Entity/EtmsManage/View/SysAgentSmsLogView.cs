using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.View
{
    public class SysAgentSmsLogView
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 是否删除  <see cref=" ETMS.Entity.Enum.EmIsDeleted"/>
        /// </summary>
        public byte IsDeleted { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public int AgentId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmSysAgentSmsLogChangeType"/>
        /// </summary>
        public int ChangeType { get; set; }

        public int ChangeCount { get; set; }

        public decimal Sum { get; set; }

        public DateTime Ot { get; set; }

        public string AgentName { get; set; }

        public string AgentPhone { get; set; }
    }
}
