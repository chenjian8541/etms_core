using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Request
{
    public class SysUpgradeMsgAddRequest : AgentRequestBase
    {
        public string VersionNo { get; set; }

        public string Title { get; set; }

        public List<string> Ot { get; set; }

        public string UpContent { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(VersionNo))
            {
                return "请输入版本号";
            }
            if (string.IsNullOrEmpty(Title))
            {
                return "请输入标题描述";
            }
            if (Ot == null || Ot.Count != 2)
            {
                return "升级时间格式错误";
            }
            if (Convert.ToDateTime(Ot[0]) <= DateTime.Now)
            {
                return "升级时间必须大于当前时间";
            }
            if (string.IsNullOrEmpty(UpContent))
            {
                return "请输入升级内容";
            }

            return string.Empty;
        }
    }
}
