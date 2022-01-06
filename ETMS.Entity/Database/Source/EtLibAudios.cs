using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ETMS.Entity.Enum;

namespace ETMS.Entity.Database.Source
{
    [Table("EtLibAudios")]
    public class EtLibAudios : Entity<long>
    {
        /// <summary>
        /// <see cref="EmLibType"/>
        /// </summary>
        public int Type { get; set; }

        public string AudioKey { get; set; }

        public string AudioUrl { get; set; }

        public string Name { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
