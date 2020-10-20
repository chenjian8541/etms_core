using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtActiveHomework")]
    public class EtActiveHomework: Entity<long>
    {
        /// <summary>
        /// 创建人(老师)
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

        /// <summary>
        /// 作业要求
        /// </summary>
        public string WorkContent { get; set; }

        /// <summary>
        /// 作业媒体文件(图片、视频、音频)
        /// 媒体文件直减以“|”隔开
        /// </summary>
        public string WorkMedias { get; set; }

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
        /// 状态  <see cref="ETMS.Entity.Enum.EmActiveHomeworkStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime Ot { get; set; }

    }
}
