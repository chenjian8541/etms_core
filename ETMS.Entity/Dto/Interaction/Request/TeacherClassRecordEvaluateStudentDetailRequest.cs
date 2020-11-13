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
                return "请求数据不合法";
            }
            return string.Empty;
        }
    }
}
