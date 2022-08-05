using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentCourseFastDeClassTimesRequest : RequestBase
    {
        public long StudentId { get; set; }

        public long CourseId { get; set; }

        public decimal DeClassTimes { get; set; }

        public string SurplusQuantityDesc { get; set; }
        public string Remark { get; set; }

        public override string Validate()
        {
            if (StudentId <= 0 || CourseId <= 0)
            {
                return "请求数据格式错误";
            }
            if (DeClassTimes < 0)
            {
                return "请输入扣的课时";
            }
            return string.Empty;
        }
    }
}
