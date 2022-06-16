using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Activity.Request
{
    public class ActivitySystemGetPagingRequest : RequestPagingBase
    {
        public int? ActivityType { get; set; }

        public int? Scenetype { get; set; }

        public override string ToString()
        {
            var condition = new StringBuilder($"IsDeleted = {EmIsDeleted.Normal}");
            if (ActivityType != null)
            {
                condition.Append($" AND ActivityType = {ActivityType}");
            }
            if (Scenetype != null)
            {
                condition.Append($" AND Scenetype = {Scenetype}");
            }
            return condition.ToString();
        }
    }
}
