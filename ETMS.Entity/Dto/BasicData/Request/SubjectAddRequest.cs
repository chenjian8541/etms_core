using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class SubjectAddRequest : RequestBase
    {
        public string Name { get; set; }

        public string Remark { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "请输入名称";
            }
            return string.Empty;
        }
    }
}
