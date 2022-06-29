using ETMS.DataAccess.Core;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysActivityRouteItemDAL : ISysActivityRouteItemDAL, IEtmsManage
    {
        public async Task<SysActivityRouteItem> GetSysActivityRouteItem(long tenantId, long routeItemId)
        {
            return await this.Find<SysActivityRouteItem>(p => p.TenantId == tenantId && p.EtActivityRouteItemId == routeItemId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task AddSysActivityRouteItem(SysActivityRouteItem entity)
        {
            await this.Insert(entity);
        }

        public async Task EdiSysActivityRouteItem(SysActivityRouteItem entity)
        {
            await this.Update(entity);
        }

        public async Task SyncActivityBascInfo(EtActivityMain bascInfo)
        {
            var sql = $"UPDATE SysActivityRouteItem SET ActivityTenantName='{bascInfo.TenantName}',ActivityName = '{bascInfo.Name}',ActivityCoverImage = '{bascInfo.CoverImage}',ActivityTitle = '{bascInfo.Title}' WHERE TenantId = {bascInfo.TenantId} AND ActivityId = {bascInfo.Id} AND IsDeleted = {EmIsDeleted.Normal}";
            await this.Execute(sql);
        }

        public async Task DelSysActivityRouteItemByTenantId(long tenantId)
        {
            var sql = $"UPDATE SysActivityRouteItem SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {tenantId}";
            await this.Execute(sql);
        }

        public async Task DelSysActivityRouteItemByActivityId(long tenantId, long activityId)
        {
            var sql = $"UPDATE SysActivityRouteItem SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {tenantId} AND ActivityId = {activityId}";
            await this.Execute(sql);
        }

        public async Task DelSysActivityRouteItemByRouteItemId(long tenantId, long routeItemId)
        {
            var sql = $"UPDATE SysActivityRouteItem SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {tenantId} AND EtActivityRouteItemId = {routeItemId}";
            await this.Execute(sql);
        }

        public async Task<Tuple<IEnumerable<SysActivityRouteItem>, int>> GetPagingRouteItem(IPagingRequest request)
        {
            return await this.ExecutePage<SysActivityRouteItem>("SysActivityRouteItem", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task UpdateActivityRouteItemStatus(long tenantId, long activityId, long activityRouteId, int newStatus)
        {
            var sql = $"UPDATE SysActivityRouteItem SET [Status] = {newStatus} WHERE TenantId = {tenantId} AND ActivityId = {activityId} AND ActivityRouteId = {activityRouteId}";
            await this.Execute(sql);
        }
    }
}
