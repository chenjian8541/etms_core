using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentChangePwdRequest : RequestBase
    {
        public long CId { get; set; }

        public string NewPwd { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据格式错误";
            }
            if (string.IsNullOrEmpty(NewPwd))
            {
                return "请输入密码";
            }
            return string.Empty;
        }
    }
}
