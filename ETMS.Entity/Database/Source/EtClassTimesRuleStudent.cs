using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtClassTimesRuleStudent")]
    public class EtClassTimesRuleStudent : BaseClassStudent
    {
        /// <summary>
        /// 排课规则
        /// </summary>
        public long RuleId { get; set; }
    }
}
