using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 学员自定义信息
    /// </summary>
    [Table("EtStudentExtendField")]
    public class EtStudentExtendField : Entity<long>
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 数据类型  <see cref="ETMS.Entity.Enum.EmStudentExtendFieldDataType"/>
        /// </summary>
        public byte DataType { get; set; }

        /// <summary>
        /// 数据值
        /// </summary>
        public string DataExtend { get; set; }

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
