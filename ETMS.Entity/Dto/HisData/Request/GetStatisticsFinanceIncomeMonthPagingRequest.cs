using ETMS.Entity.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.HisData.Request
{
    public class GetStatisticsFinanceIncomeMonthPagingRequest : RequestPagingBase
    {
        public int? Year { get; set; }

        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.EmIncomeLogType"/>
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            condition.Append($" AND [Type] = {Type}");
            if (Year != null && Year > 0)
            {
                var startTime = new DateTime(Year.Value, 1, 1);
                var endTime = startTime.AddYears(1).AddDays(-1);
                condition.Append($" AND Ot >= '{startTime.EtmsToDateString()}' AND Ot <= '{endTime.EtmsToDateString()}'");
            }
            return condition.ToString();
        }
    }
}

