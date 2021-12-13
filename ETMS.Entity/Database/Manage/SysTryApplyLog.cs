using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysTryApplyLog")]
    public class SysTryApplyLog : EManageEntity<long>
    {
        public string Name { get; set; }

        public string LinkPhone { get; set; }

        public DateTime Ot { get; set; }
    }
}
