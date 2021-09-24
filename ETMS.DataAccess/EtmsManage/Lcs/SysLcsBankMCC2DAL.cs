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
    public class SysLcsBankMCC2DAL : BaseCacheDAL<SysLcsBankMCC2Bucket>, ISysLcsBankMCC2DAL, IEtmsManage
    {
        public SysLcsBankMCC2DAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysLcsBankMCC2Bucket> GetDb(params object[] keys)
        {
            var sql = $"SELECT Id as Code,Secondname as Name FROM [SysLcsBankMCC2] WHERE Uni1Id = {keys[0]}";
            var db = await this.ExecuteObject<LcsRegionsView>(sql);
            return new SysLcsBankMCC2Bucket()
            {
                Datas = db
            };
        }

        public async Task<IEnumerable<LcsRegionsView>> GetLcsBankMCC2(string uni1Id)
        {
            var bucket = await GetCache(uni1Id);
            return bucket?.Datas;
        }
    }
}
