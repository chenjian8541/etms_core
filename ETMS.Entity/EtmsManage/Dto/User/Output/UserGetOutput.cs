using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.User.Output
{
    public class UserGetOutput
    {
        public long Id { get; set; }

        public int AgentId { get; set; }

        public int UserRoleId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }
    
        /// <summary>
        /// 是否锁定 <see cref=" ETMS.Entity.Enum.EtmsManage.EmSysAgentIsLock"/>
        /// </summary>
        public bool IsLock { get; set; }

        public string Remark { get; set; }
    }
}
