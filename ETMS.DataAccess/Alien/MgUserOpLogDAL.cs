using ETMS.DataAccess.Alien.Core;
using ETMS.DataAccess.Core;
using ETMS.DataAccess.Core.Alien;
using ETMS.Entity.Alien.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Alien;
using ETMS.Entity.Enum;
using ETMS.Entity.View.Alien;
using ETMS.IDataAccess.Alien;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETMS.DataAccess.Alien
{
    public class MgUserOpLogDAL : DataAccessBaseAlien, IMgUserOpLogDAL
    {
        public MgUserOpLogDAL(IDbWrapperAlien dbWrapper) : base(dbWrapper)
        {
        }

        public async Task AddUserOpLog(MgUserOpLog entity)
        {
            await _dbWrapper.Insert(entity);
        }

        public async Task AddUserLog(AlienRequestBase request, string content, int opType)
        {
            await _dbWrapper.Insert(new MgUserOpLog()
            {
                ClientType = request.LoginClientType,
                HeadId = request.LoginHeadId,
                IpAddress = request.IpAddress,
                IsDeleted = EmIsDeleted.Normal,
                MgUserId = request.LoginUserId,
                OpContent = content,
                Ot = DateTime.Now,
                Remark = null,
                Type = opType
            });
        }

        public async Task<Tuple<IEnumerable<MgUserOpLog>, int>> GetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<MgUserOpLog>("MgUserOpLog", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<Tuple<IEnumerable<MgUserOpLogView>, int>> GetViewPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<MgUserOpLogView>("MgUserOpLogView", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
