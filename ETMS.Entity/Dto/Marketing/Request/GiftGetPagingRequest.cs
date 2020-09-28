using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Marketing.Request
{
    public class GiftGetPagingRequest : RequestPagingBase
    {
        public string Name { get; set; }

        public long? GiftCategoryId { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (!string.IsNullOrEmpty(Name))
            {
                condition.Append($" AND Name LIKE '{Name}%'");
            }
            if (GiftCategoryId != null && GiftCategoryId > 0)
            {
                condition.Append($" AND GiftCategoryId = {GiftCategoryId.Value}");
            }
            return condition.ToString();
        }
    }
}
