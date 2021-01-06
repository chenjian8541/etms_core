using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Output
{
    public class StatisticsTenantGetOutput
    {
        /// <summary>
        /// 本月收入
        /// </summary>
        public decimal IncomeThisMonth { get; set; }

        /// <summary>
        /// 本月新增学员
        /// </summary>
        public int AddStudentThisMonth { get; set; }

        /// <summary>
        /// 本月授课课时
        /// </summary>
        public int ClassTimesThisMonth { get; set; }

        /// <summary>
        /// 今日实到人次
        /// </summary>
        public int ClassActuallyCountToday { get; set; }

        /// <summary>
        /// 今日请假人次
        /// </summary>
        public int ClassLeaveCountToday { get; set; }

        /// <summary>
        /// 今日未到人次
        /// </summary>
        public int ClassNotArrivedToday { get; set; }


        /// <summary>
        /// 是否启用数据限制
        /// </summary>
        public bool IsDataLimit { get; set; }
    }
}
