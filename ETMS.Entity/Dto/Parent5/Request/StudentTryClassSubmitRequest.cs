using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent5.Request
{
    public class StudentTryClassSubmitRequest : ParentRequestBase
    {
        public long StudentId { get; set; }

        public DateTime? ClassOt { get; set; }

        public string CourseDesc { get; set; }

        public override string Validate()
        {
            if (StudentId <= 0)
            {
                return "请选择学员";
            }
            if (ClassOt == null)
            {
                return "请选择试听时间";
            }
            if (ClassOt.Value < DateTime.Now.Date)
            {
                return "试听时间必须大于当前日期";
            }
            if (string.IsNullOrEmpty(CourseDesc))
            {
                return "请选择试听课程";
            }
            return base.Validate();
        }
    }
}
