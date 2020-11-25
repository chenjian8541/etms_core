using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Output
{
    public class TryCalssApplyLogPagingOutput
    {
        public long CId { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public string StudentName { get; set; }

        public string TouristRemark { get; set; }

        public string Phone { get; set; }

        public string CourseDesc { get; set; }

        public string ClassOtDesc { get; set; }

        public string ClassTime { get; set; }

        /// <summary>
        /// 来源  <see cref="ETMS.Entity.Enum.EmTryCalssSourceType"/>
        /// </summary>
        public byte SourceType { get; set; }

        public string SourceTypeDesc { get; set; }

        /// <summary>
        /// 推荐的学员
        /// </summary>
        public string RecommandStudentDesc { get; set; }

        /// <summary>
        /// 处理状态  <see cref="ETMS.Entity.Enum.EmTryCalssApplyHandleStatus"/>
        /// </summary>
        public byte HandleStatus { get; set; }

        public string HandleStatusDesc { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public string HandleOtDesc { get; set; }

        public string HandleUserDesc { get; set; }

        public string HandleRemark { get; set; }

        public DateTime ApplyOt { get; set; }
    }
}
