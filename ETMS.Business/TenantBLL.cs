using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ETMS.Business
{
    public class TenantBLL : ITenantBLL
    {
        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly ISmsLogDAL _smsLogDAL;

        public TenantBLL(ISysTenantDAL sysTenantDAL, ISmsLogDAL studentSmsLogDAL)
        {
            this._sysTenantDAL = sysTenantDAL;
            this._smsLogDAL = studentSmsLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _smsLogDAL);
        }

        public async Task<ResponseBase> TenantGet(TenantGetRequest request)
        {
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            return ResponseBase.Success(myTenant);
        }

        public async Task TenantSmsDeductionEventConsume(TenantSmsDeductionEvent request)
        {
            var totalDeCount = 0;
            if (request.StudentSmsLogs != null && request.StudentSmsLogs.Count > 0)
            {
                totalDeCount += request.StudentSmsLogs.Sum(p => p.DeCount);
                await _smsLogDAL.AddStudentSmsLog(request.StudentSmsLogs);
            }
            if (request.UserSmsLogs != null && request.UserSmsLogs.Count > 0)
            {
                totalDeCount += request.UserSmsLogs.Sum(p => p.DeCount);
                await _smsLogDAL.AddUserSmsLog(request.UserSmsLogs);
            }
            await _sysTenantDAL.TenantSmsDeduction(request.TenantId, totalDeCount);
        }
    }
}
