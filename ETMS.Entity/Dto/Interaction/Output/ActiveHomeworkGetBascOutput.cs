using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class ActiveHomeworkGetBascOutput
    {
        public long HomeworkId { get; set; }

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

        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 已阅数量
        /// </summary>
        public int ReadCount { get; set; }

        /// <summary>
        /// 完成数量
        /// </summary>
        public int FinishCount { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public string OtDesc { get; set; }

        public string LxStartDateDesc { get; set; }

        public string LxEndDateDesc { get; set; }

        public int LxTotalCount { get; set; }

        public List<HomeworkStudent> Students { get; set; }
    }

    public class HomeworkStudent
    {
        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public bool IsSelect { get; set; }
    }
}
