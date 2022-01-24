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

        [Description("主系统升级管理")]
        public const int VersionUpgrade = 6;

        [Description("系统公告")]
        public const int SysExplainMgr = 7;

        [Description("主系统配置")]
        public const int EtmsSysSetting = 8;

        [Description("客户端升级管理")]
        public const int SysClientUpgradeMgr = 9;

        [Description("云账户管理")]
        public const int AICloudMgr = 10;

        [Description("员工管理")]
        public const int UserMgr = 11;

        [Description("试听管理")]
        public const int TryMgr = 12;

        [Description("外部配置管理")]
        public const int ExternalConfigMgr = 13;

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
                case VersionUpgrade:
                    return "主系统升级管理";
                case SysExplainMgr:
                    return "系统公告";
                case EtmsSysSetting:
                    return "主系统配置";
                case SysClientUpgradeMgr:
                    return "客户端升级管理";
                case AICloudMgr:
                    return "云账户管理";
                case UserMgr:
                    return "员工管理";
                case TryMgr:
                    return "试听管理";
                case ExternalConfigMgr:
                    return "外部配置管理";
            }
            return string.Empty;
        }
    }
}
