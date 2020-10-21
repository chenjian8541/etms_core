using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class HomeworkAnsweredGetPagingOutput
    {
        /// <summary>
        /// 作业详情ID
        /// </summary>
        public long HomeworkDetailId { get; set; }

        /// <summary>
        /// 作业ID
        /// </summary>
        public long HomeworkId { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// 学员名称
        /// </summary>
        public string StudentName { get; set; }

        public string TeacherName { get; set; }

        /// <summary>
        /// 班级ID
        /// </summary>
        public long ClassId { get; set; }

        public string ClassName { get; set; }

        /// <summary>
        /// 作业标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///作业类型   <see cref="ETMS.Entity.Enum.EmActiveHomeworkType"/>
        /// </summary>
        public byte Type { get; set; }

        public string TypeDesc { get; set; }

        public string AnswerOtDesc { get; set; }

        /// <summary>
        ///作答状态   <see cref="ETMS.Entity.Enum.EmActiveHomeworkDetailAnswerStatus"/>
        /// </summary>
        public byte AnswerStatus { get; set; }

        public string AnswerStatusDesc { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public string OtDesc { get; set; }
    }
}
