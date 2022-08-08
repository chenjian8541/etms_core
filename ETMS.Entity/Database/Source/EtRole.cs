using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 角色
    /// </summary>
    [Table("EtRole")]
    public class EtRole : Entity<long>
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 菜单权限  
        /// 所有全选状态page权限|action权限|菜单权限
        /// </summary>
        public string AuthorityValueMenu { get; set; }

        /// <summary>
        /// 数据权限 (数据限制)
        /// 0：允许查看所有数据
        /// 1：只允许查看自己相关的数据
        /// </summary>
        public string AuthorityValueData { get; set; }

        /// <summary>
        /// 数据权限详情
        /// <see cref="ETMS.Entity.View.Role.AuthorityValueDataDetailView"/>
        /// </summary>
        public string AuthorityValueDataDetail { get; set; }

        /// <summary>
        /// 通知配置  <see cref="ETMS.Entity.Config.RoleOtherSetting"/>
        /// </summary>
        public string NoticeSetting { get; set; }

        /// <summary>
        /// 隐私类型 <see cref="ETMS.Entity.Enum.EmRoleSecrecyType"/>
        /// </summary>
        public int SecrecyType { get; set; }

        /// <summary>
        /// 隐藏类型详情
        /// <see cref="ETMS.Entity.View.Role.SecrecyDataView"/>
        /// </summary>
        public string SecrecyData { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
