using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class StudentLeaveConfigSaveRequest : RequestBase
    {
        /// <summary>
        /// 学员请假次数限制类型
        /// <see cref=" ETMS.Entity.Enum.EmStudentLeaveApplyMonthLimitType"/>
        /// </summary>
        public byte StudentLeaveApplyMonthLimitType { get; set; }

        /// <summary>
        /// 学员请假次数限制
        /// </summary>
        public int StudentLeaveApplyMonthLimitCount { get; set; }

        /// <summary>
        /// 提前多少小时请假
        /// </summary>
        public int StudentLeaveApplyMustBeforeHour { get; set; }

        public override string Validate()
        {
            if (StudentLeaveApplyMonthLimitCount > 100)
            {
                return "最多可请假次数不能大于100";
            }
            if (StudentLeaveApplyMustBeforeHour > 200)
            {
                return $"提前请假时间限制不能大于200小时";
            }
            return base.Validate();
        }
    }
}
