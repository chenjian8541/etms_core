using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysTryApplyLog")]
    public class SysTryApplyLog : EManageEntity<long>
    {
        public string Name { get; set; }

        public string LinkPhone { get; set; }

        public DateTime Ot { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmSysTryApplyLogStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string HandleRemark { get; set; }

        public long? HandleUserId { get; set; }

        public DateTime? HandleOt { get; set; }

        /// <summary>
        /// 客户端类型  <see cref="EmUserOperationLogClientType"/>
        /// </summary>
        public int ClientType { get; set; }
    }
}
