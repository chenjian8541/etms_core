using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 学员跟进记录
    /// </summary>
    [Table("EtStudentTrackLog")]
    public class EtStudentTrackLog : Entity<long>
    {
        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// 跟进人
        /// </summary>
        public long TrackUserId { get; set; }

        /// <summary>
        /// 跟进时间
        /// </summary>
        public DateTime TrackTime { get; set; }

        /// <summary>
        /// 下一次跟进时间
        /// </summary>
        public DateTime? NextTrackTime { get; set; }

        /// <summary>
        /// 跟进内容
        /// </summary>
        public string TrackContent { get; set; }

        /// <summary>
        /// 内容类型  <see cref="ETMS.Entity.Enum.EmStudentTrackContentType"/>
        /// </summary>
        public byte ContentType { get; set; }

        /// <summary>
        /// 关联的信息
        /// </summary>
        public long? RelatedInfo { get; set; }
    }
}
