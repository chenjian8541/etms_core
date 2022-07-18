using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Parent5.Output
{
    public class ActivityMainGetPagingRequestOutput
    {
        public long CId { get; set; }

        public long SystemActivityId { get; set; }

        /// <summary>
        /// <see cref="EmActivityType"/>
        /// </summary>
        public int ActivityType { get; set; }

        public string ActivityTypeDesc { get; set; }

        public string ActivityTypeStyleClass { get; set; }

        public string ScenetypeStyleClass { get; set; }

        /// <summary>
        /// <see cref="EmActivityScenetype"/>
        /// </summary>
        public int Scenetype { get; set; }

        public string ScenetypeDesc { get; set; }

        public string Name { get; set; }

        public string CoverImage { get; set; }

        public string Title { get; set; }

        public DateTime StartTime { get; set; }

        /// <summary>
        /// <see cref="EmActivityMainEndTimeType"/>
        /// </summary>
        public int EndTimeType { get; set; }

        public int EndValue { get; set; }

        public DateTime? EndTime { get; set; }

        public decimal OriginalPrice { get; set; }

        public string RuleEx1 { get; set; }

        public string RuleEx2 { get; set; }

        public string RuleEx3 { get; set; }

        public bool IsOpenPay { get; set; }

        /// <summary>
        /// <see cref="EmActivityPayType"/>
        /// </summary>
        public int PayType { get; set; }

        public decimal PayValue { get; set; }

        public int MaxCount { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte StudentHisLimitType { get; set; }

        public int PVCount { get; set; }

        public int UVCount { get; set; }

        public int TranspondCount { get; set; }

        public int VisitCount { get; set; }

        public int JoinCount { get; set; }

        public int RouteCount { get; set; }

        public int RuningCount { get; set; }

        public int FinishCount { get; set; }

        public int FinishFullCount { get; set; }

        public int FailCount { get; set; }

        public string ShareQRCode { get; set; }

        /// <summary>
        /// <see cref="EmActivityStyleType"/>
        /// </summary>
        public int StyleType { get; set; }

        public string StyleBackColor { get; set; }

        public string StyleColumnColor { get; set; }


        public DateTime? PublishTime { get; set; }


        public DateTime CreateTime { get; set; }

        /// <summary>
        /// <see cref="EmActivityStatus"/>
        /// </summary>
        public int ActivityStatus { get; set; }

        public string ActivityStatusDesc { get; set; }
    }
}
