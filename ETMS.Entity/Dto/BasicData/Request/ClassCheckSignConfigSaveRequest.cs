using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class ClassCheckSignConfigSaveRequest : RequestBase
    {
        /// <summary>
        /// 补课是否扣课时
        /// </summary>
        public bool MakeupIsDeClassTimes { get; set; }

        /// <summary>
        /// 点名奖励的积分是否需要审核
        /// </summary>
        public bool RewardPointsMustApply { get; set; }

        /// <summary>
        /// 点名时按时收费课程必须设置起止日期
        /// </summary>
        public bool DayCourseMustSetStartEndTime { get; set; }

        /// <summary>
        /// 点名时学员必须购买过此课程
        /// </summary>
        public bool MustBuyCourse { get; set; }

        /// <summary>
        /// 点名时是否必须有足够的剩余课时
        /// </summary>
        public bool MustEnoughSurplusClassTimes { get; set; }

        /// <summary>
        /// 学员试听结束是否通知跟进人
        /// </summary>
        public bool TryCalssNoticeTrackUser { get; set; }
    }
}
