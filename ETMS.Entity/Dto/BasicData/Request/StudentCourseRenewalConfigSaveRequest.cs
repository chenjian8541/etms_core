using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class StudentCourseRenewalConfigSaveRequest : RequestBase
    {
        /// <summary>
        /// 剩余课时
        /// </summary>
        public int LimitClassTimes { get; set; }

        /// <summary>
        /// 剩余天数
        /// </summary>
        public int LimitDay { get; set; }

        public override string Validate()
        {
            if (LimitClassTimes < 0 || LimitDay < 0)
            {
                return "课时和天数不能小于0";
            }
            return string.Empty;
        }
    }
}
