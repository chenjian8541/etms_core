using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ETMS.Entity.Enum.EtmsManage;
namespace ETMS.Entity.Database.Manage
{
    [Table("SysWechatMiniPgmUser")]
    public class SysWechatMiniPgmUser: EManageEntity<long>
    {
        public int? TenantId { get; set; }

        public string Phone { get; set; }

        public string NickName { get; set; }

        public string AvatarUrl { get; set; }

        public string OpenId { get; set; }

        public string Unionid { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }
    }
}
