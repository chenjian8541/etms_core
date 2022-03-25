using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum.Alien
{
    public enum EmMgUserOperationType
    {

        [Description("登录")]
        Login = 1,

        [Description("修改密码")]
        UserChangePwd = 2,

        [Description("修改用户信息")]
        UserUpdateInfo = 3,
    }

}
