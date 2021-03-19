using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class CheckOnLogGetRequest : ParentRequestBase
    {
        public long StudentCheckOnLogId { get; set; }

        public override string Validate()
        {
            if (StudentCheckOnLogId <= 0)
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}
