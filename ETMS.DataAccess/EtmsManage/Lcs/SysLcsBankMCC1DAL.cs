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
    public class SysLcsBankMCC1DAL : BaseCacheDAL<SysLcsBankMCC1Bucket>, ISysLcsBankMCC1DAL, IEtmsManage
    {
        public SysLcsBankMCC1DAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }


        protected override async Task<SysLcsBankMCC1Bucket> GetDb(params object[] keys)
        {
            var sql = "SELECT Id AS Code,FirstName as Name from [SysLcsBankMCC1]";
            var db = await this.ExecuteObject<LcsRegionsView>(sql);
            return new SysLcsBankMCC1Bucket()
            {
                Datas = db
            };
        }

        public async Task<IEnumerable<LcsRegionsView>> GetAllLcsBankMCC1()
        {
            var bucket = await GetCache();
            return bucket.Datas;
        }
    }
}
