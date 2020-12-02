using ETMS.DataAccess.Core;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class UserOperationLogDAL : DataAccessBase, IUserOperationLogDAL
    {
        public UserOperationLogDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        { }

        public async Task AddUserLog(EtUserOperationLog userLog)
        {
            await _dbWrapper.Insert(userLog);
        }
        public async Task AddUserLog(RequestBase request, string content, EmUserOperationType type, DateTime? time = null)
        {
            await _dbWrapper.Insert(new EtUserOperationLog()
            {
                IpAddress = request.IpAddress,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = content,
                Ot = time ?? DateTime.Now,
                TenantId = request.LoginTenantId,
                UserId = request.LoginUserId,
                Type = (int)type,
                ClientType = request.LoginClientType
            });
        }

        public async Task<bool> IsUserCanNotBeDelete(long userId)
        {
            var useLog = await _dbWrapper.Find<EtUserOperationLog>(p => p.UserId == userId && p.Type != (int)EmUserOperationType.Login && p.IsDeleted == EmIsDeleted.Normal);
            return useLog != null;
        }

        public async Task<bool> IsUserCanNotBeDelete2(long userId)
        {
            var sql = $"SELECT TOP 1 0 FROM EtClassTimes WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Teachers LIKE '%,{userId},%';";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            return obj != null;
        }

        public async Task<Tuple<IEnumerable<UserOperationLogView>, int>> GetPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<UserOperationLogView>("UserOperationLogView", "*", request.PageSize, request.PageCurrent, "Ot DESC", request.ToString());
        }
    }
}
