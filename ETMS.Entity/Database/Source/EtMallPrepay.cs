using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtMallPrepay")]
    public class EtMallPrepay : Entity<long>
    {
        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmMallPrepayType"/>
        /// </summary>
        public byte Type { get; set; }

        public long LcsPayLogId { get; set; }

        public string ReqContent { get; set; }

        public DateTime CreateTime { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmMallPrepayStatus"/>
        /// </summary>
        public byte Status { get; set; }
    }
}
