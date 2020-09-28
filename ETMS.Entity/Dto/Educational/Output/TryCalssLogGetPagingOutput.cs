using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Output
{
    public class TryCalssLogGetPagingOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// 试听课程
        /// </summary>
        public long CourseId { get; set; }

        public string CourseDesc { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        public long ClassId { get; set; }

        public string ClassDesc { get; set; }

        /// <summary>
        /// 班次
        /// </summary>
        public long ClassTimesId { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmTryCalssLogStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string StatusDesc { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        public long UserId { get; set; }

        public string ClassOtDesc { get; set; }

        public byte? Week { get; set; }

        public int? StartTime { get; set; }

        public int? EndTime { get; set; }

        public string Teachers { get; set; }

        public string TeachersDesc { get; set; }

        public string ClassContent { get; set; }

        public string TrackUserName { get; set; }

        public string LearningManagerName { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }
    }
}
