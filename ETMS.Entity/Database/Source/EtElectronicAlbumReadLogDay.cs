using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace ETMS.Entity.Database.Source
{
    [Table("EtElectronicAlbumReadLogDay")]
    public class EtElectronicAlbumReadLogDay : Entity<long>
    {
        public long ElectronicAlbumId { get; set; }

        public int ReadCount { get; set; }

        public DateTime Ot { get; set; }
    }
}
