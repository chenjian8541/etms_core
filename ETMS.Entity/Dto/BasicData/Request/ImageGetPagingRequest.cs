using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class ImageGetPagingRequest : RequestPagingBase
    {
        public int? Type { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (Type != null)
            {
                condition.Append($" AND [Type] = {Type.Value}");
            }
            return condition.ToString();
        }
    }
}
