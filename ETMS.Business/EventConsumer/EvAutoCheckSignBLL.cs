using ETMS.Business.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess;
using ETMS.IEventProvider;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EventConsumer
{
    public class EvAutoCheckSignBLL : IEvAutoCheckSignBLL
    {
        private readonly IClassTimesDAL _classTimesDAL;

        private readonly IClassRecordDAL _classRecordDAL;

        private readonly ITenantConfigDAL _tenantConfigDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IClassDAL _classDAL;

        private readonly IStudentCheckOnLogDAL _studentCheckOnLogDAL;

        private readonly IUserDAL _userDAL;

        private readonly ICourseDAL _courseDAL;

        public EvAutoCheckSignBLL(IClassTimesDAL classTimesDAL, IClassRecordDAL classRecordDAL, ITenantConfigDAL tenantConfigDAL,
            IEventPublisher eventPublisher, IClassDAL classDAL, IStudentCheckOnLogDAL studentCheckOnLogDAL, IUserDAL userDAL,
            ICourseDAL courseDAL)
        {
            this._classTimesDAL = classTimesDAL;
            this._classRecordDAL = classRecordDAL;
            this._tenantConfigDAL = tenantConfigDAL;
            this._eventPublisher = eventPublisher;
            this._classDAL = classDAL;
            this._studentCheckOnLogDAL = studentCheckOnLogDAL;
            this._userDAL = userDAL;
            this._courseDAL = courseDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _classTimesDAL, _classRecordDAL, _tenantConfigDAL, _classDAL,
                _studentCheckOnLogDAL, _userDAL, _courseDAL);
        }

        public async Task AutoCheckSignTenantConsumerEvent(AutoCheckSignTenantEvent request)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            if (!config.TenantOtherConfig.IsAutoCheckSign)
            {
                return;
            }
            var now = DateTime.Now;
            var minEndDateTime = now.AddMinutes(-config.TenantOtherConfig.AutoCheckSignLimitMinute);
            if (now.Date != minEndDateTime.Date)
            {
                return;
            }
            var minEndTimeVaule = EtmsHelper.GetTimeHourAndMinuteDesc(minEndDateTime);
            var unRollcallAndTimeOutData = await _classTimesDAL.GetUnRollcallAndTimeOut(now, minEndTimeVaule);
            if (unRollcallAndTimeOutData.Any())
            {
                foreach (var p in unRollcallAndTimeOutData)
                {
                    _eventPublisher.Publish(new AutoCheckSignClassTimesEvent(request.TenantId)
                    {
                        ClassTimesId = p.Id,
                        MakeupIsDeClassTimes = config.ClassCheckSignConfig.MakeupIsDeClassTimes
                    });
                }
            }
        }

        /// <summary>
        /// 模拟点名
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task AutoCheckSignClassTimesConsumerEvent(AutoCheckSignClassTimesEvent request)
        {
            var myClassTimes = await _classTimesDAL.GetClassTimes(request.ClassTimesId);
            if (myClassTimes.DataType == EmClassTimesDataType.Stop)
            {
                return;
            }
            if (myClassTimes.Status == EmClassTimesStatus.BeRollcall)
            {
                return;
            }
            //防止重复点名
            var isRepeatData = await _classRecordDAL.ExistClassRecord(myClassTimes.ClassId, myClassTimes.ClassOt, myClassTimes.StartTime, myClassTimes.EndTime);
            if (isRepeatData)
            {
                return;
            }
            var etClassBucket = await _classDAL.GetClassBucket(myClassTimes.ClassId);
            if (etClassBucket == null || etClassBucket.EtClass == null)
            {
                return;
            }
            var myClass = etClassBucket.EtClass;
            var myClassStudent = etClassBucket.EtClassStudents;
            var classTimesStudent = await _classTimesDAL.GetClassTimesStudent(request.ClassTimesId);
            if ((myClassStudent == null || myClassStudent.Count == 0) && (classTimesStudent == null || classTimesStudent.Count == 0))
            {
                return;
            }

            var checkUser = await _userDAL.GetAdminUser();
            var classOt = myClassTimes.ClassOt.Date;
            var checkOt = DateTime.Now;
            var week = (byte)classOt.DayOfWeek;
            var courseIds = myClassTimes.CourseList;
            if (string.IsNullOrEmpty(courseIds))
            {
                courseIds = myClass.CourseList;
            }
            var classRoomIds = myClassTimes.ClassRoomIds;
            if (string.IsNullOrEmpty(classRoomIds))
            {
                classRoomIds = myClass.ClassRoomIds;
            }
            var teachers = myClassTimes.Teachers;
            if (string.IsNullOrEmpty(teachers))
            {
                teachers = myClass.Teachers;
            }
            var teacherIds = EtmsHelper.AnalyzeMuIds(teachers);
            var teacherNum = teacherIds.Count;

            var classRecordStudents = new List<EtClassRecordStudent>();
            var tempBoxCouese = new DataTempBox<EtCourse>();
            if (myClassStudent != null && myClassStudent.Any())
            {
                foreach (var student in myClassStudent)
                {
                    var myCourse = await ComBusiness.GetCourse(tempBoxCouese, _courseDAL, student.CourseId);
                    if (myCourse == null)
                    {
                        continue;
                    }
                    var myClassRecordStudent = new EtClassRecordStudent()
                    {
                        CheckOt = checkOt,
                        CheckUserId = checkUser.Id,
                        ClassContent = myClassTimes.ClassContent,
                        ClassId = myClassTimes.ClassId,
                        ClassOt = classOt,
                        ClassRoomIds = classRoomIds,
                        CourseId = student.CourseId,
                        DeClassTimes = myClass.DefaultClassTimes,
                        EvaluateTeacherNum = 0,
                        DeSum = 0,
                        StartTime = myClassTimes.StartTime,
                        EndTime = myClassTimes.EndTime,
                        StudentType = EmClassStudentType.ClassStudent,
                        Remark = string.Empty,
                        ExceedClassTimes = 0,
                        ClassRecordId = 0,
                        DeType = EmDeClassTimesType.NotDe,
                        IsBeEvaluate = false,
                        IsDeleted = EmIsDeleted.Normal,
                        IsRewardPoints = myCourse.CheckPoints > 0,
                        RewardPoints = myCourse.CheckPoints,
                        Status = EmClassRecordStatus.Normal,
                        StudentCheckStatus = EmClassStudentCheckStatus.Arrived,
                        StudentId = student.StudentId,
                        StudentTryCalssLogId = null,
                        TeacherNum = teacherNum,
                        Teachers = teachers,
                        TenantId = request.TenantId,
                        Week = week
                    };
                    classRecordStudents.Add(myClassRecordStudent);
                }
            }
            if (classTimesStudent != null && classTimesStudent.Any())
            {
                foreach (var student in classTimesStudent)
                {
                    var hisLog = classRecordStudents.FirstOrDefault(j => j.StudentId == student.StudentId);
                    if (hisLog != null)
                    {
                        continue;
                    }
                    var myCourse = await ComBusiness.GetCourse(tempBoxCouese, _courseDAL, student.CourseId);
                    if (myCourse == null)
                    {
                        continue;
                    }
                    var deClassTimes = myClass.DefaultClassTimes;
                    switch (student.StudentType)
                    {
                        case EmClassStudentType.ClassStudent:
                            break;
                        case EmClassStudentType.TempStudent:
                            break;
                        case EmClassStudentType.TryCalssStudent:
                            deClassTimes = 0;
                            break;
                        case EmClassStudentType.MakeUpStudent:
                            if (!request.MakeupIsDeClassTimes)
                            {
                                deClassTimes = 0;
                            }
                            break;
                    }
                    var myClassRecordStudent = new EtClassRecordStudent()
                    {
                        CheckOt = checkOt,
                        CheckUserId = checkUser.Id,
                        ClassContent = myClassTimes.ClassContent,
                        ClassId = myClassTimes.ClassId,
                        ClassOt = classOt,
                        ClassRoomIds = classRoomIds,
                        CourseId = student.CourseId,
                        DeClassTimes = deClassTimes,
                        EvaluateTeacherNum = 0,
                        DeSum = 0,
                        StartTime = myClassTimes.StartTime,
                        EndTime = myClassTimes.EndTime,
                        StudentType = student.StudentType,
                        Remark = string.Empty,
                        ExceedClassTimes = 0,
                        ClassRecordId = 0,
                        DeType = EmDeClassTimesType.NotDe,
                        IsBeEvaluate = false,
                        IsDeleted = EmIsDeleted.Normal,
                        IsRewardPoints = myCourse.CheckPoints > 0,
                        RewardPoints = myCourse.CheckPoints,
                        Status = EmClassRecordStatus.Normal,
                        StudentCheckStatus = EmClassStudentCheckStatus.Arrived,
                        StudentId = student.StudentId,
                        StudentTryCalssLogId = student.StudentTryCalssLogId,
                        TeacherNum = teacherNum,
                        Teachers = teachers,
                        TenantId = request.TenantId,
                        Week = week
                    };
                    classRecordStudents.Add(myClassRecordStudent);
                }
            }

            var classRecord = new EtClassRecord()
            {
                AttendNumber = classRecordStudents.Count,
                CheckOt = checkOt,
                CheckUserId = checkUser.Id,
                ClassContent = myClassTimes.ClassContent,
                ClassId = myClassTimes.ClassId,
                ClassOt = classOt,
                ClassRoomIds = classRoomIds,
                ClassTimes = myClass.DefaultClassTimes,
                ClassTimesId = myClassTimes.Id,
                CourseList = courseIds,
                DeSum = 0,
                EndTime = myClassTimes.EndTime,
                IsDeleted = EmIsDeleted.Normal,
                NeedAttendNumber = classRecordStudents.Count,
                Remark = "自动点名",
                StartTime = myClassTimes.StartTime,
                Status = EmClassRecordStatus.Normal,
                TeacherNum = teacherNum,
                Teachers = teachers,
                TenantId = request.TenantId,
                Week = week,
                StudentIds = EtmsHelper.GetMuIds(classRecordStudents.Select(p => p.StudentId)),
                EvaluateStudentCount = 0,
                ClassCategoryId = myClass.ClassCategoryId
            };

            _eventPublisher.Publish(new ClassCheckSignEvent(request.TenantId)
            {
                ClassRecord = classRecord,
                ClassRecordStudents = classRecordStudents,
                EvaluateStudents = new List<EtClassRecordEvaluateStudent>(),
                ClassName = etClassBucket.EtClass.Name,
                UserId = classRecord.CheckUserId,
                LoginClientType = request.LoginClientType
            });
        }
    }
}
