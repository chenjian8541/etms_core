using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Output
{
    public class ClassTimesStudentGetOutput
    {
        public long ClassTimesStudentId { get; set; }

        public long ClassTimesId { get; set; }

        /// <summary>
        /// 班级ID
        /// </summary>
        public long ClassId { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        /// <summary>
        /// 性别  <see cref="ETMS.Entity.Enum.EmGender"/>
        /// </summary>
        public byte? Gender { get; set; }

        public string GenderDesc { get; set; }

        /// <summary>
        /// 消耗课程ID
        /// </summary>
        public long CourseId { get; set; }

        public string CourseName { get; set; }

        /// <summary>
        /// 课次剩余描述
        /// </summary>
        public string CourseSurplusDesc { get; set; }

        /// <summary>
        ///  <see cref="ETMS.Entity.Enum.EmClassStudentType"/>
        /// </summary>
        public byte StudentType { get; set; }

        /// <summary>
        /// 试听记录
        /// 试听学员对应的试听记录
        /// </summary>
        public long? StudentTryCalssLogId { get; set; }

        public string StudentTypeDesc { get; set; }

        public string DefaultClassTimes { get; set; }

        public bool IsCheckAttendance { get; set; }

        public int Points { get; set; }

        public string StudentAvatar { get; set; }

        /// <summary>
        /// 是否请假
        /// </summary>
        public bool IsLeave { get; set; }

        /// <summary>
        /// 请假时间
        /// </summary>
        public string LeaveDesc { get; set; }

        /// <summary>
        /// 请假内容
        /// </summary>
        public string LeaveContent { get; set; }
    }
}
