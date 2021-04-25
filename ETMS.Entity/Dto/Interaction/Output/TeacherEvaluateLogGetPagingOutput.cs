using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class TeacherEvaluateLogGetPagingOutput
    {
        public long EvaluateStudentRecordId { get; set; }

        /// <summary>
        /// 评价时间
        /// </summary>
        public DateTime EvaluateOt { get; set; }

        /// <summary>
        /// 评价人
        /// </summary>
        public string EvaluateUserName { get; set; }

        /// <summary>
        /// 班级名称
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 老师
        /// </summary>
        public string TeachersDesc { get; set; }

        /// <summary>
        /// 学生名称
        /// </summary>
        public string StudentName { get; set; }

        /// <summary>
        /// 学生姓名
        /// </summary>
        public string StudentPhone { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        public string EvaluateContent { get; set; }

        public List<string> EvaluateMedias { get; set; }

        public string ClassOtDesc { get; set; }

        public string ClassTimeDesc { get; set; }

        public string WeekDesc { get; set; }

        /// <summary>
        /// 是否查看
        /// </summary>
        public bool IsRead { get; set; }
    }
}
