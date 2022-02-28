using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysTenantCloudStorageDAL : BaseCacheDAL<SysTenantCloudStorageBucket>, ISysTenantCloudStorageDAL, IEtmsManage
    {
        public SysTenantCloudStorageDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysTenantCloudStorageBucket> GetDb(params object[] keys)
        {
            var tenantId = keys[0].ToInt();
            var storageLogs = await this.FindList<SysTenantCloudStorage>(p => p.TenantId == tenantId && p.IsDeleted == EmIsDeleted.Normal);
            return new SysTenantCloudStorageBucket()
            {
                StorageLogs = storageLogs
            };
        }

        public async Task SaveCloudStorage(int tenantId, List<SysTenantCloudStorage> entityList)
        {
            await this.Execute($"UPDATE SysTenantCloudStorage SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {tenantId}");
            foreach (var myItemEntity in entityList)
            {
                var oldEntity = await this.Find<SysTenantCloudStorage>(p => p.TenantId == tenantId && p.Type == myItemEntity.Type);
                if (oldEntity != null)
                {
                    oldEntity.IsDeleted = EmIsDeleted.Normal;
                    oldEntity.LastModified = myItemEntity.LastModified;
                    oldEntity.ValueMB = myItemEntity.ValueMB;
                    oldEntity.ValueGB = myItemEntity.ValueGB;
                    await this.Update(oldEntity);
                }
                else
                {
                    await this.Insert(myItemEntity);
                }
            }
            await UpdateCache(tenantId);
        }

        public async Task<List<SysTenantCloudStorage>> GetCloudStorage(int tenantId)
        {
            var bucket = await GetCache(tenantId);
            return bucket?.StorageLogs;
        }
    }
}
