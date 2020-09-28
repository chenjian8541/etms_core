using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class GiftGetParentRequest: ParentRequestPagingBase
    {
        public long? GiftCategoryId { get; set; }

        protected override string DataFilterWhere(string studentFieldName = "StudentId")
        {
            return $"TenantId = {LoginTenantId} AND IsDeleted = {EmIsDeleted.Normal}";
        }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere());
            if (GiftCategoryId != null && GiftCategoryId > 0)
            {
                condition.Append($" AND GiftCategoryId = {GiftCategoryId.Value}");
            }
            return condition.ToString();
        }
    }
}
