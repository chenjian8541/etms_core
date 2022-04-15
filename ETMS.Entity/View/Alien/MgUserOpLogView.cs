using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.View.Alien
{
    public class MgUserOpLogView
    {
        public long Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public long MgUserId { get; set; }

        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.Alien.EmMgUserOperationType"/>
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public string IpAddress { get; set; }

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

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }
    }
}
