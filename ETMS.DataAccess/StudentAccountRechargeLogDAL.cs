using ETMS.DataAccess.Core;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class StudentAccountRechargeLogDAL : DataAccessBase, IStudentAccountRechargeLogDAL
    {
        public StudentAccountRechargeLogDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task AddStudentAccountRechargeLog(EtStudentAccountRechargeLog log)
        {
            await this._dbWrapper.Insert(log);
        }

        public async Task<Tuple<IEnumerable<EtStudentAccountRechargeLog>, int>> GetPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtStudentAccountRechargeLog>("EtStudentAccountRechargeLog", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task UpdateStudentAccountRechargeLogPhone(long studentAccountRechargeId, string phone)
        {
            await _dbWrapper.Execute($"UPDATE EtStudentAccountRechargeLog SET Phone = '{phone}' WHERE TenantId = {_tenantId} AND StudentAccountRechargeId = {studentAccountRechargeId} ");
        }

        public async Task<EtStudentAccountRechargeLog> GetAccountRechargeLogByOrderId(long orderId)
        {
            return await _dbWrapper.Find<EtStudentAccountRechargeLog>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.RelatedOrderId == orderId);
        }
    }
}
