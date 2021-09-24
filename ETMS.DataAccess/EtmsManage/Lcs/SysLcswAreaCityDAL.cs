using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage.Lcs;
using ETMS.Entity.Temp;
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
    public class SysLcswAreaCityDAL : BaseCacheDAL<SysLcswAreaCityBucket>, ISysLcswAreaCityDAL, IEtmsManage
    {
        public SysLcswAreaCityDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysLcswAreaCityBucket> GetDb(params object[] keys)
        {
            var provinceId = keys[0].ToString();
            var sql = $"SELECT CityId AS Code,CityName AS Name FROM [SysLcswArea] WHERE ProvinceId = {provinceId} GROUP BY CityId,CityName";
            var db = await this.ExecuteObject<LcsRegionsView>(sql);
            return new SysLcswAreaCityBucket()
            {
                Datas = db
            };
        }

        public async Task<IEnumerable<LcsRegionsView>> GetCity(string provinceId)
        {
            var bucket = await GetCache(provinceId);
            return bucket.Datas;
        }
    }
}
