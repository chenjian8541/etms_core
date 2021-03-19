using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;
using ETMS.Entity.View;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassReservationRuleSaveRequest : RequestBase
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
        public byte CancelClassReservaType { get; set; }

        /// <summary>
        /// 取消预约时间
        /// </summary>
        public int CancelClassReservaValue { get; set; }

        public override string Validate()
        {
            if (StartClassReservaLimitType != EmStartClassReservaLimitType.NotLimit && StartClassReservaLimitValue <= 0)
            {
                return "请设置开始预约时间";
            }
            if (DeadlineClassReservaLimitType != EmDeadlineClassReservaLimitType.NotLimit && DeadlineClassReservaLimitValue <= 0)
            {
                return "请设置截止预约时间";
            }
            if (DeadlineClassReservaLimitType == EmDeadlineClassReservaLimitType.LimitDay && DeadlineClassReservaLimitDayTimeValue <= 0)
            {
                return "请设置截止预约时间";
            }
            if (MaxCountClassReservaLimitType != EmMaxCountClassReservaLimitType.NotLimit && MaxCountClassReservaLimitValue <= 0)
            {
                return "请设置预约次数限制";
            }
            if (CancelClassReservaValue <= 0)
            {
                return "请设置取消预约时间";
            }
            if (CancelClassReservaType == EmCancelClassReservaType.LimitMinute && CancelClassReservaValue < 5)
            {
                return "取消预约时间不能小于5分钟";
            }
            if (StartClassReservaLimitType != EmStartClassReservaLimitType.NotLimit
                && StartClassReservaLimitType == DeadlineClassReservaLimitType)
            {
                if (StartClassReservaLimitValue < DeadlineClassReservaLimitValue)
                {
                    return "截止预约时间不能早于开始预约时间";
                }
                if (StartClassReservaLimitValue == DeadlineClassReservaLimitValue && StartClassReservaLimitValue != EmStartClassReservaLimitType.LimitDay)
                {
                    return "截止预约时间不能早于开始预约时间";
                }
            }
            return string.Empty;
        }
    }
}
