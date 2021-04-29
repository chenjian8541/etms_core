using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Temp.Request
{
    public class TempActiveGrowthRecordDetailGetPagingRequest : RequestPagingBase
    {
        public long GrowthRecordId { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            condition.Append($" AND GrowthRecordId = {GrowthRecordId}");
            return condition.ToString();
        }
    }
}
