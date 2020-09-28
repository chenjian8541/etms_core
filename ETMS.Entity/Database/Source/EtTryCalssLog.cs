using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 试听记录
    /// </summary>
    [Table("EtTryCalssLog")]
    public class EtTryCalssLog : Entity<long>
    {
        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// 试听课程
        /// </summary>
        public long CourseId { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        public long ClassId { get; set; }

        /// <summary>
        /// 班次
        /// </summary>
        public long ClassTimesId { get; set; }

        /// <summary>
        /// 试听时间(上课时间)
        /// </summary>
        public DateTime TryOt { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmTryCalssLogStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        public long UserId { get; set; }
    }
}
