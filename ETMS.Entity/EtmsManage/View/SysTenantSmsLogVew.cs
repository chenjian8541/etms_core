using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.View
{
    public class SysTenantSmsLogVew
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

        public int TenantId { get; set; }

        public int AgentId { get; set; }

        public int ChangeType { get; set; }

        public int ChangeCount { get; set; }

        public decimal Sum { get; set; }

        public DateTime Ot { get; set; }

        public string TenantName { get; set; }

        public string TenantPhone { get; set; }

        public long UserId { get; set; }
    }
}
