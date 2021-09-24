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
    public class SysLcswAreaProvinceDAL : BaseCacheDAL<SysLcswAreaProvinceBucket>, ISysLcswAreaProvinceDAL, IEtmsManage
    {
        public SysLcswAreaProvinceDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysLcswAreaProvinceBucket> GetDb(params object[] keys)
        {
            var sql = "SELECT ProvinceId AS Code,ProvinceName AS Name FROM [SysLcswArea] group by ProvinceId ,ProvinceName";
            var db = await this.ExecuteObject<LcsRegionsView>(sql);
            return new SysLcswAreaProvinceBucket()
            {
                Datas = db
            };
        }

        public async Task<IEnumerable<LcsRegionsView>> GetAllProvince()
        {
            var bucket = await GetCache();
            return bucket.Datas;
        }
    }
}
