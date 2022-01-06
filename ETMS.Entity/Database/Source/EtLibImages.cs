using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ETMS.Entity.Enum;

namespace ETMS.Entity.Database.Source
{
    [Table("EtLibImages")]
    public class EtLibImages : Entity<long>
    {
        /// <summary>
        /// <see cref="EmLibType"/>
        /// </summary>
        public int Type { get; set; }

        public string ImgKey { get; set; }

        public string ImgUrl { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
