using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket.MicroWeb;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.IDataAccess.MicroWeb;
using ETMS.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ETMS.DataAccess.MicroWeb
{
    public class MicroWebColumnDAL : DataAccessBase<MicroWebColumnBucket>, IMicroWebColumnDAL
    {
        public MicroWebColumnDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }
        protected override async Task<MicroWebColumnBucket> GetDb(params object[] keys)
        {
            var logs = await _dbWrapper.FindList<EtMicroWebColumn>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
            if (logs == null || logs.Count == 0)
            {
                RemoveCache(_tenantId);
                return null;
            }
            return new MicroWebColumnBucket()
            {
                MicroWebColumns = logs
            };
        }

        public async Task<bool> AddMicroWebColumn(EtMicroWebColumn entity)
        {
            await this._dbWrapper.Insert(entity);
            await UpdateCache(_tenantId);
            return true;
        }

        public async Task<bool> EditMicroWebColumn(EtMicroWebColumn entity)
        {
            await this._dbWrapper.Update(entity);
            await UpdateCache(_tenantId);
            return true;
        }

        public async Task<bool> ChangeStatus(long id, byte newStatus)
        {
            await _dbWrapper.Execute($"UPDATE EtMicroWebColumn SET [Status] = {newStatus} WHERE Id = {id} AND TenantId = {_tenantId}");
            await UpdateCache(_tenantId);
            return true;
        }

        public async Task<bool> DelMicroWebColumn(long id)
        {
            await _dbWrapper.Execute($"UPDATE EtMicroWebColumn SET IsDeleted = {EmIsDeleted.Deleted} WHERE Id = {id} AND TenantId = {_tenantId}");
            await UpdateCache(_tenantId);
            return true;
        }

        public async Task<List<EtMicroWebColumn>> GetMicroWebColumn()
        {
            var bucket = await GetCache(_tenantId);
            return bucket?.MicroWebColumns;
        }

        public async Task<EtMicroWebColumn> GetMicroWebColumn(long id)
        {
            var allMicroWebColumn = await GetMicroWebColumn();
            if (allMicroWebColumn == null)
            {
                return null;
            }
            return allMicroWebColumn.FirstOrDefault(p => p.Id == id);
        }
    }
}
