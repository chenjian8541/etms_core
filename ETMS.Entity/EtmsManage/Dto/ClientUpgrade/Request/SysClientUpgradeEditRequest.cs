using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;
using ETMS.Entity.Enum.EtmsManage;

namespace ETMS.Entity.EtmsManage.Dto.ClientUpgrade.Request
{
    public class SysClientUpgradeEditRequest : AgentRequestBase
    {
        public int Id { get; set; }

        /// <summary>
        ///  <see cref="EmSysClientUpgradeUpgradeType"/>
        /// </summary>
        public byte UpgradeType { get; set; }

        public string VersionNo { get; set; }

        public string UpgradeContent { get; set; }

        public string FileUrl { get; set; }

        public override string Validate()
        {
            if (Id <= 0)
            {
                return "数据格式错误";
            }
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
