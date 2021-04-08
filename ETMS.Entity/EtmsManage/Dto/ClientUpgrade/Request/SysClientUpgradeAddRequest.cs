using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;
using ETMS.Entity.Enum.EtmsManage;

namespace ETMS.Entity.EtmsManage.Dto.ClientUpgrade.Request
{
    public class SysClientUpgradeAddRequest : AgentRequestBase
    {
        /// <summary>
        ///  <see cref="EmSysClientUpgradeClientType"/>
        /// </summary>
        public int ClientType { get; set; }

        /// <summary>
        ///  <see cref="EmSysClientUpgradeUpgradeType"/>
        /// </summary>
        public byte UpgradeType { get; set; }

        public string VersionNo { get; set; }

        public string UpgradeContent { get; set; }

        public string FileUrl { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(VersionNo))
            {
                return "请输入版本号";
            }
            if (string.IsNullOrEmpty(UpgradeContent))
            {
                return "请输入升级内容";
            }
            if (string.IsNullOrEmpty(FileUrl))
            {
                return "请输入升级包地址";
            }
            return string.Empty;
        }
    }
}
