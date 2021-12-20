using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysDangerousIp")]
    public class SysDangerousIp : EManageEntity<long>
    {
        public string Url { get; set; }

        public string RemoteIpAddress { get; set; }

        public string LocalIpAddress { get; set; }

        public DateTime Ot { get; set; }
    }
}
