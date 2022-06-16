using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Activity.Output
{
    public class ActivityMainGetSimpleOutput
    {
        public long CId { get; set; }

        public long SystemActivityId { get; set; }

        /// <summary>
        /// <see cref="EmActivityType"/>
        /// </summary>
        public int ActivityType { get; set; }

        public string ActivityTypeDesc { get; set; }

        /// <summary>
        /// <see cref="EmActivityScenetype"/>
        /// </summary>
        public int Scenetype { get; set; }

        public string ScenetypeDesc { get; set; }

        public string Name { get; set; }

        public string CoverImageUrl { get; set; }

        public string Title { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime? PublishTime { get; set; }

        public string ShareQRCode { get; set; }

        public DateTime CreateTime { get; set; }

        /// <summary>
        /// <see cref="EmActivityStatus"/>
        /// </summary>
        public int ActivityStatus { get; set; }

        public string ActivityStatusDesc { get; set; }

        public int PVCount { get; set; }

        public int UVCount { get; set; }

        public int TranspondCount { get; set; }

        public int VisitCount { get; set; }

        public int JoinCount { get; set; }

        public int RouteCount { get; set; }

        public int RuningCount { get; set; }

        public int FinishCount { get; set; }

        public int FailCount { get; set; }

        public string StudentFieldName1 { get; set; }

        public string StudentFieldName2 { get; set; }
    }
}
