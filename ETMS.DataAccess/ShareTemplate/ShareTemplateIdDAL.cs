using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket.ShareTemplate;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.ICache;
using ETMS.IDataAccess.ShareTemplate;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.ShareTemplate
{
    public class ShareTemplateIdDAL : DataAccessBase<ShareTemplateIdBucket>, IShareTemplateIdDAL
    {
        private readonly IShareTemplateUseTypeDAL _shareTemplateUseTypeDAL;

        public ShareTemplateIdDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider, IShareTemplateUseTypeDAL shareTemplateUseTypeDAL) : base(dbWrapper, cacheProvider)
        {
            this._shareTemplateUseTypeDAL = shareTemplateUseTypeDAL;
        }

        public override void InitTenantId(int tenantId)
        {
            base.InitTenantId(tenantId);
            this._shareTemplateUseTypeDAL.InitTenantId(tenantId);
        }
        public override void ResetTenantId(int tenantId)
        {
            base.ResetTenantId(tenantId);
            this._shareTemplateUseTypeDAL.ResetTenantId(tenantId);
        }

        protected override async Task<ShareTemplateIdBucket> GetDb(params object[] keys)
        {
            var id = keys[1].ToLong();
            var myLog = await this._dbWrapper.Find<EtShareTemplate>(p => p.Id == id && p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
            if (myLog == null)
            {
                return null;
            }
            return new ShareTemplateIdBucket()
            {
                MyShareTemplate = myLog
            };
        }

        public async Task<EtShareTemplate> GetShareTemplate(long id)
        {
            var bucket = await GetCache(_tenantId, id);
            return bucket?.MyShareTemplate;
        }

        public async Task AddShareTemplate(EtShareTemplate entity)
        {
            await this._dbWrapper.Insert(entity);
            await UpdateCache(_tenantId, entity.Id);
            await this._shareTemplateUseTypeDAL.UpdateShareTemplate(entity.UseType);
        }

        public async Task EditShareTemplate(EtShareTemplate entity)
        {
            await this._dbWrapper.Update(entity);
            await UpdateCache(_tenantId, entity.Id);
            await this._shareTemplateUseTypeDAL.UpdateShareTemplate(entity.UseType);
        }

        public async Task DelShareTemplate(long id, int useType)
        {
            await _dbWrapper.Execute($"UPDATE EtShareTemplate SET IsDeleted = {EmIsDeleted.Deleted} WHERE id = {id}");
            RemoveCache(_tenantId, id);
            await this._shareTemplateUseTypeDAL.UpdateShareTemplate(useType);
        }

        public async Task ChangeShareTemplateStatus(EtShareTemplate entity, byte newStatus)
        {
            if (newStatus == EmShareTemplateStatus.Enabled)//启用
            {
                var enableIds = await _dbWrapper.ExecuteObject<OnlyId>($"SELECT Id FROM EtShareTemplate WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [Type] = {entity.Type} AND UseType = {entity.UseType} AND [Status] = {EmShareTemplateStatus.Enabled}");
                if (enableIds.Any())
                {
                    await _dbWrapper.Execute($"UPDATE EtShareTemplate SET [Status] = {EmShareTemplateStatus.Disabled} WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [Type] = {entity.Type} AND UseType = {entity.UseType} AND [Status] = {EmShareTemplateStatus.Enabled}");
                    foreach (var p in enableIds)
                    {
                        RemoveCache(_tenantId, p.Id);
                    }
                }
                await _dbWrapper.Update($"UPDATE EtShareTemplate SET [Status] = {EmShareTemplateStatus.Enabled} WHERE Id = {entity.Id} AND TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal}");
                await UpdateCache(_tenantId, entity.Id);
            }
            else
            {
                await _dbWrapper.Update($"UPDATE EtShareTemplate SET [Status] = {EmShareTemplateStatus.Disabled} WHERE Id = {entity.Id} AND TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal}");
                await UpdateCache(_tenantId, entity.Id);
                var enableIds = await _dbWrapper.ExecuteObject<OnlyId>($"SELECT Id FROM EtShareTemplate WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [Type] = {entity.Type} AND UseType = {entity.UseType} AND [Status] = {EmShareTemplateStatus.Enabled}");
                if (!enableIds.Any())
                {
                    var firstSystemLog = await _dbWrapper.Find<EtShareTemplate>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Type == entity.Type && p.UseType == entity.UseType);
                    if (firstSystemLog != null)
                    {
                        await _dbWrapper.Update($"UPDATE EtShareTemplate SET [Status] = {EmShareTemplateStatus.Enabled} WHERE Id = {firstSystemLog.Id} AND TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal}");
                        await UpdateCache(_tenantId, firstSystemLog.Id);
                    }
                }
            }
            await this._shareTemplateUseTypeDAL.UpdateShareTemplate(entity.UseType);
        }

        public async Task<Tuple<IEnumerable<EtShareTemplate>, int>> GetPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtShareTemplate>("EtShareTemplate", "*", request.PageSize, request.PageCurrent, "Status DESC,Id DESC", request.ToString());
        }

        public async Task<ShareTemplateUseTypeBucket> GetShareTemplateUseTypeBucket(int useType)
        {
            return await this._shareTemplateUseTypeDAL.GetShareTemplate(useType);
        }
    }
}
