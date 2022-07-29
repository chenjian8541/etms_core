using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract.Achievement
{
    public class SyncAchievementAllEvent : Event
    {
        public SyncAchievementAllEvent(int tenantId) : base(tenantId)
        {
        }

        public long AchievementId { get; set; }

        public bool IsSendStudentNotice { get; set; }
    }
}
