using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtActiveHomeworkDetail")]
    public class EtActiveHomeworkDetail : Entity<long>
    {
        /// <summary>
        /// 作业ID
        /// </summary>
        public long HomeworkId { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public long CreateUserId { get; set; }

        /// <summary>
        /// 班级ID
        /// </summary>
        public long ClassId { get; set; }

        /// <summary>
        /// 作业标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///作业类型   <see cref="ETMS.Entity.Enum.EmActiveHomeworkType"/>
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// 截止时间
        /// </summary>
        public DateTime? ExDate { get; set; }

        public int? LxExTime { get; set; }

        /// <summary>
        /// 作业要求
        /// </summary>
        public string WorkContent { get; set; }

        /// <summary>
        /// 作业媒体文件(图片、视频)
        /// </summary>
        public string WorkMedias { get; set; }

        /// <summary>
        /// 作答内容
        /// </summary>
        public string AnswerContent { get; set; }

        /// <summary>
        /// 作答媒体文件（图片、视频）
        /// </summary>
        public string AnswerMedias { get; set; }

        public DateTime? AnswerOt { get; set; }

        /// <summary>
        ///作答状态   <see cref="ETMS.Entity.Enum.EmActiveHomeworkDetailAnswerStatus"/>
        /// </summary>
        public byte AnswerStatus { get; set; }

        /// <summary>
        /// 阅读状态  <see cref="ETMS.Entity.Enum.EmActiveHomeworkDetailReadStatus"/>
        /// </summary>
        public byte ReadStatus { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 发布日期
        /// </summary>
        public DateTime? OtDate { get; set; }
    }
}
