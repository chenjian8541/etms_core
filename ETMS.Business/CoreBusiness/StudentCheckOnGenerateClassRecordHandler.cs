using ETMS.Business.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
using ETMS.IDataAccess;
using ETMS.IEventProvider;
using ETMS.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class StudentCheckOnGenerateClassRecordHandler
    {
        private readonly IClassDAL _classDAL;

        private readonly IClassTimesDAL _classTimesDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly IStudentCheckOnLogDAL _studentCheckOnLogDAL;

        private readonly IClassRecordDAL _classRecordDAL;

        private readonly IStudentCourseConsumeLogDAL _studentCourseConsumeLogDAL;

        private readonly ITempStudentNeedCheckDAL _tempStudentNeedCheckDAL;

        private readonly ITryCalssLogDAL _tryCalssLogDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IStudentDAL _studentDAL;

        private readonly IStudentTrackLogDAL _studentTrackLogDAL;

        private readonly IUserDAL _userDAL;

        private int _tenantId;

        private long _classTimesId;

        private bool _makeupIsDeClassTimes;

        private bool _isNotComeCharge;

        private string _teachers;

        private int _teacherNums;

        private decimal _totalDeSum;

        private long _checkUserId;

        private decimal _defaultClassTimes;

        private bool _tryCalssNoticeTrackUser;

        private DateTime _checkOt;

        private EtClassTimes _myClassTimes;

        private List<EtStudentCheckOnLog> _checkInLog;

        private List<EtClassRecordStudent> _classRecordStudents;

        private List<EtStudentCourseConsumeLog> _studentCourseConsumeLogs;

        private readonly IClassTimesRuleStudentDAL _classTimesRuleStudentDAL;

        public StudentCheckOnGenerateClassRecordHandler(IClassDAL classDAL, IClassTimesDAL classTimesDAL, ICourseDAL courseDAL,
             IStudentCourseDAL studentCourseDAL, IStudentCheckOnLogDAL studentCheckOnLogDAL, IClassRecordDAL classRecordDAL,
             IStudentCourseConsumeLogDAL studentCourseConsumeLogDAL, ITempStudentNeedCheckDAL tempStudentNeedCheckDAL,
             ITryCalssLogDAL tryCalssLogDAL, IEventPublisher eventPublisher, IStudentDAL studentDAL, IStudentTrackLogDAL studentTrackLogDAL,
             IUserDAL userDAL, IClassTimesRuleStudentDAL classTimesRuleStudentDAL)
        {
            this._classDAL = classDAL;
            this._classTimesDAL = classTimesDAL;
            this._courseDAL = courseDAL;
            this._studentCourseDAL = studentCourseDAL;
            this._studentCheckOnLogDAL = studentCheckOnLogDAL;
            this._classRecordDAL = classRecordDAL;
            this._studentCourseConsumeLogDAL = studentCourseConsumeLogDAL;
            this._tempStudentNeedCheckDAL = tempStudentNeedCheckDAL;
            this._tryCalssLogDAL = tryCalssLogDAL;
            this._eventPublisher = eventPublisher;
            this._studentDAL = studentDAL;
            this._studentTrackLogDAL = studentTrackLogDAL;
            this._userDAL = userDAL;
            this._classTimesRuleStudentDAL = classTimesRuleStudentDAL;
        }

        private EtClassRecordStudent InitAnClassRecordStudent(EtClassTimes myClassTimes, long checkUserId, string teachers, int teacherNum)
        {
            return new EtClassRecordStudent()
            {
                ClassContent = myClassTimes.ClassContent,
                Remark = string.Empty,
                CheckOt = _checkOt,
                ClassOt = myClassTimes.ClassOt,
                CheckUserId = checkUserId,
                StartTime = myClassTimes.StartTime,
                EndTime = myClassTimes.EndTime,
                ClassId = myClassTimes.ClassId,
                ClassRoomIds = myClassTimes.ClassRoomIds,
                ClassRecordId = 0,
                EvaluateCount = 0,
                EvaluateReadCount = 0,
                EvaluateTeacherNum = 0,
                IsBeEvaluate = false,
                IsDeleted = EmIsDeleted.Normal,
                IsRewardPoints = false,
                Teachers = teachers,
                RewardPoints = 0,
                StudentTryCalssLogId = null,
                IsTeacherEvaluate = EmBool.False,
                TenantId = myClassTimes.TenantId,
                Status = EmClassRecordStatus.Normal,
                Week = myClassTimes.Week,
                TeacherNum = teacherNum,
            };
        }

        private void AddClassRecordStudentIsCheckIn(long courseId, long studentId, byte studentType, EtStudentCheckOnLog myCheckInLog)
        {
            var repeatClassStudent = _classRecordStudents.FirstOrDefault(p => p.StudentId == studentId);
            if (repeatClassStudent != null)
            {
                LOG.Log.Error($"[StudentCheckOnGenerateClassRecordHandler]课次出现重复学员,tenantId:{_tenantId},classTimesId:{_classTimesId},studentId:{studentId}", this.GetType());
                return;
            }
            var myClassRecordStudent = InitAnClassRecordStudent(_myClassTimes, _checkUserId, _teachers, _teacherNums);
            myClassRecordStudent.CourseId = courseId;
            myClassRecordStudent.StudentId = studentId;
            myClassRecordStudent.StudentType = studentType;

            myClassRecordStudent.DeClassTimes = myCheckInLog.DeClassTimes;
            myClassRecordStudent.DeStudentCourseDetailId = myCheckInLog.DeStudentCourseDetailId;
            myClassRecordStudent.DeSum = myCheckInLog.DeSum;
            myClassRecordStudent.DeType = myCheckInLog.DeType;
            myClassRecordStudent.ExceedClassTimes = myCheckInLog.ExceedClassTimes;
            myClassRecordStudent.StudentCheckStatus = EmClassStudentCheckStatus.Arrived;
            if (myCheckInLog.Points > 0)
            {
                myClassRecordStudent.IsRewardPoints = true;
                myClassRecordStudent.RewardPoints = myCheckInLog.Points;
            }

            _totalDeSum += myClassRecordStudent.DeSum;
            _classRecordStudents.Add(myClassRecordStudent);
        }

        private async Task AddClassRecordStudentNotComeChargeComStudent(long courseId, long studentId, byte studentType)
        {
            var repeatClassStudent = _classRecordStudents.FirstOrDefault(p => p.StudentId == studentId);
            if (repeatClassStudent != null)
            {
                LOG.Log.Error($"[StudentCheckOnGenerateClassRecordHandler]课次出现重复学员,tenantId:{_tenantId},classTimesId:{_classTimesId},studentId:{studentId}", this.GetType());
                return;
            }
            var myClassRecordStudent = InitAnClassRecordStudent(_myClassTimes, _checkUserId, _teachers, _teacherNums);
            myClassRecordStudent.CourseId = courseId;
            myClassRecordStudent.StudentId = studentId;
            myClassRecordStudent.StudentType = studentType;

            var deStudentClassTimesResult = DeStudentClassTimesResult.GetNotDeEntity();
            if (_isNotComeCharge)
            {
                deStudentClassTimesResult = await CoreBusiness.DeStudentClassTimes(_studentCourseDAL, new DeStudentClassTimesTempRequest()
                {
                    ClassOt = _myClassTimes.ClassOt,
                    CourseId = courseId,
                    DeClassTimes = _defaultClassTimes,
                    StudentId = studentId,
                    TenantId = _tenantId
                });

                if (deStudentClassTimesResult.DeType != EmDeClassTimesType.NotDe)
                {
                    _studentCourseConsumeLogs.Add(new EtStudentCourseConsumeLog()
                    {
                        CourseId = courseId,
                        DeClassTimes = deStudentClassTimesResult.DeClassTimes,
                        DeType = deStudentClassTimesResult.DeType,
                        IsDeleted = EmIsDeleted.Normal,
                        OrderId = deStudentClassTimesResult.OrderId,
                        OrderNo = deStudentClassTimesResult.OrderNo,
                        Ot = _checkOt,
                        SourceType = EmStudentCourseConsumeSourceType.ClassCheckSign,
                        StudentId = myClassRecordStudent.StudentId,
                        TenantId = myClassRecordStudent.TenantId,
                        DeClassTimesSmall = 0
                    });
                }
            }
            myClassRecordStudent.StudentCheckStatus = EmClassStudentCheckStatus.NotArrived;
            myClassRecordStudent.DeSum = deStudentClassTimesResult.DeSum;
            myClassRecordStudent.DeType = deStudentClassTimesResult.DeType;
            myClassRecordStudent.ExceedClassTimes = deStudentClassTimesResult.ExceedClassTimes;
            if (deStudentClassTimesResult.DeType != EmDeClassTimesType.ClassTimes)
            {
                myClassRecordStudent.DeClassTimes = 0;
            }
            else
            {
                myClassRecordStudent.DeClassTimes = deStudentClassTimesResult.DeClassTimes;
            }
            myClassRecordStudent.DeStudentCourseDetailId = deStudentClassTimesResult.DeStudentCourseDetailId;

            _totalDeSum += myClassRecordStudent.DeSum;
            _classRecordStudents.Add(myClassRecordStudent);
        }

        private void AddClassRecordStudentNotDeClassTimes(long courseId, long studentId, byte studentType, long? studentTryCalssLogId = null)
        {
            var myClassRecordStudent = InitAnClassRecordStudent(_myClassTimes, _checkUserId, _teachers, _teacherNums);
            myClassRecordStudent.CourseId = courseId;
            myClassRecordStudent.StudentId = studentId;
            myClassRecordStudent.StudentType = studentType;

            myClassRecordStudent.StudentCheckStatus = EmClassStudentCheckStatus.NotArrived;
            myClassRecordStudent.DeSum = 0;
            myClassRecordStudent.DeType = EmDeClassTimesType.NotDe;
            myClassRecordStudent.ExceedClassTimes = 0;
            myClassRecordStudent.DeClassTimes = 0;
            myClassRecordStudent.DeStudentCourseDetailId = null;
            myClassRecordStudent.StudentTryCalssLogId = studentTryCalssLogId;
            _classRecordStudents.Add(myClassRecordStudent);
        }

        public async Task Process(int tenantId, long classTimesId, DateTime date, long adminUserId, bool makeupIsDeClassTimes,
            bool tryCalssNoticeTrackUser)
        {
            this._tenantId = tenantId;
            this._classTimesId = classTimesId;
            this._makeupIsDeClassTimes = makeupIsDeClassTimes;
            this._tryCalssNoticeTrackUser = tryCalssNoticeTrackUser;
            this._checkOt = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);

            _myClassTimes = await _classTimesDAL.GetClassTimes(_classTimesId);
            if (_myClassTimes == null)
            {
                LOG.Log.Error($"[StudentCheckOnAutoGenerateClassRecordProcess]考勤课次不存在-tenantId:{_tenantId}-classTimesId:{_classTimesId}", this.GetType());
                return;
            }
            if (_myClassTimes.Status == EmClassTimesStatus.BeRollcall)
            {
                LOG.Log.Error($"[StudentCheckOnAutoGenerateClassRecordProcess]无法处理已点名的课次,{JsonConvert.SerializeObject(_myClassTimes)}", this.GetType());
                return;
            }
            _classRecordStudents = new List<EtClassRecordStudent>();
            _studentCourseConsumeLogs = new List<EtStudentCourseConsumeLog>();

            var etClassBucket = await _classDAL.GetClassBucket(_myClassTimes.ClassId);
            var isLeaveCharge = etClassBucket.EtClass.IsLeaveCharge;
            _isNotComeCharge = etClassBucket.EtClass.IsNotComeCharge;
            _defaultClassTimes = etClassBucket.EtClass.DefaultClassTimes;
            _totalDeSum = 0;
            _checkInLog = await _studentCheckOnLogDAL.GetStudentCheckOnLogByClassTimesId(classTimesId);

            _teachers = _myClassTimes.Teachers;
            if (string.IsNullOrEmpty(_teachers))
            {
                _teachers = etClassBucket.EtClass.Teachers;
            }
            var myTeacherIds = EtmsHelper.AnalyzeMuIds(_teachers);
            _teacherNums = myTeacherIds.Count;
            _checkUserId = adminUserId;
            if (myTeacherIds.Any())
            {
                _checkUserId = myTeacherIds[0];
            }

            var attendStudentIds = new List<long>();
            var classStudent = (await ComBusiness6.GetClassStudent(etClassBucket, _classTimesRuleStudentDAL, _myClassTimes.RuleId)).ToList();
            if (classStudent != null && classStudent.Any()) //班级学员
            {
                foreach (var myStudent in classStudent)
                {
                    var myCheckInLog = _checkInLog.FirstOrDefault(p => p.StudentId == myStudent.StudentId);
                    if (myCheckInLog != null) //已考勤（到课）
                    {
                        attendStudentIds.Add(myStudent.StudentId);
                        this.AddClassRecordStudentIsCheckIn(myStudent.CourseId, myStudent.StudentId, EmClassStudentType.ClassStudent, myCheckInLog);
                    }
                    else
                    {
                        await this.AddClassRecordStudentNotComeChargeComStudent(myStudent.CourseId, myStudent.StudentId, EmClassStudentType.ClassStudent);
                    }
                }
            }
            var classTimesStudent = await _classTimesDAL.GetClassTimesStudent(classTimesId);
            if (classTimesStudent != null && classTimesStudent.Any()) //课次临时学员
            {
                foreach (var myStudent in classTimesStudent)
                {
                    var myCheckInLog = _checkInLog.FirstOrDefault(p => p.StudentId == myStudent.StudentId);
                    if (myCheckInLog != null) //已考勤（到课）
                    {
                        attendStudentIds.Add(myStudent.StudentId);
                        this.AddClassRecordStudentIsCheckIn(myStudent.CourseId, myStudent.StudentId, myStudent.StudentType, myCheckInLog);
                    }
                    else
                    {
                        if (_isNotComeCharge) //未到扣课时
                        {
                            switch (myStudent.StudentType)
                            {
                                case EmClassStudentType.ClassStudent:
                                    await this.AddClassRecordStudentNotComeChargeComStudent(myStudent.CourseId, myStudent.StudentId, myStudent.StudentType);
                                    break;
                                case EmClassStudentType.TempStudent:
                                    await this.AddClassRecordStudentNotComeChargeComStudent(myStudent.CourseId, myStudent.StudentId, myStudent.StudentType);
                                    break;
                                case EmClassStudentType.MakeUpStudent:
                                    if (_makeupIsDeClassTimes)
                                    {
                                        await this.AddClassRecordStudentNotComeChargeComStudent(myStudent.CourseId, myStudent.StudentId, myStudent.StudentType);
                                    }
                                    else
                                    {
                                        this.AddClassRecordStudentNotDeClassTimes(myStudent.CourseId, myStudent.StudentId, myStudent.StudentType);
                                    }
                                    break;
                                case EmClassStudentType.TryCalssStudent:
                                    this.AddClassRecordStudentNotDeClassTimes(myStudent.CourseId, myStudent.StudentId, myStudent.StudentType, myStudent.StudentTryCalssLogId);
                                    break;
                            }
                        }
                        else
                        {
                            this.AddClassRecordStudentNotDeClassTimes(myStudent.CourseId, myStudent.StudentId, myStudent.StudentType);
                        }
                    }
                }
            }

            var attendStudent = _classRecordStudents.Where(p => p.StudentCheckStatus == EmClassStudentCheckStatus.Arrived ||
            p.StudentCheckStatus == EmClassStudentCheckStatus.BeLate);
            var classRecord = new EtClassRecord()
            {
                AttendNumber = attendStudent.Count(),
                CheckOt = _checkOt,
                CheckUserId = _checkUserId,
                ClassContent = _myClassTimes.ClassContent,
                ClassId = _myClassTimes.ClassId,
                ClassOt = _myClassTimes.ClassOt,
                ClassRoomIds = _myClassTimes.ClassRoomIds,
                ClassTimes = _defaultClassTimes,
                ClassTimesId = _myClassTimes.Id,
                CourseList = _myClassTimes.CourseList,
                DeSum = _totalDeSum,
                EndTime = _myClassTimes.EndTime,
                IsDeleted = EmIsDeleted.Normal,
                NeedAttendNumber = _classRecordStudents.Count,
                Remark = string.Empty,
                StartTime = _myClassTimes.StartTime,
                Status = EmClassRecordStatus.Normal,
                TeacherNum = _teacherNums,
                Teachers = _teachers,
                TenantId = _tenantId,
                Week = _myClassTimes.Week,
                StudentIds = EtmsHelper.GetMuIds(_classRecordStudents.Select(p => p.StudentId)),
                EvaluateStudentCount = 0,
                ClassCategoryId = etClassBucket.EtClass.ClassCategoryId
            };
            var recordId = await _classRecordDAL.AddEtClassRecord(classRecord, _classRecordStudents); //上课记录和详情
            if (_studentCourseConsumeLogs.Any())
            {
                _studentCourseConsumeLogDAL.AddStudentCourseConsumeLog(_studentCourseConsumeLogs); //课消记录
            }

            await _classTimesDAL.UpdateClassTimesIsClassCheckSign(classRecord.ClassTimesId.Value, recordId, EmClassTimesStatus.BeRollcall, classRecord);
            await _tempStudentNeedCheckDAL.TempStudentNeedCheckClassSetIsAttendClass(classRecord.ClassTimesId.Value);
            await _studentCheckOnLogDAL.UpdateStudentCheckOnIsBeRollcall(classRecord.ClassTimesId.Value);
            await _classTimesDAL.ClassTimesReservationLogEditStatusBuyClassCheck(classRecord.ClassTimesId.Value, attendStudentIds);

            var classRecordAbsenceLogs = new List<EtClassRecordAbsenceLog>();
            var studentTrackLogs = new List<EtStudentTrackLog>();
            foreach (var student in _classRecordStudents)
            {
                //缺勤记录
                ComBusiness3.ClassRecordAbsenceProcess(classRecordAbsenceLogs, student, recordId);

                if (student.StudentType == EmClassStudentType.TryCalssStudent && student.StudentTryCalssLogId != null)
                {
                    //试听处理
                    await ComBusiness3.TryCalssStudentProcess(_tryCalssLogDAL, _studentDAL, _eventPublisher,
                        studentTrackLogs, classRecord, student, recordId, _tryCalssNoticeTrackUser);
                }

                //学员课程详情
                _eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(student.TenantId)
                {
                    StudentId = student.StudentId,
                    CourseId = student.CourseId
                });
            }


            if (classRecordAbsenceLogs.Any())
            {
                _classRecordDAL.AddClassRecordAbsenceLog(classRecordAbsenceLogs);
            }
            if (studentTrackLogs.Any())
            {
                await _studentTrackLogDAL.AddStudentTrackLog(studentTrackLogs);
            }

            //老师上课统计
            var teachers = EtmsHelper.AnalyzeMuIds(classRecord.Teachers);
            foreach (var teacherId in teachers)
            {
                await _userDAL.AddTeacherClassTimesInfo(teacherId, classRecord.ClassTimes, 1);
                await _userDAL.AddTeacherMonthClassTimes(teacherId, classRecord.ClassOt, classRecord.ClassTimes, 1);
            }

            _eventPublisher.Publish(new StatisticsClassEvent(_tenantId)
            {
                ClassOt = classRecord.ClassOt
            });
            _eventPublisher.Publish(new ResetTenantToDoThingEvent(_tenantId));
            _eventPublisher.Publish(new StatisticsClassFinishCountEvent(_tenantId)
            {
                ClassId = classRecord.ClassId
            });

            var notArrivedStudents = _classRecordStudents.Where(p => p.StudentCheckStatus == EmClassStudentCheckStatus.NotArrived).ToList();
            if (notArrivedStudents.Count > 0)
            {
                _eventPublisher.Publish(new NoticeUserContractsNotArrivedEvent(_tenantId)
                {
                    ClassRecordNotArrivedStudents = notArrivedStudents,
                    ClassRecord = classRecord
                });
            }

            if (!EtmsHelper2.IsThisMonth(classRecord.ClassOt))
            {
                _eventPublisher.Publish(new StatisticsEducationEvent(_tenantId)
                {
                    Time = classRecord.ClassOt
                });
            }

            _eventPublisher.Publish(new StatisticsTeacherSalaryClassTimesEvent(_tenantId)
            {
                ClassRecordId = recordId
            });
            _eventPublisher.Publish(new StatisticsTeacherSalaryClassDayEvent(_tenantId)
            {
                Time = classRecord.ClassOt
            });
        }
    }
}
