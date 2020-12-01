using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class StudentCourseNotEnoughCountSaveRequest : RequestBase
    {
        /// <summary>
        /// 学员课时不足提醒提醒次数
        /// </summary>
        public int StudentCourseNotEnoughCount { get; set; }


        public override string Validate()
        {
            if (StudentCourseNotEnoughCount <= 0)
            {
                return "提醒次数必须大于0";
            }
            return string.Empty;
        }
    }
}
