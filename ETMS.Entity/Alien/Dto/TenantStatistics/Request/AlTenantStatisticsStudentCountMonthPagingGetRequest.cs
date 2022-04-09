using ETMS.Entity.Alien.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.TenantStatistics.Request
{
    public class AlTenantStatisticsStudentCountMonthPagingGetRequest: AlienTenantRequestPagingBase
    {
        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(TenantDataFilterWhere());
            return condition.ToString();
        }
    }
}
