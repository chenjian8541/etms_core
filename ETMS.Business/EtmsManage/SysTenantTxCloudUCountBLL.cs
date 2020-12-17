using ETMS.Event.DataContract;
using ETMS.IBusiness.EtmsManage;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EtmsManage
{
    public class SysTenantTxCloudUCountBLL : ISysTenantTxCloudUCountBLL
    {
        private readonly ISysTenantTxCloudUCountDAL _sysTenantTxCloudUCountDAL;

        private readonly IStudentDAL _studentDAL;

        public SysTenantTxCloudUCountBLL(ISysTenantTxCloudUCountDAL sysTenantTxCloudUCountDAL, IStudentDAL studentDAL)
        {
            this._sysTenantTxCloudUCountDAL = sysTenantTxCloudUCountDAL;
            this._studentDAL = studentDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentDAL);
        }

        public async Task TenantTxCloudUCountConsumerEvent(TenantTxCloudUCountEvent request)
        {
            var now = DateTime.Now;
            await _sysTenantTxCloudUCountDAL.AddTenantTxCloudUCount(request.TenantId, now, request.Type, request.AddUseCount);
            if (request.StudentId > 0)
            {
                await _studentDAL.UpdateStudentFaceUseLastTime(request.StudentId, now);
            }
        }
    }
}
