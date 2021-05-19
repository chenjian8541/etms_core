using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.User.Output
{
    public class UserRoleGetPagingOutput
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string AgentName { get; set; }

        public string DataLimitDesc { get; set; }

        public string Remark { get; set; }
    }
}
