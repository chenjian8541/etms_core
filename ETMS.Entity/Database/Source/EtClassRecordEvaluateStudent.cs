using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 课后评价学员
    /// </summary>
    [Table("EtClassRecordEvaluateStudent")]
    public class EtClassRecordEvaluateStudent : Entity<long>
    {
        /// <summary>
		/// 班级ID
		/// </summary>
		public long ClassId { get; set; }

        /// <summary>
        /// 点名记录
        /// </summary>
        public long ClassRecordId { get; set; }

        /// <summary>
        /// 点名记录学员ID
        /// </summary>
        public long ClassRecordStudentId { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// 学员类型 <see cref=" ETMS.Entity.Enum.EmClassStudentType"/>
        /// </summary>
        public byte StudentType { get; set; }

        /// <summary>
        /// 上课老师
        /// </summary>
        public string Teachers { get; set; }

        /// <summary>
        /// 老师ID
        /// </summary>
        public long TeacherId { get; set; }

        /// <summary>
        /// 评论的媒体文件信息
        /// </summary>
        public string EvaluateImg { get; set; }

        /// <summary>
        /// 点评内容
        /// </summary>
        public string EvaluateContent { get; set; }

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
        /// 评价时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 点名时间
        /// </summary>
        public DateTime CheckOt { get; set; }

        /// <summary>
        /// 点名老师
        /// </summary>
        public long CheckUserId { get; set; }

        /// <summary>
        /// 是否已读
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmClassRecordStatus"/>
        /// </summary>
        public byte Status { get; set; }
    }
}
