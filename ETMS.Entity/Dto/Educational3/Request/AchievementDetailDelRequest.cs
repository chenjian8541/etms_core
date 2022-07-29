using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational3.Request
{
    public class AchievementDetailDelRequest: RequestBase
    {
        public long DetailId { get; set; }

        public override string Validate()
        {
            if (DetailId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
