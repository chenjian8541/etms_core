using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 课程收费规则
    /// </summary>
    [Table("EtCoursePriceRule")]
    public class EtCoursePriceRule : BaseCoursePrice
    {
        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmCoursePriceRuleExTimeLimitTimeType"/>
        /// </summary>
        public int? ExLimitTimeType { get; set; }

        public int? ExLimitTimeValue { get; set; }

        public int? ExLimitDeValue { get; set; }
    }
}
