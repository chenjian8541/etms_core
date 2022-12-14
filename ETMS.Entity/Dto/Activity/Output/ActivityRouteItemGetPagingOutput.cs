using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Activity.Output
{
    public class ActivityRouteItemGetPagingOutput
    {
        public long ItemId { get; set; }

        public int ActivityType { get; set; }
        public long ActivityRouteId { get; set; }

        public long ActivityId { get; set; }

        public long MiniPgmUserId { get; set; }

        public string AvatarUrl { get; set; }

        public string NickName { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public string StudentFieldValue1 { get; set; }

        public string StudentFieldValue2 { get; set; }

        public decimal PaySum { get; set; }

        public DateTime? PayFinishTime { get; set; }

        public DateTime CreateTime { get; set; }

        public string Tag { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmActivityRoutePayStatus"/>
        /// </summary>
        public int PayStatus { get; set; }

        public string PayStatusDesc { get; set; }

        public string PayOrderNo { get; set; }
    }
}
