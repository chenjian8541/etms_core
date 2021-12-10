using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentLeaveApplyLogPagingOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long CId { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public string StartDateDesc { get; set; }

        /// <summary>
        /// 截至日期
        /// </summary>
        public string EndDateDesc { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTimeDesc { get; set; }

        /// <summary>
        /// 截至时间
        /// </summary>
        public string EndTimeDesc { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime ApplyOt { get; set; }

        /// <summary>
        /// 请假理由
        /// </summary>
        public string LeaveContent { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmStudentLeaveApplyHandleStatus"/>
        /// </summary>
        public byte HandleStatus { get; set; }

        public string HandleStatusDesc { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public string HandleOtDesc { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public long? HandleUser { get; set; }

        public string HandleUserDesc { get; set; }

        /// <summary>
        /// 审核备注
        /// </summary>
        public string HandleRemark { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        /// <summary>
        /// 请假媒体文件
        /// </summary>
        public List<string> LeaveMediasUrl { get; set; }

        public string StudentAvatarUrl { get; set; }
    }
}
