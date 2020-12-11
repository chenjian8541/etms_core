using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.View
{
    public class UserOperationLogView
    {
        /// <summary>
        /// 操作用户
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 操作类型  <see cref="ETMS.Entity.Enum.EmUserOperationType"/>
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

        public string UserName { get; set; }

        public string UserPhone { get; set; }

        /// <summary>
        /// 客户端类型 <see cref="Enum.EmUserOperationLogClientType"/>
        /// </summary>
        public int ClientType { get; set; }
    }
}
