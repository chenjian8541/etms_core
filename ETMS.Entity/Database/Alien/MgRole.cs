using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Alien
{
    [Table("MgRole")]
    public class MgRole : EAlienEntity<int>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 菜单权限
        /// </summary>
        public string AuthorityValueMenu { get; set; }

        /// <summary>
        /// 数据权限
        /// </summary>
        public string AuthorityValueData { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
