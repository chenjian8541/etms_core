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
    public class SysLcsBankDAL : BaseCacheDAL<SysLcsBankHeadBucket>, ISysLcsBankDAL, IEtmsManage
    {
        public SysLcsBankDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysLcsBankHeadBucket> GetDb(params object[] keys)
        {
            var sql = "SELECT HeadNumber as Code,HeadName as Name from [SysLcsBank] group by HeadNumber,HeadName order by HeadNumber";
            var db = await this.ExecuteObject<LcsRegionsView>(sql);
            return new SysLcsBankHeadBucket()
            {
                Datas = db
            };
        }

        public async Task<IEnumerable<LcsRegionsView>> GetAllBankHead()
        {
            var bucket = await GetCache();
            return bucket.Datas;
        }

        public async Task<IEnumerable<LcsRegionsView>> GetBank(string headNumber, string cityCode)
        {
            var sql = $"SELECT SubbranchNumber AS Code,SubbranchName AS Name from [SysLcsBank] WHERE HeadNumber = '{headNumber}' AND CityCode = '{cityCode}'";
            return await this.ExecuteObject<LcsRegionsView>(sql);
        }
    }
}
