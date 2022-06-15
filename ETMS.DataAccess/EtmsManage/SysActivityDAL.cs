using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Common;
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
    public class SysActivityDAL : BaseCacheDAL<SysActivityBucket>, ISysActivityDAL, IEtmsManage
    {
        public SysActivityDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysActivityBucket> GetDb(params object[] keys)
        {
            var id = keys.ToLong();
            var log = await this.Find<SysActivity>(p => p.Id == id && p.IsDeleted == EmIsDeleted.Normal);
            if (log == null)
            {
                return null;
            }
            return new SysActivityBucket()
            {
                SysActivity = log
            };
        }

        public async Task<SysActivity> GetSysActivity(long id)
        {
            var bucket = await GetCache(id);
            return bucket?.SysActivity;
        }

        public async Task<Tuple<IEnumerable<SysActivity>, int>> GetPaging(IPagingRequest request)
        {
            return await this.ExecutePage<SysActivity>("SysActivity", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
