using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract.Statistics;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EventConsumer
{
    public class EvStudentCheckOnBLL : IEvStudentCheckOnBLL
    {
        private readonly IStudentCheckOnLogDAL _studentCheckOnLogDAL;

        public EvStudentCheckOnBLL(IStudentCheckOnLogDAL studentCheckOnLogDAL)
        {
            this._studentCheckOnLogDAL = studentCheckOnLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentCheckOnLogDAL);
        }

        public async Task StatisticsStudentCheckConsumerEvent(StatisticsStudentCheckEvent request)
        {
            var myCheckInCount = await _studentCheckOnLogDAL.GetStatisticsCheckInCount(request.CheckOt);
            var myCheckOutCount = await _studentCheckOnLogDAL.GetStatisticsCheckOutCount(request.CheckOt);
            var myCheckAttendClassCount = await _studentCheckOnLogDAL.GetStatisticsCheckAttendClassCount(request.CheckOt);
            var entity = new EtStudentCheckOnStatistics()
            {
                CheckAttendClassCount = myCheckAttendClassCount,
                CheckInCount = myCheckInCount,
                CheckOutCount = myCheckOutCount,
                IsDeleted = EmIsDeleted.Normal,
                Ot = request.CheckOt,
                TenantId = request.TenantId
            };
            await _studentCheckOnLogDAL.SaveStudentCheckOnStatistics(entity);
        }
    }
}
