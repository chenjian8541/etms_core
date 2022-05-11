using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 班级表
    /// </summary>
    [Table("EtClass")]
    public class EtClass : Entity<long>
    {
        /// <summary>
        /// 班级类型  <see cref="ETMS.Entity.Enum.EmClassType"/>
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public long? ClassCategoryId { get; set; }

        /// <summary>
        /// 课程列表，各课程Id之间以“,”隔开
        /// </summary>
        public string CourseList { get; set; }

        /// <summary>
        /// 学员列表,各学员Id之间以“,”隔开
        /// </summary>
        public string StudentIds { get; set; }

        /// <summary>
        /// 学员个数
        /// </summary>
        public int StudentNums { get; set; }

        /// <summary>
        /// 班级容量
        /// </summary>
        public int? LimitStudentNums { get; set; }

        /// <summary>
        /// 班级容量类型
        /// <see cref="ETMS.Entity.Enum.EmLimitStudentNumsType"/>
        /// </summary>
        public byte LimitStudentNumsType { get; set; }

        /// <summary>
        /// 默认课时
        /// </summary>
        public decimal DefaultClassTimes { get; set; }

        /// <summary>
        /// 教室, 各教室Id之间以“,”隔开
        /// </summary>
        public string ClassRoomIds { get; set; }

        /// <summary>
        /// 老师，各老师Id之间以“,”隔开
        /// </summary>
        public string Teachers { get; set; }

        /// <summary>
        /// 老师个数
        /// </summary>
        public int TeacherNum { get; set; }

        /// <summary>
        /// 已排课次数
        /// </summary>
        public int PlanCount { get; set; }

        /// <summary>
        /// 已完成课次数
        /// </summary>
        public int FinishCount { get; set; }

        /// <summary>
        /// 已完成课时 
        /// </summary>
        public int FinishClassTimes { get; set; }

        /// <summary>
        /// 请假是否收费
        /// </summary>
        public bool IsLeaveCharge { get; set; }

        /// <summary>
        /// 未到是否收费
        /// </summary>
        public bool IsNotComeCharge { get; set; }

        /// <summary>
        /// 是否允许在线选班
        /// </summary>
        public bool IsCanOnlineSelClass { get; set; }

        /// <summary>
        /// 排课状态  <see cref="ETMS.Entity.Enum.EmClassScheduleStatus"/>
        /// </summary>
        public byte ScheduleStatus { get; set; }

        /// <summary>
        /// 结业状态 <see cref="ETMS.Entity.Enum.EmClassCompleteStatus"/>
        /// </summary>
        public byte CompleteStatus { get; set; }

        /// <summary>
        /// 结业时间
        /// </summary>
        public DateTime? CompleteTime { get; set; }

        public long? OrderId { get; set; }

        /// <summary>
        /// 课程分析JOB最后执行时间
        /// </summary>
        public DateTime LastJobProcessTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 一对一约课类型 <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte ReservationType { get; set; }

        public int DurationHour { get; set; }

        public int DurationMinute { get; set; }

        public int DunIntervalMinute { get; set; }

        /// <summary>
        /// 数据类型   <see cref="ETMS.Entity.Enum.EmClassDataType"/>
        /// </summary>
        public byte DataType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
