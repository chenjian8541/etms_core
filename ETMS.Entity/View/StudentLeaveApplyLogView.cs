using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.View
{
    public class StudentLeaveApplyLogView
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
        /// 开始日期
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 截至日期
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public int StartTime { get; set; }

        /// <summary>
        /// 截至时间
        /// </summary>
        public int EndTime { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime ApplyOt { get; set; }

        /// <summary>
        /// 请假理由
        /// </summary>
        public string LeaveContent { get; set; }

        /// <summary>
        /// 请假图片或者视频
        /// </summary>
        public string LeaveMedias { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmStudentLeaveApplyHandleStatus"/>
        /// </summary>
        public byte HandleStatus { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? HandleOt { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public long? HandleUser { get; set; }

        /// <summary>
        /// 审核备注
        /// </summary>
        public string HandleRemark { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }
    }
}
