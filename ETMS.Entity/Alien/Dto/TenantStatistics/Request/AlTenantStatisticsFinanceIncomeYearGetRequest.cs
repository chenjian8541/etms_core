using ETMS.Entity.Alien.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.TenantStatistics.Request
{
    public class AlTenantStatisticsFinanceIncomeYearGetRequest : AlienTenantRequestBase
    {
        public int? Year { get; set; }

        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.EmIncomeLogType"/>
        /// </summary>
        public byte Type { get; set; }

        public override string Validate()
        {
            if (Year == null)
            {
                return "请选择年份";
            }
            return base.Validate();
        }
    }
}
