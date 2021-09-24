using ETMS.DataAccess.Core;
using ETMS.Entity.Temp;
using ETMS.Entity.View;
using ETMS.IDataAccess.EtmsManage.Lcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage.Lcs
{
    public class SysLcswAreaRegionDAL : ISysLcswAreaRegionDAL, IEtmsManage
    {
        public async Task<IEnumerable<LcsRegionsView>> GetRegion(string cityId)
        {
            var sql = $"SELECT AreaId AS Code,AreaName AS Name FROM [SysLcswArea] WHERE CityId = {cityId} GROUP BY AreaId,AreaName";
            var db = await this.ExecuteObject<LcsRegionsView>(sql);
            return db;
        }
    }
}
