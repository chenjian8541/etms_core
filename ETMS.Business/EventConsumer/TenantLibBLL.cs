using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IDataAccess.EtmsManage.Statistics;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EventConsumer
{
    public class TenantLibBLL : ITenantLibBLL
    {
        private readonly IStudentDAL _studentDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IClassDAL _classDAL;

        private readonly INoticeConfigDAL _noticeConfigDAL;

        private readonly IComDAL _comDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly ISysTenantCloudStorageDAL _sysTenantCloudStorageDAL;

        private readonly IJobAnalyze2DAL _jobAnalyze2DAL;

        private readonly ISysTenantStatistics2DAL _sysTenantStatistics2DAL;

        private readonly ISysTenantStatisticsWeekDAL _sysTenantStatisticsWeekDAL;

        private readonly ISysTenantStatisticsMonthDAL _sysTenantStatisticsMonthDAL;
        public TenantLibBLL(IStudentDAL studentDAL, ICourseDAL courseDAL, IClassDAL classDAL, INoticeConfigDAL noticeConfigDAL,
            IComDAL comDAL, IUserOperationLogDAL userOperationLogDAL, ISysTenantDAL sysTenantDAL,
            ISysTenantCloudStorageDAL sysTenantCloudStorageDAL, IJobAnalyze2DAL jobAnalyze2DAL,
            ISysTenantStatistics2DAL sysTenantStatistics2DAL, ISysTenantStatisticsWeekDAL sysTenantStatisticsWeekDAL,
            ISysTenantStatisticsMonthDAL sysTenantStatisticsMonthDAL)
        {
            this._studentDAL = studentDAL;
            this._courseDAL = courseDAL;
            this._classDAL = classDAL;
            this._noticeConfigDAL = noticeConfigDAL;
            this._comDAL = comDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._sysTenantCloudStorageDAL = sysTenantCloudStorageDAL;
            this._jobAnalyze2DAL = jobAnalyze2DAL;
            this._sysTenantStatistics2DAL = sysTenantStatistics2DAL;
            this._sysTenantStatisticsWeekDAL = sysTenantStatisticsWeekDAL;
            this._sysTenantStatisticsMonthDAL = sysTenantStatisticsMonthDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentDAL, _courseDAL, _classDAL, _noticeConfigDAL, _comDAL,
                _userOperationLogDAL, _jobAnalyze2DAL);
        }

        public async Task<EtNoticeConfig> NoticeConfigGet(int type, byte peopleType, int scenesType)
        {
            return await _noticeConfigDAL.GetNoticeConfig(type, peopleType, scenesType);
        }

        public async Task<IEnumerable<EtClass>> GetStudentInClass(long studentId)
        {
            return await _classDAL.GetStudentClass(studentId);
        }

        public async Task ComSqlHandleConsumerEvent(ComSqlHandleEvent request)
        {
            await _comDAL.ExecuteSql(request.Sql);
        }

        public async Task SyncTenantLastOpTimeConsumerEvent(SyncTenantLastOpTimeEvent request)
        {
            var lastOpTime = await _userOperationLogDAL.GetLastOpTime(request.TenantId);
            if (lastOpTime != null)
            {
                await _sysTenantDAL.UpdateTenantLastOpTime(request.TenantId, lastOpTime.Value);
            }
        }

        public async Task CloudStorageAnalyzeConsumerEvent(CloudStorageAnalyzeEvent request)
        {
            var now = DateTime.Now;
            var unitCvtGb = 1024;
            var tenantId = request.TenantId;
            var tenantCloudStorageList = new List<SysTenantCloudStorage>();
            var aliyunOssCall = new AliyunOssCall();
            var lastOldPrefix = $"{SystemConfig.ComConfig.OSSRootFolderProd}/{tenantId}/";
            var lastOldSizeMb = aliyunOssCall.Statistics(lastOldPrefix);
            var lastOldSizeGb = lastOldSizeMb / unitCvtGb;
            tenantCloudStorageList.Add(new SysTenantCloudStorage()
            {
                IsDeleted = EmIsDeleted.Normal,
                AgentId = request.AgentId,
                LastModified = now,
                Remark = null,
                TenantId = tenantId,
                Type = 0,
                ValueMB = lastOldSizeMb,
                ValueGB = lastOldSizeGb
            });
            var totalMb = lastOldSizeMb;
            var totalGb = lastOldSizeGb;
            foreach (var itemTag in EmTenantCloudStorageType.TenantCloudStorageTypeTags)
            {
                var itemPrefix = $"{SystemConfig.ComConfig.OSSRootNewFolder}/{itemTag.Tag}/{SystemConfig.ComConfig.OSSRootFolderProd}/{tenantId}/";
                var itemSizeMb = aliyunOssCall.Statistics(itemPrefix);
                var itemSizeGb = itemSizeMb / unitCvtGb;
                tenantCloudStorageList.Add(new SysTenantCloudStorage()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    AgentId = request.AgentId,
                    LastModified = now,
                    Remark = null,
                    TenantId = tenantId,
                    Type = itemTag.Type,
                    ValueMB = totalMb,
                    ValueGB = itemSizeGb
                });
                totalMb += itemSizeMb;
                totalGb += itemSizeGb;
            }
            await _sysTenantDAL.UpdateTenantCloudStorage(tenantId, totalMb, totalGb);
            await _sysTenantCloudStorageDAL.SaveCloudStorage(tenantId, tenantCloudStorageList);
        }

        public async Task SysTenantStatistics2ConsumerEvent(SysTenantStatistics2Event request)
        {
            var studentTypeCountStatistics = await _jobAnalyze2DAL.GetStudentTypeCount();
            var teacherCount = await _jobAnalyze2DAL.GetTeahcerOfWork();
            var studentReadCount = 0;
            var studentPotentialCount = 0;
            var studentHistoryCount = 0;
            if (studentTypeCountStatistics != null && studentTypeCountStatistics.Any())
            {
                var studentCount1Log = studentTypeCountStatistics.FirstOrDefault(p => p.StudentType == EmStudentType.HiddenStudent); //潜在学员
                if (studentCount1Log != null)
                {
                    studentPotentialCount = studentCount1Log.MyCount;
                }
                var studentCount2Log = studentTypeCountStatistics.FirstOrDefault(p => p.StudentType == EmStudentType.ReadingStudent); //潜在学员
                if (studentCount2Log != null)
                {
                    studentReadCount = studentCount2Log.MyCount;
                }
                var studentCount3Log = studentTypeCountStatistics.FirstOrDefault(p => p.StudentType == EmStudentType.HistoryStudent); //潜在学员
                if (studentCount3Log != null)
                {
                    studentHistoryCount = studentCount3Log.MyCount;
                }
            }

            var entity = await _sysTenantStatistics2DAL.GetSysTenantStatistics(request.TenantId);
            if (entity == null)
            {
                entity = new SysTenantStatistics2()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    StudentPotentialCount = studentPotentialCount,
                    StudentReadCount = studentReadCount,
                    StudentHistoryCount = studentHistoryCount,
                    TeacherCount = teacherCount,
                    TenantId = request.TenantId
                };
            }
            else
            {
                entity.StudentPotentialCount = studentPotentialCount;
                entity.StudentReadCount = studentReadCount;
                entity.StudentHistoryCount = studentHistoryCount;
                entity.TeacherCount = teacherCount;
            }
            await _sysTenantStatistics2DAL.SaveSysTenantStatistics2(entity);
        }

        public async Task SysTenantStatisticsWeekAndMonthConsumerEvent(SysTenantStatisticsWeekAndMonthEvent request)
        {
            await SysTenantStatisticsWeekAndMonthConsumerEventWeek(request);
            await SysTenantStatisticsWeekAndMonthConsumerEventMonth(request);
        }

        private async Task SysTenantStatisticsWeekAndMonthConsumerEventWeek(SysTenantStatisticsWeekAndMonthEvent request)
        {
            var thisWeekDate = EtmsHelper2.GetThisWeek(DateTime.Now);
            var lastWeekDate = EtmsHelper2.GetLastWeek(DateTime.Now);
            var dateThisWeekStart = thisWeekDate.Item1;
            var dateThisWeekEnd = thisWeekDate.Item2;
            var dateLastWeekStart = lastWeekDate.Item1;
            var dateLastWeekEnd = lastWeekDate.Item2;
            var weekLog = await _sysTenantStatisticsWeekDAL.GetSysTenantStatisticsWeek(request.TenantId);
            if (weekLog == null)
            {
                weekLog = new SysTenantStatisticsWeek()
                {
                    TenantId = request.TenantId,
                    IsDeleted = EmIsDeleted.Normal
                };
            }

            if (request.Type == StatisticsWeekAndMonthType.ALL || request.Type == StatisticsWeekAndMonthType.Income)
            {
                var thisWeekDataIncome = await _jobAnalyze2DAL.GetIncomeLogGroupType(dateThisWeekStart, dateThisWeekEnd);
                var lastWeekDataIncome = await _jobAnalyze2DAL.GetIncomeLogGroupType(dateLastWeekStart, dateLastWeekEnd);
                if (thisWeekDataIncome.Any())
                {
                    var thisWeekDataIncome0 = thisWeekDataIncome.FirstOrDefault(p => p.Type == EmIncomeLogType.AccountIn);
                    var thisWeekDataIncome1 = thisWeekDataIncome.FirstOrDefault(p => p.Type == EmIncomeLogType.AccountOut);
                    weekLog.IncomeThis = thisWeekDataIncome0 == null ? 0 : thisWeekDataIncome0.TotalSum;
                    weekLog.ExpensesThis = thisWeekDataIncome1 == null ? 0 : thisWeekDataIncome1.TotalSum;
                }
                if (lastWeekDataIncome.Any())
                {
                    var lastWeekDataIncome0 = lastWeekDataIncome.FirstOrDefault(p => p.Type == EmIncomeLogType.AccountIn);
                    var lastWeekDataIncome1 = lastWeekDataIncome.FirstOrDefault(p => p.Type == EmIncomeLogType.AccountOut);
                    weekLog.IncomeLast = lastWeekDataIncome0 == null ? 0 : lastWeekDataIncome0.TotalSum;
                    weekLog.ExpensesLast = lastWeekDataIncome1 == null ? 0 : lastWeekDataIncome1.TotalSum;
                }
            }

            if (request.Type == StatisticsWeekAndMonthType.ALL || request.Type == StatisticsWeekAndMonthType.ClassTimes)
            {
                var thisWeekClass = await _jobAnalyze2DAL.GetStatisticsClassTimes(dateThisWeekStart, dateThisWeekEnd);
                var lastWeekClass = await _jobAnalyze2DAL.GetStatisticsClassTimes(dateLastWeekStart, dateLastWeekEnd);
                if (thisWeekClass != null)
                {
                    weekLog.ClassDeTimesThis = thisWeekClass.TotalClassTimes;
                    weekLog.ClassDeSumThis = thisWeekClass.TotalDeSum;
                }
                if (lastWeekClass != null)
                {
                    weekLog.ClassDeTimesLast = lastWeekClass.TotalClassTimes;
                    weekLog.ClassDeSumLast = lastWeekClass.TotalDeSum;
                }
            }

            if (request.Type == StatisticsWeekAndMonthType.ALL || request.Type == StatisticsWeekAndMonthType.BuyCourse)
            {
                var thisWeekBuyCourseCount = await _jobAnalyze2DAL.GetStudentBuyCourseCount(dateThisWeekStart, dateThisWeekEnd);
                var lastWeekBuyCourseCount = await _jobAnalyze2DAL.GetStudentBuyCourseCount(dateLastWeekStart, dateLastWeekEnd);
                weekLog.BuyCourseCountThis = thisWeekBuyCourseCount;
                weekLog.BuyCourseCountLast = lastWeekBuyCourseCount;
            }

            await _sysTenantStatisticsWeekDAL.SaveSysTenantStatisticsWeek(weekLog);
        }

        private async Task SysTenantStatisticsWeekAndMonthConsumerEventMonth(SysTenantStatisticsWeekAndMonthEvent request)
        {
            var thisMonthDate = EtmsHelper2.GetThisMonth(DateTime.Now);
            var lastMonthDate = EtmsHelper2.GetLastMonth(DateTime.Now);
            var dateThisMonthStart = thisMonthDate.Item1;
            var dateThisMonthEnd = thisMonthDate.Item2;
            var dateLastMonthStart = lastMonthDate.Item1;
            var dateLastMonthEnd = lastMonthDate.Item2;
            var monthLog = await _sysTenantStatisticsMonthDAL.GetSysTenantStatisticsMonth(request.TenantId);
            if (monthLog == null)
            {
                monthLog = new SysTenantStatisticsMonth()
                {
                    TenantId = request.TenantId,
                    IsDeleted = EmIsDeleted.Normal
                };
            }

            if (request.Type == StatisticsWeekAndMonthType.ALL || request.Type == StatisticsWeekAndMonthType.Income)
            {
                var thisMonthDataIncome = await _jobAnalyze2DAL.GetIncomeLogGroupType(dateThisMonthStart, dateThisMonthEnd);
                var lastMonthDataIncome = await _jobAnalyze2DAL.GetIncomeLogGroupType(dateLastMonthStart, dateLastMonthEnd);
                if (thisMonthDataIncome.Any())
                {
                    var thisMonthDataIncome0 = thisMonthDataIncome.FirstOrDefault(p => p.Type == EmIncomeLogType.AccountIn);
                    var thisMonthDataIncome1 = thisMonthDataIncome.FirstOrDefault(p => p.Type == EmIncomeLogType.AccountOut);
                    monthLog.IncomeThis = thisMonthDataIncome0 == null ? 0 : thisMonthDataIncome0.TotalSum;
                    monthLog.ExpensesThis = thisMonthDataIncome1 == null ? 0 : thisMonthDataIncome1.TotalSum;
                }
                if (lastMonthDataIncome.Any())
                {
                    var lastMonthDataIncome0 = lastMonthDataIncome.FirstOrDefault(p => p.Type == EmIncomeLogType.AccountIn);
                    var lastMonthDataIncome1 = lastMonthDataIncome.FirstOrDefault(p => p.Type == EmIncomeLogType.AccountOut);
                    monthLog.IncomeLast = lastMonthDataIncome0 == null ? 0 : lastMonthDataIncome0.TotalSum;
                    monthLog.ExpensesLast = lastMonthDataIncome1 == null ? 0 : lastMonthDataIncome1.TotalSum;
                }
            }

            if (request.Type == StatisticsWeekAndMonthType.ALL || request.Type == StatisticsWeekAndMonthType.ClassTimes)
            {
                var thisMonthClass = await _jobAnalyze2DAL.GetStatisticsClassTimes(dateThisMonthStart, dateThisMonthEnd);
                var lastMonthClass = await _jobAnalyze2DAL.GetStatisticsClassTimes(dateLastMonthStart, dateLastMonthEnd);
                if (thisMonthClass != null)
                {
                    monthLog.ClassDeTimesThis = thisMonthClass.TotalClassTimes;
                    monthLog.ClassDeSumThis = thisMonthClass.TotalDeSum;
                }
                if (lastMonthClass != null)
                {
                    monthLog.ClassDeTimesLast = lastMonthClass.TotalClassTimes;
                    monthLog.ClassDeSumLast = lastMonthClass.TotalDeSum;
                }
            }

            if (request.Type == StatisticsWeekAndMonthType.ALL || request.Type == StatisticsWeekAndMonthType.BuyCourse)
            {
                var thisMonthBuyCourseCount = await _jobAnalyze2DAL.GetStudentBuyCourseCount(dateThisMonthStart, dateThisMonthEnd);
                var lastMonthBuyCourseCount = await _jobAnalyze2DAL.GetStudentBuyCourseCount(dateLastMonthStart, dateLastMonthEnd);
                monthLog.BuyCourseCountThis = thisMonthBuyCourseCount;
                monthLog.BuyCourseCountLast = lastMonthBuyCourseCount;
            }

            await _sysTenantStatisticsMonthDAL.SaveSysTenantStatisticsMonth(monthLog);
        }
    }
}
