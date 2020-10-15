using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysConnectionStringDAL : BaseCacheDAL<SysConnectionStringBucket>, ISysConnectionStringDAL, IEtmsManage
    {
        public SysConnectionStringDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysConnectionStringBucket> GetDb(params object[] keys)
        {
            var sysConnectionStrings = await this.FindList<SysConnectionString>(p => p.IsDeleted == EmIsDeleted.Normal);
            return new SysConnectionStringBucket()
            {
                SysConnectionStrings = sysConnectionStrings
            };
        }

        public async Task<List<SysConnectionString>> GetSysConnectionString()
        {
            var buckrt = await GetCache();
            return buckrt.SysConnectionStrings;
        }
    }
}
