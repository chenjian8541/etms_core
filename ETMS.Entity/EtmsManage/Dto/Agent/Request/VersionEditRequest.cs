using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Request
{
    public class VersionEditRequest: AgentRequestBase
    {
        public int CId { get; set; }

        public string Name { get; set; }

        public string DetailInfo { get; set; }

        public List<int> PageIds { get; set; }

        public List<int> PageRouteIds { get; set; }

        public List<int> ActionIds { get; set; }

        public string Remark { get; set; }

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
            if (PageIds == null || PageIds.Count == 0)
            {
                return "请选择权限";
            }
            return string.Empty;
        }
    }
}
