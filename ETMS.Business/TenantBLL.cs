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

        private readonly IStudentSmsLogDAL _studentSmsLogDAL;

        public TenantBLL(ISysTenantDAL sysTenantDAL, IStudentSmsLogDAL studentSmsLogDAL)
        {
            this._sysTenantDAL = sysTenantDAL;
            this._studentSmsLogDAL = studentSmsLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentSmsLogDAL);
        }

        public async Task<ResponseBase> TenantGet(TenantGetRequest request)
        {
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            return ResponseBase.Success(myTenant);
        }

        public async Task TenantSmsDeductionEventConsume(TenantSmsDeductionEvent request)
        {
            var totalDeCount = request.SmsLogs.Sum(p => p.DeCount);
            await _sysTenantDAL.TenantSmsDeduction(request.TenantId, totalDeCount);
            await _studentSmsLogDAL.AddStudentSmsLog(request.SmsLogs);
        }
    }
}
