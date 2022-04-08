using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.TenantStatistics.Output
{
    public class AlTenantStatisticsFinanceIncomeMonthGetPagingOutput
    {
        public DateTime Ot { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.EmIncomeLogType"/>
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// 项目名称  <see cref=" ETMS.Entity.Enum.EmIncomeLogProjectType"/>
        /// </summary>
        public long ProjectType { get; set; }

        public string ProjectTypeDesc { get; set; }

        public int TotalCount { get; set; }

        /// <summary>
        /// 合计金额
        /// </summary>
        public decimal TotalSum { get; set; }
    }
}
