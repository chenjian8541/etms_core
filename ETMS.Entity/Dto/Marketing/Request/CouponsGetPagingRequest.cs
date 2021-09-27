using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Marketing.Request
{
    public class CouponsGetPagingRequest : RequestPagingBase
    {
        public string Title { get; set; }

        public int? Status { get; set; }

        public bool IsGetValid { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (!string.IsNullOrEmpty(Title))
            {
                condition.Append($" AND Title like '{Title}%'");
            }
            if (Status != null)
            {
                condition.Append($" AND [Status] = {Status.Value}");
            }
            if (IsGetValid)
            {
                condition.Append($" AND ( EndTime IS NULL OR EndTime = '' OR EndTime >= '{DateTime.Now.ToString("yyyy-MM-dd")}')");
            }
            return condition.ToString();
        }
    }
}
