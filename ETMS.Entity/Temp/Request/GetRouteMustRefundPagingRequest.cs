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
    public class GetRouteMustRefundPagingRequest : IPagingRequest
    {
        /// <summary>
        /// 每页数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int PageCurrent { get; set; }

        public int TenantId { get; set; }

        public override string ToString()
        {
            var now = DateTime.Now;
            return $"TenantId = {TenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [PayStatus] = {EmActivityRoutePayStatus.Paid} AND RouteStatus = {EmActivityRouteStatus.Normal} AND [Status] = {EmSysActivityRouteItemStatus.Going} AND ActivityEndTime <= '{now.EtmsToString()}' AND CountLimit > CountFinish";
        }
    }
}
