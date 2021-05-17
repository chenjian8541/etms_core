using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace ETMS.Entity.Database.Manage
{
    [Table("SysTenantOperationLog")]
    public class SysTenantOperationLog : EManageEntity<long>
    {
        public int AgentId { get; set; }

        public int TenantId { get; set; }

        public long UserId { get; set; }

        public int Type { get; set; }

        public string IpAddress { get; set; }

        public int ClientType { get; set; }

        public string OpContent { get; set; }

        public DateTime Ot { get; set; }
    }
}
