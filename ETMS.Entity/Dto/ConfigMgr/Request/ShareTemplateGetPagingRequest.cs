using ETMS.Entity.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.ConfigMgr.Request
{
    public class ShareTemplateGetPagingRequest : RequestPagingBase
    {
        public string Name { get; set; }

        public int? UseType { get; set; }

        public byte? Type { get; set; }

        public byte? Status { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (!string.IsNullOrEmpty(Name))
            {
                condition.Append($" AND Name LIKE '%{Name}%'");
            }
            if (UseType != null)
            {
                condition.Append($" AND [UseType] = {UseType}");
            }
            if (Type != null)
            {
                condition.Append($" AND [Type] = {Type}");
            }
            if (Status != null)
            {
                condition.Append($" AND [Status] = {Status}");
            }
            return condition.ToString();
        }
    }
}
