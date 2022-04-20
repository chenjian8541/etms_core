using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.IEventProvider;
using ETMS.Utility;
using ETMS.Business.Common;

namespace ETMS.Business.EventConsumer
{
    public class EvStudentBLL : IEvStudentBLL
    {
        private readonly IStudentDAL _studentDAL;

        private readonly IStudentPointsLogDAL _studentPointsLogDAL;

        private readonly IStudentAccountRechargeDAL _studentAccountRechargeDAL;

        private readonly IStudentAccountRechargeLogDAL _studentAccountRechargeLogDAL;

        private readonly IAppConfigDAL _appConfigDAL;

        private readonly ITenantConfigDAL _tenantConfigDAL;

        private readonly IStudentAccountRechargeCoreBLL _studentAccountRechargeCoreBLL;

        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly IClassDAL _classDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IStudentExtendFieldDAL _studentExtendFieldDAL;

        private readonly IStudentCourseOpLogDAL _studentCourseOpLogDAL;

        private readonly IStudentWechatDAL _studentWechatDAL;
        public EvStudentBLL(IStudentDAL studentDAL, IStudentPointsLogDAL studentPointsLogDAL, IStudentAccountRechargeDAL studentAccountRechargeDAL,
            IStudentAccountRechargeLogDAL studentAccountRechargeLogDAL, IAppConfigDAL appConfigDAL, ITenantConfigDAL tenantConfigDAL,
            IStudentAccountRechargeCoreBLL studentAccountRechargeCoreBLL, IStudentCourseDAL studentCourseDAL, IClassDAL classDAL,
            IEventPublisher eventPublisher, IStudentExtendFieldDAL studentExtendFieldDAL, IStudentWechatDAL studentWechatDAL,
            IStudentCourseOpLogDAL studentCourseOpLogDAL)
        {
            this._studentDAL = studentDAL;
            this._studentPointsLogDAL = studentPointsLogDAL;
            this._studentAccountRechargeDAL = studentAccountRechargeDAL;
            this._studentAccountRechargeLogDAL = studentAccountRechargeLogDAL;
            this._appConfigDAL = appConfigDAL;
            this._tenantConfigDAL = tenantConfigDAL;
            this._studentAccountRechargeCoreBLL = studentAccountRechargeCoreBLL;
            this._studentCourseDAL = studentCourseDAL;
            this._classDAL = classDAL;
            this._eventPublisher = eventPublisher;
            this._studentExtendFieldDAL = studentExtendFieldDAL;
            this._studentCourseOpLogDAL = studentCourseOpLogDAL;
            this._studentWechatDAL = studentWechatDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._studentAccountRechargeCoreBLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _studentDAL, _studentPointsLogDAL, _studentAccountRechargeDAL, _studentAccountRechargeLogDAL,
               _appConfigDAL, _tenantConfigDAL, _studentCourseDAL, _classDAL, _studentExtendFieldDAL,
               _studentCourseOpLogDAL, _studentWechatDAL);
        }

        public async Task StudentRecommendRewardConsumerEvent(StudentRecommendRewardEvent request)
        {
            if (request.Student.RecommendStudentId == null)
            {
                return;
            }
            var config = await _tenantConfigDAL.GetTenantConfig();
            var studentRecommendConfig = config.StudentRecommendConfig;
            if (request.Type == StudentRecommendRewardType.Registered)
            {
                await StudentRecommendRewardConsumerRegistered(request, studentRecommendConfig);
                return;
            }
            if (request.Type == StudentRecommendRewardType.Buy)
            {
                await StudentRecommendRewardConsumerBuy(request, studentRecommendConfig);
                return;
            }
        }

        private async Task StudentRecommendRewardConsumerRegistered(StudentRecommendRewardEvent request, StudentRecommendConfig studentRecommendConfig)
        {
            if (!studentRecommendConfig.IsOpenRegistered || (studentRecommendConfig.RegisteredGivePoints <= 0 && studentRecommendConfig.RegisteredGiveMoney <= 0))
            {
                return;
            }
            var recommendStudentBucket = await _studentDAL.GetStudent(request.Student.RecommendStudentId.Value);
            if (recommendStudentBucket == null || recommendStudentBucket.Student == null)
            {
                LOG.Log.Error("[StudentRecommendRewardConsumerRegistered]推荐的学员未找到", request, this.GetType());
                return;
            }
            var recommendStudent = recommendStudentBucket.Student;
            var now = DateTime.Now;
            if (studentRecommendConfig.RegisteredGivePoints > 0)
            {
                //奖励积分
                await _studentDAL.AddPoint(recommendStudent.Id, studentRecommendConfig.RegisteredGivePoints);
                await _studentPointsLogDAL.AddStudentPointsLog(new EtStudentPointsLog()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    No = string.Empty,
                    Ot = now,
                    Points = studentRecommendConfig.RegisteredGivePoints,
                    Remark = $"推荐学员注册[{request.Student.Name},{request.Student.Phone}]奖励{studentRecommendConfig.RegisteredGivePoints}积分",
                    StudentId = recommendStudent.Id,
                    TenantId = recommendStudent.TenantId,
                    Type = EmStudentPointsLogType.RecommendStudentRegistered
                });
            }
            if (studentRecommendConfig.RegisteredGiveMoney > 0)
            {
                //奖励金额
                var accountLog = await _studentAccountRechargeCoreBLL.GetStudentAccountRechargeByStudentId2(recommendStudent.Id);
                if (accountLog == null)
                {
                    return;
                }
                await _studentAccountRechargeCoreBLL.StudentAccountRechargeChange(new StudentAccountRechargeChangeEvent(recommendStudent.TenantId)
                {
                    AddBalanceReal = 0,
                    AddBalanceGive = studentRecommendConfig.RegisteredGiveMoney,
                    AddRechargeSum = 0,
                    AddRechargeGiveSum = 0,
                    StudentAccountRechargeId = accountLog.Id,
                    TryCount = 0
                });
                await _studentAccountRechargeLogDAL.AddStudentAccountRechargeLog(new EtStudentAccountRechargeLog()
                {
                    CgNo = string.Empty,
                    CgBalanceReal = 0,
                    CgBalanceGive = studentRecommendConfig.RegisteredGiveMoney,
                    CgServiceCharge = 0,
                    CommissionUser = string.Empty,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = now,
                    Phone = accountLog.Phone,
                    RelatedOrderId = null,
                    Remark = $"推荐学员注册[{request.Student.Name},{request.Student.Phone}]奖励{studentRecommendConfig.RegisteredGiveMoney}金额",
                    Status = EmStudentAccountRechargeLogStatus.Normal,
                    StudentAccountRechargeId = accountLog.Id,
                    TenantId = accountLog.TenantId,
                    UserId = request.UserId,
                    Type = EmStudentAccountRechargeLogType.RecommendStudentRegistered
                });
            }
        }

        private async Task StudentRecommendRewardConsumerBuy(StudentRecommendRewardEvent request, StudentRecommendConfig studentRecommendConfig)
        {
            if (!studentRecommendConfig.IsOpenBuy || (studentRecommendConfig.BuyGivePoints <= 0 && studentRecommendConfig.BuyGiveMoney <= 0))
            {
                return;
            }
            var recommendStudentBucket = await _studentDAL.GetStudent(request.Student.RecommendStudentId.Value);
            if (recommendStudentBucket == null || recommendStudentBucket.Student == null)
            {
                LOG.Log.Error("[StudentRecommendRewardConsumerBuy]推荐的学员未找到", request, this.GetType());
                return;
            }
            var recommendStudent = recommendStudentBucket.Student;
            var now = DateTime.Now;
            if (studentRecommendConfig.BuyGivePoints > 0)
            {
                await _studentDAL.AddPoint(recommendStudent.Id, studentRecommendConfig.BuyGivePoints);
                await _studentPointsLogDAL.AddStudentPointsLog(new EtStudentPointsLog()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    No = request.Order.No,
                    Ot = now,
                    Points = studentRecommendConfig.BuyGivePoints,
                    Remark = $"推荐学员消费[{request.Student.Name},{request.Student.Phone}]奖励{studentRecommendConfig.BuyGivePoints}积分",
                    StudentId = recommendStudent.Id,
                    TenantId = recommendStudent.TenantId,
                    Type = EmStudentPointsLogType.RecommendStudentBuy
                });
            }
            if (studentRecommendConfig.BuyGiveMoney > 0)
            {
                var accountLog = await _studentAccountRechargeCoreBLL.GetStudentAccountRechargeByStudentId2(recommendStudent.Id);
                if (accountLog == null)
                {
                    return;
                }
                await _studentAccountRechargeCoreBLL.StudentAccountRechargeChange(new StudentAccountRechargeChangeEvent(recommendStudent.TenantId)
                {
                    AddBalanceReal = 0,
                    AddBalanceGive = studentRecommendConfig.BuyGiveMoney,
                    AddRechargeSum = 0,
                    AddRechargeGiveSum = 0,
                    StudentAccountRechargeId = accountLog.Id,
                    TryCount = 0
                });

                await _studentAccountRechargeLogDAL.AddStudentAccountRechargeLog(new EtStudentAccountRechargeLog()
                {
                    CgNo = request.Order.No,
                    CgBalanceReal = 0,
                    CgBalanceGive = studentRecommendConfig.BuyGiveMoney,
                    CgServiceCharge = 0,
                    CommissionUser = string.Empty,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = now,
                    Phone = accountLog.Phone,
                    RelatedOrderId = request.Order.Id,
                    Remark = $"推荐学员消费[{request.Student.Name},{request.Student.Phone}]奖励{studentRecommendConfig.BuyGiveMoney}金额",
                    Status = EmStudentAccountRechargeLogStatus.Normal,
                    StudentAccountRechargeId = accountLog.Id,
                    TenantId = accountLog.TenantId,
                    UserId = request.UserId,
                    Type = EmStudentAccountRechargeLogType.RecommendStudentBuy
                });
            }
        }

        public async Task StudentAutoMarkGraduationConsumerEvent(StudentAutoMarkGraduationEvent request)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            if (!config.TenantOtherConfig.AutoMarkGraduation)
            {
                return;
            }
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return;
            }
            if (studentBucket.Student.StudentType == EmStudentType.HistoryStudent)
            {
                return;
            }
            var myCourseDetails = await _studentCourseDAL.GetStudentCourseDetail(request.StudentId);
            if (myCourseDetails.Count > 0)
            {
                var notEndClass = myCourseDetails.FirstOrDefault(p => p.Status != EmStudentCourseStatus.EndOfClass);
                if (notEndClass == null) //没有未结课的课程
                {
                    await _studentDAL.EditStudentType(request.StudentId, EmStudentType.HistoryStudent, DateTime.Now);
                    var affectClassList = await _classDAL.RemoveStudent(request.StudentId);
                    if (affectClassList != null && affectClassList.Any())
                    {
                        foreach (var myClass in affectClassList)
                        {
                            _eventPublisher.Publish(new SyncClassInfoEvent(request.TenantId, myClass.ClassId));
                        }
                    }
                    _eventPublisher.Publish(new StatisticsStudentEvent(request.TenantId)
                    {
                        OpType = EmStatisticsStudentType.StudentType
                    });
                    _eventPublisher.Publish(new SyncStudentClassInfoEvent(request.TenantId)
                    {
                        StudentId = request.StudentId
                    });
                }
            }
        }

        public async Task SyncStudentClassInfoConsumerEvent(SyncStudentClassInfoEvent request)
        {
            await _studentDAL.UpdateStudentClassInfo(request.StudentId);
        }

        public async Task UpdateStudentInfoConsumerEvent(UpdateStudentInfoEvent request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.MyStudent.Id);
            if (studentBucket == null || studentBucket.Student == null)
            {
                LOG.Log.Error("[UpdateStudentInfoConsumerEvent]未找到学员", request, this.GetType());
                return;
            }
            var student = studentBucket.Student;
            var myAgeResut = student.Birthday.EtmsGetAge();
            var newAge = myAgeResut?.Item1;
            var newAgeMonth = myAgeResut?.Item2;
            if (newAge == student.Age && newAgeMonth == student.AgeMonth)
            {
                return;
            }

            var bindingWechatInfo = await _studentWechatDAL.GetStudentWechatByPhone(student.Phone);
            student.Age = newAge;
            student.AgeMonth = newAgeMonth;
            student.ClassIds = await GetStudentClassIds(student.Id);
            student.CourseIds = await GetGetStudentCourseIds(student.Id);
            student.IsBindingWechat = bindingWechatInfo == null ? EmIsBindingWechat.No : EmIsBindingWechat.Yes;
            await _studentDAL.EditStudent2(student);
        }

        public async Task ImportExtendFieldExcelConsumerEvent(ImportExtendFieldExcelEvent request)
        {
            if (request.Students == null || request.Students.Count == 0)
            {
                return;
            }
            if (request.StudentExtendFieldItems == null || request.StudentExtendFieldItems.Count == 0)
            {
                return;
            }
            var studentExtendFieldAll = await _studentExtendFieldDAL.GetAllStudentExtendField();
            if (studentExtendFieldAll == null || studentExtendFieldAll.Count == 0)
            {
                return;
            }
            var studentList = request.Students;
            var studentExtendInfos = new List<EtStudentExtendInfo>();
            foreach (var p in request.StudentExtendFieldItems)
            {
                if (p.ExtendInfoList == null || p.ExtendInfoList.Count == 0)
                {
                    continue;
                }
                var myStudent = studentList.FirstOrDefault(j => j.Name == p.StudentName && j.Phone == p.Phone);
                if (myStudent == null)
                {
                    continue;
                }
                foreach (var a in p.ExtendInfoList)
                {
                    var myExtendField = studentExtendFieldAll.FirstOrDefault(j => j.DisplayName == a.DisplayName);
                    if (myExtendField == null)
                    {
                        continue;
                    }
                    studentExtendInfos.Add(new EtStudentExtendInfo()
                    {
                        ExtendFieldId = myExtendField.Id,
                        IsDeleted = EmIsDeleted.Normal,
                        Remark = string.Empty,
                        StudentId = myStudent.Id,
                        TenantId = request.TenantId,
                        Value1 = a.Value
                    });
                }
            }
            if (studentExtendInfos.Any())
            {
                _studentDAL.AddStudentExtend(studentExtendInfos);
            }
        }

        public async Task SyncStudentCourseStatusConsumerEvent(SyncStudentCourseStatusEvent request)
        {
            var studentAllStatus = await _studentCourseDAL.StudentCourseStatusGet(request.StudentId);
            var newCourseStatus = ComBusiness4.GetStudentCourseStatus(studentAllStatus);
            await _studentDAL.UpdateStudentCourseStatus(request.StudentId, newCourseStatus);
        }

        public async Task StudentCourseRestoreTimeBatchConsumerEvent(StudentCourseRestoreTimeBatchEvent request)
        {
            var allStopCourseIds = await _studentCourseDAL.StudentStopCourseGet(request.StudentId);
            if (!allStopCourseIds.Any())
            {
                return;
            }
            var restoreDate = DateTime.Now.Date;
            var studentCourseOpLogs = new List<EtStudentCourseOpLog>();
            foreach (var myCourse in allStopCourseIds)
            {
                await ComBusiness3.RestoreStudentCourse(_studentCourseDAL, request.TenantId, request.StudentId, myCourse.CourseId,
                    restoreDate);
                _eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(request.TenantId)
                {
                    CourseId = myCourse.CourseId,
                    StudentId = request.StudentId,
                    IsSendNoticeStudent = true
                });
                studentCourseOpLogs.Add(new EtStudentCourseOpLog()
                {
                    CourseId = myCourse.CourseId,
                    IsDeleted = EmIsDeleted.Normal,
                    OpTime = DateTime.Now,
                    OpType = EmStudentCourseOpLogType.CourseRestore,
                    OpUser = request.UserId,
                    StudentId = request.StudentId,
                    TenantId = request.TenantId,
                    OpContent = "课程批量复课",
                    Remark = string.Empty
                });
            }

            try
            {
                _studentCourseOpLogDAL.AddStudentCourseOpLog(studentCourseOpLogs);
            }
            catch (Exception ex)
            {
                LOG.Log.Error("[StudentCourseRestoreTimeBatchConsumerEvent]错误", ex, this.GetType());
            }
        }

        private async Task<string> GetStudentClassIds(long studentId)
        {
            var myClassStudentLog = await _classDAL.GetStudentClass2(studentId);
            var strIds = string.Empty;
            if (myClassStudentLog.Any())
            {
                strIds = EtmsHelper.GetMuIds(myClassStudentLog.Select(j => j.ClassId));
            }
            return strIds;
        }

        public async Task SyncStudentStudentClassIdsConsumerEvent(SyncStudentStudentClassIdsEvent request)
        {
            var strIds = await GetStudentClassIds(request.StudentId);
            await _studentDAL.UpdateStudentClassIds(request.StudentId, strIds);
        }

        private async Task<string> GetGetStudentCourseIds(long studentId)
        {
            var myCourseIds = await _studentCourseDAL.GetStudentBuyCourseIdIsReading(studentId);
            var strIds = string.Empty;
            if (myCourseIds.Any())
            {
                strIds = EtmsHelper.GetMuIds(myCourseIds.Select(j => j.CourseId));
            }
            return strIds;
        }

        public async Task SyncStudentStudentCourseIdsConsumerEvent(SyncStudentStudentCourseIdsEvent request)
        {
            var strIds = await GetGetStudentCourseIds(request.StudentId);
            await _studentDAL.UpdateStudentCourseIds(request.StudentId, strIds);
        }
    }
}
