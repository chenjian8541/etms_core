using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.TenantStatistics.Output
{
    public class AlTenantStatisticsStudentCountPagingGetOutput
    {
        public long Id { get; set; }

        /// <summary>
        /// 增加学员人数
        /// </summary>
        public int AddStudentCount { get; set; }

        /// <summary>
        /// 时间(日期)
        /// </summary>
        public DateTime Ot { get; set; }
    }
}
