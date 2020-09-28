using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 学员自定义信息
    /// </summary>
    [Table("EtStudentExtendInfo")]
    public class EtStudentExtendInfo : Entity<long>
    {
        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// 扩展ID
        /// </summary>
        public long ExtendFieldId { get; set; }

        /// <summary>
        /// 值1
        /// </summary>
        public string Value1 { get; set; }

        /// <summary>
        /// 值2
        /// </summary>
        public string Value2 { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }
}
