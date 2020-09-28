using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.View
{
    public class StudentOperationLogView
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 机构ID
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// 是否删除  <see cref=" ETMS.Entity.Enum.EmIsDeleted"/>
        /// </summary>
        public byte IsDeleted { get; set; }

        /// <summary>
        /// 操作用户
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// 操作类型  <see cref="ETMS.Entity.Enum.EmStudentOperationLogType"/>
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 操作内容
        /// </summary>
        public string OpContent { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }
    }
}
