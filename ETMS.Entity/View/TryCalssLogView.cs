using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.View
{
    public class TryCalssLogView
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

        public DateTime? ClassOt { get; set; }

        public byte? Week { get; set; }

        public int? StartTime { get; set; }

        public int? EndTime { get; set; }

        public string Teachers { get; set; }

        public string ClassContent { get; set; }
    }
}
