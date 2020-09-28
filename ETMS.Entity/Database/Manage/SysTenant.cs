using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    /// <summary>
    /// 机构
    /// </summary>
    [Table("SysTenant")]
    public class SysTenant
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 机构名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 机构代码
        /// </summary>
        public string TenantCode { get; set; }

        /// <summary>
        /// 机构电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 数据库ID
        /// </summary>
        public int ConnectionId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 数据状态
        /// </summary>
        public byte IsDeleted { get; set; }
    }
}
