using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.User.Request
{
    public class UserRoleEditRequest : AgentRequestBase
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<int> PageIds { get; set; }

        public List<int> PageRouteIds { get; set; }

        public List<int> ActionIds { get; set; }

        public string Remark { get; set; }

        public bool IsMyDataLimit { get; set; }

        public override string Validate()
        {
            if (Id <= 0)
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
