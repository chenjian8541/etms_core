using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class ElectronicAlbumInitEvent : Event
    {
        public ElectronicAlbumInitEvent(int tenantId) : base(tenantId)
        {
        }

        public EtElectronicAlbum MyElectronicAlbum { get; set; }
    }
}
