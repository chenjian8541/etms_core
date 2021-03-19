using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class EvaluateTeacherSubmitRequest : ParentRequestBase
    {
        public long Id { get; set; }

        public long TeacherId { get; set; }

        public int StarValue { get; set; }

        public string EvaluateContent { get; set; }

        public override string Validate()
        {
            if (Id <= 0 || TeacherId <= 0)
            {
                return "请求数据格式错误";
            }
            if (StarValue <= 0 && string.IsNullOrEmpty(EvaluateContent))
            {
                return "请输入评价内容";
            }
            return base.Validate();
        }
    }
}