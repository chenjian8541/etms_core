using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.View
{
    public class StudentCourseView
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 机构ID
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// 是否删除  <see cref=" ETMS.Entity.Enum.EmIsDeleted"/>
        /// </summary>
        public byte IsDeleted { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// 课程ID
        /// </summary>
        public long CourseId { get; set; }

        /// <summary>
        /// 课消方式 <see cref="ETMS.Entity.Enum.EmDeClassTimesType"/>
        /// </summary>
        public byte DeType { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        public int BuyQuantity { get; set; }

        /// <summary>
        /// 购买数量(小单位)
        /// </summary>
        public int BuySmallQuantity { get; set; }

        /// <summary>
        /// 购买单位   <see cref="ETMS.Entity.Enum.EmCourseUnit"/>
        /// </summary>
        public byte BugUnit { get; set; }

        /// <summary>
        /// 赠送数量(课时/月)
        /// </summary>
        public int GiveQuantity { get; set; }

        /// <summary>
        /// 赠送数量 (天)
        /// </summary>
        public int GiveSmallQuantity { get; set; }

        /// <summary>
        /// 消耗数量
        /// </summary>
        public decimal UseQuantity { get; set; }

        /// <summary>
        /// 消耗单位  <see cref="ETMS.Entity.Enum.EmCourseUnit"/>
        /// </summary>
        public byte UseUnit { get; set; }

        /// <summary>
        /// 剩余数量(课时/月)
        /// </summary>
        public decimal SurplusQuantity { get; set; }

        /// <summary>
        /// 剩余数量(天)
        /// </summary>
        public decimal SurplusSmallQuantity { get; set; }

        /// <summary>
        /// 课程分析JOB最后执行时间
        /// </summary>
        public DateTime LastJobProcessTime { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmStudentCourseStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 停课时间
        /// </summary>
        public DateTime? StopTime { get; set; }

        /// <summary>
        /// 复课时间
        /// </summary>
        public DateTime? RestoreTime { get; set; }

        /// <summary>
        /// 超上课时
        /// </summary>
        public int ExceedTotalClassTimes { get; set; }

        public int NotEnoughRemindCount { get; set; }

        public DateTime? NotEnoughRemindLastTime { get; set; }

        public string StudentName { get; set; }

        /// <summary>
        /// 性别  <see cref="ETMS.Entity.Enum.EmGender"/>
        /// </summary>
        public byte? Gender { get; set; }
        public string StudentPhone { get; set; }

        /// <summary>
        /// 学员类型   <see cref="ETMS.Entity.Enum.EmStudentType"/>
        /// </summary>
        public byte StudentType { get; set; }

        public string Avatar { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte StudentCheckDefault { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public long CreateBy { get; set; }

        /// <summary>
        /// 跟进人
        /// </summary>
        public long? TrackUser { get; set; }

        /// <summary>
        /// 学管师
        /// </summary>
        public long? LearningManager { get; set; }

        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 截至时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 是否限制约课
        /// </summary>
        public bool IsLimitReserve { get; set; }
    }
}
