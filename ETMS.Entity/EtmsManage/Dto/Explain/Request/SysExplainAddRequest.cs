using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Explain.Request
{
    public class SysExplainAddRequest : AgentRequestBase
    {
        public int Type { get; set; }

        public string Title { get; set; }

        public string RelationUrl { get; set; }

        public string Remark { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(Title))
            {
                return "内容不能为空";
            }
            if (string.IsNullOrEmpty(RelationUrl))
            {
                return "链接不能为空";
            }
            return string.Empty;
        }
    }
}
