using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.View
{
    public class ClassReservationSettingView
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

        /// <summary>
        /// 约次数限制 <see cref="EmMaxCountClassReservaLimitType"/>
        /// </summary>
        public byte MaxCountClassReservaLimitType { get; set; }

        public int MaxCountClassReservaLimitValue { get; set; }

        /// <summary>
        /// 家长端报名人数展示
        /// </summary>
        public bool IsParentShowClassCount { get; set; }

        /// <summary>
        /// 取消预约类型  <see cref="EmCancelClassReservaType"/>
        /// </summary>
        public byte CancelClassReservaType { get; set; } = 2;

        /// <summary>
        /// 取消预约时间
        /// </summary>
        public int CancelClassReservaValue { get; set; } = 1;
    }

    public struct EmStartClassReservaLimitType
    {
        /// <summary>
        /// 不限制
        /// </summary>
        public const byte NotLimit = 0;

        /// <summary>
        /// 上课前XX小时
        /// </summary>
        public const byte LimitHour = 2;

        /// <summary>
        /// 上课前XX天
        /// </summary>
        public const byte LimitDay = 3;
    }

    public struct EmDeadlineClassReservaLimitType
    {
        /// <summary>
        /// 不限制
        /// </summary>
        public const byte NotLimit = 0;

        /// <summary>
        /// 上课前XX分钟
        /// </summary>
        public const byte LimitMinute = 1;

        /// <summary>
        /// 上课前XX小时
        /// </summary>
        public const byte LimitHour = 2;

        /// <summary>
        /// 上课前XX天
        /// </summary>
        public const byte LimitDay = 3;
    }

    public struct EmMaxCountClassReservaLimitType
    {
        /// <summary>
        /// 不限制
        /// </summary>
        public const byte NotLimit = 0;

        public const byte SameCourseLimit = 1;
    }

    public struct EmCancelClassReservaType
    {
        /// <summary>
        /// 上课前XX分钟
        /// </summary>
        public const byte LimitMinute = 1;

        /// <summary>
        /// 上课前XX小时
        /// </summary>
        public const byte LimitHour = 2;

        /// <summary>
        /// 上课前XX天
        /// </summary>
        public const byte LimitDay = 3;
    }
}
