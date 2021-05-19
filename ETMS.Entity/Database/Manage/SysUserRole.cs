using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
namespace ETMS.Entity.Database.Manage
{
    [Table("SysUserRole")]
    public class SysUserRole : EManageEntity<int>
    {
        public int AgentId { get; set; }

        public string Name { get; set; }

        public string AuthorityValueMenu { get; set; }

        public string AuthorityValueData { get; set; }
    }
}
