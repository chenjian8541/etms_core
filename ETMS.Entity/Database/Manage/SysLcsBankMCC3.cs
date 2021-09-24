using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysLcsBankMCC3")]
    public class SysLcsBankMCC3
    {
        public int Id { get; set; }

        public string Firstname { get; set; }

        public string Secondname { get; set; }

        public string Thirdname { get; set; }

        public string Code { get; set; }

        public int Uni2Id { get; set; }
    }
}
