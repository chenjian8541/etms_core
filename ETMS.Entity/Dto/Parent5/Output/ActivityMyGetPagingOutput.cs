using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Parent5.Output
{
    public class ActivityMyGetPagingOutput
    {
        public long CId { get; set; }

        public long ActivityId { get; set; }

        public long ActivityRouteId { get; set; }

        public long MiniPgmUserId { get; set; }

        public long? StudentId { get; set; }

        public string NickName { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        /// <summary>
        /// <see cref="EmActivityType"/>
        /// </summary>
        public int ActivityType { get; set; }

        public string ActivityTypeDesc { get; set; }

        public string ActivityTypeStyleClass { get; set; }

        public string ScenetypeStyleClass { get; set; }

        /// <summary>
        ///  <see cref="EmActivityScenetype"/>
        /// </summary>
        public int ActivityScenetype { get; set; }

        public string ActivityScenetypeDesc { get; set; }

        public string ActivityName { get; set; }

        public string ActivityCoverImage { get; set; }

        public string ActivityTitle { get; set; }

        public DateTime ActivityStartTime { get; set; }

        public DateTime ActivityEndTime { get; set; }

        public decimal ActivityOriginalPrice { get; set; }

        public string ActivityRuleEx1 { get; set; }

        public string ActivityRuleEx2 { get; set; }


        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmActivityRoutePayStatus"/>
        /// </summary>
        public int PayStatus { get; set; }

        public decimal PaySum { get; set; }

        public DateTime? PayFinishTime { get; set; }

        public bool IsTeamLeader { get; set; }

        public string ShareQRCode { get; set; }

        /// <summary>
        /// 拼团状态 <see cref="EmSysActivityRouteItemStatus"/>
        /// </summary>
        public int Status { get; set; }

        public string StatusDesc { get; set; }

        public DateTime CreateTime { get; set; }

        public int ExTimeCountDown { get; set; }

        public int CountLimit { get; set; }

        public int CountFinish { get; set; }

    }
}
