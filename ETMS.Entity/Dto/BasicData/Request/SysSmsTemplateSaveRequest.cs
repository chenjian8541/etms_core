using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class SysSmsTemplateSaveRequest : RequestBase
    {
        public string SmsContent { get; set; }

        public int Type { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(SmsContent))
            {
                return "短信内容不能为空";
            }
            return base.Validate();
        }
    }
}
