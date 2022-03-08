using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 上课记录
    /// </summary>
    [Table("EtClassRecord")]
    public class EtClassRecord : Entity<long>
    {
        /// <summary>
        /// 班级ID
        /// </summary>
        public long ClassId { get; set; }

        /// <summary>
        /// 班级类别
        /// </summary>
        public long? ClassCategoryId { get; set; }

        /// <summary>
        /// 课次ID
        /// </summary>
        public long? ClassTimesId { get; set; }

        /// <summary>
        /// 上课时间
        /// </summary>
        public DateTime ClassOt { get; set; }

        /// <summary>
        /// 课次列表，各课程之间以“,”隔开
        /// </summary>
        public string CourseList { get; set; }

        /// <summary>
        /// 周
        /// </summary>
        public byte Week { get; set; }

        /// <summary>
        /// 开始时间（记录小时分钟）
        /// 08:30->830
        /// 11:30->1130
        /// </summary>
        public int StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public int EndTime { get; set; }

        /// <summary>
        /// 上课内容
        /// </summary>
        public string ClassContent { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        public string ClassRoomIds { get; set; }

        /// <summary>
        /// 上课老师
        /// </summary>
        public string Teachers { get; set; }

        /// <summary>
        /// 上课老师个数
        /// </summary>
        public int TeacherNum { get; set; }

        /// <summary>
        /// 学员
        /// </summary>
        public string StudentIds { get; set; }

        /// <summary>
        /// 授课课时
        /// </summary>
        public decimal ClassTimes { get; set; }

        /// <summary>
        /// 课消金额
        /// </summary>
        public decimal DeSum { get; set; }

        /// <summary>
        /// 到课人数
        /// </summary>
        public int AttendNumber { get; set; }

        /// <summary>
        /// 应到课人数
        /// </summary>
        public int NeedAttendNumber { get; set; }

        /// <summary>
        /// 评价学员数量
        /// </summary>
        public int EvaluateStudentCount { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmClassRecordStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 点名时间
        /// </summary>
        public DateTime CheckOt { get; set; }

        /// <summary>
        /// 点名老师
        /// </summary>
        public long CheckUserId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
