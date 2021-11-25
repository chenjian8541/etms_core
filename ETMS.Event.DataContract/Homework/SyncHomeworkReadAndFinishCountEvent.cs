using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class SyncHomeworkReadAndFinishCountEvent : Event
    {
        public SyncHomeworkReadAndFinishCountEvent(int tenantId) : base(tenantId)
        { }

        /// <summary>
        /// <see cref="SyncHomeworkReadAndFinishCountOpType"/>
        /// </summary>
        public byte OpType { get; set; }

        public long HomeworkId { get; set; }

        public long StudentId { get; set; }
    }

    public struct SyncHomeworkReadAndFinishCountOpType
    {
        public const byte Read = 0;

        public const byte Answer = 1;
    }
}
