using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class StartClassNoticeSaveRequest : RequestBase
    {
        /// <summary>
        /// 是否提前一天提醒（学员上课提醒）
        /// </summary>
        public bool StartClassDayBeforeIsOpen { get; set; }

        /// <summary>
        /// 提前一天提醒,默认晚上7点(学员上课提醒)
        /// </summary>
        public int StartClassDayBeforeTimeValue { get; set; }

        /// <summary>
        /// 是否提前几分钟提醒(学员上课提醒)
        /// </summary>
        public bool StartClassBeforeMinuteIsOpen { get; set; }

        /// <summary>
        /// 前几分钟提醒，默认30分钟(学员上课提醒)
        /// </summary>
        public int StartClassBeforeMinuteValue { get; set; }
    }
}
