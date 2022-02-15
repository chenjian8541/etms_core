using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Open2.Request
{
    public class MicroWebArticleGetPagingRequest : Open2PagingBase
    {
        public long ColumnId { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            condition.Append($" AND [Status] = {EmMicroWebStatus.Enable} AND ColumnId = {ColumnId}");
            return condition.ToString();
        }

        public override string Validate()
        {
            return base.Validate();
        }
    }
}
