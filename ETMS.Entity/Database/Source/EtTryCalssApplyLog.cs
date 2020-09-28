using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 试听申请记录
    /// </summary>
    [Table("EtTryCalssApplyLog")]
    public class EtTryCalssApplyLog : Entity<long>
    {
        /// <summary>
        /// 学员ID
        /// </summary>
        public long? StudentId { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        public long? CourseId { get; set; }

        public string CourseDesc { get; set; }

        public DateTime? ClassOt { get; set; }

        public string ClassTime { get; set; }

        /// <summary>
        /// 来源  <see cref="ETMS.Entity.Enum.EmTryCalssSourceType"/>
        /// </summary>
        public byte SourceType { get; set; }

        /// <summary>
        /// 推荐的学员
        /// </summary>
        public long? RecommandStudentId { get; set; }

        /// <summary>
        /// 处理状态  <see cref="ETMS.Entity.Enum.EmTryCalssApplyHandleStatus"/>
        /// </summary>
        public byte HandleStatus { get; set; }

        public DateTime? HandleOt { get; set; }

        public long? HandleUser { get; set; }

        public string HandleRemark { get; set; }

        public DateTime ApplyOt { get; set; }
    }
}
