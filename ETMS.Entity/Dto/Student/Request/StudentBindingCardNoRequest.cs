using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentBindingCardNoRequest : RequestBase
    {
        public long CId { get; set; }

        public string NewCardNo { get; set; }

        public byte HkCardStatus { get; set; }
        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据格式错误";
            }
            if (string.IsNullOrEmpty(NewCardNo))
            {
                return "卡号不能为空";
            }
            if (NewCardNo.Length > 30)
            {
                return "卡号长度不能超过30位";
            }
            return string.Empty;
        }
    }
}

