using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.User.Output
{
    public class UserGetPagingOutput
    {
        public long CId { get; set; }

        /// <summary>
        /// 所属组织Id
        /// </summary>
        public long? OrganizationId { get; set; }

        public string OrgName { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public int? MgRoleId { get; set; }

        public string RoleName { get; set; }

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
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public List<JobAtTenantOutput> JobAtTenantList { get; set; }

        public string JobAtTenantListDesc { get; set; }
    }

    public class JobAtTenantOutput
    {
        public int TenantId { get; set; }

        public string Name { get; set; }
    }
}
