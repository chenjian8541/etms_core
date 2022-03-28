using ETMS.Entity.Alien.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.User.Request
{
    public class UserGetPagingRequest : AlienRequestPagingBase
    {
        /// <summary>
        /// 姓名/手机号码
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public int? RoleId { get; set; }

        /// <summary>
        /// 组织机构
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 就职校区
        /// </summary>
        public int? JobAtTenantId { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (!string.IsNullOrEmpty(Key))
            {
                condition.Append($" AND (Name LIKE '{Key}%' OR Phone LIKE '{Key}%')");
            }
            if (RoleId != null)
            {
                condition.Append($" AND MgRoleId = {RoleId}");
            }
            if (OrgId != null)
            {
                condition.Append($" AND OrganizationId = {OrgId}");
            }
            if (JobAtTenantId != null)
            {
                condition.Append($" AND JobAtTenants LIKE '%,{JobAtTenantId},%'");
            }
            return condition.ToString();
        }
    }
}
