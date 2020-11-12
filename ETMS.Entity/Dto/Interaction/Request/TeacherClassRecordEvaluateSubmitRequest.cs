using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class TeacherClassRecordEvaluateSubmitRequest : RequestBase
    {
        public long ClassRecordId { get; set; }

        public long StudentId { get; set; }

        public string EvaluateContent { get; set; }

        public override string Validate()
        {
            if (ClassRecordId <= 0 || StudentId <= 0)
            {
                return "请求数据不合法";
            }
            if (string.IsNullOrEmpty(EvaluateContent))
            {
                return "请输入点评内容";
            }
            return string.Empty;
        }
    }
}
