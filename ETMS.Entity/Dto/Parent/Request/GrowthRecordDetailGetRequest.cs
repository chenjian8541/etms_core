using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class GrowthRecordDetailGetRequest : IValidate
    {
        public int TenantId { get; set; }

        public long GrowthRecordDetailId { get; set; }

        public bool IsLogin { get; set; }

        public string Validate()
        {
            if (TenantId <= 0 || GrowthRecordDetailId <= 0)
            {
                return "数据校验不合法";
            }
            return string.Empty;
        }
    }
}
