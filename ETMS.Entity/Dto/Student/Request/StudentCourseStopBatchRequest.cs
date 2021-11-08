using ETMS.Entity.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentCourseStopBatchRequest : RequestBase
    {
        public List<long> StudentIds { get; set; }

        public DateTime? RestoreTime { get; set; }

        public string Remark { get; set; }

        public override string Validate()
        {
            if (StudentIds == null || StudentIds.Count == 0)
            {
                return "请选择学员";
            }
            if (RestoreTime != null)
            {
                if (!RestoreTime.Value.IsEffectiveDate())
                {
                    return "复课日期格式错误";
                }
                if (RestoreTime.Value <= DateTime.Now)
                {
                    return "复课日期必须大于当前时间";
                }
            }
            return base.Validate();
        }
    }
}
