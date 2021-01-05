using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 课程
    /// </summary>
    [Table("EtCourse")]
    public class EtCourse : Entity<long>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 类型   <see cref="ETMS.Entity.Enum.EmCourseType"/>
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// 课程颜色
        /// </summary>
        public string StyleColor { get; set; }

        /// <summary>
        /// 是否启用 <see cref="ETMS.Entity.Enum.EmCourseStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 点名或者考勤时默认赠送的积分
        /// </summary>
        public int CheckPoints { get; set; }

        /// <summary>
        /// 收费类型   <see cref="ETMS.Entity.Enum.EmCoursePriceType"/>
        /// </summary>
        public byte PriceType { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
