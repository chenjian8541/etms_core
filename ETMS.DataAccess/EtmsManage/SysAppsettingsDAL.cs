using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket;
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
    public class SysAppsettingsDAL : BaseCacheDAL<SysAppsettingsBucket>, ISysAppsettingsDAL, IEtmsManage
    {
        public SysAppsettingsDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysAppsettingsBucket> GetDb(params object[] keys)
        {
            var type = keys[0].ToInt();
            var db = await this.Find<SysAppsettings>(p => p.Type == type && p.IsDeleted == EmIsDeleted.Normal);
            if (db == null)
            {
                return null;
            }
            return new SysAppsettingsBucket
            {
                SysAppsettings = db
            };
        }

        public async Task<SysAppsettings> GetAppsettings(int type)
        {
            var bucket = await GetCache(type);
            return bucket?.SysAppsettings;
        }
    }
}
