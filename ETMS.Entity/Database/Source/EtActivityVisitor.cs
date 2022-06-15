using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtActivityVisitor")]
    public class EtActivityVisitor: Entity<long>
    {
        public long ActivityId { get; set; }

        public long MiniPgmUserId { get; set; }

        public string OpenId { get; set; }

        public string Unionid { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
