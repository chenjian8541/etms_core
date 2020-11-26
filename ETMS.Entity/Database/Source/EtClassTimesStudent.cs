using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 班级排课学员信息
    /// </summary>
    [Table("EtClassTimesStudent")]
    public class EtClassTimesStudent : Entity<long>
    {
        /// <summary>
		/// 班级Id
		/// </summary>
		public long ClassId { get; set; }

        /// <summary>
        /// 生成规则ID
        /// </summary>
        public long RuleId { get; set; }

        /// <summary>
        /// 课次Id
        /// </summary>
        public long ClassTimesId { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// 学员类型  <see cref="ETMS.Entity.Enum.EmClassStudentType"/>
        /// </summary>
        public byte StudentType { get; set; }

        /// <summary>
        /// 试听记录
        /// 试听学员对应的试听记录
        /// 补课学员对应的补课记录
        /// </summary>
        public long? StudentTryCalssLogId { get; set; }

        /// <summary>
        /// 上课时间
        /// </summary>
        public DateTime ClassOt { get; set; }

        /// <summary>
        /// 课程ID
        /// </summary>
        public long CourseId { get; set; }

        /// <summary>
        /// 状态   <see cref="ETMS.Entity.Enum.EmClassTimesStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
