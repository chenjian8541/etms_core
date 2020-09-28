using ETMS.Entity.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class ClassSetAddRequest : RequestBase
    {
        public string StartTimeDesc { get; set; }

        public string EndTimeDesc { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(StartTimeDesc))
            {
                return "请输入开始时间";
            }
            if (string.IsNullOrEmpty(EndTimeDesc))
            {
                return "请输入结束时间";
            }
            if (EndTimeDesc.Replace(":", "").ToInt() <= StartTimeDesc.Replace(":", "").ToInt())
            {
                return "结束时间必须大于开始时间";
            }
            return string.Empty;
        }
    }
}
