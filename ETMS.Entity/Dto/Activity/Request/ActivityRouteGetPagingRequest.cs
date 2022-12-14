using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Activity.Request
{
    public class ActivityRouteGetPagingRequest : RequestPagingBase
    {
        public long ActivityMainId { get; set; }

        public string Tag { get; set; }

        public int? Status { get; set; }

        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            condition.Append($" AND ActivityId = {ActivityMainId} AND RouteStatus = {EmActivityRouteStatus.Normal}");
            if (!string.IsNullOrEmpty(Tag))
            {
                condition.Append($" AND Tag LIKE '%{Tag}%'");
            }
            if (Status != null)
            {
                if (Status == 0)
                {
                    condition.Append($" AND CountLimit > CountFinish");
                }
                else
                {
                    condition.Append($" AND CountLimit <= CountFinish");
                }
            }
            return condition.ToString();
        }

        public override string Validate()
        {
            if (ActivityMainId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
