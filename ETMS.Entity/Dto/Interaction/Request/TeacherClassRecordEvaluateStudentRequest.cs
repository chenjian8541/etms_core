using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class TeacherClassRecordEvaluateStudentRequest : RequestBase
    {
        public long ClassRecordId { get; set; }

        public override string Validate()
        {
            if (ClassRecordId <= 0)
            {
                return "请求数据不合法";
            }
            return string.Empty;
        }
    }
}