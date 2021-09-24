using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysLcsBankMCC1")]
    public class SysLcsBankMCC1
    {
        public int Id { get; set; }

        public string Firstname { get; set; }
    }
}
