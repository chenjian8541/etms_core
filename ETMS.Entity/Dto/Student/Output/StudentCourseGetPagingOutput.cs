using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentCourseGetPagingOutput
    {
        public long CId { get; set; }

        public long StudentId { get; set; }

        /// <summary>
        /// 课程ID
        /// </summary>
        public string CourseName { get; set; }

        public long CourseId { get; set; }

        /// <summary>
        /// 课消方式 <see cref="ETMS.Entity.Enum.EmDeClassTimesType"/>
        /// </summary>
        public string DeTypeDesc { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        public string BuyQuantityDesc { get; set; }

        public string GiveQuantityDesc { get; set; }

        public string UseQuantityDesc { get; set; }

        public string SurplusQuantityDesc { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmStudentCourseStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string StatusDesc { get; set; }

        /// <summary>
        /// 超上课时
        /// </summary>
        public int ExceedTotalClassTimes { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public int NotEnoughRemindCount { get; set; }

        public long Value { get; set; }

        public string Label { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public string BirthdayDesc { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        public long? GradeId { get; set; }

        public string GradeIdDesc { get; set; }

        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }

        /// <summary>
        /// 家庭住址
        /// </summary>
        public string HomeAddress { get; set; }

        /// <summary>
        /// 跟进人
        /// </summary>
        public long? TrackUser { get; set; }

        public string TrackUserDesc { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public long? SourceId { get; set; }

        public string SourceIdDesc { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// 学管师
        /// </summary>
        public long? LearningManager { get; set; }

        public string LearningManagerDesc { get; set; }

        public string ExTimeDesc { get; set; }

        /// <summary>
        /// 性别  <see cref="ETMS.Entity.Enum.EmGender"/>
        /// </summary>
        public byte? Gender { get; set; }

        public string LastDeTimeDesc { get; set; }

        /// <summary>
        /// 剩余学费
        /// </summary>
        public string SurplusMoneyDesc { get; set; }
    }
}
