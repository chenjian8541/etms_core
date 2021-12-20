using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ETMS.Entity.Enum;

namespace ETMS.Entity.Database.Source
{
    [Table("EtShareTemplate")]
    public class EtShareTemplate : Entity<long>
    {
        public long UserId { get; set; }

        /// <summary>
        /// <see cref="EmShareTemplateType"/>
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// <see cref="EmShareTemplateUseType"/>
        /// </summary>
        public int UseType { get; set; }

        public string ImgKey { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// <see cref="EmShareTemplateStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// <see cref="EmBool"/>
        /// </summary>
        public byte IsSystem { get; set; }

        public DateTime? UpdateTime { get; set; }

        public DateTime? CreateTime { get; set; }
    }
}
