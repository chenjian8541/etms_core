using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Enum;
using ETMS.IEventProvider;
using ETMS.Utility;
using ETMS.IDataAccess.Statistics;
using ETMS.Event.DataContract.Statistics;
using ETMS.Entity.Temp.View;
using Newtonsoft.Json;
using ETMS.Entity.Database.Source;
using ETMS.Business.Common;
using ETMS.Entity.Temp;
using ETMS.Entity.Dto.Educational.Request;

namespace ETMS.Business.EventConsumer
{
    public class EvStudentAccountRechargeBLL : IEvStudentAccountRechargeBLL
    {
        private readonly IStudentAccountRechargeDAL _studentAccountRechargeDAL;

        public EvStudentAccountRechargeBLL(IStudentAccountRechargeDAL studentAccountRechargeDAL)
        {
            this._studentAccountRechargeDAL = studentAccountRechargeDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentAccountRechargeDAL);
        }

        public async Task SyncStudentAccountRechargeRelationStudentIdsConsumerEvent(SyncStudentAccountRechargeRelationStudentIdsEvent request)
        {
            var myStudentAccountRecharge = await _studentAccountRechargeDAL.GetStudentAccountRechargeStudentIds(request.StudentAccountRechargeId);
            var strIds = string.Empty;
            if (myStudentAccountRecharge.Any())
            {
                var ids = myStudentAccountRecharge.Select(p => p.StudentId);
                strIds = EtmsHelper.GetMuIds(ids);
            }
            await _studentAccountRechargeDAL.UpdatetStudentAccountRechargeStudentIds(request.StudentAccountRechargeId, strIds);
        }
    }
}
