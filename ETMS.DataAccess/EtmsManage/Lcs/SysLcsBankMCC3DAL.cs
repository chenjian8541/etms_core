using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage.Lcs;
using ETMS.Entity.View;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage.Lcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage.Lcs
{
    public class SysLcsBankMCC3DAL : BaseCacheDAL<SysLcsBankMCC3Bucket>, ISysLcsBankMCC3DAL, IEtmsManage
    {
        public SysLcsBankMCC3DAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysLcsBankMCC3Bucket> GetDb(params object[] keys)
        {
            var sql = $"SELECT Code,Thirdname AS Name FROM [SysLcsBankMCC3] WHERE Uni2Id = {keys[0]}";
            var db = await this.ExecuteObject<LcsRegionsView>(sql);
            return new SysLcsBankMCC3Bucket()
            {
                Datas = db
            };
        }

        public async Task<IEnumerable<LcsRegionsView>> GetLcsBankMCC3(string uni2Id)
        {
            var bucket = await GetCache(uni2Id);
            return bucket?.Datas;
        }
    }
}
