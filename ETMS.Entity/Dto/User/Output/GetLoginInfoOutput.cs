using ETMS.Entity.Config;
using ETMS.Entity.Config.Router;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Output
{
    public class GetLoginInfoOutput
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 头像地址key
        /// </summary>
        public string AvatarKey { get; set; }

        /// <summary>
        /// 头像地址url
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 昵称 （学员端显示）
        /// </summary>
        public string NickName { get; set; }

        public List<RouteConfig> RouteConfigs { get; set; }

        /// <summary>
        /// 机构名称
        /// </summary>
        public string OrgName { get; set; }

        /// <summary>
        /// 角色设置权限
        /// </summary>
        public RoleNoticeSettingOutput RoleSetting { get; set; }

        public TenantConfig TenantConfig { get; set; }

        public TenantOEMInfoOutput TenantOEMInfo { get; set; }

        public string TenantNo { get; set; }

        public string TenantName { get; set; }

        public List<ExternalConfigOutput> ExternalConfigList { get; set; }
    }


}
