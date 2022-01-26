using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtElectronicAlbumShareLogDay")]
    public class EtElectronicAlbumShareLogDay : Entity<long>
    {
        public long ElectronicAlbumId { get; set; }

        public int ShareCount { get; set; }

        public DateTime Ot { get; set; }
    }
}
