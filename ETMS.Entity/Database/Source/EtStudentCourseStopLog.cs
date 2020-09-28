using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 学员停课记录
    /// </summary>
    [Table("EtStudentCourseStopLog")]
    public class EtStudentCourseStopLog : Entity<long>
    {
        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// 课程ID
        /// </summary>
        public long CourseId { get; set; }

        /// <summary>
        /// 停课时间
        /// </summary>
        public DateTime StopTime { get; set; }

        /// <summary>
        /// 复课时间(复课后自动填充)
        /// </summary>
        public DateTime? RestoreTime { get; set; }

        /// <summary>
        /// 停课天数
        /// </summary>
        public int StopDay { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
