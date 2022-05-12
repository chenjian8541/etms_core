using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Marketing.Request
{
    public class StudentSmsSendRequest : RequestBase
    {
        public string SmsContent { get; set; }

        public List<SmsSendStudentInfo> Students { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(SmsContent))
            {
                return "短信内容不能为空";
            }
            if (SmsContent.Length > 200)
            {
                return "短信内容不能超过200个字符";
            }
            if (Students == null || Students.Count == 0)
            {
                return "请选择学员";
            }
            if (Students.Count > 100)
            {
                return "一次性最多给100位学员发送短信";
            }
            return base.Validate();
        }
    }

    public class SmsSendStudentInfo
    {
        public long CId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }
    }
}
