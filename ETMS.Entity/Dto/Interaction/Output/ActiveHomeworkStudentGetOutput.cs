using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class ActiveHomeworkStudentGetOutput
    {
        public long HomeworkId { get; set; }

        public long HomeworkDetailId { get; set; }

        public string TeacherName { get; set; }

        public string TeacherAvatar { get; set; }

        /// <summary>
        /// 班级ID
        /// </summary>
        public long ClassId { get; set; }

        /// <summary>
        /// 班级名称
        /// </summary>
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

        public string ExDateDesc { get; set; }

        /// <summary>
        /// 作业要求
        /// </summary>
        public string WorkContent { get; set; }

        public List<string> WorkMediasUrl { get; set; }

        public byte ReadStatus { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public string OtDesc { get; set; }

        /// <summary>
        /// <see cref="AnsweredOutputAnswerStatus"/>
        /// </summary>
        public byte AnswerStatus { get; set; }

        public string AnswerStatusDesc { get; set; }

        public HomeworkDetailAnswerInfo2 AnswerInfo { get; set; }

        public List<CommentOutput> CommentOutputs { get; set; }
    }

    public class HomeworkDetailAnswerInfo2
    {
        public string AnswerOtDesc { get; set; }

        public string AnswerContent { get; set; }

        public List<string> AnswerMediasUrl { get; set; }
    }
}
