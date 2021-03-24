using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentReservationEvent : Event
    {
        public NoticeStudentReservationEvent(int tenantId) : base(tenantId)
        { }

        public EtClassTimesStudent ClassTimesStudent { get; set; }

        /// <summary>
        /// 允许为空
        /// </summary>
        public EtClassTimes ClassTimes { get; set; }

        /// <summary>
        /// <see cref="NoticeStudentReservationOpType"/>
        /// </summary>
        public byte OpType { get; set; }
    }

    public struct NoticeStudentReservationOpType
    {
        public const byte Success = 0;

        public const byte Cancel = 1;
    }
}