using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassOneToOneSetReservationBatchRequest : RequestBase
    {
        public List<long> ClassIds { get; set; }

        /// <summary>
        /// 一对一约课类型 <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte ReservationType { get; set; }

        public int DurationHour { get; set; }

        public int DurationMinute { get; set; }

        public int DunIntervalMinute { get; set; }

        public override string Validate()
        {
            if (ClassIds == null || ClassIds.Count == 0)
            {
                return "请选择班级";
            }
            if (ClassIds.Count > 20)
            {
                return "一次性最多设置20个班级";
            }
            if (ReservationType == EmBool.True && DurationHour == 0 && DurationMinute == 0)
            {
                return "请设置老师上课时长";
            }
            return base.Validate();
        }
    }
}
