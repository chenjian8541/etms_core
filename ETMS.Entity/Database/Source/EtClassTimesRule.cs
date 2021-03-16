using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 班级排课规则
    /// </summary>
    [Table("EtClassTimesRule")]
    public class EtClassTimesRule : Entity<long>
    {
        /// <summary>
        /// 班级ID
        /// </summary>
        public long ClassId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 循环类型  <see cref=" ETMS.Entity.Enum.EmClassTimesRuleType"/>
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// 周
        /// </summary>
        public byte Week { get; set; }

        /// <summary>
        /// 开始
        /// </summary>
        public int StartTime { get; set; }

        /// <summary>
        /// 结束
        /// </summary>
        public int EndTime { get; set; }

        /// <summary>
        /// 预约类型  <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte ReservationType { get; set; }

        /// <summary>
        /// 上课内容
        /// </summary>
        public string ClassContent { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        public string ClassRoomIds { get; set; }

        /// <summary>
        /// 教程
        /// </summary>
        public string CourseList { get; set; }

        /// <summary>
        /// 老师
        /// </summary>
        public string Teachers { get; set; }

        /// <summary>
        /// 是否跳过节假日
        /// </summary>
        public bool IsJumpHoliday { get; set; }

        /// <summary>
        /// 是否需要job定时生成课次
        /// </summary>
        public bool IsNeedLoop { get; set; }

        /// <summary>
        /// 课程分析JOB最后执行时间
        /// </summary>
        public DateTime LastJobProcessTime { get; set; }

        /// <summary>
        /// 排课方式描述
        /// </summary>
        public string RuleDesc { get; set; }

        /// <summary>
        /// 上课日期描述
        /// </summary>
        public string DateDesc { get; set; }

        /// <summary>
        /// 上课时间描述
        /// </summary>
        public string TimeDesc { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }
}
