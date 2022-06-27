using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.OpenParent2.Output
{
    public class WxMiniActivityRouteItemGetPagingOutput
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        public int TenantId { get; set; }

        public long ActivityId { get; set; }

        public long ActivityRouteId { get; set; }

        public long MiniPgmUserId { get; set; }

        public long EtActivityRouteItemId { get; set; }

        public string NickName { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public string StudentFieldValue1 { get; set; }

        public string StudentFieldValue2 { get; set; }

        /// <summary>
        /// <see cref="EmActivityType"/>
        /// </summary>
        public int ActivityType { get; set; }

        public string ActivityTypeStyleClass { get; set; }

        public string ScenetypeStyleClass { get; set; }

        /// <summary>
        ///  <see cref="EmActivityScenetype"/>
        /// </summary>
        public int ActivityScenetype { get; set; }

        public string ActivityName { get; set; }

        public string ActivityTenantName { get; set; }

        public string ActivityCoverImage { get; set; }

        public string ActivityTitle { get; set; }

        public DateTime ActivityStartTime { get; set; }

        public DateTime ActivityEndTime { get; set; }

        public decimal ActivityOriginalPrice { get; set; }

        public decimal PaySum { get; set; }

        public DateTime? PayFinishTime { get; set; }

        public bool IsTeamLeader { get; set; }

        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 拼团状态 <see cref="EmSysActivityRouteItemStatus"/>
        /// </summary>
        public int Status { get; set; }
    }
}
