using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentCheckByCardRequest : RequestBase
    {
        public string CardNo { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(CardNo))
            {
                return "请输入卡号";
            }
            return string.Empty;
        }
    }
}
