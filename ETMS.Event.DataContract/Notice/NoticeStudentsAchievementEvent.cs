using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentsAchievementEvent: Event
    {
        public NoticeStudentsAchievementEvent(int tenantId) : base(tenantId)
        { }

        public long AchievementId { get; set; }

        public List<EtAchievementDetail> AchievementDetailList { get; set; }
    }
}
