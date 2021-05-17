using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Enum;

namespace ETMS.Business.EventConsumer
{
    public class TenantDataCollect : ITenantDataCollect
    {
        private readonly ISysTenantOperationLogDAL _sysTenantOperationLogDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly ISysTenantStatisticsDAL _sysTenantStatisticsDAL;

        private readonly IJobAnalyze2DAL _jobAnalyze2DAL;

        public TenantDataCollect(ISysTenantOperationLogDAL sysTenantOperationLogDAL, ISysTenantDAL sysTenantDAL,
            ISysTenantStatisticsDAL sysTenantStatisticsDAL, IJobAnalyze2DAL jobAnalyze2DAL)
        {
            this._sysTenantOperationLogDAL = sysTenantOperationLogDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._sysTenantStatisticsDAL = sysTenantStatisticsDAL;
            this._jobAnalyze2DAL = jobAnalyze2DAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _jobAnalyze2DAL);
        }

        public async Task SysTenantOperationLogConsumerEvent(SysTenantOperationLogEvent request)
        {
            var myTenant = await _sysTenantDAL.GetTenant(request.TenantId);
            var p = request.UserOperationLog;
            var sysTenantOperationLog = new SysTenantOperationLog()
            {
                ClientType = p.ClientType,
                TenantId = p.TenantId,
                AgentId = myTenant.AgentId,
                IpAddress = p.IpAddress,
                IsDeleted = p.IsDeleted,
                OpContent = p.OpContent,
                Ot = p.Ot,
                Remark = p.Remark,
                Type = p.Type,
                UserId = p.UserId
            };
            await _sysTenantOperationLogDAL.AddSysTenantOperationLog(sysTenantOperationLog);
        }

        public async Task TenantAgentStatisticsConsumerEvent(TenantAgentStatisticsEvent request)
        {
            var myTenant = await _sysTenantDAL.GetTenant(request.TenantId);
            var studentTypeCountStatistics = await _jobAnalyze2DAL.GetStudentTypeCount();
            var userCountStatistics = await _jobAnalyze2DAL.GetUserCount();
            var orderCountStatistics = await _jobAnalyze2DAL.GetOrderCount();
            var classRecordCountStatistics = await _jobAnalyze2DAL.GetClassRecordCount();
            var classCountStatistics = await _jobAnalyze2DAL.GetClassCount();
            var studentCount1 = 0;
            var studentCount2 = 0;
            var studentCount3 = 0;
            var classCount1 = 0;
            var classCount2 = 0;
            if (studentTypeCountStatistics != null && studentTypeCountStatistics.Any())
            {
                var studentCount1Log = studentTypeCountStatistics.FirstOrDefault(p => p.StudentType == EmStudentType.HiddenStudent); //潜在学员
                if (studentCount1Log != null)
                {
                    studentCount1 = studentCount1Log.MyCount;
                }
                var studentCount2Log = studentTypeCountStatistics.FirstOrDefault(p => p.StudentType == EmStudentType.ReadingStudent); //潜在学员
                if (studentCount2Log != null)
                {
                    studentCount2 = studentCount2Log.MyCount;
                }
                var studentCount3Log = studentTypeCountStatistics.FirstOrDefault(p => p.StudentType == EmStudentType.HistoryStudent); //潜在学员
                if (studentCount3Log != null)
                {
                    studentCount3 = studentCount3Log.MyCount;
                }
            }
            if (classCountStatistics != null && classCountStatistics.Any())
            {
                var classCount1Log = classCountStatistics.FirstOrDefault(p => p.Type == EmClassType.OneToMany); //一对多
                if (classCount1Log != null)
                {
                    classCount1 = classCount1Log.MyCount;
                }
                var classCount2Log = classCountStatistics.FirstOrDefault(p => p.Type == EmClassType.OneToOne); //一对多
                if (classCount2Log != null)
                {
                    classCount2 = classCount2Log.MyCount;
                }
            }

            var sysTenantStatistics = await _sysTenantStatisticsDAL.GetSysTenantStatistics(request.TenantId);
            if (sysTenantStatistics != null)
            {
                sysTenantStatistics.ClassCount1 = classCount1;
                sysTenantStatistics.ClassCount2 = classCount2;
                sysTenantStatistics.ClassRecordCount = classRecordCountStatistics;
                sysTenantStatistics.StudentCount1 = studentCount1;
                sysTenantStatistics.StudentCount2 = studentCount2;
                sysTenantStatistics.StudentCount3 = studentCount3;
                sysTenantStatistics.UserCount = userCountStatistics;
                sysTenantStatistics.OrderCount = orderCountStatistics;
            }
            else
            {
                sysTenantStatistics = new SysTenantStatistics()
                {
                    AgentId = myTenant.AgentId,
                    ClassCount1 = classCount1,
                    ClassCount2 = classCount2,
                    ClassRecordCount = classRecordCountStatistics,
                    StudentCount1 = studentCount1,
                    StudentCount2 = studentCount2,
                    StudentCount3 = studentCount3,
                    UserCount = userCountStatistics,
                    OrderCount = orderCountStatistics,
                    IsDeleted = EmIsDeleted.Normal,
                    Remark = string.Empty,
                    TenantId = myTenant.Id
                };
            }
            await _sysTenantStatisticsDAL.SaveSysTenantStatistics(sysTenantStatistics);
        }
    }
}
