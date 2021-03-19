using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class TeacherClassRecordEvaluateStudentDetailRequest : RequestBase
    {
        public long ClassRecordStudentId { get; set; }

        public override string Validate()
        {
            if (ClassRecordStudentId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
