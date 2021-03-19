using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesAddMakeupStudentRequest : RequestBase
    {
        public long ClassTimesId { get; set; }

        public long ClassRecordAbsenceLogId { get; set; }

        public override string Validate()
        {
            if (ClassTimesId <= 0 || ClassRecordAbsenceLogId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}