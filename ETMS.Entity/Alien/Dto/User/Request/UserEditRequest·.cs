using ETMS.Entity.Alien.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.User.Request
{
    public class UserEditRequest: AlienRequestBase
    {
        public long CId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string Remark { get; set; }

        public int? RoleId { get; set; }

        public long? OrgId { get; set; }

        public byte? Gender { get; set; }

        public List<long> JobAtTenants { get; set; }

        public byte IsLock { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据格式错误";
            }
            if (string.IsNullOrEmpty(Name))
            {
                return "请输入名称";
            }
            if (string.IsNullOrEmpty(Phone))
            {
                return "请输入手机号码";
            }
            return string.Empty;
        }
    }
}
