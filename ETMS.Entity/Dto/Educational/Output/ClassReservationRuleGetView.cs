﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Output
{
    public class ClassReservationRuleGetView
    {
        /// <summary>
        /// 开始预约时间(类型)  <see cref="EmStartClassReservaLimitType"/>
        /// </summary>
        public byte StartClassReservaLimitType { get; set; }

        /// <summary>
        /// 开始预约时间(值)
        /// </summary>
        public int StartClassReservaLimitValue { get; set; }

        /// <summary>
        /// 截止预约时间(类型) <see cref="EmDeadlineClassReservaLimitType"/>
        /// </summary>
        public byte DeadlineClassReservaLimitType { get; set; }

        /// <summary>
        /// 截止预约时间(值)
        /// </summary>
        public byte DeadlineClassReservaLimitValue { get; set; }

        /// <summary>
        /// 当截止时间为天时，则从值代表具体时间
        /// </summary>
        public int DeadlineClassReservaLimitDayTimeValue { get; set; }

        public string DeadlineClassReservaLimitDayTimeValueDesc { get; set; }

        /// <summary>
        /// 约次数限制 <see cref="EmMaxCountClassReservaLimitType"/>
        /// </summary>
        public byte MaxCountClassReservaLimitType { get; set; }

        public int MaxCountClassReservaLimitValue { get; set; }

        /// <summary>
        /// 家长端报名人数展示
        /// </summary>
        public bool IsParentShowClassCount { get; set; }
    }
}
