using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess;
using ETMS.IDataAccess.TeacherSalary;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EventConsumer
{
    public class EvEducationBLL : IEvEducationBLL
    {
        private readonly IClassRecordDAL _classRecordDAL;

        private readonly ITeacherSalaryClassDAL _teacherSalaryClassDAL;

        public EvEducationBLL(IClassRecordDAL classRecordDAL, ITeacherSalaryClassDAL teacherSalaryClassDAL)
        {
            this._classRecordDAL = classRecordDAL;
            this._teacherSalaryClassDAL = teacherSalaryClassDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _classRecordDAL, _teacherSalaryClassDAL);
        }

        public async Task StatisticsTeacherSalaryClassTimesConsumerEvent(StatisticsTeacherSalaryClassTimesEvent request)
        {
            var classRecord = await _classRecordDAL.GetClassRecord(request.ClassRecordId);
            if (classRecord.Status == EmClassRecordStatus.Revoked)
            {
                await _teacherSalaryClassDAL.DelTeacherSalaryClassTimes(request.ClassRecordId);
                return;
            }
            var classRecordStudent = await _classRecordDAL.GetClassRecordStudents(request.ClassRecordId);
            var studentClassTimes = 0M;
            var deSum = 0M;
            var arrivedAndBeLateCount = 0;
            var arrivedCount = 0;
            var beLateCount = 0;
            var leaveCount = 0;
            var notArrivedCount = 0;
            var tryCalssStudentCount = 0;
            var makeUpStudentCount = 0;
            var tryCalssEffectiveCount = 0;
            var makeUpEffectiveCount = 0;
            foreach (var p in classRecordStudent)
            {
                studentClassTimes += p.DeClassTimes;
                deSum += p.DeSum;
                switch (p.StudentCheckStatus)
                {
                    case EmClassStudentCheckStatus.Arrived:
                        arrivedAndBeLateCount++;
                        arrivedCount++;
                        break;
                    case EmClassStudentCheckStatus.BeLate:
                        arrivedAndBeLateCount++;
                        beLateCount++;
                        break;
                    case EmClassStudentCheckStatus.Leave:
                        leaveCount++;
                        break;
                    case EmClassStudentCheckStatus.NotArrived:
                        notArrivedCount++;
                        break;
                }
                if (p.StudentType == EmClassStudentType.TryCalssStudent)
                {
                    tryCalssStudentCount++;
                    if (EmClassStudentCheckStatus.CheckIsAttend(p.StudentCheckStatus))
                    {
                        tryCalssEffectiveCount++;
                    }
                }
                if (p.StudentType == EmClassStudentType.MakeUpStudent)
                {
                    makeUpStudentCount++;
                    if (EmClassStudentCheckStatus.CheckIsAttend(p.StudentCheckStatus))
                    {
                        makeUpEffectiveCount++;
                    }
                }
            }
            var entitys = new List<EtTeacherSalaryClassTimes>();

            var ot = classRecord.ClassOt.Date;
            var teacherIds = EtmsHelper.AnalyzeMuIds(classRecord.Teachers);
            var courseIds = EtmsHelper.AnalyzeMuIds(classRecord.CourseList);
            if (teacherIds.Count == 1 && courseIds.Count == 1)  //大部分情况下是一位老师 一门课程
            {
                entitys.Add(new EtTeacherSalaryClassTimes()
                {
                    TeacherId = teacherIds[0],
                    CourseId = courseIds[0],
                    ArrivedCount = arrivedCount,
                    ArrivedAndBeLateCount = arrivedAndBeLateCount,
                    BeLateCount = beLateCount,
                    ClassId = classRecord.ClassId,
                    ClassRecordId = classRecord.Id,
                    IsDeleted = EmIsDeleted.Normal,
                    DeSum = deSum,
                    LeaveCount = leaveCount,
                    MakeUpStudentCount = makeUpStudentCount,
                    NotArrivedCount = notArrivedCount,
                    Ot = ot,
                    StudentClassTimes = studentClassTimes,
                    TeacherClassTimes = classRecord.ClassTimes,
                    TenantId = classRecord.TenantId,
                    TryCalssStudentCount = tryCalssStudentCount,
                    TryCalssEffectiveCount = tryCalssEffectiveCount,
                    MakeUpEffectiveCount = makeUpEffectiveCount,
                    EndTime = classRecord.EndTime,
                    StartTime = classRecord.StartTime,
                    Week = classRecord.Week,
                });
            }
            else
            {
                foreach (var teacherId in teacherIds)
                {
                    foreach (var courseId in courseIds)
                    {
                        entitys.Add(new EtTeacherSalaryClassTimes()
                        {
                            TeacherId = teacherId,
                            CourseId = courseId,
                            ArrivedCount = arrivedCount,
                            ArrivedAndBeLateCount = arrivedAndBeLateCount,
                            BeLateCount = beLateCount,
                            ClassId = classRecord.ClassId,
                            ClassRecordId = classRecord.Id,
                            IsDeleted = EmIsDeleted.Normal,
                            DeSum = deSum,
                            LeaveCount = leaveCount,
                            MakeUpStudentCount = makeUpStudentCount,
                            NotArrivedCount = notArrivedCount,
                            Ot = ot,
                            StudentClassTimes = studentClassTimes,
                            TeacherClassTimes = classRecord.ClassTimes,
                            TenantId = classRecord.TenantId,
                            TryCalssStudentCount = tryCalssStudentCount,
                            TryCalssEffectiveCount = tryCalssEffectiveCount,
                            MakeUpEffectiveCount = makeUpEffectiveCount,
                            EndTime = classRecord.EndTime,
                            StartTime = classRecord.StartTime,
                            Week = classRecord.Week
                        });
                    }
                }
            }

            await _teacherSalaryClassDAL.SaveTeacherSalaryClassTimes(request.ClassRecordId, entitys);
        }

        public async Task StatisticsTeacherSalaryClassDayConsumerEvent(StatisticsTeacherSalaryClassDayEvent request)
        {
            if (!request.IsJobRequest)
            {
                if (request.Time.IsToday()) // 今天的数据在JOB里边进行统计
                {
                    return;
                }
            }
            var ot = request.Time.Date;
            var thisDayClassRecord = await _classRecordDAL.GetClassRecord(ot);
            if (thisDayClassRecord == null || thisDayClassRecord.Count == 0)
            {
                await _teacherSalaryClassDAL.DelTeacherSalaryClassDay(ot);
                return;
            }

            var entitys = new List<EtTeacherSalaryClassDay>();
            decimal studentClassTimes;
            decimal deSum = 0M;
            int arrivedAndBeLateCount;
            int arrivedCount;
            int beLateCount;
            int leaveCount;
            int notArrivedCount;
            int tryCalssStudentCount;
            int makeUpStudentCount;
            decimal teacherClassTimes;
            int tryCalssEffectiveCount;
            int makeUpEffectiveCount;
            foreach (var myClassRecord in thisDayClassRecord)
            {
                studentClassTimes = 0M;
                deSum = 0M;
                arrivedAndBeLateCount = 0;
                arrivedCount = 0;
                beLateCount = 0;
                leaveCount = 0;
                notArrivedCount = 0;
                tryCalssStudentCount = 0;
                makeUpStudentCount = 0;
                tryCalssEffectiveCount = 0;
                makeUpEffectiveCount = 0;
                teacherClassTimes = myClassRecord.ClassTimes;
                var myClassRecordStudent = await _classRecordDAL.GetClassRecordStudents(myClassRecord.Id);
                foreach (var p in myClassRecordStudent)
                {
                    studentClassTimes += p.DeClassTimes;
                    deSum += p.DeSum;
                    switch (p.StudentCheckStatus)
                    {
                        case EmClassStudentCheckStatus.Arrived:
                            arrivedAndBeLateCount++;
                            arrivedCount++;
                            break;
                        case EmClassStudentCheckStatus.BeLate:
                            arrivedAndBeLateCount++;
                            beLateCount++;
                            break;
                        case EmClassStudentCheckStatus.Leave:
                            leaveCount++;
                            break;
                        case EmClassStudentCheckStatus.NotArrived:
                            notArrivedCount++;
                            break;
                    }
                    if (p.StudentType == EmClassStudentType.TryCalssStudent)
                    {
                        tryCalssStudentCount++;
                        if (EmClassStudentCheckStatus.CheckIsAttend(p.StudentCheckStatus))
                        {
                            tryCalssEffectiveCount++;
                        }
                    }
                    if (p.StudentType == EmClassStudentType.MakeUpStudent)
                    {
                        makeUpStudentCount++;
                        if (EmClassStudentCheckStatus.CheckIsAttend(p.StudentCheckStatus))
                        {
                            makeUpEffectiveCount++;
                        }
                    }
                }
                var teacherIds = EtmsHelper.AnalyzeMuIds(myClassRecord.Teachers);
                if (teacherIds.Count == 1)  //大部分时候  都是一位老师
                {
                    var log = entitys.FirstOrDefault(p => p.TeacherId == teacherIds[0] && p.ClassId == myClassRecord.ClassId);
                    if (log == null)
                    {
                        entitys.Add(new EtTeacherSalaryClassDay()
                        {
                            ClassId = myClassRecord.ClassId,
                            TeacherId = teacherIds[0],
                            IsDeleted = EmIsDeleted.Normal,
                            Ot = myClassRecord.ClassOt,
                            TenantId = myClassRecord.TenantId,
                            ArrivedAndBeLateCount = arrivedAndBeLateCount,
                            ArrivedCount = arrivedCount,
                            BeLateCount = beLateCount,
                            DeSum = deSum,
                            LeaveCount = leaveCount,
                            MakeUpStudentCount = makeUpStudentCount,
                            NotArrivedCount = notArrivedCount,
                            StudentClassTimes = studentClassTimes,
                            TeacherClassTimes = teacherClassTimes,
                            TryCalssStudentCount = tryCalssStudentCount,
                            TryCalssEffectiveCount = tryCalssEffectiveCount,
                            MakeUpEffectiveCount = makeUpEffectiveCount
                        });
                    }
                    else
                    {
                        log.ArrivedAndBeLateCount += arrivedAndBeLateCount;
                        log.ArrivedCount += arrivedCount;
                        log.BeLateCount += beLateCount;
                        log.DeSum += deSum;
                        log.LeaveCount += leaveCount;
                        log.MakeUpStudentCount += makeUpStudentCount;
                        log.NotArrivedCount += notArrivedCount;
                        log.StudentClassTimes += studentClassTimes;
                        log.TeacherClassTimes += teacherClassTimes;
                        log.TryCalssStudentCount += tryCalssStudentCount;
                        log.TryCalssEffectiveCount += tryCalssEffectiveCount;
                        log.MakeUpEffectiveCount += makeUpEffectiveCount;
                    }
                }
                else
                {
                    foreach (var teacherId in teacherIds)
                    {
                        var log = entitys.FirstOrDefault(p => p.TeacherId == teacherId && p.ClassId == myClassRecord.ClassId);
                        if (log == null)
                        {
                            entitys.Add(new EtTeacherSalaryClassDay()
                            {
                                ClassId = myClassRecord.ClassId,
                                TeacherId = teacherId,
                                IsDeleted = EmIsDeleted.Normal,
                                Ot = myClassRecord.ClassOt,
                                TenantId = myClassRecord.TenantId,
                                ArrivedAndBeLateCount = arrivedAndBeLateCount,
                                ArrivedCount = arrivedCount,
                                BeLateCount = beLateCount,
                                DeSum = deSum,
                                LeaveCount = leaveCount,
                                MakeUpStudentCount = makeUpStudentCount,
                                NotArrivedCount = notArrivedCount,
                                StudentClassTimes = studentClassTimes,
                                TeacherClassTimes = teacherClassTimes,
                                TryCalssStudentCount = tryCalssStudentCount,
                                TryCalssEffectiveCount = tryCalssEffectiveCount,
                                MakeUpEffectiveCount = makeUpEffectiveCount
                            });
                        }
                        else
                        {
                            log.ArrivedAndBeLateCount += arrivedAndBeLateCount;
                            log.ArrivedCount += arrivedCount;
                            log.BeLateCount += beLateCount;
                            log.DeSum += deSum;
                            log.LeaveCount += leaveCount;
                            log.MakeUpStudentCount += makeUpStudentCount;
                            log.NotArrivedCount += notArrivedCount;
                            log.StudentClassTimes += studentClassTimes;
                            log.TeacherClassTimes += teacherClassTimes;
                            log.TryCalssStudentCount += tryCalssStudentCount;
                            log.TryCalssEffectiveCount += tryCalssEffectiveCount;
                            log.MakeUpEffectiveCount += makeUpEffectiveCount;
                        }
                    }
                }
            }

            await _teacherSalaryClassDAL.SaveTeacherSalaryClassDay(ot, entitys);
        }
    }
}
