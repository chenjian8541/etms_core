using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ETMS.Entity.Enum.EtmsManage;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysNoticeBulletinRead")]
    public class SysNoticeBulletinRead : EManageEntity<long>
    {
        public int BulletinId { get; set; }

        public int TenantId { get; set; }

        public long UserId { get; set; }

        public DateTime ReadTime { get; set; }
    }
}
