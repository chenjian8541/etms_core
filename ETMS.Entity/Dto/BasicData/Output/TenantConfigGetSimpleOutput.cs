using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.BasicData.Output
{
    public class TenantConfigGetSimpleOutput
    {
        /// <summary>
        /// 考勤记上课  只是直接扣课时
        /// </summary>
        public bool IsEnableStudentCheckDeClassTimes { get; set; }

        /// <summary>
        /// 点名时扣减课时是否允许输入小数
        /// </summary>
        public bool IsCanDeDecimal { get; set; }
    }
}
