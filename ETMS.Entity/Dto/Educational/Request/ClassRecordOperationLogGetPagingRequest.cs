using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassRecordOperationLogGetPagingRequest : RequestPagingBase
    {
        public long ClassRecordId { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            condition.Append($" AND ClassRecordId = {ClassRecordId}");
            return condition.ToString();
        }
    }
}
