using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Product.Request
{
    public class GoodsGetPagingRequest : RequestPagingBase
    {
        public string Name { get; set; }

        public byte? Status { get; set; }

        public bool? IsQueryLimitQuantity { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (!string.IsNullOrEmpty(Name))
            {
                condition.Append($" AND Name like '%{Name}%'");
            }
            if (Status != null)
            {
                condition.Append($" AND [Status] = {Status.Value}");
            }
            if (IsQueryLimitQuantity != null && IsQueryLimitQuantity.Value)
            {
                condition.Append($" AND [Status] = {EmGoodsStatus.Enabled} AND LimitQuantity IS NOT NULL AND InventoryQuantity <= LimitQuantity");
            }
            return condition.ToString();
        }
    }
}
