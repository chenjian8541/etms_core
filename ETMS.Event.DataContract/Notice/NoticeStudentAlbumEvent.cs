using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentAlbumEvent : Event
    {
        public NoticeStudentAlbumEvent(int tenantId) : base(tenantId)
        { }

        public long AlbumId { get; set; }

        public string Name { get; set; }

        public byte Type { get; set; }

        public long RelatedId { get; set; }

        public DateTime Time { get; set; }
    }
}

