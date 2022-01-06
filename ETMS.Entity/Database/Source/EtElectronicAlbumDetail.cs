using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtElectronicAlbumDetail")]
    public class EtElectronicAlbumDetail : Entity<long>
    {
        public long UserId { get; set; }

        public long ElectronicAlbumId { get; set; }

        public long StudentId { get; set; }

        public string Name { get; set; }

        public string CoverKey { get; set; }

        public int ReadCount { get; set; }

        public int ShareCount { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmElectronicAlbumStatus"/>
        /// </summary>
        public byte Status { get; set; }
    }
}
