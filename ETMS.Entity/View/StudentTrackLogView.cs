using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.View
{
    public class StudentTrackLogView
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 机构ID
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// 是否删除  <see cref=" ETMS.Entity.Enum.EmIsDeleted"/>
        /// </summary>
        public byte IsDeleted { get; set; }

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

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public string StudentPhoneBak { get; set; }
    }
}
