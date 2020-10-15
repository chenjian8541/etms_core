using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmSysAgentOpLogType
    {
        [Description("登录")]
        public const int Login = 1;

        [Description("代理商管理")]
        public const int AgentMange = 2;

        [Description("系统版本管理")]
        public const int VersionMange = 3;

        [Description("角色管理")]
        public const int RoleMange = 4;

        [Description("机构管理")]
        public const int TenantMange = 5;

        public static string GetSysAgentOpLogTypeDesc(int type)
        {
            switch (type)
            {
                case Login:
                    return "登录";
                case AgentMange:
                    return "代理商管理";
                case VersionMange:
                    return "系统版本管理";
                case RoleMange:
                    return "角色管理";
                case TenantMange:
                    return "机构管理";
            }
            return string.Empty;
        }
    }
}
