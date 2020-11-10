using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysUpgradeMsgRead")]
    public class SysUpgradeMsgRead : EManageEntity<int>
    {
        public int UpgradeMsgId { get; set; }

        public int TenantId { get; set; }

        public long UserId { get; set; }

        public DateTime ReadTime { get; set; }
    }
}
