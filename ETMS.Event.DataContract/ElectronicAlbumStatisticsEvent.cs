using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class ElectronicAlbumStatisticsEvent : Event
    {
        public ElectronicAlbumStatisticsEvent(int tenantId) : base(tenantId)
        {
        }

        public long ElectronicAlbumDetailId { get; set; }

        /// <summary>
        /// <see cref="ElectronicAlbumStatisticsOpType"/>
        /// </summary>
        public int OpType { get; set; }

        public DateTime Ot { get; set; }
    }

    public struct ElectronicAlbumStatisticsOpType
    {
        public const int Read = 0;

        public const int Share = 1;
    }
}
