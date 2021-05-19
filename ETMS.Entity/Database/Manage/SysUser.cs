using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysUser")]
    public class SysUser : EManageEntity<long>
    {
        public int AgentId { get; set; }

        public int UserRoleId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// 是否锁定 <see cref=" ETMS.Entity.Enum.EtmsManage.EmSysAgentIsLock"/>
        /// </summary>
        public byte IsLock { get; set; }

        public DateTime? LastLoginOt { get; set; }

        public int CreatedAgentId { get; set; }

        public DateTime Ot { get; set; }

        /// <summary>
        /// 是否为管理员  <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsAdmin { get; set; }
    }
}
