using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 班级排课信息
    /// </summary>
    [Table("EtClassTimes")]
    public class EtClassTimes : Entity<long>
    {
        /// <summary>
		/// 班级ID
		/// </summary>
		public long ClassId { get; set; }

        /// <summary>
        /// 规则ID
        /// </summary>
        public long RuleId { get; set; }

        /// <summary>
        /// 上课时间
        /// </summary>
        public DateTime ClassOt { get; set; }

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
        /// 学员（临时+试听） 
        /// </summary>
        public string StudentIdsTemp { get; set; }

        /// <summary>
        /// 学员（班级）
        /// </summary>
        public string StudentIdsClass { get; set; }

        /// <summary>
        /// 学员(试听)
        /// </summary>
        public string StudentIdsReservation { get; set; }

        /// <summary>
        /// 学生个数
        /// </summary>
        public int StudentCount { get; set; }

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
        /// 班级容量是否跟随班级改变
        /// </summary>
        public bool LimitStudentNumsIsAlone { get; set; }

        /// <summary>
        /// 此课次的授课课程信息是否跟随班级改变
        /// </summary>
        public bool CourseListIsAlone { get; set; }

        /// <summary>
        /// 授课课程
        /// </summary>
        public string CourseList { get; set; }

        /// <summary>
        /// 此课次的教室信息是否跟随班级改变
        /// </summary>
        public bool ClassRoomIdsIsAlone { get; set; }

        /// <summary>
        /// 教室信息
        /// </summary>
        public string ClassRoomIds { get; set; }

        /// <summary>
        /// 此课次的老师信息是否不跟随班级改变
        /// </summary>
        public bool TeachersIsAlone { get; set; }

        /// <summary>
        /// 老师
        /// </summary>
        public string Teachers { get; set; }

        /// <summary>
        /// 老师个数
        /// </summary>
        public int TeacherNum { get; set; }

        /// <summary>
        /// 点名记录
        /// </summary>
        public long? ClassRecordId { get; set; }

        /// <summary>
        /// 状态   <see cref="ETMS.Entity.Enum.EmClassTimesStatus"/>
        /// </summary>
        public byte Status { get; set; }
    }
}
