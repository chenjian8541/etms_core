using ETMS.DataAccess.Core;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
using ETMS.Event.DataContract;
using ETMS.IDataAccess;
using ETMS.IEventProvider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class UserOperationLogDAL : DataAccessBase, IUserOperationLogDAL
    {
        private readonly IEventPublisher _eventPublisher;

        public UserOperationLogDAL(IDbWrapper dbWrapper, IEventPublisher eventPublisher) : base(dbWrapper)
        {
            this._eventPublisher = eventPublisher;
        }

        public async Task AddUserLog(EtUserOperationLog userLog)
        {
            await _dbWrapper.Insert(userLog);
            _eventPublisher.Publish(new SysTenantOperationLogEvent(_tenantId)
            {
                UserOperationLog = userLog
            });
        }
        public async Task AddUserLog(RequestBase request, string content, EmUserOperationType type, DateTime? time = null)
        {
            var userLog = new EtUserOperationLog()
            {
                IpAddress = request.IpAddress,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = content,
                Ot = time ?? DateTime.Now,
                TenantId = request.LoginTenantId,
                UserId = request.LoginUserId,
                Type = (int)type,
                ClientType = request.LoginClientType
            };
            await _dbWrapper.Insert(userLog);
            _eventPublisher.Publish(new SysTenantOperationLogEvent(_tenantId)
            {
                UserOperationLog = userLog
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
