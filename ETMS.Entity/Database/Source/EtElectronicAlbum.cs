using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtElectronicAlbum")]
    public class EtElectronicAlbum : Entity<long>
    {
        public long UserId { get; set; }

        /// <summary>
        /// 临时表ID
        /// </summary>
        public long TempId { get; set; }

        public long TemplateId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmElectronicAlbumType"/>
        /// </summary>
        public byte Type { get; set; }

        public long RelatedId { get; set; }

        public string Name { get; set; }

        public string CoverKey { get; set; }

        public string RenderData { get; set; }

        public int ReadCount { get; set; }

        public int ShareCount { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmElectronicAlbumStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string CIdNo { get; set; }

        public DateTime? UpdateTime { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
