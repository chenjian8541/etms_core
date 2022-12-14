using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Request
{
    public class VersionAddRequest : AgentRequestBase
    {
        public string Name { get; set; }

        public string DetailInfo { get; set; }

        public List<int> PageIds { get; set; }

        public List<int> PageRouteIds { get; set; }

        public List<int> ActionIds { get; set; }

        public string Remark { get; set; }

        public bool IsLimitLoginPC { get; set; }

        public bool IsLimitLoginWxParent { get; set; }

        public bool IsLimitLoginWxTeacher { get; set; }

        public bool IsLimitLoginAppTeacher { get; set; }

        public override string Validate()
        {
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
