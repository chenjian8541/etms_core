using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class StudentCheckInConfigSaveRequest : RequestBase
    {
        /// <summary>
        /// 学员刷卡间隔时间(单位秒)
        /// </summary>
        public int IntervalTimeCard { get; set; }

        /// <summary>
        /// 学员是否需要刷卡签退  <see cref="EmBool"/>
        /// </summary>
        public byte IsMustCheckOutCard { get; set; }

        /// <summary>
        /// 刷卡记上课
        /// </summary>
        public byte IsRelationClassTimesCard { get; set; }

        /// <summary>
        /// 关联的上课课次时间不超过的分钟数
        /// </summary>
        public int RelationClassTimesLimitMinuteCard { get; set; }

        /// <summary>
        /// 学员刷脸间隔时间(单位秒)
        /// </summary>
        public int IntervalTimeFace { get; set; }

        /// <summary>
        /// 学员是否需要刷卡签退  <see cref="EmBool"/>
        /// </summary>
        public byte IsMustCheckOutFace { get; set; }

        /// <summary>
        /// 刷脸记上课
        /// </summary>
        public byte IsRelationClassTimesFace { get; set; }

        /// <summary>
        /// 关联的上课课次时间不超过的分钟数
        /// </summary>
        public int RelationClassTimesLimitMinuteFace { get; set; }

        public override string Validate()
        {
            if (IntervalTimeCard < 5 || IntervalTimeFace < 5 || IntervalTimeCard > 6000 || IntervalTimeFace > 6000)
            {
                return "间隔时间范围5~6000秒";
            }
            if (RelationClassTimesLimitMinuteCard > 60 || RelationClassTimesLimitMinuteFace > 60)
            {
                return "考勤时间与上课时间的间隔范围0~60分钟";
            }
            return base.Validate();
        }
    }
}
