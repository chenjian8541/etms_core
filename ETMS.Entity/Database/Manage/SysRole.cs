using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    /// <summary>
    /// 角色
    /// </summary>
    [Table("SysRole")]
    public class SysRole : EManageEntity<int>
    {
        public string Name { get; set; }

        public string AuthorityValueMenu { get; set; }

        public string AuthorityValueData { get; set; }
    }
}
