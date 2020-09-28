using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 年级
    /// </summary>
    [Table("EtGrade")]
    public class EtGrade : Entity<long>
    {
        /// <summary>
		/// 名称
		/// </summary>
		public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
