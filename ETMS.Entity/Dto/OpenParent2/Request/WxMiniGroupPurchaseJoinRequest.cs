using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.OpenParent2.Request
{
    public class WxMiniGroupPurchaseJoinRequest: OpenParent2RequestBase
    {
        public int TenantId { get; set; }

        public long ActivityRouteId { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public string StudentFieldValue1 { get; set; }

        public string StudentFieldValue2 { get; set; }

        public override string Validate()
        {
            if (TenantId <= 0)
            {
                return "请求数据格式错误";
            }
            if (ActivityRouteId <= 0)
            {
                return "请求数据格式错误";
            }
            if (string.IsNullOrEmpty(StudentName))
            {
                return "请输入姓名";
            }
            if (string.IsNullOrEmpty(StudentPhone))
            {
                return "请输入手机号码";
            }
            return base.Validate();
        }
    }
}
