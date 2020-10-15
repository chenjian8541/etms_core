using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysVersionDAL : BaseCacheDAL<SysVersionBucket>, ISysVersionDAL, IEtmsManage
    {
        public SysVersionDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysVersionBucket> GetDb(params object[] keys)
        {
            var versions = await this.FindList<SysVersion>(p => p.IsDeleted == EmIsDeleted.Normal);
            return new SysVersionBucket()
            {
                SysVersions = versions
            };
        }

        public async Task<SysVersion> GetVersion(int id)
        {
            var versions = await GetVersions();
            return versions.FirstOrDefault(p => p.Id == id);
        }

        public async Task<List<SysVersion>> GetVersions()
        {
            var bucket = await GetCache();
            if (bucket == null || bucket.SysVersions == null)
            {
                return new List<SysVersion>();
            }
            return bucket.SysVersions;
        }

        public async Task<bool> AddVersion(SysVersion entity)
        {
            await this.Insert(entity);
            await UpdateCache();
            return true;
        }

        public async Task<bool> EditVersion(SysVersion entity)
        {
            await this.Update(entity);
            await UpdateCache();
            return true;
        }

        public async Task<bool> IsCanNotDelete(int id)
        {
            var log = await this.Find<SysAgentEtmsAccount>(p => p.VersionId == id && p.IsDeleted == EmIsDeleted.Normal);
            return log != null;
        }

        public async Task<bool> DelVersion(int id)
        {
            var entity = await this.GetVersion(id);
            entity.IsDeleted = EmIsDeleted.Deleted;
            return await EditVersion(entity);
        }
    }
}
