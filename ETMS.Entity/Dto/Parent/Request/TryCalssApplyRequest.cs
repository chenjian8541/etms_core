using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class TryCalssApplyRequest : IValidate
    {
        public int TenantId { get; set; }

        public long GrowthRecordDetailId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Remark { get; set; }

        public string Validate()
        {
            if (TenantId <= 0 || GrowthRecordDetailId <= 0)
            {
                return "请求数据不合法";
            }
            if (string.IsNullOrEmpty(Phone))
            {
                return "请输入手机号码";
            }
            return string.Empty;
        }
    }
}
