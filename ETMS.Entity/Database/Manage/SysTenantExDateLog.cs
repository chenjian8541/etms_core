using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ETMS.Entity.Enum.EtmsManage;
namespace ETMS.Entity.Database.Manage
{
    [Table("SysTenantExDateLog")]
    public class SysTenantExDateLog: EManageEntity<long>
    {
        public int AgentId { get; set; }

        public int TenantId { get; set; }

        public DateTime? BeforeDate { get; set; }

        public DateTime AfterDate { get; set; }

        public byte ChangeType { get; set; }

        public string ChangeDesc { get; set; }

        public DateTime Ot { get; set; }
    }
}
