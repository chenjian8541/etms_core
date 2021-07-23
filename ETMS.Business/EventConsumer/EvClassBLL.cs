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

namespace ETMS.Business.EventConsumer
{
    public class EvClassBLL : IEvClassBLL
    {
        private readonly IClassDAL _classDAL;

        private readonly IClassTimesDAL _classTimesDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IStatisticsEducationDAL _statisticsEducationDAL;

        private readonly IClassRecordDAL _classRecordDAL;

        private readonly ITenantConfigDAL _tenantConfigDAL;

        private readonly IStudentCheckOnLogDAL _studentCheckOnLogDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IUserDAL _userDAL;

        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly IStudentCourseConsumeLogDAL _studentCourseConsumeLogDAL;

        private readonly ITempStudentNeedCheckDAL _tempStudentNeedCheckDAL;

        private readonly IStudentTrackLogDAL _studentTrackLogDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly ITryCalssLogDAL _tryCalssLogDAL;

        public EvClassBLL(IClassDAL classDAL, IEventPublisher eventPublisher, IClassTimesDAL classTimesDAL,
            IStatisticsEducationDAL statisticsEducationDAL, IClassRecordDAL classRecordDAL, ITenantConfigDAL tenantConfigDAL,
            IStudentCheckOnLogDAL studentCheckOnLogDAL, ICourseDAL courseDAL, IUserDAL userDAL, IStudentCourseDAL studentCourseDAL,
            IStudentCourseConsumeLogDAL studentCourseConsumeLogDAL, ITempStudentNeedCheckDAL tempStudentNeedCheckDAL,
            IStudentTrackLogDAL studentTrackLogDAL, IStudentDAL studentDAL, ITryCalssLogDAL tryCalssLogDAL)
        {
            this._classDAL = classDAL;
            this._eventPublisher = eventPublisher;
            this._classTimesDAL = classTimesDAL;
            this._statisticsEducationDAL = statisticsEducationDAL;
            this._classRecordDAL = classRecordDAL;
            this._tenantConfigDAL = tenantConfigDAL;
            this._studentCheckOnLogDAL = studentCheckOnLogDAL;
            this._courseDAL = courseDAL;
            this._userDAL = userDAL;
            this._studentCourseDAL = studentCourseDAL;
            this._studentCourseConsumeLogDAL = studentCourseConsumeLogDAL;
            this._tempStudentNeedCheckDAL = tempStudentNeedCheckDAL;
            this._studentTrackLogDAL = studentTrackLogDAL;
            this._studentDAL = studentDAL;
            this._tryCalssLogDAL = tryCalssLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _classDAL, _classTimesDAL, _statisticsEducationDAL,
                _classRecordDAL, _tenantConfigDAL, _studentCheckOnLogDAL, _courseDAL, _userDAL, _studentCourseDAL,
                _studentCourseConsumeLogDAL, _tempStudentNeedCheckDAL, _studentTrackLogDAL);
        }

        public async Task ClassOfOneAutoOverConsumerEvent(ClassOfOneAutoOverEvent request)
        {
            var myOneToOneClass = await _classDAL.GetStudentOneToOneClassNormal(request.StudentId, request.CourseId);
            if (myOneToOneClass.Any())
            {
                var now = DateTime.Now;
                foreach (var p in myOneToOneClass)
                {
                    var thisClassBucket = await _classDAL.GetClassBucket(p.Id);
                    var thisClass = thisClassBucket.EtClass;
                    var thisClassStudent = thisClassBucket.EtClassStudents;
                    if (thisClass.Type == EmClassType.OneToOne)
                    {
                        if (thisClassStudent == null || thisClassStudent.Count == 0 ||
                            thisClassStudent.FirstOrDefault(j => j.StudentId == request.StudentId) != null)
                        {
                            await _classDAL.SetClassOverOneToOne(p.Id, now);
                            LOG.Log.Info($"[ClassOfOneAutoOverConsumerEvent]TenantId:{p.TenantId},ClassId:{p.Id}自动结课", this.GetType());
                        }
                    }
                }
            }
        }

        public async Task ClassRemoveStudentConsumerEvent(ClassRemoveStudentEvent request)
        {
            var myInClassList = await _classDAL.GetStudentCourseInClass(request.StudentId, request.CourseId);
            foreach (var myClass in myInClassList)
            {
                await _classDAL.DelClassStudentByStudentId(myClass.ClassId, request.StudentId);
                _eventPublisher.Publish(new SyncClassInfoEvent(request.TenantId, myClass.ClassId));
            }
        }

        public async Task SyncClassTimesStudentConsumerEvent(SyncClassTimesStudentEvent request)
        {
            var classTimes = await _classTimesDAL.GetClassTimes(request.ClassTimesId);
            var classBucket = await _classDAL.GetClassBucket(classTimes.ClassId);
            if (classBucket == null || classBucket.EtClass == null)
            {
                LOG.Log.Error("[SyncClassTimesStudentConsumerEvent]班级不存在", request, this.GetType());
                return;
            }
            var classStudent = classBucket.EtClassStudents;
            var classTimesStudent = await _classTimesDAL.GetClassTimesStudent(classTimes.Id);
            var studentCount = 0;
            var studentTempCount = 0;
            var strStudentIdsClass = string.Empty;
            var strStudentIdsTemp = string.Empty;
            var strStudentIdsReservation = string.Empty;
            if (classStudent != null && classStudent.Count > 0)
            {
                strStudentIdsClass = EtmsHelper.GetMuIds(classStudent.Select(p => p.StudentId));
                studentCount += classStudent.Count;
            }
            if (classTimesStudent != null && classTimesStudent.Count > 0)
            {
                var reservationStudent = classTimesStudent.Where(p => p.IsReservation == EmBool.True);
                var notReservationStudent = classTimesStudent.Where(p => p.IsReservation == EmBool.False);
                if (reservationStudent.Any())
                {
                    strStudentIdsReservation = EtmsHelper.GetMuIds(reservationStudent.Select(p => p.StudentId));
                }
                if (notReservationStudent.Any())
                {
                    strStudentIdsTemp = EtmsHelper.GetMuIds(notReservationStudent.Select(p => p.StudentId));
                }
                studentTempCount = classTimesStudent.Count;
                studentCount += classTimesStudent.Count;
            }
            classTimes.StudentIdsClass = strStudentIdsClass;
            classTimes.StudentIdsTemp = strStudentIdsTemp;
            classTimes.StudentIdsReservation = strStudentIdsReservation;
            classTimes.StudentCount = studentCount;
            classTimes.StudentTempCount = studentTempCount;
            await _classTimesDAL.EditClassTimes(classTimes);
        }

        public async Task StatisticsEducationConsumerEvent(StatisticsEducationEvent request)
        {
            await _statisticsEducationDAL.StatisticsEducationUpdate(request.Time);
        }

        public async Task StatisticsClassFinishCountConsumerEvent(StatisticsClassFinishCountEvent request)
        {
            var classRecordStatistics = await _classRecordDAL.GetClassRecordStatistics(request.ClassId);
            if (classRecordStatistics == null)
            {
                classRecordStatistics = new ClassRecordStatistics()
                {
                    TotalFinishClassTimes = 0,
                    TotalFinishCount = 0
                };
            }
            await _classDAL.UpdateClassFinishInfo(request.ClassId, classRecordStatistics.TotalFinishCount, classRecordStatistics.TotalFinishClassTimes);
        }

        private DataTempBox<EtCourse> _tempBoxCourse;

        public async Task StudentCheckOnAutoGenerateClassRecordConsumerEvent(StudentCheckOnAutoGenerateClassRecordEvent request)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            var checkInConfig = config.StudentCheckInConfig;
            if (checkInConfig.IsRelationClassTimesAutoGenerateClassRecord == EmBool.False)
            {
                LOG.Log.Info($"[StudentCheckOnAutoGenerateClassRecordConsumerEvent]未开启考勤自动生成点名记录-{request.TenantId}", this.GetType());
                return;
            }
            var studentUseCardCheckIn = checkInConfig.StudentUseCardCheckIn;
            var studentUseFaceCheckIn = checkInConfig.StudentUseFaceCheckIn;
            if (!((studentUseCardCheckIn.IsRelationClassTimesCard == EmBool.True &&
                studentUseCardCheckIn.RelationClassTimesCardType == EmAttendanceRelationClassTimesType.RelationClassTimes) ||
                (studentUseFaceCheckIn.IsRelationClassTimesFace == EmBool.True &&
                studentUseFaceCheckIn.RelationClassTimesFaceType == EmAttendanceRelationClassTimesType.RelationClassTimes)))
            {
                LOG.Log.Info($"[StudentCheckOnAutoGenerateClassRecordConsumerEvent]未开启考勤自动生成点名记录-{request.TenantId}", this.GetType());
                return;
            }

            var checkOnClassTimesIds = await _studentCheckOnLogDAL.GetOneDayStudentCheckInAllClassTimes(request.AnalyzeDate);
            if (checkOnClassTimesIds == null || !checkOnClassTimesIds.Any())
            {
                LOG.Log.Warn($"[StudentCheckOnAutoGenerateClassRecordConsumerEvent]未查询到考勤课次-{request.TenantId}", this.GetType());
                return;
            }
            var adminUser = await _userDAL.GetAdminUser();
            this._tempBoxCourse = new DataTempBox<EtCourse>();
            var handler = new StudentCheckOnGenerateClassRecordHandler(_classDAL, _classTimesDAL, _courseDAL, _studentCourseDAL,
                _studentCheckOnLogDAL, _classRecordDAL, _studentCourseConsumeLogDAL, _tempStudentNeedCheckDAL, _tryCalssLogDAL,
                _eventPublisher, _studentDAL, _studentTrackLogDAL, _userDAL);
            foreach (var p in checkOnClassTimesIds)
            {
                try
                {
                    await handler.Process(request.TenantId, p.Id, request.AnalyzeDate, adminUser.Id, config.ClassCheckSignConfig.MakeupIsDeClassTimes,
                        config.ClassCheckSignConfig.TryCalssNoticeTrackUser);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[StudentCheckOnAutoGenerateClassRecordConsumerEvent]处理考勤自动生成点名记录失败-{request.TenantId}-{p.Id}", ex, this.GetType());
                    continue;
                }
            }
        }
    }
}
