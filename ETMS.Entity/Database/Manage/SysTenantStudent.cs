using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysTenantStudent")]
    public class SysTenantStudent : EManageEntity<long>
    {
        public int TenantId { get; set; }

        public long StudentId { get; set; }

        public string Phone { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmSysTenantPeopleStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public DateTime? LastOpTime { get; set; }
    }
}
