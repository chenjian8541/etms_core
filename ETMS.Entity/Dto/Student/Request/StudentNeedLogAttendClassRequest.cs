using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentNeedLogAttendClassRequest : RequestBase
    {
        public long NeedCheckClassLogId { get; set; }

        public string CheckTime { get; set; }

        public override string Validate()
        {
            if (NeedCheckClassLogId <= 0)
            {
                return "请求数据不合法";
            }
            if (string.IsNullOrEmpty(CheckTime))
            {
                return "请选择签到时间";
            }
            return string.Empty;
        }
    }
}