using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.User.Output
{
    public class UserOperationLogGetPagingOutput
    {
        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.Alien.EmMgUserOperationType"/>
        /// </summary>
        public string TypeDesc { get; set; }

        /// <summary>
        /// 操作内容
        /// </summary>
        public string OpContent { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 客户端类型 <see cref="Enum.EmUserOperationLogClientType"/>
        /// </summary>
        public int ClientType { get; set; }

        public string UserDesc { get; set; }
    }
}
