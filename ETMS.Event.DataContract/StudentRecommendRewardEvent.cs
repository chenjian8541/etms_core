using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class StudentRecommendRewardEvent : Event
    {
        public StudentRecommendRewardEvent(int tenantId) : base(tenantId)
        {
        }

        public EtStudent Student { get; set; }

        public EtOrder Order { get; set; }

        public StudentRecommendRewardType Type { get; set; }
    }

    public enum StudentRecommendRewardType
    {
        Registered = 0,

        Buy = 1
    }
}
