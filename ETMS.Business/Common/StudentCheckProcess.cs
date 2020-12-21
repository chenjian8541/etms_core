using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Student.Output;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Event.DataContract;
using ETMS.IDataAccess;
using ETMS.IEventProvider;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Common
{
    public class StudentCheckProcess
    {
        public StudentCheckProcessRequest _request;

        private readonly IEventPublisher _eventPublisher;

        private readonly IClassTimesDAL _classTimesDAL;

        private readonly IClassDAL _classDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IStudentCheckOnLogDAL _studentCheckOnLogDAL;

        private readonly IUserDAL _userDAL;

        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly IStudentCourseConsumeLogDAL _studentCourseConsumeLogDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        public StudentCheckProcess(StudentCheckProcessRequest request, IClassTimesDAL classTimesDAL, IClassDAL classDAL, ICourseDAL courseDAL,
            IEventPublisher eventPublisher, IStudentCheckOnLogDAL studentCheckOnLogDAL, IUserDAL userDAL, IStudentCourseDAL studentCourseDAL,
            IStudentCourseConsumeLogDAL studentCourseConsumeLogDAL, IUserOperationLogDAL userOperationLogDAL)
        {
            this._request = request;
            this._eventPublisher = eventPublisher;
            this._classTimesDAL = classTimesDAL;
            this._classDAL = classDAL;
            this._courseDAL = courseDAL;
            this._studentCheckOnLogDAL = studentCheckOnLogDAL;
            this._userDAL = userDAL;
            this._studentCourseDAL = studentCourseDAL;
            this._studentCourseConsumeLogDAL = studentCourseConsumeLogDAL;
            this._userOperationLogDAL = userOperationLogDAL;
        }

        private async Task<long> AddNotDeStudentCheckOnLog(byte checkType, string remark = "")
        {
            return await _studentCheckOnLogDAL.AddStudentCheckOnLog(new EtStudentCheckOnLog()
            {
                StudentId = _request.Student.Id,
                CheckForm = _request.CheckForm,
                CheckOt = _request.CheckOt,
                CheckType = checkType,
                IsDeleted = EmIsDeleted.Normal,
                Remark = remark,
                Status = EmStudentCheckOnLogStatus.NormalNotClass,
                TenantId = _request.Student.TenantId,
                CheckMedium = _request.CheckMedium,
                DeType = EmDeClassTimesType.NotDe
            });
        }

        private async Task<long> AddDeStudentCheckOnLog(byte checkType, DeStudentClassTimesResult deStudentClassTimesResult,
            EtClassTimes p, long deCourseId, string remark = "")
        {
            var deClassTimes = 0M;
            if (deStudentClassTimesResult.DeType == EmDeClassTimesType.ClassTimes)
            {
                deClassTimes = deStudentClassTimesResult.DeClassTimes;
            }
            return await _studentCheckOnLogDAL.AddStudentCheckOnLog(new EtStudentCheckOnLog()
            {
                StudentId = _request.Student.Id,
                CheckForm = _request.CheckForm,
                CheckOt = _request.CheckOt,
                CheckType = checkType,
                IsDeleted = EmIsDeleted.Normal,
                Remark = string.Empty,
                Status = EmStudentCheckOnLogStatus.NormalAttendClass,
                TenantId = _request.Student.TenantId,
                CheckMedium = _request.CheckMedium,
                DeType = deStudentClassTimesResult.DeType,
                ClassOtDesc = $"{p.ClassOt.EtmsToDateString()}（{EtmsHelper.GetTimeDesc(p.StartTime)}~{EtmsHelper.GetTimeDesc(p.EndTime)}）",
                ClassId = p.ClassId,
                ClassTimesId = p.Id,
                CourseId = deCourseId,
                DeClassTimes = deClassTimes,
                DeStudentCourseDetailId = deStudentClassTimesResult.DeStudentCourseDetailId,
                ExceedClassTimes = deStudentClassTimesResult.ExceedClassTimes,
                DeSum = deStudentClassTimesResult.DeSum
            });
        }

        private async Task<Tuple<long, string>> CheckInAndDeClassTimes(EtClassTimes myClassTimes, byte checkType)
        {
            //直接扣课时
            var studentCheckOnLogId = 0L;
            var deClassTimesDesc = string.Empty;
            var myStudentDeLog = await _studentCheckOnLogDAL.GetStudentDeLog(myClassTimes.Id, _request.Student.Id);
            if (myStudentDeLog != null) //已存在扣课时记录,繁殖重复扣
            {
                studentCheckOnLogId = await AddNotDeStudentCheckOnLog(checkType, "已存在扣课次记录");
            }
            else
            {
                var classStudent = await _classTimesDAL.GetClassTimesStudent(myClassTimes.Id);
                var myClassStudent = classStudent.FirstOrDefault(p => p.StudentId == _request.Student.Id);
                var isProcess = false;
                var deCourseId = 0L;
                if (myClassStudent != null)
                {
                    deCourseId = myClassStudent.CourseId;
                    var notDeResult = DeStudentClassTimesResult.GetNotDeEntity();
                    if (myClassStudent.StudentType == EmClassStudentType.TryCalssStudent)
                    {
                        studentCheckOnLogId = await AddDeStudentCheckOnLog(checkType, notDeResult, myClassTimes, deCourseId, "试听学员不扣课时");
                        isProcess = true;
                        LOG.Log.Info($"[StudentCheckProcess]试听学员不扣课时,LoginTenantId:{_request.LoginTenantId},Student:{_request.Student.Id}", this.GetType());
                    }
                    if (!_request.MakeupIsDeClassTimes && myClassStudent.StudentType == EmClassStudentType.MakeUpStudent)
                    {
                        studentCheckOnLogId = await AddDeStudentCheckOnLog(checkType, notDeResult, myClassTimes, deCourseId, "补课学员不扣课时");
                        isProcess = true;
                        LOG.Log.Info($"[StudentCheckProcess]补课学员不扣课时,LoginTenantId:{_request.LoginTenantId},Student:{_request.Student.Id}", this.GetType());
                    }
                }
                if (!isProcess)
                {
                    //扣减课时
                    var myClass = await _classDAL.GetClassBucket(myClassTimes.ClassId);
                    if (myClass == null || myClass.EtClass == null)
                    {
                        studentCheckOnLogId = await AddNotDeStudentCheckOnLog(checkType, "班级不存在");
                        isProcess = true;
                    }
                    else
                    {
                        if (deCourseId == 0 && myClass.EtClassStudents != null && myClass.EtClassStudents.Count > 0)
                        {
                            var thisStudent = myClass.EtClassStudents.FirstOrDefault(p => p.StudentId == _request.Student.Id);
                            if (thisStudent != null)
                            {
                                deCourseId = thisStudent.CourseId;
                            }
                        }
                        if (deCourseId == 0)
                        {
                            studentCheckOnLogId = await AddNotDeStudentCheckOnLog(checkType, "未查询到消耗课程信息");
                            isProcess = true;
                        }
                        else
                        {
                            var deStudentClassTimesResult = await CoreBusiness.DeStudentClassTimes(_studentCourseDAL, new DeStudentClassTimesTempRequest()
                            {
                                ClassOt = _request.CheckOt,
                                TenantId = _request.LoginTenantId,
                                StudentId = _request.Student.Id,
                                DeClassTimes = myClass.EtClass.DefaultClassTimes,
                                CourseId = deCourseId
                            });
                            studentCheckOnLogId = await AddDeStudentCheckOnLog(checkType, deStudentClassTimesResult, myClassTimes, deCourseId);
                            if (deStudentClassTimesResult.DeType != EmDeClassTimesType.NotDe)
                            {
                                await _studentCourseConsumeLogDAL.AddStudentCourseConsumeLog(new EtStudentCourseConsumeLog()
                                {
                                    CourseId = deCourseId,
                                    DeClassTimes = deStudentClassTimesResult.DeClassTimes,
                                    DeType = deStudentClassTimesResult.DeType,
                                    IsDeleted = EmIsDeleted.Normal,
                                    OrderId = deStudentClassTimesResult.OrderId,
                                    OrderNo = deStudentClassTimesResult.OrderNo,
                                    Ot = _request.CheckOt,
                                    SourceType = EmStudentCourseConsumeSourceType.StudentCheckIn,
                                    StudentId = _request.Student.Id,
                                    TenantId = _request.LoginTenantId,
                                    DeClassTimesSmall = 0
                                });
                            }
                            _eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(_request.LoginTenantId)
                            {
                                StudentId = _request.Student.Id,
                                CourseId = deCourseId
                            });
                            deClassTimesDesc = $"记上课-班级:{myClass.EtClass.Name},扣课时:{ComBusiness2.GetDeClassTimesDesc(deStudentClassTimesResult.DeType, deStudentClassTimesResult.DeClassTimes, deStudentClassTimesResult.ExceedClassTimes)}";
                        }
                    }
                }
            }
            return Tuple.Create(studentCheckOnLogId, deClassTimesDesc);
        }

        public async Task<ResponseBase> Process()
        {
            var lastChekLog = await _studentCheckOnLogDAL.GetStudentCheckOnLastTime(_request.Student.Id);
            byte checkType = EmStudentCheckOnLogCheckType.CheckIn;
            if (lastChekLog != null)
            {
                if (_request.CheckOt.Date == lastChekLog.CheckOt.Date) //只判断同一天
                {
                    var diffSecond = (_request.CheckOt - lastChekLog.CheckOt).TotalSeconds;
                    if (diffSecond <= _request.IntervalTime)
                    {
                        return ResponseBase.CommonError($"{_request.IntervalTime}s内无法重复考勤");
                    }
                    if (lastChekLog.CheckType == EmStudentCheckOnLogCheckType.CheckIn && _request.IsMustCheckOut == EmBool.True) //判断是否为签退
                    {
                        checkType = EmStudentCheckOnLogCheckType.CheckOut;
                    }
                }
            }

            var output = new StudentCheckOutput();
            var studentCheckOnLogId = 0L;
            var deClassTimesDesc = string.Empty;
            if (checkType == EmStudentCheckOnLogCheckType.CheckOut || _request.IsRelationClassTimes == EmBool.False)
            {
                studentCheckOnLogId = await AddNotDeStudentCheckOnLog(checkType);
            }
            else
            {
                var myClassTimesList = await _classTimesDAL.GetStudentCheckOnAttendClass(_request.CheckOt, _request.Student.Id, _request.RelationClassTimesLimitMinute);
                var myClassTimesCount = myClassTimesList.Count();
                if (myClassTimesCount == 0)
                {
                    studentCheckOnLogId = await AddNotDeStudentCheckOnLog(checkType, "未查询到匹配的课次");
                }
                else
                {

                    if (myClassTimesCount > 1)
                    {
                        //返回课次给前端，选择上课的课次
                        studentCheckOnLogId = await AddNotDeStudentCheckOnLog(checkType, "查询到多个匹配的课次");
                        var tempBoxClass = new DataTempBox<EtClass>();
                        var tempBoxUser = new DataTempBox<EtUser>();
                        var tempBoxCourse = new DataTempBox<EtCourse>();
                        output.NeedDeClassTimes = new List<StudentNeedDeClassTimes>();
                        foreach (var p in myClassTimesList)
                        {
                            var myClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, p.ClassId);
                            output.NeedDeClassTimes.Add(new StudentNeedDeClassTimes()
                            {
                                ClassName = myClass?.Name,
                                ClassOtDesc = $"{p.ClassOt.EtmsToDateString()} {EtmsHelper.GetTimeDesc(p.StartTime)}~{EtmsHelper.GetTimeDesc(p.EndTime)}",
                                ClassTimesId = p.Id,
                                CourseName = await ComBusiness.GetCourseNames(tempBoxCourse, _courseDAL, p.CourseList),
                                TeacherDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, p.Teachers)
                            });
                        }
                    }
                    else
                    {
                        //扣课时
                        var deResult = await CheckInAndDeClassTimes(myClassTimesList.First(), checkType);
                        studentCheckOnLogId = deResult.Item1;
                        deClassTimesDesc = deResult.Item2;
                    }
                }
            }
            _eventPublisher.Publish(new NoticeStudentsCheckOnEvent(_request.LoginTenantId)
            {
                StudentCheckOnLogId = studentCheckOnLogId
            });
            output.CheckState = StudentCheckOutputCheckState.Success;
            output.CheckResult = new CheckResult()
            {
                CheckOt = _request.CheckOt.EtmsToDateString(),
                CheckTime = _request.CheckOt.EtmsToOnlyMinuteString(),
                CheckType = checkType,
                CheckTypeDesc = EmStudentCheckOnLogCheckType.GetStudentCheckOnLogCheckTypeDesc(checkType),
                StudentId = _request.Student.Id,
                StudentName = _request.Student.Name,
                StudentPhone = _request.Student.Phone,
                StudentCheckOnLogId = studentCheckOnLogId,
                StudentAvatar = _request.StudentAvatar,
                DeClassTimesDesc = deClassTimesDesc
            };
            output.FaceWhite = _request.FaceWhite;
            await _userOperationLogDAL.AddUserLog(_request.RequestBase, $"学员:{output.CheckResult.StudentName},手机号码:{output.CheckResult.StudentPhone} 考勤{output.CheckResult.CheckTypeDesc} {deClassTimesDesc}", EmUserOperationType.StudentCheckOn, _request.CheckOt);
            return ResponseBase.Success(output);
        }
    }

    public class StudentCheckProcessRequest
    {
        public RequestBase RequestBase { get; set; }

        /// 补课是否扣课时
        /// </summary>
        public bool MakeupIsDeClassTimes { get; set; }

        /// <summary>
        /// 机构ID
        /// </summary>
        public int LoginTenantId { get; set; }

        /// <summary>
        /// 学员
        /// </summary>
        public EtStudent Student { get; set; }

        public string StudentAvatar { get; set; }

        /// <summary> 
        /// 考勤形式  <see cref="ETMS.Entity.Enum.EmStudentCheckOnLogCheckForm"/>
        /// </summary>
        public byte CheckForm { get; set; }

        /// <summary>
        /// 考勤介质
        /// 磁卡卡号/人脸图片key
        /// </summary>
        public string CheckMedium { get; set; }

        /// <summary>
        /// 考勤时间
        /// </summary>
        public DateTime CheckOt { get; set; }

        /// <summary>
        /// 学员考勤间隔时间
        /// </summary>
        public int IntervalTime { get; set; }

        /// <summary>
        /// 学员是否需要刷卡签退  <see cref="EmBool"/>
        /// </summary>
        public byte IsMustCheckOut { get; set; }

        /// <summary>
        /// 是否需要记上课
        /// </summary>
        public byte IsRelationClassTimes { get; set; }

        /// <summary>
        /// 上课时间的间隔不超过 分钟
        /// </summary>
        public int RelationClassTimesLimitMinute { get; set; }

        /// <summary>
        /// 白名单
        /// </summary>
        public FaceInfo FaceWhite { get; set; }
    }
}
