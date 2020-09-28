using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Output
{
    public class UserOperationLogGetPagingOutput
    {
        /// <summary>
        /// 操作类型  <see cref="ETMS.Entity.Enum.EmUserOperationType"/>
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

        public string UserName { get; set; }

        public string UserPhone { get; set; }
    }
}
