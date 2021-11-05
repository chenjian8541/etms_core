using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtMallCart")]
    public class EtMallCart : Entity<long>
    {
        public string CartContent { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
