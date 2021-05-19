using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.User.Request
{
    public class UserAddRequest : AgentRequestBase
    {
        public int UserRoleId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public bool IsLock { get; set; }

        public string Remark { get; set; }

        public string Address { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "名称不能为空";
            }
            Phone = Phone.Trim();
            if (string.IsNullOrEmpty(Phone))
            {
                return "手机号码不能为空";
            }
            return string.Empty;
        }
    }
}
