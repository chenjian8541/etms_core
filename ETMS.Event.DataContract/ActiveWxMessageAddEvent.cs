using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class ActiveWxMessageAddEvent : Event
    {
        public ActiveWxMessageAddEvent(int tenantId) : base(tenantId)
        { }

        public long WxMessageAddId { get; set; }

        /// <summary>
        /// 学员类型   <see cref="ETMS.Entity.Enum.EmStudentType"/>
        /// </summary>
        public byte? StudentType { get; set; }
    }
}
