using ETMS.DataAccess.Core;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class IncomeLogDAL : DataAccessBase, IIncomeLogDAL
    {
        public IncomeLogDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        { }

        public async Task<bool> AddIncomeLog(EtIncomeLog etIncomeLog)
        {
            return await _dbWrapper.Insert(etIncomeLog);
        }

        public bool AddIncomeLog(List<EtIncomeLog> etIncomeLogs)
        {
            return _dbWrapper.InsertRange(etIncomeLogs);
        }

        public async Task<List<EtIncomeLog>> GetIncomeLogByOrderId(long orderId)
        {
            return await _dbWrapper.FindList<EtIncomeLog>(p => p.OrderId == orderId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<Tuple<IEnumerable<EtIncomeLog>, int>> GetIncomeLogPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtIncomeLog>("EtIncomeLog", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<EtIncomeLog> GetIncomeLog(long id)
        {
            return await _dbWrapper.Find<EtIncomeLog>(id);
        }

        public async Task<bool> EditIncomeLog(EtIncomeLog log)
        {
            await _dbWrapper.Update(log);
            return true;
        }

        public async Task<bool> DelIncomeLog(List<long> orderIds)
        {
            if (orderIds == null || orderIds.Count == 0)
            {
                return false;
            }
            if (orderIds.Count == 1)
            {
                await _dbWrapper.Execute($"UPDATE EtIncomeLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND OrderId = {orderIds[0]} ");
            }
            else
            {
                await _dbWrapper.Execute($"UPDATE EtIncomeLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND OrderId IN ({string.Join(',', orderIds)}) ");
            }
            return true;
        }

        public async Task RepealIncomeLog(long projectType, long relationId, long repealUserId, DateTime repealOt)
        {
            await _dbWrapper.Execute($"UPDATE EtIncomeLog SET [Status] = {EmIncomeLogStatus.Repeal},RepealUserId = {repealUserId},RepealOt = '{repealOt.EtmsToDateString()}' WHERE TenantId = {_tenantId} AND ProjectType = {projectType} AND RelationId = {relationId} ");
        }
    }
}
