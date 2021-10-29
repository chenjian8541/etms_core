using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess;
using ETMS.IDataAccess.MallGoodsDAL;
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

        private readonly ITeacherSalaryMonthStatisticsDAL _teacherSalaryMonthStatisticsDAL;

        private readonly ITeacherSalaryPayrollDAL _teacherSalaryPayrollDAL;

        private readonly IUserDAL _userDAL;

        private readonly IMallGoodsDAL _mallGoodsDAL;

        public EvEducationBLL(IClassRecordDAL classRecordDAL, ITeacherSalaryClassDAL teacherSalaryClassDAL,
            ITeacherSalaryMonthStatisticsDAL teacherSalaryMonthStatisticsDAL, ITeacherSalaryPayrollDAL teacherSalaryPayrollDAL,
            IUserDAL userDAL, IMallGoodsDAL mallGoodsDAL)
        {
            this._classRecordDAL = classRecordDAL;
            this._teacherSalaryClassDAL = teacherSalaryClassDAL;
            this._teacherSalaryMonthStatisticsDAL = teacherSalaryMonthStatisticsDAL;
            this._teacherSalaryPayrollDAL = teacherSalaryPayrollDAL;
            this._userDAL = userDAL;
            this._mallGoodsDAL = mallGoodsDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _classRecordDAL, _teacherSalaryClassDAL, _teacherSalaryMonthStatisticsDAL,
                _teacherSalaryPayrollDAL, _userDAL, _mallGoodsDAL);
        }

        public async Task StatisticsTeacherSalaryClassTimesConsumerEvent(StatisticsTeacherSalaryClassTimesEvent request)
        {
            var classRecord = await _classRecordDAL.GetClassRecord(request.ClassRecordId);
            if (classRecord.Status == EmClassRecordStatus.Revoked)
            {
                await _teacherSalaryClassDAL.DelTeacherSalaryClassTimes(request.ClassRecordId);
                await _teacherSalaryClassDAL.DelTeacherSalaryClassTimes2(request.ClassRecordId);
                return;
            }
            var classRecordStudent = await _classRecordDAL.GetClassRecordStudents(request.ClassRecordId);
            if (classRecordStudent == null || classRecordStudent.Count == 0)
            {
                await _teacherSalaryClassDAL.DelTeacherSalaryClassTimes(request.ClassRecordId);
                await _teacherSalaryClassDAL.DelTeacherSalaryClassTimes2(request.ClassRecordId);
                return;
            }
            var statisticsViewOneCourse = GetStatisticsTeacherSalaryClassTimesOneCourse(classRecord, classRecordStudent);
            await StatisticsTeacherSalaryClassTimesConsumerEvent2(classRecord, statisticsViewOneCourse);
            await StatisticsTeacherSalaryClassTimesConsumerEvent1(classRecord, classRecordStudent, statisticsViewOneCourse);
        }

        private List<EtTeacherSalaryClassTimes> _tempEntitysList;

        /// <summary>
        /// 按班级和课程分析
        /// </summary>
        /// <param name="classRecord"></param>
        /// <param name="classRecordStudent"></param>
        /// <param name="statisticsView"></param>
        /// <returns></returns>
        public async Task StatisticsTeacherSalaryClassTimesConsumerEvent1(EtClassRecord classRecord,
            List<EtClassRecordStudent> classRecordStudent, TempStatisticsTeacherSalaryClassTimesOneCourse statisticsView)
        {
            _tempEntitysList = new List<EtTeacherSalaryClassTimes>();
            var ot = classRecord.ClassOt.Date;
            var teacherIds = EtmsHelper.AnalyzeMuIds(classRecord.Teachers);
            var myCourseIds = classRecordStudent.GroupBy(p => p.CourseId).Select(p => p.Key).ToList();
            if (myCourseIds.Count == 1)
            {
                AddStatisticsTeacherSalaryClassTimesConsumerEvent1(classRecord, ot, teacherIds, myCourseIds[0], statisticsView);
            }
            else
            {
                //处理多门课程的情况
                foreach (var myCourseId in myCourseIds)
                {
                    var myClassRecordStudent = classRecordStudent.Where(p => p.CourseId == myCourseId).ToList();
                    if (myClassRecordStudent.Any())
                    {
                        var myStatisticsView = GetStatisticsTeacherSalaryClassTimesOneCourse(classRecord, myClassRecordStudent);
                        AddStatisticsTeacherSalaryClassTimesConsumerEvent1(classRecord, ot, teacherIds, myCourseId, myStatisticsView);
                    }
                }
            }
            if (_tempEntitysList.Any())
            {
                await _teacherSalaryClassDAL.SaveTeacherSalaryClassTimes(classRecord.Id, _tempEntitysList);
            }
        }

        private void AddStatisticsTeacherSalaryClassTimesConsumerEvent1(EtClassRecord classRecord, DateTime ot,
            List<long> teacherIds, long myCourseId, TempStatisticsTeacherSalaryClassTimesOneCourse statisticsView)
        {
            if (teacherIds.Count == 1)
            {
                _tempEntitysList.Add(new EtTeacherSalaryClassTimes()
                {
                    TeacherId = teacherIds[0],
                    CourseId = myCourseId,
                    ArrivedCount = statisticsView.ArrivedCount,
                    ArrivedAndBeLateCount = statisticsView.ArrivedAndBeLateCount,
                    BeLateCount = statisticsView.BeLateCount,
                    ClassId = classRecord.ClassId,
                    ClassRecordId = classRecord.Id,
                    IsDeleted = EmIsDeleted.Normal,
                    DeSum = statisticsView.DeSum,
                    LeaveCount = statisticsView.LeaveCount,
                    MakeUpStudentCount = statisticsView.MakeUpStudentCount,
                    NotArrivedCount = statisticsView.NotArrivedCount,
                    Ot = ot,
                    StudentClassTimes = statisticsView.StudentClassTimes,
                    TeacherClassTimes = classRecord.ClassTimes,
                    TenantId = classRecord.TenantId,
                    TryCalssStudentCount = statisticsView.TryCalssStudentCount,
                    TryCalssEffectiveCount = statisticsView.TryCalssEffectiveCount,
                    MakeUpEffectiveCount = statisticsView.MakeUpEffectiveCount,
                    EndTime = classRecord.EndTime,
                    StartTime = classRecord.StartTime,
                    Week = classRecord.Week
                });
            }
            else
            {
                foreach (var teacherId in teacherIds)
                {
                    _tempEntitysList.Add(new EtTeacherSalaryClassTimes()
                    {
                        TeacherId = teacherId,
                        CourseId = myCourseId,
                        ArrivedCount = statisticsView.ArrivedCount,
                        ArrivedAndBeLateCount = statisticsView.ArrivedAndBeLateCount,
                        BeLateCount = statisticsView.BeLateCount,
                        ClassId = classRecord.ClassId,
                        ClassRecordId = classRecord.Id,
                        IsDeleted = EmIsDeleted.Normal,
                        DeSum = statisticsView.DeSum,
                        LeaveCount = statisticsView.LeaveCount,
                        MakeUpStudentCount = statisticsView.MakeUpStudentCount,
                        NotArrivedCount = statisticsView.NotArrivedCount,
                        Ot = ot,
                        StudentClassTimes = statisticsView.StudentClassTimes,
                        TeacherClassTimes = classRecord.ClassTimes,
                        TenantId = classRecord.TenantId,
                        TryCalssStudentCount = statisticsView.TryCalssStudentCount,
                        TryCalssEffectiveCount = statisticsView.TryCalssEffectiveCount,
                        MakeUpEffectiveCount = statisticsView.MakeUpEffectiveCount,
                        EndTime = classRecord.EndTime,
                        StartTime = classRecord.StartTime,
                        Week = classRecord.Week
                    });
                }
            }
        }

        /// <summary>
        /// 按班级分析
        /// </summary>
        /// <param name="classRecord"></param>
        /// <param name="statisticsView"></param>
        /// <returns></returns>
        public async Task StatisticsTeacherSalaryClassTimesConsumerEvent2(EtClassRecord classRecord, TempStatisticsTeacherSalaryClassTimesOneCourse statisticsView)
        {
            var entitys = new List<EtTeacherSalaryClassTimes2>();
            var ot = classRecord.ClassOt.Date;
            var teacherIds = EtmsHelper.AnalyzeMuIds(classRecord.Teachers);
            if (teacherIds.Count == 1)
            {
                entitys.Add(new EtTeacherSalaryClassTimes2()
                {
                    TeacherId = teacherIds[0],
                    ArrivedCount = statisticsView.ArrivedCount,
                    ArrivedAndBeLateCount = statisticsView.ArrivedAndBeLateCount,
                    BeLateCount = statisticsView.BeLateCount,
                    ClassId = classRecord.ClassId,
                    ClassRecordId = classRecord.Id,
                    IsDeleted = EmIsDeleted.Normal,
                    DeSum = statisticsView.DeSum,
                    LeaveCount = statisticsView.LeaveCount,
                    MakeUpStudentCount = statisticsView.MakeUpStudentCount,
                    NotArrivedCount = statisticsView.NotArrivedCount,
                    Ot = ot,
                    StudentClassTimes = statisticsView.StudentClassTimes,
                    TeacherClassTimes = classRecord.ClassTimes,
                    TenantId = classRecord.TenantId,
                    TryCalssStudentCount = statisticsView.TryCalssStudentCount,
                    TryCalssEffectiveCount = statisticsView.TryCalssEffectiveCount,
                    MakeUpEffectiveCount = statisticsView.MakeUpEffectiveCount,
                    EndTime = classRecord.EndTime,
                    StartTime = classRecord.StartTime,
                    Week = classRecord.Week
                });
            }
            else
            {
                foreach (var teacherId in teacherIds)
                {
                    entitys.Add(new EtTeacherSalaryClassTimes2()
                    {
                        TeacherId = teacherId,
                        ArrivedCount = statisticsView.ArrivedCount,
                        ArrivedAndBeLateCount = statisticsView.ArrivedAndBeLateCount,
                        BeLateCount = statisticsView.BeLateCount,
                        ClassId = classRecord.ClassId,
                        ClassRecordId = classRecord.Id,
                        IsDeleted = EmIsDeleted.Normal,
                        DeSum = statisticsView.DeSum,
                        LeaveCount = statisticsView.LeaveCount,
                        MakeUpStudentCount = statisticsView.MakeUpStudentCount,
                        NotArrivedCount = statisticsView.NotArrivedCount,
                        Ot = ot,
                        StudentClassTimes = statisticsView.StudentClassTimes,
                        TeacherClassTimes = classRecord.ClassTimes,
                        TenantId = classRecord.TenantId,
                        TryCalssStudentCount = statisticsView.TryCalssStudentCount,
                        TryCalssEffectiveCount = statisticsView.TryCalssEffectiveCount,
                        MakeUpEffectiveCount = statisticsView.MakeUpEffectiveCount,
                        EndTime = classRecord.EndTime,
                        StartTime = classRecord.StartTime,
                        Week = classRecord.Week
                    });
                }
            }

            await _teacherSalaryClassDAL.SaveTeacherSalaryClassTimes2(classRecord.Id, entitys);
        }

        public TempStatisticsTeacherSalaryClassTimesOneCourse GetStatisticsTeacherSalaryClassTimesOneCourse(EtClassRecord classRecord, List<EtClassRecordStudent> classRecordStudent)
        {
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
                studentClassTimes += p.DeClassTimes + p.ExceedClassTimes;
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
            return new TempStatisticsTeacherSalaryClassTimesOneCourse()
            {
                ArrivedAndBeLateCount = arrivedAndBeLateCount,
                ArrivedCount = arrivedCount,
                BeLateCount = beLateCount,
                DeSum = deSum,
                LeaveCount = leaveCount,
                MakeUpEffectiveCount = makeUpEffectiveCount,
                MakeUpStudentCount = makeUpStudentCount,
                NotArrivedCount = notArrivedCount,
                StudentClassTimes = studentClassTimes,
                TryCalssEffectiveCount = tryCalssEffectiveCount,
                TryCalssStudentCount = tryCalssStudentCount
            };
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
                    studentClassTimes += p.DeClassTimes + p.ExceedClassTimes;
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

        public async Task StatisticsTeacherSalaryMonthConsumerEvent(StatisticsTeacherSalaryMonthEvent request)
        {
            var salaryPayrollBucket = await _teacherSalaryPayrollDAL.GetTeacherSalaryPayrollBucket(request.PayrollId);
            if (salaryPayrollBucket == null || salaryPayrollBucket.TeacherSalaryPayroll == null)
            {
                return;
            }
            var ot = salaryPayrollBucket.TeacherSalaryPayroll.PayDate.Value;
            var year = ot.Year;
            var month = ot.Month;

            foreach (var myItem in salaryPayrollBucket.TeacherSalaryPayrollUsers)
            {
                await _teacherSalaryMonthStatisticsDAL.UpdateTeacherSalaryMonthStatistics(myItem.UserId, year, month);
            }
        }

        public async Task SyncTeacherMonthClassTimesConsumerEvent(SyncTeacherMonthClassTimesEvent request)
        {
            if (request.YearAndMonthList == null || request.YearAndMonthList.Count == 0)
            {
                return;
            }
            if (request.TeacherIds == null || request.TeacherIds.Count == 0)
            {
                return;
            }
            decimal newClassTimes;
            int newClassCount;
            foreach (var p in request.YearAndMonthList)
            {
                var startDate = new DateTime(p.Year, p.Month, 1);
                var endDate = startDate.AddMonths(1);
                foreach (var myTeacherId in request.TeacherIds)
                {
                    var myStatistics = await _classRecordDAL.GetClassRecordTeacherStatistics(myTeacherId, startDate, endDate);
                    newClassTimes = 0;
                    newClassCount = 0;
                    if (myStatistics != null)
                    {
                        newClassTimes = myStatistics.TotalClassTimes;
                        newClassCount = myStatistics.TotalCount;
                    }
                    await _userDAL.UpdateTeacherMonthClassTimes(myTeacherId, startDate, newClassTimes, newClassCount);
                }
            }
            foreach (var myTeacherId in request.TeacherIds)
            {
                await _userDAL.UpdateTeacherClassTimes(myTeacherId);
            }
        }

        public async Task SyncMallGoodsRelatedNameConsumerEvent(SyncMallGoodsRelatedNameEvent request)
        {
            await _mallGoodsDAL.UpdateRelatedName(request.ProductType, request.RelatedId, request.NewName);
        }
    }
}
