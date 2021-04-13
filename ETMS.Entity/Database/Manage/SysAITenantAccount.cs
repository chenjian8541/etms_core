using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace ETMS.Entity.Database.Manage
{
    [Table("SysAITenantAccount")]
    public class SysAITenantAccount : EManageEntity<int>
    {
        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmSysAIInterfaceType"/>
        /// </summary>
        public int Type { get; set; }

        public string SecretId { get; set; }

        public string SecretKey { get; set; }

        public string Endpoint { get; set; }

        public string Region { get; set; }
    }
}
