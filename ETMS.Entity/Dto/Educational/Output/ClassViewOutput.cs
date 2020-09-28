using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Output
{
    public class ClassViewOutput
    {
        public long CId { get; set; }

        /// <summary>
        /// 班级类型  <see cref="ETMS.Entity.Enum.EmClassType"/>
        /// </summary>
        public byte Type { get; set; }

        public string TypeDesc { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string ClassCategoryDesc { get; set; }

        /// <summary>
        /// 课程列表，各课程Id之间以“,”隔开
        /// </summary>
        public string CourseDesc { get; set; }

        /// <summary>
        /// 学员个数
        /// </summary>
        public int StudentNums { get; set; }

        /// <summary>
        /// 班级容量
        /// </summary>
        public int? LimitStudentNums { get; set; }

        public string LimitStudentNumsDesc { get; set; }

        /// <summary>
        /// 默认课时
        /// </summary>
        public int DefaultClassTimes { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        public string ClassRoomDesc { get; set; }

        /// <summary>
        /// 老师，各老师Id之间以“,”隔开
        /// </summary>
        public string TeachersDesc { get; set; }

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
        /// 排课状态  <see cref="ETMS.Entity.Enum.EmClassScheduleStatus"/>
        /// </summary>
        public byte ScheduleStatus { get; set; }

        public string ScheduleStatusDesc { get; set; }

        /// <summary>
        /// 结业状态 <see cref="ETMS.Entity.Enum.EmClassCompleteStatus"/>
        /// </summary>
        public byte CompleteStatus { get; set; }

        public string CompleteStatusDesc { get; set; }
        public string CompleteTimeDesc { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public string OneToOneStudentName { get; set; }

        public string OneToOneStudentPhone { get; set; }

        public string CourseList { get; set; }

        public long Value { get; set; }

        public string Label { get; set; }
    }
}
