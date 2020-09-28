using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentOperationLogPagingOutput
    {
        /// <summary>
        /// 操作类型  <see cref="ETMS.Entity.Enum.EmStudentOperationLogType"/>
        /// </summary>
        public string TypeDesc { get; set; }

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
