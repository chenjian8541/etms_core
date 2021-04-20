using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Request
{
    public class RoleAddRequest : RequestBase
    {
        public string Name { get; set; }

        public List<int> PageIds { get; set; }

        public List<int> PageRouteIds { get; set; }

        public List<int> ActionIds { get; set; }

        public string Remark { get; set; }

        public bool IsMyDataLimit { get; set; }

        public RoleNoticeSettingRequest RoleNoticeSetting { get; set; }

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
            if (RoleNoticeSetting == null)
            {
                return "请填写配置信息";
            }
            return string.Empty;
        }
    }
}
