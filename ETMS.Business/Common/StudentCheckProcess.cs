using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Student.Output;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
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

        private readonly ITempStudentNeedCheckDAL _tempStudentNeedCheckDAL;

        private readonly ITempDataCacheDAL _tempDataCacheDAL;

        private readonly IStudentCourseAnalyzeBLL _studentCourseAnalyzeBLL;
        public StudentCheckProcess(StudentCheckProcessRequest request, IClassTimesDAL classTimesDAL, IClassDAL classDAL, ICourseDAL courseDAL,
            IEventPublisher eventPublisher, IStudentCheckOnLogDAL studentCheckOnLogDAL, IUserDAL userDAL, IStudentCourseDAL studentCourseDAL,
            IStudentCourseConsumeLogDAL studentCourseConsumeLogDAL, IUserOperationLogDAL userOperationLogDAL, ITempStudentNeedCheckDAL tempStudentNeedCheckDAL,
            ITempDataCacheDAL tempDataCacheDAL, IStudentCourseAnalyzeBLL studentCourseAnalyzeBLL)
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
            this._tempStudentNeedCheckDAL = tempStudentNeedCheckDAL;
            this._tempDataCacheDAL = tempDataCacheDAL;
            this._studentCourseAnalyzeBLL = studentCourseAnalyzeBLL;
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
                Remark = remark,
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
            if (myStudentDeLog != null) //已存在扣课时记录,防止重复扣
            {
                studentCheckOnLogId = await AddNotDeStudentCheckOnLog(checkType, "已存在扣课次记录");
            }
            else
            {
                var deStudentClassTimesResultTuple = await CoreBusiness.DeStudentClassTimes(_studentCourseDAL, _classTimesDAL, _classDAL,
                    _request.MakeupIsDeClassTimes, myClassTimes, _request.Student.Id, _request.CheckOt);
                if (!string.IsNullOrEmpty(deStudentClassTimesResultTuple.Item1))
                {
                    studentCheckOnLogId = await AddNotDeStudentCheckOnLog(checkType, deStudentClassTimesResultTuple.Item1);
                }
                else
                {
                    var deStudentClassTimesResult = deStudentClassTimesResultTuple.Item2;
                    studentCheckOnLogId = await AddDeStudentCheckOnLog(checkType, deStudentClassTimesResult, myClassTimes, deStudentClassTimesResult.DeCourseId);
                    await _tempStudentNeedCheckDAL.TempStudentNeedCheckClassSetIsAttendClass(myClassTimes.Id, _request.Student.Id);
                    deClassTimesDesc = "已记上课";
                    if (deStudentClassTimesResult.DeType != EmDeClassTimesType.NotDe)
                    {
                        await _studentCourseConsumeLogDAL.AddStudentCourseConsumeLog(new EtStudentCourseConsumeLog()
                        {
                            CourseId = deStudentClassTimesResult.DeCourseId,
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
                        //_eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(_request.LoginTenantId)
                        //{
                        //    StudentId = _request.Student.Id,
                        //    CourseId = deStudentClassTimesResult.DeCourseId
                        //});
                        await _studentCourseAnalyzeBLL.CourseDetailAnalyze(new StudentCourseDetailAnalyzeEvent(_request.LoginTenantId)
                        {
                            StudentId = _request.Student.Id,
                            CourseId = deStudentClassTimesResult.DeCourseId
                        });
                    }
                }
            }
            return Tuple.Create(studentCheckOnLogId, deClassTimesDesc);
        }

        public async Task<ResponseBase> Process()
        {
            var studentCheckLastTime = _tempDataCacheDAL.GetStudentCheckLastTimeBucket(_request.LoginTenantId, _request.Student.Id);
            if (studentCheckLastTime != null)
            {
                if (_request.CheckOt.Date == studentCheckLastTime.StudentCheckLastTime.Date) //只判断同一天
                {
                    var diffSecond = (_request.CheckOt - studentCheckLastTime.StudentCheckLastTime).TotalSeconds;
                    if (diffSecond <= _request.IntervalTime)
                    {
                        var msg = $"同一学员{_request.IntervalTime}s内无法重复考勤";
                        if (_request.CheckForm == EmStudentCheckOnLogCheckForm.Card)
                        {
                            return ResponseBase.CommonError(msg);
                        }
                        else
                        {
                            return ResponseBase.Success(StudentCheckOutput.CheckFail(msg, _request.FaceWhite));
                        }
                    }
                }
            }

            //此记录需要经过一些逻辑执行，所以单独在缓存中保存了最后一次考勤时间
            var lastChekLog = await _studentCheckOnLogDAL.GetStudentCheckOnLastTime(_request.Student.Id);
            byte checkType = EmStudentCheckOnLogCheckType.CheckIn;
            if (lastChekLog != null)
            {
                if (_request.CheckOt.Date == lastChekLog.CheckOt.Date) //只判断同一天
                {
                    var diffSecond = (_request.CheckOt - lastChekLog.CheckOt).TotalSeconds;
                    if (diffSecond <= _request.IntervalTime)
                    {
                        var msg = $"同一学员{_request.IntervalTime}s内无法重复考勤";
                        if (_request.CheckForm == EmStudentCheckOnLogCheckForm.Card)
                        {
                            return ResponseBase.CommonError(msg);
                        }
                        else
                        {
                            return ResponseBase.Success(StudentCheckOutput.CheckFail(msg, _request.FaceWhite));
                        }
                    }
                    if (lastChekLog.CheckType == EmStudentCheckOnLogCheckType.CheckIn && _request.IsMustCheckOut == EmBool.True) //判断是否为签退
                    {
                        checkType = EmStudentCheckOnLogCheckType.CheckOut;
                    }
                }
            }
            _tempDataCacheDAL.SetStudentCheckLastTimeBucket(_request.LoginTenantId, _request.Student.Id, _request.CheckOt);

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
            if (checkType == EmStudentCheckOnLogCheckType.CheckIn)
            {
                await _tempStudentNeedCheckDAL.TempStudentNeedCheckSetIsCheckIn(_request.Student.Id, _request.CheckOt);
            }
            else
            {
                await _tempStudentNeedCheckDAL.TempStudentNeedCheckSetIsCheckOut(_request.Student.Id, _request.CheckOt);
            }
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
                StudentAvatar = _request.FaceAvatar,
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

        public string FaceAvatar { get; set; }

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
