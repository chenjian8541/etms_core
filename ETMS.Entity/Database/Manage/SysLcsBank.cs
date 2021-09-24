using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysLcsBank")]
    public class SysLcsBank
    {
        public string HeadNumber { get; set; }

        public string HeadName { get; set; }

        public string SubbranchNumber { get; set; }

        public string SubbranchName { get; set; }

        public string CityCode { get; set; }

        public string SubbranchAddress { get; set; }
    }
}
