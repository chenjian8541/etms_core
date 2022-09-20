using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Student.Output;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
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

        private readonly IStudentDAL _studentDAL;

        private readonly IStudentPointsLogDAL _studentPointsLogDAL;

        private IDistributedLockDAL _distributedLockDAL;

        private readonly IStudentTrackLogDAL _studentTrackLogDAL;

        private byte _isCanDeNotEnoughCourse;

        public StudentCheckProcess(StudentCheckProcessRequest request, IClassTimesDAL classTimesDAL, IClassDAL classDAL, ICourseDAL courseDAL,
            IEventPublisher eventPublisher, IStudentCheckOnLogDAL studentCheckOnLogDAL, IUserDAL userDAL, IStudentCourseDAL studentCourseDAL,
            IStudentCourseConsumeLogDAL studentCourseConsumeLogDAL, IUserOperationLogDAL userOperationLogDAL, ITempStudentNeedCheckDAL tempStudentNeedCheckDAL,
            ITempDataCacheDAL tempDataCacheDAL, IStudentCourseAnalyzeBLL studentCourseAnalyzeBLL, IStudentDAL studentDAL,
            IStudentPointsLogDAL studentPointsLogDAL, IDistributedLockDAL distributedLockDAL, IStudentTrackLogDAL studentTrackLogDAL)
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
            this._studentDAL = studentDAL;
            this._studentPointsLogDAL = studentPointsLogDAL;
            this._distributedLockDAL = distributedLockDAL;
            this._studentTrackLogDAL = studentTrackLogDAL;
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
                DeType = EmDeClassTimesType.NotDe,
                CurrTemperature = _request.CurrTemperature,
                IsAbnomalTemperature = _request.IsAbnomalTemperature,
            });
        }

        /// <summary>
        /// 考勤记上课(需要排课)
        /// </summary>
        /// <param name="checkType"></param>
        /// <param name="deStudentClassTimesResult"></param>
        /// <param name="p"></param>
        /// <param name="deCourseId"></param>
        /// <param name="points"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        private async Task<long> AddDeStudentCheckOnLog(byte checkType, DeStudentClassTimesResult deStudentClassTimesResult,
            EtClassTimes p, long deCourseId, int points, string remark = "")
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
                DeSum = deStudentClassTimesResult.DeSum,
                Points = points,
                CurrTemperature = _request.CurrTemperature,
                IsAbnomalTemperature = _request.IsAbnomalTemperature,
            });
        }

        /// <summary>
        /// 考勤直接扣课时
        /// </summary>
        /// <param name="checkType"></param>
        /// <param name="deStudentClassTimesResult"></param>
        /// <param name="deCourseId"></param>
        /// <param name="points"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        private async Task<long> AddDeStudentCheckOnLog(byte checkType, DeStudentClassTimesResult deStudentClassTimesResult,
            long deCourseId, int points, string remark = "")
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
                ClassOtDesc = "",
                ClassId = null,
                ClassTimesId = null,
                CourseId = deCourseId,
                DeClassTimes = deClassTimes,
                DeStudentCourseDetailId = deStudentClassTimesResult.DeStudentCourseDetailId,
                ExceedClassTimes = deStudentClassTimesResult.ExceedClassTimes,
                DeSum = deStudentClassTimesResult.DeSum,
                Points = points,
                CurrTemperature = _request.CurrTemperature,
                IsAbnomalTemperature = _request.IsAbnomalTemperature,
            });
        }

        /// <summary>
        /// 关联记上课
        /// </summary>
        /// <returns></returns>
        private async Task<StudentBeginClassRelationClassTimesOutput> StudentBeginClassRelationClassTimes(byte checkType)
        {
            var output = new StudentBeginClassRelationClassTimesOutput();
            var myClassTimesList = await _classTimesDAL.GetStudentCheckOnAttendClass(_request.CheckOt, _request.Student.Id, _request.RelationClassTimesLimitMinute);
            var myClassTimesCount = myClassTimesList.Count();
            if (myClassTimesCount > 0)
            {
                //排除已记上课的课次
                var myDeLog = await _studentCheckOnLogDAL.GetStudentDeLog(myClassTimesList.Select(p => p.Id).ToList(), _request.Student.Id);
                if (myDeLog.Any())
                {
                    var myDeLogIds = myDeLog.Select(j => j.ClassTimesId).ToList();
                    myClassTimesList = myClassTimesList.Where(p => !myDeLogIds.Exists(j => j == p.Id)).ToList();
                    myClassTimesCount = myClassTimesList.Count();
                }
            }
            if (myClassTimesCount == 0)
            {
                output.StudentCheckOnLogId = await AddNotDeStudentCheckOnLog(checkType, "未查询到匹配的课次");
            }
            else
            {

                if (myClassTimesCount > 1)
                {
                    //返回课次给前端，选择上课的课次
                    output.StudentCheckOnLogId = await AddNotDeStudentCheckOnLog(checkType, "查询到多个匹配的课次");
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
                    output.StudentCheckOnLogId = deResult.StudentCheckOnLogId;
                    output.DeClassTimesDesc = deResult.DeClassTimesDesc;
                    output.DeClassTimesValueDesc = deResult.DeClassTimesValueDesc;
                    output.CourseName = deResult.CourseName;
                    output.CourseSurplusDesc = deResult.CourseSurplusDesc;
                    output.IsCourseNotEnough = deResult.IsCourseNotEnough;
                }
            }
            return output;
        }

        /// <summary>
        /// 直接扣课时
        /// </summary>
        /// <returns></returns>
        private async Task<StudentBeginClassGoDeStudentCourseOutput> StudentBeginClassGoDeStudentCourse(byte checkType, int dayLimitValueDeStudentCourse)
        {
            if (dayLimitValueDeStudentCourse == 0)
            {
                dayLimitValueDeStudentCourse = 1;
            }
            var output = new StudentBeginClassGoDeStudentCourseOutput();
            var attendClassCount = await _studentCheckOnLogDAL.GetStudentOneDayAttendClassCount(_request.Student.Id, _request.CheckOt);
            if (attendClassCount >= dayLimitValueDeStudentCourse)
            {
                output.StudentCheckOnLogId = await AddNotDeStudentCheckOnLog(checkType, $"已超过每天最多记{dayLimitValueDeStudentCourse}次上课限制");
                return output;
            }
            var myStudentCourse = await _studentCourseDAL.GetStudentCourse(_request.Student.Id);
            if (myStudentCourse == null || myStudentCourse.Count == 0)
            {
                output.StudentCheckOnLogId = await AddNotDeStudentCheckOnLog(checkType, "学员未购买课程");
                return output;
            }
            var myValidCourse = myStudentCourse.Where(p => p.Status != EmStudentCourseStatus.EndOfClass); //是否存在未结课的课程
            if (myValidCourse.Count() == 0)
            {
                if (_isCanDeNotEnoughCourse == EmBool.False)
                {
                    output.StudentCheckOnLogId = await AddNotDeStudentCheckOnLog(checkType, "未查询到学员“未结课”的课程");
                    return output;
                }
                else
                {
                    myValidCourse = myStudentCourse;
                }
            }
            var courseIds = myValidCourse.Select(p => p.CourseId).Distinct(); //是否有多门课程
            var deCourseId = 0L;
            if (courseIds.Count() > 1)
            {
                if (_request.StudentCheckInConfig.RelationClassTimesGoDeStudentCourseMulCourseType ==
                    EmRelationClassTimesGoDeStudentCourseMulCourseType.PopupsChooseStudentCouese)
                {
                    output.StudentCheckOnLogId = await AddNotDeStudentCheckOnLog(checkType, "学员有多门有效课程，请选择需要记上课课程");
                    var popupsChooseStudentCoueses = new List<PopupsChooseStudentCouese>();
                    foreach (var myItemCourseId in courseIds)
                    {
                        var myEtCourse = await _courseDAL.GetCourse(myItemCourseId);
                        if (myEtCourse != null && myEtCourse.Item1 != null)
                        {
                            var myItemCourseDetails = myValidCourse.Where(p => p.CourseId == myItemCourseId).ToList();
                            popupsChooseStudentCoueses.Add(new PopupsChooseStudentCouese()
                            {
                                CourseId = myItemCourseId,
                                CourseName = myEtCourse.Item1.Name,
                                SurplusQuantityDesc = ComBusiness.GetStudentCourseDesc(myItemCourseDetails)
                            });
                        }
                    }
                    if (popupsChooseStudentCoueses.Count == 1)
                    {
                        deCourseId = popupsChooseStudentCoueses.First().CourseId;
                    }
                    else
                    {
                        output.PopupsChooseStudentCoueses = popupsChooseStudentCoueses;
                        return output;
                    }
                }
                else
                {
                    var studentCheckDefaultCourse = myValidCourse.FirstOrDefault(p => p.StudentCheckDefault == EmBool.True);
                    if (studentCheckDefaultCourse != null)
                    {
                        deCourseId = studentCheckDefaultCourse.CourseId;
                    }
                    else
                    {
                        var normalCourse = myValidCourse.Where(p => p.Status == EmStudentCourseStatus.Normal); //判断正常的课程
                        var myNormalCourseIds = normalCourse.Select(p => p.CourseId).Distinct();
                        if (myNormalCourseIds.Count() == 1)
                        {
                            deCourseId = myNormalCourseIds.First();
                        }
                    }
                }
            }
            else
            {
                deCourseId = courseIds.First();
            }
            if (deCourseId == 0)
            {
                var vaildEndCourse = myStudentCourse.FirstOrDefault(p => p.StudentCheckDefault == EmBool.True && p.Status == EmStudentCourseStatus.EndOfClass);
                if (vaildEndCourse != null)
                {
                    if (_isCanDeNotEnoughCourse == EmBool.False)
                    {
                        output.IsCourseNotEnough = true;
                        output.StudentCheckOnLogId = await AddNotDeStudentCheckOnLog(checkType, "学员课时不足");
                        return output;
                    }
                    else
                    {
                        deCourseId = vaildEndCourse.CourseId;
                    }
                }
                else
                {
                    output.StudentCheckOnLogId = await AddNotDeStudentCheckOnLog(checkType, "未设置学员“考勤记上课课程”");
                    return output;
                }
            }
            var myDeCourse = await _courseDAL.GetCourse(deCourseId);
            if (myDeCourse == null || myDeCourse.Item1 == null)
            {
                output.StudentCheckOnLogId = await AddNotDeStudentCheckOnLog(checkType, "学员报读课程不存在");
                return output;
            }
            output.CourseName = myDeCourse.Item1.Name;
            var deStudentClassTimesResult = await CoreBusiness.DeStudentClassTimes(_studentCourseDAL, new DeStudentClassTimesTempRequest()
            {
                ClassOt = _request.CheckOt,
                TenantId = _request.LoginTenantId,
                StudentId = _request.Student.Id,
                DeClassTimes = myDeCourse.Item1.StudentCheckDeClassTimes,
                CourseId = deCourseId
            });
            if (myDeCourse.Item1.CheckPoints > 0)
            {
                await _studentDAL.AddPoint(_request.Student.Id, myDeCourse.Item1.CheckPoints);
                await _studentPointsLogDAL.AddStudentPointsLog(new EtStudentPointsLog()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    No = string.Empty,
                    Ot = _request.CheckOt,
                    Points = myDeCourse.Item1.CheckPoints,
                    Remark = string.Empty,
                    StudentId = _request.Student.Id,
                    TenantId = _request.LoginTenantId,
                    Type = EmStudentPointsLogType.StudentCheckOn
                });
            }
            output.StudentCheckOnLogId = await AddDeStudentCheckOnLog(checkType, deStudentClassTimesResult, deCourseId, myDeCourse.Item1.CheckPoints);
            await _tempStudentNeedCheckDAL.TempStudentNeedCheckClassSetIsAttendClassByStudentId(_request.Student.Id, _request.CheckOt);
            output.DeClassTimesDesc = "已记上课";
            output.DeClassTimesValueDesc = ComBusiness2.GetDeClassTimesDesc(deStudentClassTimesResult.DeType,
                deStudentClassTimesResult.DeClassTimes, deStudentClassTimesResult.ExceedClassTimes);
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
                    DeClassTimesSmall = 0,
                    DeSum = deStudentClassTimesResult.DeSum
                });
                //_eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(_request.LoginTenantId)
                //{
                //    StudentId = _request.Student.Id,
                //    CourseId = deStudentClassTimesResult.DeCourseId
                //});
                var res = await _studentCourseAnalyzeBLL.CourseDetailAnalyze(new StudentCourseDetailAnalyzeEvent(_request.LoginTenantId)
                {
                    StudentId = _request.Student.Id,
                    CourseId = deStudentClassTimesResult.DeCourseId,
                    IsNeedCheckCourseIsNotEnough = true,
                });
                output.CourseSurplusDesc = ComBusiness.GetStudentCourseDesc(res.NewCourse);
                output.IsCourseNotEnough = res.IsCourseNotEnough;
            }
            return output;
        }

        private async Task<CheckInAndDeClassTimes> CheckInAndDeClassTimes(EtClassTimes myClassTimes, byte checkType)
        {
            //直接扣课时
            var studentCheckOnLogId = 0L;
            var deClassTimesDesc = string.Empty;
            var deClassTimesValueDesc = string.Empty;
            var output = new CheckInAndDeClassTimes();
            var myStudentDeLog = await _studentCheckOnLogDAL.GetStudentDeLog(myClassTimes.Id, _request.Student.Id);
            if (myStudentDeLog != null) //已存在扣课时记录,防止重复扣
            {
                studentCheckOnLogId = await AddNotDeStudentCheckOnLog(checkType, "已存在扣课次记录");
            }
            else
            {
                var isJumpStudentCourseNotEnough = _request.TenantConfig.ClassCheckSignConfig.MustEnoughSurplusClassTimes;
                var deStudentClassTimesResultTuple = await CoreBusiness.DeStudentClassTimes(_studentCourseDAL, _classTimesDAL, _classDAL,
                    _request.MakeupIsDeClassTimes, myClassTimes, _request.Student.Id, _request.CheckOt, isJumpStudentCourseNotEnough);
                if (!string.IsNullOrEmpty(deStudentClassTimesResultTuple.Item1))
                {
                    studentCheckOnLogId = await AddNotDeStudentCheckOnLog(checkType, deStudentClassTimesResultTuple.Item1);
                }
                else
                {
                    var deStudentClassTimesResult = deStudentClassTimesResultTuple.Item2;
                    var myCourse = await _courseDAL.GetCourse(deStudentClassTimesResult.DeCourseId);
                    output.CourseName = myCourse.Item1.Name;
                    if (myCourse.Item1.CheckPoints > 0)
                    {
                        await _studentDAL.AddPoint(_request.Student.Id, myCourse.Item1.CheckPoints);
                        await _studentPointsLogDAL.AddStudentPointsLog(new EtStudentPointsLog()
                        {
                            IsDeleted = EmIsDeleted.Normal,
                            No = string.Empty,
                            Ot = _request.CheckOt,
                            Points = myCourse.Item1.CheckPoints,
                            Remark = string.Empty,
                            StudentId = _request.Student.Id,
                            TenantId = _request.LoginTenantId,
                            Type = EmStudentPointsLogType.StudentCheckOn
                        });
                    }
                    studentCheckOnLogId = await AddDeStudentCheckOnLog(checkType, deStudentClassTimesResult, myClassTimes,
                        deStudentClassTimesResult.DeCourseId, myCourse.Item1.CheckPoints);
                    await _tempStudentNeedCheckDAL.TempStudentNeedCheckClassSetIsAttendClass(myClassTimes.Id, _request.Student.Id);
                    deClassTimesDesc = "已记上课";
                    deClassTimesValueDesc = ComBusiness2.GetDeClassTimesDesc(deStudentClassTimesResult.DeType,
                        deStudentClassTimesResult.DeClassTimes, deStudentClassTimesResult.ExceedClassTimes);
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
                            DeClassTimesSmall = 0,
                            DeSum = deStudentClassTimesResult.DeSum
                        });
                        //_eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(_request.LoginTenantId)
                        //{
                        //    StudentId = _request.Student.Id,
                        //    CourseId = deStudentClassTimesResult.DeCourseId
                        //});
                        var res = await _studentCourseAnalyzeBLL.CourseDetailAnalyze(new StudentCourseDetailAnalyzeEvent(_request.LoginTenantId)
                        {
                            StudentId = _request.Student.Id,
                            CourseId = deStudentClassTimesResult.DeCourseId,
                            IsNeedCheckCourseIsNotEnough = true,
                        });
                        output.CourseSurplusDesc = ComBusiness.GetStudentCourseDesc(res.NewCourse);
                        output.IsCourseNotEnough = res.IsCourseNotEnough;
                    }
                }
            }
            output.StudentCheckOnLogId = studentCheckOnLogId;
            output.DeClassTimesDesc = deClassTimesDesc;
            output.DeClassTimesValueDesc = deClassTimesValueDesc;
            return output;
        }

        public async Task<ResponseBase> Process()
        {
            var lockKey = new StudentCheckToken(_request.LoginTenantId, _request.Student.Id);
            if (_distributedLockDAL.LockTake(lockKey))
            {
                try
                {
                    return await this.CheckStudentInvoke();
                }
                finally
                {
                    _distributedLockDAL.LockRelease(lockKey);
                }
            }
            return ResponseBase.CommonError("请勿重复考勤");
        }

        private bool StudentCheckInLimit()
        {
            if (_request.StudentCheckInConfig.StudentCheckInLimitTimeType == EmStudentCheckInLimitTimeType.Time)
            {
                var nowTime = EtmsHelper.GetTimeHourAndMinuteDesc(_request.CheckOt);
                if (_request.StudentCheckInConfig.StudentCheckInLimitTimeStart > 0 && nowTime < _request.StudentCheckInConfig.StudentCheckInLimitTimeStart)
                {
                    return false;
                }
                if (_request.StudentCheckInConfig.StudentCheckInLimitTimeEnd > 0 && nowTime > _request.StudentCheckInConfig.StudentCheckInLimitTimeEnd)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<ResponseBase> CheckStudentInvoke()
        {
            if (!StudentCheckInLimit())
            {
                return ResponseBase.CommonError("此时间段无法考勤");
            }
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
                            return ResponseBase.Success(StudentCheckOutput.CheckFail("请勿重复考勤", _request.FaceWhite));
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
                            return ResponseBase.Success(StudentCheckOutput.CheckFail("请勿重复考勤", _request.FaceWhite));
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
            var deClassTimesValueDesc = string.Empty;
            var courseName = string.Empty;
            var courseSurplusDesc = string.Empty;
            var isCourseNotEnough = false;
            if (checkType == EmStudentCheckOnLogCheckType.CheckOut || _request.IsRelationClassTimes == EmBool.False)
            {
                studentCheckOnLogId = await AddNotDeStudentCheckOnLog(checkType);
            }
            else
            {
                var dayLimitValueDeStudentCourse = 1;
                var relationClassTimesType = EmAttendanceRelationClassTimesType.RelationClassTimes;
                if (_request.CheckForm == EmStudentCheckOnLogCheckForm.Face)
                {
                    dayLimitValueDeStudentCourse = _request.StudentCheckInConfig.StudentUseFaceCheckIn.RelationClassTimesFaceType1DayLimitValue;
                    relationClassTimesType = _request.StudentCheckInConfig.StudentUseFaceCheckIn.RelationClassTimesFaceType;
                    _isCanDeNotEnoughCourse = _request.StudentCheckInConfig.StudentUseFaceCheckIn.IsCanDeNotEnoughCourseFace;
                }
                else
                {
                    //刷卡、忘记带卡老师手动补签
                    dayLimitValueDeStudentCourse = _request.StudentCheckInConfig.StudentUseCardCheckIn.RelationClassTimesCardType1DayLimitValue;
                    relationClassTimesType = _request.StudentCheckInConfig.StudentUseCardCheckIn.RelationClassTimesCardType;
                    _isCanDeNotEnoughCourse = _request.StudentCheckInConfig.StudentUseCardCheckIn.IsCanDeNotEnoughCourseCard;
                }
                if (relationClassTimesType == EmAttendanceRelationClassTimesType.RelationClassTimes)
                {
                    var resultRelationClassTimes = await StudentBeginClassRelationClassTimes(checkType);
                    studentCheckOnLogId = resultRelationClassTimes.StudentCheckOnLogId;
                    deClassTimesDesc = resultRelationClassTimes.DeClassTimesDesc;
                    deClassTimesValueDesc = resultRelationClassTimes.DeClassTimesValueDesc;
                    output.NeedDeClassTimes = resultRelationClassTimes.NeedDeClassTimes;
                    courseName = resultRelationClassTimes.CourseName;
                    courseSurplusDesc = resultRelationClassTimes.CourseSurplusDesc;
                    isCourseNotEnough = resultRelationClassTimes.IsCourseNotEnough;
                }
                else
                {
                    var resultGoDeStudentCourse = await StudentBeginClassGoDeStudentCourse(checkType, dayLimitValueDeStudentCourse);
                    studentCheckOnLogId = resultGoDeStudentCourse.StudentCheckOnLogId;
                    deClassTimesDesc = resultGoDeStudentCourse.DeClassTimesDesc;
                    deClassTimesValueDesc = resultGoDeStudentCourse.DeClassTimesValueDesc;
                    output.PopupsChooseStudentCoueses = resultGoDeStudentCourse.PopupsChooseStudentCoueses;
                    courseName = resultGoDeStudentCourse.CourseName;
                    courseSurplusDesc = resultGoDeStudentCourse.CourseSurplusDesc;
                    isCourseNotEnough = resultGoDeStudentCourse.IsCourseNotEnough;
                }
            }
            _eventPublisher.Publish(new NoticeStudentsCheckOnEvent(_request.LoginTenantId)
            {
                StudentCheckOnLogId = studentCheckOnLogId
            });
            _eventPublisher.Publish(new NoticeUserAboutStudentCheckOnEvent(_request.LoginTenantId)
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
            var tempBoxUser = new DataTempBox<EtUser>();
            output.CheckResult = new CheckResult()
            {
                CheckOt = _request.CheckOt.EtmsToDateString(),
                CheckTime = _request.CheckOt.EtmsToOnlyMinuteString(),
                CheckType = checkType,
                CheckTypeDesc = EmStudentCheckOnLogCheckType.GetStudentCheckOnLogCheckTypeDesc(checkType),
                StudentId = _request.Student.Id,
                StudentName = _request.Student.Name,
                StudentPhone = ComBusiness3.PhoneSecrecy(_request.Student.Phone, _request.RequestBase.SecrecyType, _request.RequestBase.SecrecyDataBag),
                StudentCheckOnLogId = studentCheckOnLogId,
                StudentAvatar = _request.FaceAvatar,
                DeClassTimesDesc = deClassTimesDesc,
                DeClassTimesValueDesc = deClassTimesValueDesc,
                CourseName = courseName,
                CourseSurplusDesc = courseSurplusDesc,
                IsCourseNotEnough = isCourseNotEnough,
                GenderDesc = EmGender.GetGenderDesc(_request.Student.Gender),
                OtDesc = _request.Student.Ot.EtmsToDateString(),
                CheckForm = _request.CheckForm,
                CheckFormDesc = EmStudentCheckOnLogCheckForm.GetStudentCheckOnLogCheckFormDesc(_request.CheckForm),
                TrackUserDesc = await ComBusiness.GetUserName(tempBoxUser, _userDAL, _request.Student.TrackUser),
                LearningManagerDesc = await ComBusiness.GetUserName(tempBoxUser, _userDAL, _request.Student.LearningManager)
            };
            if (!string.IsNullOrEmpty(courseName))
            {
                _eventPublisher.Publish(new SyncStudentLastGoClassTimeEvent(_request.LoginTenantId)
                {
                    StudentId = _request.Student.Id,
                    ClassOt = _request.CheckOt
                });
            }
            if (_request.LoginTenantId == 7321) //要求展示跟进记录的客户
            {
                var studentTrackLogs = await _studentTrackLogDAL.GetStudentTrackLog(_request.Student.Id);
                if (studentTrackLogs != null && studentTrackLogs.Any())
                {
                    var lastTrackLog = studentTrackLogs.First();
                    var trackUser = await _userDAL.GetUser(lastTrackLog.TrackUserId);
                    output.CheckResult.StudentTrackLogLast = new StudentTrackLogLast()
                    {
                        CId = lastTrackLog.Id,
                        TrackTimeDesc = lastTrackLog.TrackTime.EtmsToMinuteString(),
                        NextTrackTimeDesc = lastTrackLog.NextTrackTime.EtmsToMinuteString(),
                        TrackContent = lastTrackLog.TrackContent,
                        TrackUserName = trackUser?.Name,
                        TrackImgUrl = EtmsHelper2.GetMediasUrl(lastTrackLog.TrackImg),
                        TrackUserAvatarUrl = AliyunOssUtil.GetAccessUrlHttps(trackUser?.Avatar)
                    };
                }
            }
            output.FaceWhite = _request.FaceWhite;
            await _userOperationLogDAL.AddUserLog(_request.RequestBase, $"学员:{output.CheckResult.StudentName},手机号码:{output.CheckResult.StudentPhone} 考勤{output.CheckResult.CheckTypeDesc} {deClassTimesDesc}", EmUserOperationType.StudentCheckOn, _request.CheckOt);
            _eventPublisher.Publish(new StatisticsStudentCheckEvent(_request.LoginTenantId)
            {
                CheckOt = _request.CheckOt.Date
            });
            return ResponseBase.Success(output);
        }
    }

    public class StudentCheckProcessRequest
    {
        public StudentCheckProcessRequest(TenantConfig config)
        {
            this.TenantConfig = config;
            this.StudentCheckInConfig = config.StudentCheckInConfig;
        }

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

        public StudentCheckInConfig StudentCheckInConfig { get; set; }

        public TenantConfig TenantConfig { get; set; }

        /// <summary>
        /// 温度
        /// </summary>
        public decimal CurrTemperature { get; set; }

        /// <summary>
        /// 温度是否异常
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsAbnomalTemperature { get; set; }
    }

    public class StudentBeginClassRelationClassTimesOutput
    {

        public long StudentCheckOnLogId { get; set; }

        /// <summary>
        /// 扣减课时
        /// </summary>
        public string DeClassTimesDesc { get; set; }

        public string DeClassTimesValueDesc { get; set; }

        /// <summary>
        /// 需要记上课的课次
        /// </summary>
        public List<StudentNeedDeClassTimes> NeedDeClassTimes { get; set; }

        /// <summary>
        /// 课次剩余描述
        /// </summary>
        public string CourseSurplusDesc { get; set; }

        public bool IsCourseNotEnough { get; set; }

        public string CourseName { get; set; }
    }

    public class StudentBeginClassGoDeStudentCourseOutput
    {
        public long StudentCheckOnLogId { get; set; }

        /// <summary>
        /// 扣减课时
        /// </summary>
        public string DeClassTimesDesc { get; set; }

        public string DeClassTimesValueDesc { get; set; }

        public List<PopupsChooseStudentCouese> PopupsChooseStudentCoueses { get; set; }

        /// <summary>
        /// 课次剩余描述
        /// </summary>
        public string CourseSurplusDesc { get; set; }

        public bool IsCourseNotEnough { get; set; }

        public string CourseName { get; set; }
    }

    public class CheckInAndDeClassTimes
    {

        public long StudentCheckOnLogId { get; set; }

        public string DeClassTimesDesc { get; set; }

        /// <summary>
        /// 课次剩余描述
        /// </summary>
        public string CourseSurplusDesc { get; set; }

        public bool IsCourseNotEnough { get; set; }

        public string CourseName { get; set; }

        public string DeClassTimesValueDesc { get; set; }
    }
}
