using ETMS.Entity.Config;
using ETMS.Entity.Config.Menu;
using ETMS.Entity.View.Role;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Output
{
    public class RoleGetOutput
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public bool IsDataLimit { get; set; }

        /// <summary>
        /// 隐私类型 <see cref="ETMS.Entity.Enum.EmRoleSecrecyType"/>
        /// </summary>
        public int SecrecyType { get; set; }

        public List<RoleMenuViewOutput> Menus { get; set; }

        public RoleNoticeSettingOutput RoleNoticeSetting { get; set; }

        public AuthorityValueDataDetailView AuthorityValueDataBag { get; set; }

        public SecrecyDataView SecrecyDataBag { get; set; }
    }
}
