using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Output
{
    public class TenantConfigGetOutput
    {
        /// <summary>
        /// 点名设置
        /// </summary>
        public ClassCheckSignConfig ClassCheckSignConfig { get; set; }

        /// <summary>
        /// 通知设置
        /// </summary>
        public StudentNoticeConfig StudentNoticeConfig { get; set; }

        /// <summary>
        /// 续费预警设置
        /// </summary>
        public StudentCourseRenewalConfig StudentCourseRenewalConfig { get; set; }

        /// <summary>
        /// 打印设置
        /// </summary>
        public PrintConfig PrintConfig { get; set; }

        /// <summary>
        /// 家长端设置
        /// </summary>
        public ParentSetConfig ParentSetConfig { get; set; }

        public OtherOutput OtherOutput { get; set; }
    }

    /// <summary>
    /// 需要处理的数据
    /// </summary>
    public class OtherOutput
    {
        /// <summary>
        /// 提前一天提醒,默认晚上7点(学员上课提醒)
        /// </summary>
        public string StartClassDayBeforeTimeValueDesc { get; set; }
    }
}
