using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum.Alien
{
    public struct EmMgUserOperationType
    {
        /// <summary>
        /// 登录
        /// </summary>
        public const int Login = 1;

        /// <summary>
        /// 修改密码
        /// </summary>
        public const int UserChangePwd = 2;

        /// <summary>
        /// 员工管理
        /// </summary>
        public const int UserMgr = 3;

        /// <summary>
        /// 角色管理
        /// </summary>
        public const int RoleMgr = 4;

        /// <summary>
        /// 组织管理
        /// </summary>
        public const int OrgMgr = 5;

        public static string GetMgUserOperationTypeDesc(int type)
        {
            switch (type)
            {
                case Login:
                    return "登录";
                case UserChangePwd:
                    return "修改密码";
                case UserMgr:
                    return "员工管理";
                case RoleMgr:
                    return "角色管理";
                case OrgMgr:
                    return "组织管理";
            }
            return string.Empty;
        }
    }
}
