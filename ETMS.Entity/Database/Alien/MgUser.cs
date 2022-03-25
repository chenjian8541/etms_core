using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Alien
{
    [Table("MgUser")]
    public class MgUser : EAlienEntity<long>
    {
        /// <summary>
        /// 所属组织Id
        /// </summary>
        public long? OrganizationId { get; set; }

        /// <summary>
        /// 所属组织
        /// </summary>
        public string OrgParentsAll { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public int? MgRoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 性别  <see cref="ETMS.Entity.Enum.EmGender"/>
        /// </summary>
        public byte? Gender { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 是否锁定 <see cref=" ETMS.Entity.Enum.EtmsManage.EmSysAgentIsLock"/>
        /// </summary>
        public byte IsLock { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginOt { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 是否为管理员  <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsAdmin { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 就职校区
        /// </summary>
        public string JobAtTenants { get; set; }
    }
}
