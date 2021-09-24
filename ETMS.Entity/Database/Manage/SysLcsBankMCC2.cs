using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysLcsBankMCC2")]
    public class SysLcsBankMCC2
    {
        public int Id { get; set; }

        public string Firstname { get; set; }

        public string Secondname { get; set; }

        public int Uni1Id { get; set; }
    }
}
