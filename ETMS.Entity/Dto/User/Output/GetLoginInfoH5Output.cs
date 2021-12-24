using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Output
{
    public class GetLoginInfoH5Output
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
        /// 头像地址url
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 昵称 （学员端显示）
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 机构名称
        /// </summary>
        public string OrgName { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        public PermissionOutput Permission { get; set; }

        /// <summary>
        /// 角色设置权限
        /// </summary>
        public RoleNoticeSettingOutput RoleSetting { get; set; }

        public TenantConfig TenantConfig { get; set; }

        public string TenantNo { get; set; }

        public string TenantName { get; set; }

        public List<MenuH5Output> Menus { get; set; }

        public bool IsShowMoreMenus { get; set; }
    }
}
