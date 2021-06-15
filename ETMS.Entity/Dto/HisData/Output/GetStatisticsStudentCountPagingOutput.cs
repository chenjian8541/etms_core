using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Output
{
    public class GetStatisticsStudentCountPagingOutput
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
