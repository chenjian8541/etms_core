using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.OpenParent2.Request
{
    public class WxMiniHagglingLogGetPagingRequest : OpenParent2RequestPagingBase
    {
        public int TenantId { get; set; }

        public long ActivityMainId { get; set; }

        public long ActivityRouteId { get; set; }

        public override string Validate()
        {
            if (TenantId <= 0)
            {
                return "请求数据格式错误";
            }
            if (ActivityMainId <= 0)
            {
                return "请求数据格式错误";
            }
            if (ActivityRouteId <= 0)
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }

        public override string ToString()
        {
            return $"IsDeleted = {EmIsDeleted.Normal} AND TenantId = {TenantId} AND ActivityId = {ActivityMainId} AND ActivityRouteId = {ActivityRouteId}";
        }
    }
}
