using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Parent4.Output
{
    public class TeacherEvaluateGetPagingOutput
    {
        public long Id { get; set; }
        public long ClassId { get; set; }

        /// <summary>
        /// 班级ID
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        public string StudentName { get; set; }

        /// <summary>
        /// 老师ID
        /// </summary>
        public long TeacherId { get; set; }

        public string TeacherName { get; set; }

        /// <summary>
        /// 评论的媒体文件信息
        /// </summary>
        public List<string> Evaluates { get; set; }

        /// <summary>
        /// 点评内容
        /// </summary>
        public string EvaluateContent { get; set; }

        /// <summary>
        /// 上课时间
        /// </summary>
        public string ClassOt { get; set; }

        /// <summary>
        /// 周
        /// </summary>
        public byte Week { get; set; }

        /// <summary>
        /// 开始
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 结束
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 评价时间
        /// </summary>
        public DateTime Ot { get; set; }
    }
}
