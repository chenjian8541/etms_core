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

namespace ETMS.Business.EventConsumer
{
    public class EvClassBLL : IEvClassBLL
    {
        private readonly IClassDAL _classDAL;

        private readonly IClassTimesDAL _classTimesDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IStatisticsEducationDAL _statisticsEducationDAL;

        public EvClassBLL(IClassDAL classDAL, IEventPublisher eventPublisher, IClassTimesDAL classTimesDAL, IStatisticsEducationDAL statisticsEducationDAL)
        {
            this._classDAL = classDAL;
            this._eventPublisher = eventPublisher;
            this._classTimesDAL = classTimesDAL;
            this._statisticsEducationDAL = statisticsEducationDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _classDAL, _classTimesDAL, _statisticsEducationDAL);
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
    }
}
