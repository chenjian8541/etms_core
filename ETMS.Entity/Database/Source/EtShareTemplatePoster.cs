using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ETMS.Entity.Enum;

namespace ETMS.Entity.Database.Source
{
    [Table("EtShareTemplatePoster")]
    public class EtShareTemplatePoster : Entity<long>
    {
        public long ShareTemplateId { get; set; }

        /// <summary>
        /// <see cref="EmShareTemplateUseType"/>
        /// </summary>
        public int UseType { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string Subtitle { get; set; }

        public string ImgKey { get; set; }

        /// <summary>
        /// <see cref="EmShareTemplateStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// <see cref="EmBool"/>
        /// </summary>
        public byte IsSystem { get; set; }
    }
}
