using ETMS.DataAccess.Core;
using ETMS.DataAccess.Repository;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class HolidaySettingDAL : SimpleRepository<EtHolidaySetting, HolidaySettingBucket<EtHolidaySetting>>, IHolidaySettingDAL
    {
        public HolidaySettingDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        public async Task<bool> AddHolidaySetting(EtHolidaySetting entity)
        {
            return await base.AddEntity(entity);
        }

        public async Task DelHolidaySetting(long id)
        {
            await base.DelEntity(id);
        }

        public async Task<List<EtHolidaySetting>> GetAllHolidaySetting()
        {
            return await base.GetAll();
        }
    }
}