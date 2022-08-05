using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentCourseAboutFastDeClassTimesGetRequest: RequestBase
    {
        public long SId { get; set; }

        public override string Validate()
        {
            if (SId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
