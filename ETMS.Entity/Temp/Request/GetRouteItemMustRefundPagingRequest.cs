using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Temp.Request
{
    public class GetRouteItemMustRefundPagingRequest : IPagingRequest
    {
        public int PageSize { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int PageCurrent { get; set; }

        public int TenantId { get; set; }

        public long ActivityRouteId { get; set; }

        public override string ToString()
        {
            return $"TenantId = {TenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [PayStatus] = {EmActivityRoutePayStatus.Paid} AND RouteStatus = {EmActivityRouteStatus.Normal} AND ActivityRouteId = {ActivityRouteId}";
        }
    }
}
