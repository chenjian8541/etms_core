using ETMS.Entity.Dto.Common.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Output
{
    public class ClassGetOutput
    {
        public long CId { get; set; }

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
        public string DefaultClassTimes { get; set; }

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
        /// 是否允许在线选班
        /// </summary>
        public bool IsCanOnlineSelClass { get; set; }

        /// <summary>
        /// 未到是否收费
        /// </summary>
        public bool IsNotComeCharge { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 一对一约课类型 <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte ReservationType { get; set; }

        public int DurationHour { get; set; }

        public int DurationMinute { get; set; }

        public int DunIntervalMinute { get; set; }

        public List<MultiSelectValueRequest> CourseIds { get; set; }

        public List<MultiSelectValueRequest> TeacherIds { get; set; }

        public List<long> ClassRoomIds { get; set; }

    }
}
