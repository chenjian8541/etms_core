using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Activity.Request
{
    public class ActivityMainGetPagingRequest : RequestPagingBase
    {
        public string Name { get; set; }

        public int? ActivityType { get; set; }

        public int? ActivityStatus { get; set; }

        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (!string.IsNullOrEmpty(Name))
            {
                condition.Append($" AND Name LIKE '%{Name}%'");
            }
            if (ActivityType != null)
            {
                condition.Append($" AND ActivityType = {ActivityType}");
            }
            if (ActivityStatus != null)
            {
                if (ActivityStatus == EmActivityStatus.Over)
                {
                    condition.Append($" AND ActivityStatus = {EmActivityStatus.Processing} AND EndTime < {DateTime.Now.EtmsToString()}");
                }
                else
                {
                    condition.Append($" AND ActivityStatus = {ActivityStatus}");
                }
            }
            return condition.ToString();
        }
    }
}
