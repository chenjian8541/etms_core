using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
namespace ETMS.Entity.Database.Source
{
    [Table("EtElectronicAlbumTemp")]
    public class EtElectronicAlbumTemp : Entity<long>
    {
        public long UserId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmElectronicAlbumType"/>
        /// </summary>
        public byte Type { get; set; }

        public long RelatedId { get; set; }

        public long TemplateId { get; set; }

        public string Name { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
