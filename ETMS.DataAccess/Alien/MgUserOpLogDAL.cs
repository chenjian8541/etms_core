using ETMS.DataAccess.Core;
using ETMS.DataAccess.Core.Alien;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Alien;
using ETMS.IDataAccess.Alien;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETMS.DataAccess.Alien
{
    public class MgUserOpLogDAL : DataAccessBaseAlien, IMgUserOpLogDAL
    {
        public MgUserOpLogDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task AddMgUserOpLog(MgUserOpLog entity)
        {
            await _dbWrapper.Insert(entity);
        }

        public async Task<Tuple<IEnumerable<MgUserOpLog>, int>> GetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<MgUserOpLog>("MgUserOpLog", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
