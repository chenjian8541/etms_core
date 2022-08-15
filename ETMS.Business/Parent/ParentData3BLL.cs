using ETMS.Business.Common;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Parent.Output;
using ETMS.Entity.Dto.Parent.Request;
using ETMS.Entity.Dto.Parent2.Output;
using ETMS.Entity.Dto.Parent2.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Entity.View;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IEventProvider;
using ETMS.Utility;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class ParentData3BLL : IParentData3BLL
    {
        private readonly IActiveWxMessageDAL _activeWxMessageDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IActiveWxMessageParentReadDAL _activeWxMessageParentReadDAL;

        private readonly IActiveGrowthRecordDAL _activeGrowthRecordDAL;

        private readonly ITryCalssApplyLogDAL _tryCalssApplyLogDAL;

        private readonly IStudentCheckOnLogDAL _studentCheckOnLogDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IStudentAccountRechargeLogDAL _studentAccountRechargeLogDAL;

        private readonly IAppConfig2BLL _appConfig2BLL;

        private readonly IUserDAL _userDAL;

        private readonly IClassTimesDAL _classTimesDAL;

        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly IClassDAL _classDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IClassRoomDAL _classRoomDAL;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly IStudentOperationLogDAL _studentOperationLogDAL;

        private readonly IStudentAccountRechargeCoreBLL _studentAccountRechargeCoreBLL;

        private readonly IStudentLeaveApplyLogDAL _studentLeaveApplyLogDAL;

        public ParentData3BLL(IActiveWxMessageDAL activeWxMessageDAL, IStudentDAL studentDAL, IActiveWxMessageParentReadDAL activeWxMessageParentReadDAL,
            IActiveGrowthRecordDAL activeGrowthRecordDAL, ITryCalssApplyLogDAL tryCalssApplyLogDAL, IStudentCheckOnLogDAL studentCheckOnLogDAL,
            IEventPublisher eventPublisher, IStudentAccountRechargeLogDAL studentAccountRechargeLogDAL,
           IAppConfig2BLL appConfig2BLL, IUserDAL userDAL, IClassTimesDAL classTimesDAL, IStudentCourseDAL studentCourseDAL,
           IClassDAL classDAL, ICourseDAL courseDAL, IClassRoomDAL classRoomDAL, IStudentAccountRechargeCoreBLL studentAccountRechargeCoreBLL,
           IHttpContextAccessor httpContextAccessor, IAppConfigurtaionServices appConfigurtaionServices, IStudentOperationLogDAL studentOperationLogDAL,
           IStudentLeaveApplyLogDAL studentLeaveApplyLogDAL)
        {
            this._activeWxMessageDAL = activeWxMessageDAL;
            this._studentDAL = studentDAL;
            this._activeWxMessageParentReadDAL = activeWxMessageParentReadDAL;
            this._activeGrowthRecordDAL = activeGrowthRecordDAL;
            this._tryCalssApplyLogDAL = tryCalssApplyLogDAL;
            this._studentCheckOnLogDAL = studentCheckOnLogDAL;
            this._eventPublisher = eventPublisher;
            this._studentAccountRechargeLogDAL = studentAccountRechargeLogDAL;
            this._appConfig2BLL = appConfig2BLL;
            this._userDAL = userDAL;
            this._classTimesDAL = classTimesDAL;
            this._studentCourseDAL = studentCourseDAL;
            this._classDAL = classDAL;
            this._courseDAL = courseDAL;
            this._classRoomDAL = classRoomDAL;
            this._httpContextAccessor = httpContextAccessor;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._studentOperationLogDAL = studentOperationLogDAL;
            this._studentAccountRechargeCoreBLL = studentAccountRechargeCoreBLL;
            this._studentLeaveApplyLogDAL = studentLeaveApplyLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._appConfig2BLL.InitTenantId(tenantId);
            this._studentAccountRechargeCoreBLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _activeWxMessageDAL, _studentDAL, _activeWxMessageParentReadDAL, _activeGrowthRecordDAL,
                _tryCalssApplyLogDAL, _studentCheckOnLogDAL, _studentAccountRechargeLogDAL,
                _userDAL, _classTimesDAL, _studentCourseDAL, _classDAL, _courseDAL, _classRoomDAL, _studentOperationLogDAL,
                _studentLeaveApplyLogDAL);
        }

        public async Task<ResponseBase> WxMessageDetailPaging(WxMessageDetailPagingRequest request)
        {
            var pagingData = await _activeWxMessageDAL.GetDetailPaging(request);
            var output = new List<WxMessageDetailPagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new WxMessageDetailPagingOutput()
                {
                    IsConfirm = p.IsConfirm,
                    IsNeedConfirm = p.IsNeedConfirm,
                    IsRead = p.IsRead,
                    OtDesc = p.Ot.EtmsToDateShortString(),
                    StudentId = p.StudentId,
                    Title = p.Title,
                    WxMessageDetailId = p.Id,
                    WxMessageId = p.WxMessageId
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<WxMessageDetailPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> WxMessageDetailGet(WxMessageDetailGetRequest request)
        {
            var wxMessageDetail = await _activeWxMessageDAL.GetActiveWxMessageDetail(request.WxMessageDetailId);
            if (wxMessageDetail == null)
            {
                return ResponseBase.CommonError("通知记录不存在");
            }
            var wxMessage = await _activeWxMessageDAL.GetActiveWxMessage(wxMessageDetail.WxMessageId);
            if (wxMessage == null)
            {
                return ResponseBase.CommonError("通知记录不存在");
            }
            var student = await _studentDAL.GetStudent(wxMessageDetail.StudentId);
            if (student == null || student.Student == null)
            {
                return ResponseBase.CommonError("学员信息不存在");
            }
            var output = new WxMessageDetailGetOutput()
            {
                WxMessageId = wxMessageDetail.WxMessageId,
                IsConfirm = wxMessageDetail.IsConfirm,
                IsNeedConfirm = wxMessageDetail.IsNeedConfirm,
                IsRead = wxMessageDetail.IsRead,
                MessageContent = wxMessage.MessageContent,
                Title = wxMessage.Title,
                Ot = wxMessage.Ot,
                StudentId = wxMessageDetail.StudentId,
                WxMessageDetailId = wxMessageDetail.Id,
                StudentName = student.Student.Name
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> WxMessageDetailSetRead(WxMessageDetailSetReadRequest request)
        {
            var wxMessageDetail = await _activeWxMessageDAL.GetActiveWxMessageDetail(request.WxMessageDetailId);
            if (wxMessageDetail == null)
            {
                return ResponseBase.CommonError("通知记录不存在");
            }
            if (wxMessageDetail.IsRead == EmBool.True)
            {
                LOG.Log.Warn("[WxMessageDetailSetRead]重复提交设置已读请求", this.GetType());
                return ResponseBase.Success();
            }
            var wxMessage = await _activeWxMessageDAL.GetActiveWxMessage(wxMessageDetail.WxMessageId);
            if (wxMessage == null)
            {
                return ResponseBase.CommonError("通知记录不存在");
            }

            wxMessageDetail.IsRead = EmBool.True;
            await _activeWxMessageDAL.EditActiveWxMessageDetail(wxMessageDetail);

            wxMessage.ReadCount += 1;
            await _activeWxMessageDAL.EditActiveWxMessage(wxMessage);

            await _activeWxMessageParentReadDAL.UpdateParentRead(request.LoginPhone, request.ParentStudentIds);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> WxMessageDetailSetConfirm(WxMessageDetailSetConfirmRequest request)
        {
            var wxMessageDetail = await _activeWxMessageDAL.GetActiveWxMessageDetail(request.WxMessageDetailId);
            if (wxMessageDetail == null)
            {
                return ResponseBase.CommonError("通知记录不存在");
            }
            if (wxMessageDetail.IsConfirm == EmBool.True)
            {
                LOG.Log.Warn("[WxMessageDetailSetConfirm]重复提交确认请求", this.GetType());
                return ResponseBase.Success();
            }
            var wxMessage = await _activeWxMessageDAL.GetActiveWxMessage(wxMessageDetail.WxMessageId);
            if (wxMessage == null)
            {
                return ResponseBase.CommonError("通知记录不存在");
            }

            wxMessageDetail.IsConfirm = EmBool.True;
            await _activeWxMessageDAL.EditActiveWxMessageDetail(wxMessageDetail);

            wxMessage.ConfirmCount += 1;
            await _activeWxMessageDAL.EditActiveWxMessage(wxMessage);

            return ResponseBase.Success();
        }

        public async Task<ResponseBase> WxMessageGetUnreadCount(WxMessageGetUnreadCountRequest request)
        {
            return ResponseBase.Success(await _activeWxMessageParentReadDAL.GetParentUnreadCount(request.LoginPhone, request.ParentStudentIds));
        }

        public async Task<ResponseBase> TryCalssApply(TryCalssApplyRequest request)
        {
            var log = await _activeGrowthRecordDAL.GetActiveGrowthRecordDetail(request.GrowthRecordDetailId);
            if (log == null)
            {
                return ResponseBase.CommonError("成长档案不存在");
            }
            var applyLog = new EtTryCalssApplyLog()
            {
                ApplyOt = DateTime.Now,
                ClassOt = null,
                ClassTime = string.Empty,
                CourseDesc = string.Empty,
                CourseId = null,
                HandleOt = null,
                HandleRemark = string.Empty,
                HandleStatus = EmTryCalssApplyHandleStatus.Unreviewed,
                HandleUser = null,
                IsDeleted = EmIsDeleted.Normal,
                Phone = request.Phone,
                RecommandStudentId = log.StudentId,
                SourceType = EmTryCalssSourceType.Tourists,
                StudentId = null,
                TenantId = request.TenantId,
                TouristName = request.Name,
                TouristRemark = request.Remark
            };
            await _tryCalssApplyLogDAL.AddTryCalssApplyLog(applyLog);

            _eventPublisher.Publish(new ResetTenantToDoThingEvent(log.TenantId));
            _eventPublisher.Publish(new NoticeUserTryCalssApplyEvent(log.TenantId)
            {
                TryCalssApplyLog = applyLog
            });

            return ResponseBase.Success();
        }

        public async Task<ResponseBase> CheckOnLogGet(CheckOnLogGetRequest request)
        {
            var log = await _studentCheckOnLogDAL.GetStudentCheckOnLog(request.StudentCheckOnLogId);
            if (log == null)
            {
                return ResponseBase.CommonError("考勤记录不存在");
            }
            var checkMediumUrl = string.Empty;
            var maxDate = DateTime.Now.AddDays(-7);
            if (log.CheckOt > maxDate)
            {
                checkMediumUrl = AliyunOssUtil.GetAccessUrlHttps(log.CheckMedium);
            }
            return ResponseBase.Success(new CheckOnLogGetOutput()
            {
                CheckOt = log.CheckOt,
                CheckMediumUrl = checkMediumUrl,
                CheckType = log.CheckType,
                CheckTypeDesc = EmStudentCheckOnLogCheckType.GetStudentCheckOnLogCheckTypeDesc(log.CheckType)
            });
        }

        public async Task<ResponseBase> CheckOnLogGetPaging(CheckOnLogGetPagingRequest request)
        {
            var pagingData = await _studentCheckOnLogDAL.GetPaging(request);
            var output = new List<CheckOnLogGetPagingOutput>();
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var tempBoxStudent = new DataTempBox<EtStudent>();
            var maxDate = DateTime.Now.AddDays(-7);
            string courseName;
            string deClassTimesDesc;
            string checkMedium;
            foreach (var p in pagingData.Item1)
            {
                var student = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId);
                if (student == null)
                {
                    continue;
                }
                var explain = string.Empty;
                courseName = string.Empty;
                deClassTimesDesc = string.Empty;
                checkMedium = string.Empty;
                if (p.CheckType == EmStudentCheckOnLogCheckType.CheckIn)
                {
                    switch (p.Status)
                    {
                        case EmStudentCheckOnLogStatus.NormalNotClass:
                            break;
                        case EmStudentCheckOnLogStatus.NormalAttendClass:
                        case EmStudentCheckOnLogStatus.BeRollcall:
                            if (p.ClassTimesId == null)
                            {
                                //直接扣减课时
                                var ckCourse = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, p.CourseId.Value);
                                courseName = ckCourse;
                                deClassTimesDesc = ComBusiness2.GetDeClassTimesDesc(p.DeType, p.DeClassTimes, p.ExceedClassTimes);
                            }
                            else
                            {
                                var ckCourse = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, p.CourseId.Value);
                                courseName = ckCourse;
                                deClassTimesDesc = ComBusiness2.GetDeClassTimesDesc(p.DeType, p.DeClassTimes, p.ExceedClassTimes);
                            }
                            break;
                        case EmStudentCheckOnLogStatus.Revoke:
                            break;
                    }
                }
                else
                {
                    explain = "签退成功";
                }
                if (p.CheckForm == EmStudentCheckOnLogCheckForm.Face
                    && !string.IsNullOrEmpty(p.CheckMedium) && p.CheckOt > maxDate)
                {
                    checkMedium = AliyunOssUtil.GetAccessUrlHttps(p.CheckMedium);
                }
                if (p.CheckForm == EmStudentCheckOnLogCheckForm.Card)
                {
                    checkMedium = p.CheckMedium;
                }
                output.Add(new CheckOnLogGetPagingOutput()
                {
                    CheckForm = p.CheckForm,
                    CheckFormDesc = EmStudentCheckOnLogCheckForm.GetStudentCheckOnLogCheckFormDesc(p.CheckForm),
                    CheckOt = p.CheckOt,
                    CheckType = p.CheckType,
                    CheckTypeDesc = EmStudentCheckOnLogCheckType.GetStudentCheckOnLogCheckTypeDesc(p.CheckType),
                    StudentCheckOnLogId = p.Id,
                    StudentId = p.StudentId,
                    StudentName = student.Name,
                    CheckMedium = checkMedium,
                    CourseName = courseName,
                    DeClassTimesDesc = deClassTimesDesc
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<CheckOnLogGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> StudentAccountRechargeGet(StudentAccountRechargeGetRequest request)
        {
            var accountLogBucket = await _studentAccountRechargeCoreBLL.GetStudentAccountRechargeByPhone(request.LoginPhone);
            if (accountLogBucket == null || accountLogBucket.StudentAccountRecharge == null)
            {
                return ResponseBase.CommonError("账户不存在");
            }
            var accountLog = accountLogBucket.StudentAccountRecharge;
            var output = new StudentAccountRechargeGetOutput()
            {
                BalanceGiveDesc = accountLog.BalanceGive.ToString("F2"),
                Id = accountLog.Id,
                BalanceRealDesc = accountLog.BalanceReal.ToString("F2"),
                BalanceSumDesc = accountLog.BalanceSum.ToString("F2"),
                Ot = accountLog.Ot,
                Phone = accountLog.Phone,
                Students = new List<AccountRechargeBinder>()
            };
            if (accountLogBucket.Binders != null && accountLogBucket.Binders.Count > 0)
            {
                foreach (var p in accountLogBucket.Binders)
                {
                    output.Students.Add(new AccountRechargeBinder()
                    {
                        StudentAvatarUrl = p.StudentAvatarUrl,
                        StudentId = p.StudentId,
                        StudentName = p.StudentName,
                        StudentPhone = p.StudentPhone
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentAccountRechargeRuleGet(StudentAccountRechargeRuleGetRequest request)
        {
            var rechargeRuleView = await _appConfig2BLL.GetStudentAccountRechargeRule();
            return ResponseBase.Success(new StudentAccountRechargeRuleGetOutput()
            {
                Explain = rechargeRuleView.Explain,
                ImgUrlKeyUrl = UrlHelper.GetUrl(rechargeRuleView.ImgUrlKey),
            });
        }

        public async Task<ResponseBase> StudentAccountRechargeLogGetPaging(StudentAccountRechargeLogGetPagingRequest request)
        {
            var pagingData = await _studentAccountRechargeLogDAL.GetPaging(request);
            var output = new List<StudentAccountRechargeLogGetPagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new StudentAccountRechargeLogGetPagingOutput()
                {
                    CgNo = p.CgNo,
                    OtDesc = EtmsHelper.GetChangeLogTimeDesc(p.Ot),
                    StudentAccountRechargeId = p.StudentAccountRechargeId,
                    Type = p.Type,
                    TypeDesc = EmStudentAccountRechargeLogType.GetStudentAccountRechargeLogTypeDesc(p.Type),
                    CgBalanceRealDesc = EmStudentAccountRechargeLogType.GetValueDesc(p.CgBalanceReal, p.Type),
                    CgBalanceGiveDesc = EmStudentAccountRechargeLogType.GetValueDesc(p.CgBalanceGive, p.Type),
                    CgBalanceTotalDesc = EmStudentAccountRechargeLogType.GetValueDesc(p.CgBalanceTotal, p.Type),
                    ChangeType = EmStudentAccountRechargeLogType.GetValueChangeType(p.Type),
                    Id = p.Id
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentAccountRechargeLogGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> TeacherGetPaging(TeacherGetPagingRequest request)
        {
            var pagingData = await _userDAL.GetUserPaging(request);
            var output = new List<TeacherGetPagingOutput>();
            return ResponseBase.Success(new ResponsePagingDataBase<TeacherGetPagingOutput>(pagingData.Item2, pagingData.Item1.Select(p => new TeacherGetPagingOutput()
            {
                Id = p.Id,
                TeacherName = ComBusiness2.GetParentTeacherName(p.Name, p.NickName)
            })));
        }

        public async Task<ResponseBase> StudentReservationTimetable(StudentReservationTimetableRequest request)
        {
            var output = new List<StudentTimetableCountOutput>();
            var studentCourseIds = await _studentCourseDAL.GetStudentCourseIdAboutCanReserve(request.StudentId);
            if (studentCourseIds == null || studentCourseIds.Count == 0)
            {
                return ResponseBase.Success(output);
            }
            request.StudentCourseIds = studentCourseIds; //购买的课程
            var classTimeGroupCount = await _classTimesDAL.ClassTimesClassOtGroupCount(request);
            return ResponseBase.Success(classTimeGroupCount.Select(p => new StudentTimetableCountOutput()
            {
                ClassTimesCount = p.TotalCount,
                Date = p.ClassOt.EtmsToDateString()
            }));
        }

        private List<EtStudentLeaveApplyLog> _studentLeaves;

        private bool _isGetStudentLeaves;

        private StudentIsLeaveCheck _studentLeaveCheck;

        /// <summary>
        /// 获取一节课中学员的个数（移除请假的学员）
        /// </summary>
        /// <param name="myClassTimes"></param>
        /// <returns></returns>
        private async Task<int> GetClassTimesEffectiveStudentCount(EtClassTimes myClassTimes)
        {
            if (!_isGetStudentLeaves)
            {
                var allStudentLeaves = await _studentLeaveApplyLogDAL.GetStudentLeaveApplyPassLog(myClassTimes.ClassOt);
                _isGetStudentLeaves = true;
                _studentLeaveCheck = new StudentIsLeaveCheck(allStudentLeaves);
            }
            _studentLeaves = _studentLeaveCheck.GetStudentLeaveList(myClassTimes.StartTime, myClassTimes.EndTime, myClassTimes.ClassOt);
            if (_studentLeaves == null || _studentLeaves.Count == 0)
            {
                return myClassTimes.StudentCount;
            }
            var allStudents = new List<long>();
            if (!string.IsNullOrEmpty(myClassTimes.StudentIdsClass))
            {
                allStudents.AddRange(EtmsHelper.AnalyzeMuIds(myClassTimes.StudentIdsClass));
            }
            if (!string.IsNullOrEmpty(myClassTimes.StudentIdsTemp))
            {
                allStudents.AddRange(EtmsHelper.AnalyzeMuIds(myClassTimes.StudentIdsTemp));
            }
            if (!string.IsNullOrEmpty(myClassTimes.StudentIdsReservation))
            {
                allStudents.AddRange(EtmsHelper.AnalyzeMuIds(myClassTimes.StudentIdsReservation));
            }
            if (allStudents.Count == 0)
            {
                return myClassTimes.StudentCount;
            }
            var isMyLeavesCount = _studentLeaves.Where(p => allStudents.Exists(j => j == p.StudentId)).Select(p => p.StudentId).Distinct().Count();
            return myClassTimes.StudentCount - isMyLeavesCount;
        }

        private async Task<ClassTimesReservationLimit> CheckClassTimesReservationLimit(EtClassTimes myClassTimes, long studentId, DateTime now)
        {
            var result = new ClassTimesReservationLimit()
            {
                IsCanReservation = false,
                Status = EmStudentReservationTimetableOutputStatus.Normal,
                NewStudentCount = myClassTimes.StudentCount
            };
            var thisDate = now.Date;
            var thisTime = EtmsHelper.GetTimeHourAndMinuteDesc(now.AddMinutes(-1));//至少要提前一分钟预约
            if (myClassTimes.Status == EmClassTimesStatus.BeRollcall || myClassTimes.ClassOt < thisDate)
            {
                result.Status = EmStudentReservationTimetableOutputStatus.Over;
                result.CantReservationErrDesc = "课次已结束，无法预约";
            }
            if (thisDate == myClassTimes.ClassOt && myClassTimes.StartTime < thisTime)
            {
                if (myClassTimes.EndTime <= thisTime)
                {
                    result.Status = EmStudentReservationTimetableOutputStatus.Over;
                    result.CantReservationErrDesc = "课次已结束，无法预约";
                }
                else
                {
                    result.Status = EmStudentReservationTimetableOutputStatus.Over;
                    result.CantReservationErrDesc = "已开始上课，无法预约";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(myClassTimes.StudentIdsReservation) && myClassTimes.StudentIdsReservation.IndexOf($",{studentId},") != -1)
                {
                    result.Status = EmStudentReservationTimetableOutputStatus.IsReservationed;
                }
                else
                {
                    result.IsCanReservation = true;
                }
            }

            if (result.IsCanReservation)
            {
                if (!string.IsNullOrEmpty(myClassTimes.StudentIdsClass) && myClassTimes.StudentIdsClass.IndexOf($",{studentId},") != -1)
                {
                    result.IsCanReservation = false;
                    result.CantReservationErrDesc = "学员已在此班级，无法预约";
                }
                if (!string.IsNullOrEmpty(myClassTimes.StudentIdsTemp) && myClassTimes.StudentIdsTemp.IndexOf($",{studentId},") != -1)
                {
                    result.IsCanReservation = false;
                    result.CantReservationErrDesc = "学员已在此课次，无法预约";
                }
            }

            if (myClassTimes.LimitStudentNums == null)
            {
                result.StudentCountLimitDesc = "-";
                result.StudentCountSurplusDesc = "不限制";
            }
            else
            {
                var myStudentCount = await GetClassTimesEffectiveStudentCount(myClassTimes);
                result.NewStudentCount = myStudentCount;
                result.StudentCountLimitDesc = myClassTimes.LimitStudentNums.Value.ToString();
                if (myStudentCount >= myClassTimes.LimitStudentNums &&
                    myClassTimes.LimitStudentNumsType == EmLimitStudentNumsType.NotOverflow)
                {
                    result.IsCanReservation = false;
                    result.CantReservationErrDesc = "班级容量限制，无法预约";
                }
                if (myClassTimes.LimitStudentNumsType == EmLimitStudentNumsType.CanOverflow)
                {
                    result.StudentCountSurplusDesc = "不限制";
                }
                else
                {
                    if (myStudentCount >= myClassTimes.LimitStudentNums)
                    {
                        result.StudentCountSurplusDesc = "0";
                    }
                    else
                    {
                        result.StudentCountSurplusDesc = (myClassTimes.LimitStudentNums.Value - myStudentCount).ToString();
                    }
                }

            }
            return result;
        }

        public async Task<ResponseBase> StudentReservationTimetableDetail(StudentReservationTimetableDetailRequest request)
        {
            var output = new List<StudentReservationTimetableDetailOutput>();
            var studentCourseIds = await _studentCourseDAL.GetStudentCourseIdAboutCanReserve(request.StudentId);
            if (studentCourseIds == null || studentCourseIds.Count == 0)
            {
                return ResponseBase.Success(output);
            }
            request.StudentCourseIds = studentCourseIds; //购买的课程
            var classTimesList = await _classTimesDAL.GetClassTimes(request);
            if (classTimesList.Any())
            {
                var tempBoxCourse = new DataTempBox<EtCourse>();
                var tempClass = new DataTempBox<EtClass>();
                var tempBoxUser = new DataTempBox<EtUser>();
                var now = DateTime.Now;
                foreach (var myClassTimes in classTimesList)
                {
                    var etClass = await ComBusiness.GetClass(tempClass, _classDAL, myClassTimes.ClassId);
                    var courseInfo = await ComBusiness.GetCourseNameAndColor(tempBoxCourse, _courseDAL, myClassTimes.CourseList);
                    if (etClass == null)
                    {
                        continue;
                    }
                    var courseStyleColor = courseInfo.Item2;
                    if (string.IsNullOrEmpty(courseStyleColor))
                    {
                        courseStyleColor = SystemConfig.ComConfig.CourseDefaultStyleColor;
                    }
                    var reservationLimit = await CheckClassTimesReservationLimit(myClassTimes, request.StudentId, now);
                    output.Add(new StudentReservationTimetableDetailOutput()
                    {
                        ClassName = etClass.Name,
                        ClassTimesId = myClassTimes.Id,
                        Color = courseStyleColor,
                        CourseListDesc = courseInfo.Item1,
                        EndTimeDesc = EtmsHelper.GetTimeDesc(myClassTimes.EndTime),
                        StartTimeDesc = EtmsHelper.GetTimeDesc(myClassTimes.StartTime),
                        TeachersDesc = await ComBusiness.GetParentTeachers(tempBoxUser, _userDAL, myClassTimes.Teachers, "无"),
                        Status = reservationLimit.Status,
                        StatusDesc = EmStudentReservationTimetableOutputStatus.GetStudentReservationTimetableOutputStatusDesc(reservationLimit.Status),
                        IsCanReservation = reservationLimit.IsCanReservation,
                        StudentCountFinish = reservationLimit.NewStudentCount,
                        StudentCountLimitDesc = reservationLimit.StudentCountLimitDesc,
                        StartTime = myClassTimes.StartTime,
                        EndTime = myClassTimes.EndTime
                    });
                }
            }
            return ResponseBase.Success(output.OrderByDescending(p => p.IsCanReservation).ThenBy(p => p.StartTime));
        }

        private async Task<ClassTimesReservationLimit2> GetCheckClassTimesReservationLimit2(EtClassTimes classTimes, long studentId, DateTime now)
        {
            var result = new ClassTimesReservationLimit2();
            var classDateTime = EtmsHelper.GetTime(classTimes.ClassOt, classTimes.StartTime);
            var classTimesCourseIds = EtmsHelper.AnalyzeMuIds(classTimes.CourseList);
            var cantReservationErrDesc = string.Empty;
            var courseId = 0L;
            var reservationLimit = await CheckClassTimesReservationLimit(classTimes, studentId, now);
            cantReservationErrDesc = reservationLimit.CantReservationErrDesc;

            var diffTime = classDateTime - now;
            var diffTotalMinutes = diffTime.TotalMinutes;
            if (reservationLimit.IsCanReservation)
            {
                //默认1分种之内无法预约
                if (diffTotalMinutes <= 1)
                {
                    reservationLimit.IsCanReservation = false;
                    cantReservationErrDesc = "截止预约时间限制，无法预约";
                }
            }

            if (reservationLimit.IsCanReservation)
            {
                //判断购买的课程 课时是否足够
                var isHasSurplusCourse = false;
                var myCourse = await _studentCourseDAL.GetStudentCourse(studentId);
                foreach (var id in classTimesCourseIds)
                {
                    var myCourseDetail = myCourse.Where(p => p.CourseId == id);
                    if (myCourseDetail.Any())
                    {
                        if (ComBusiness3.CheckStudentCourseHasSurplus(myCourseDetail))
                        {
                            isHasSurplusCourse = true;
                            courseId = id; //课预约的课程
                            break;
                        }
                    }
                }

                if (!isHasSurplusCourse)
                {
                    reservationLimit.IsCanReservation = false;
                    cantReservationErrDesc = "学员课程剩余课时不足，无法预约";
                }
            }

            #region 预约配置
            var ruleConfig = await _appConfig2BLL.GetClassReservationSetting();
            if (ruleConfig.StartClassReservaLimitType != EmStartClassReservaLimitType.NotLimit) //开始预约时间
            {
                switch (ruleConfig.StartClassReservaLimitType)
                {
                    case EmStartClassReservaLimitType.LimitHour:
                        var tempLimitMinutes = ruleConfig.StartClassReservaLimitValue * 60;
                        if (diffTotalMinutes > tempLimitMinutes)
                        {
                            reservationLimit.IsCanReservation = false;
                            cantReservationErrDesc = "开始预约时间限制，无法预约";
                        }
                        result.RuleStartClassReservaLimitDesc = $"上课前{ruleConfig.StartClassReservaLimitValue}小时内可以预约";
                        break;
                    case EmStartClassReservaLimitType.LimitDay:
                        var startReservationDate = classTimes.ClassOt.AddDays(-ruleConfig.StartClassReservaLimitValue).Date;
                        if (now < startReservationDate)
                        {
                            reservationLimit.IsCanReservation = false;
                            cantReservationErrDesc = "开始预约时间限制，无法预约";
                        }
                        if (now.Date == startReservationDate && ruleConfig.StartClassReservaLimitTimeValue > 0) //同一天 判断时间
                        {
                            if (EtmsHelper.GetTimeHourAndMinuteDesc(now) < ruleConfig.StartClassReservaLimitTimeValue)
                            {
                                reservationLimit.IsCanReservation = false;
                                cantReservationErrDesc = "开始预约时间限制，无法预约";
                            }
                        }
                        if (ruleConfig.StartClassReservaLimitTimeValue > 0)
                        {
                            result.RuleStartClassReservaLimitDesc = $"上课前{ruleConfig.StartClassReservaLimitValue}天的{ruleConfig.StartClassReservaLimitTimeValueDesc}后可预约";
                        }
                        else
                        {
                            result.RuleStartClassReservaLimitDesc = $"上课前{ruleConfig.StartClassReservaLimitValue}天内可以预约";
                        }
                        break;
                }
            }

            if (ruleConfig.DeadlineClassReservaLimitType != EmDeadlineClassReservaLimitType.NotLimit)//截止预约时间
            {
                switch (ruleConfig.DeadlineClassReservaLimitType)
                {
                    case EmDeadlineClassReservaLimitType.LimitMinute:
                        if (diffTotalMinutes < ruleConfig.DeadlineClassReservaLimitValue)
                        {
                            reservationLimit.IsCanReservation = false;
                            cantReservationErrDesc = "截止预约时间限制，无法预约";
                        }
                        result.RuleDeadlineClassReservaLimitDesc = $"上课前{ruleConfig.DeadlineClassReservaLimitValue}分钟截止预约";
                        break;
                    case EmDeadlineClassReservaLimitType.LimitHour:
                        var tempLimitHourValue = ruleConfig.DeadlineClassReservaLimitValue * 60;
                        if (diffTotalMinutes < tempLimitHourValue)
                        {
                            reservationLimit.IsCanReservation = false;
                            cantReservationErrDesc = "截止预约时间限制，无法预约";
                        }
                        result.RuleDeadlineClassReservaLimitDesc = $"上课前{ruleConfig.DeadlineClassReservaLimitValue}小时截止预约";
                        break;
                    case EmDeadlineClassReservaLimitType.LimitDay:
                        var limitReservaDateTime = EtmsHelper.GetTime(classTimes.ClassOt.AddDays(-ruleConfig.DeadlineClassReservaLimitValue),
                            ruleConfig.DeadlineClassReservaLimitDayTimeValue);
                        if (now > limitReservaDateTime)
                        {
                            reservationLimit.IsCanReservation = false;
                            cantReservationErrDesc = "截止预约时间限制，无法预约";
                        }
                        result.RuleDeadlineClassReservaLimitDesc = $"上课前{ruleConfig.DeadlineClassReservaLimitValue}天的{EtmsHelper.GetTimeDesc(ruleConfig.DeadlineClassReservaLimitDayTimeValue)}截止预约";
                        break;
                }
            }

            if (ruleConfig.MaxCountClassReservaLimitType != EmMaxCountClassReservaLimitType.NotLimit) //预约次数限制
            {
                var sameCount = await _classTimesDAL.ClassTimesReservationLogGetCount(courseId, studentId, now);
                if (sameCount >= ruleConfig.MaxCountClassReservaLimitValue)
                {
                    reservationLimit.IsCanReservation = false;
                    cantReservationErrDesc = "预约次数限制，无法预约";
                }
                result.RuleMaxCountClassReservaLimitDesc = $"同一门课程最多可约{ruleConfig.MaxCountClassReservaLimitValue}次";
            }

            switch (ruleConfig.CancelClassReservaType)
            {
                case EmCancelClassReservaType.LimitMinute:
                    result.RuleCancelClassReservaDesc = $"必须提前{ruleConfig.CancelClassReservaValue}分钟才能取消已预约的课次";
                    break;
                case EmCancelClassReservaType.LimitHour:
                    result.RuleCancelClassReservaDesc = $"必须提前{ruleConfig.CancelClassReservaValue}小时才能取消已预约的课次";
                    break;
                case EmCancelClassReservaType.LimitDay:
                    result.RuleCancelClassReservaDesc = $"必须提前{ruleConfig.CancelClassReservaValue}天才能取消已预约的课次";
                    break;
            }
            #endregion

            if (reservationLimit.Status == EmStudentReservationTimetableOutputStatus.IsReservationed)
            {
                DateTime tempTime = classDateTime;
                switch (ruleConfig.CancelClassReservaType)
                {
                    case EmCancelClassReservaType.LimitMinute:
                        tempTime = classDateTime.AddMinutes(-ruleConfig.CancelClassReservaValue);
                        break;
                    case EmCancelClassReservaType.LimitHour:
                        tempTime = classDateTime.AddHours(-ruleConfig.CancelClassReservaValue);
                        break;
                    case EmCancelClassReservaType.LimitDay:
                        tempTime = classDateTime.AddDays(-ruleConfig.CancelClassReservaValue);
                        break;
                }
                result.CancelDesc = $"{tempTime.ToString("MM月dd日 HH:mm前可取消")}";
                result.IsCanCancel = tempTime > now;
            }

            result.Status = reservationLimit.Status;
            result.IsCanReservation = reservationLimit.IsCanReservation;
            result.StudentCountLimitDesc = reservationLimit.StudentCountLimitDesc;
            result.StudentCountSurplusDesc = reservationLimit.StudentCountSurplusDesc;
            result.CourseId = courseId;
            result.CantReservationErrDesc = cantReservationErrDesc;
            result.NewStudentCount = reservationLimit.NewStudentCount;
            return result;
        }

        public async Task<ResponseBase> StudentReservationDetail(StudentReservationDetailRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var classTimes = await _classTimesDAL.GetClassTimes(request.ClassTimesId);
            if (classTimes == null)
            {
                return ResponseBase.CommonError("课次不存在");
            }
            var etClass = await _classDAL.GetClassBucket(classTimes.ClassId);
            if (etClass == null || etClass.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            var now = DateTime.Now;
            var limitResult = await GetCheckClassTimesReservationLimit2(classTimes, request.StudentId, now);

            StudentReservationSuccess reservationSuccess = null;
            if (limitResult.Status == EmStudentReservationTimetableOutputStatus.IsReservationed)
            {
                reservationSuccess = new StudentReservationSuccess();
                reservationSuccess.StudentName = studentBucket.Student.Name;
                reservationSuccess.StudentAvatarUrl = UrlHelper.GetUrl(_httpContextAccessor,
                    _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, studentBucket.Student.Avatar);
                reservationSuccess.CancelDesc = limitResult.CancelDesc;
            }

            var classRoomIdsDesc = "未安排教室";
            if (!string.IsNullOrEmpty(classTimes.ClassRoomIds))
            {
                var allClassRoom = await _classRoomDAL.GetAllClassRoom();
                classRoomIdsDesc = ComBusiness.GetDesc(allClassRoom, classTimes.ClassRoomIds, "未安排教室");
            }
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var tempBoxUser = new DataTempBox<EtUser>();
            var output = new StudentReservationDetailOutput()
            {
                ClassType = classTimes.ClassType,
                ClassName = etClass.EtClass.Name,
                CantReservationErrDesc = limitResult.CantReservationErrDesc,
                ClassContent = classTimes.ClassContent,
                ClassId = classTimes.ClassId,
                ClassOt = classTimes.ClassOt.EtmsToDateString(),
                ClassRoomIdsDesc = classRoomIdsDesc,
                ClassTimesId = classTimes.Id,
                CourseListDesc = await ComBusiness.GetCourseNames(tempBoxCourse, _courseDAL, classTimes.CourseList),
                IsCanReservation = limitResult.IsCanReservation,
                StudentCountFinish = limitResult.NewStudentCount,
                StudentCountLimitDesc = limitResult.StudentCountLimitDesc,
                StudentCountSurplusDesc = limitResult.StudentCountSurplusDesc,
                TeachersDesc = await ComBusiness.GetParentTeachers(tempBoxUser, _userDAL, classTimes.Teachers, "未安排老师"),
                TimeDesc = $"{EtmsHelper.GetTimeDesc(classTimes.StartTime)}~{EtmsHelper.GetTimeDesc(classTimes.EndTime)}",
                WeekDesc = $"周{EtmsHelper.GetWeekDesc(classTimes.Week)}",
                RuleCancelClassReservaDesc = limitResult.RuleCancelClassReservaDesc,
                RuleDeadlineClassReservaLimitDesc = limitResult.RuleDeadlineClassReservaLimitDesc,
                RuleMaxCountClassReservaLimitDesc = limitResult.RuleMaxCountClassReservaLimitDesc,
                RuleStartClassReservaLimitDesc = limitResult.RuleStartClassReservaLimitDesc,
                CourseId = limitResult.CourseId,
                Status = limitResult.Status,
                StudentReservationSuccess = reservationSuccess,
                IsCanCancel = limitResult.IsCanCancel,
                CancelDesc = limitResult.CancelDesc
            };

            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentReservationLogGetPaging(StudentReservationLogGetPagingRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var pagingData = await _classTimesDAL.GetPaging(request);
            var output = new List<StudentReservationLogGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var allClassRoom = await _classRoomDAL.GetAllClassRoom();
                var tempBoxCourse = new DataTempBox<EtCourse>();
                var tempBoxUser = new DataTempBox<EtUser>();
                var tempBoxClass = new DataTempBox<EtClass>();
                foreach (var classTimes in pagingData.Item1)
                {
                    var etClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, classTimes.ClassId);
                    var studentCountLimitDesc = string.Empty;
                    var studentCountSurplusDesc = string.Empty;
                    if (classTimes.LimitStudentNums == null)
                    {
                        studentCountLimitDesc = "-";
                        studentCountSurplusDesc = "不限制";
                    }
                    else
                    {
                        studentCountLimitDesc = classTimes.LimitStudentNums.Value.ToString();
                        if (classTimes.LimitStudentNumsType == EmLimitStudentNumsType.CanOverflow)
                        {
                            studentCountSurplusDesc = "不限制";
                        }
                        else
                        {
                            if (classTimes.StudentCount >= classTimes.LimitStudentNums)
                            {
                                studentCountSurplusDesc = "0";
                            }
                            else
                            {
                                studentCountSurplusDesc = (classTimes.LimitStudentNums.Value - classTimes.StudentCount).ToString();
                            }
                        }

                    }
                    output.Add(new StudentReservationLogGetPagingOutput()
                    {
                        ClassContent = classTimes.ClassContent,
                        ClassId = classTimes.ClassId,
                        ClassType = classTimes.ClassType,
                        ClassName = etClass?.Name,
                        ClassOt = classTimes.ClassOt.EtmsToDateString(),
                        ClassRoomIdsDesc = ComBusiness.GetDesc(allClassRoom, classTimes.ClassRoomIds),
                        ClassTimesId = classTimes.Id,
                        CourseListDesc = await ComBusiness.GetCourseNames(tempBoxCourse, _courseDAL, classTimes.CourseList),
                        TimeDesc = $"{EtmsHelper.GetTimeDesc(classTimes.StartTime)}~{EtmsHelper.GetTimeDesc(classTimes.EndTime)}",
                        WeekDesc = $"周{EtmsHelper.GetWeekDesc(classTimes.Week)}",
                        TeachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, classTimes.Teachers),
                        StudentName = studentBucket.Student.Name,
                        StudentAvatarUrl = UrlHelper.GetUrl(_httpContextAccessor,
                        _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, studentBucket.Student.Avatar),
                        StudentCountLimitDesc = studentCountLimitDesc,
                        StudentCountSurplusDesc = studentCountSurplusDesc,
                        StudentCountFinish = classTimes.StudentCount
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentReservationLogGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> StudentReservationLogGetPaging2(StudentReservationLogGetPaging2Request request)
        {
            var output = new List<StudentReservationLogGetPaging2Output>();
            var pagingData = await _classTimesDAL.ReservationLogGetPaging(request);
            if (pagingData.Item1.Any())
            {
                var tempBoxCourse = new DataTempBox<EtCourse>();
                var tempBoxClass = new DataTempBox<EtClass>();
                var now = DateTime.Now.Date;
                foreach (var log in pagingData.Item1)
                {
                    var etClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, log.ClassId);
                    if (etClass == null)
                    {
                        continue;
                    }
                    output.Add(new StudentReservationLogGetPaging2Output()
                    {
                        ClassId = log.ClassId,
                        ClassName = etClass.Name,
                        ClassType = etClass.Type,
                        ClassOt = log.ClassOt.EtmsToDateString(),
                        ClassTimesId = log.ClassTimesId,
                        CourseId = log.CourseId,
                        CourseName = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, log.CourseId),
                        CreateOt = log.CreateOt,
                        EndTime = log.EndTime,
                        Id = log.Id,
                        RuleId = log.RuleId,
                        StartTime = log.StartTime,
                        Status = EmClassTimesReservationLogStatus.GetClassTimesReservationLogStatus(log.Status, now, log.ClassOt),
                        StatusDesc = EmClassTimesReservationLogStatus.GetClassTimesReservationLogStatusDesc(log.Status, now, log.ClassOt),
                        TimeDesc = $"{EtmsHelper.GetTimeDesc(log.StartTime)}~{EtmsHelper.GetTimeDesc(log.EndTime)}",
                        Week = log.Week,
                        WeekDesc = $"周{EtmsHelper.GetWeekDesc(log.Week)}"
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentReservationLogGetPaging2Output>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> StudentReservationSubmit(StudentReservationSubmitRequest request)
        {
            //可以考虑加一把锁 ParentStudentReservationSubmitToken
            return await ProcessStudentReservationSubmit(request);
        }

        private async Task<ResponseBase> ProcessStudentReservationSubmit(StudentReservationSubmitRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var classTimes = await _classTimesDAL.GetClassTimes(request.ClassTimesId);
            if (classTimes == null)
            {
                return ResponseBase.CommonError("课次不存在");
            }
            var etClass = await _classDAL.GetClassBucket(classTimes.ClassId);
            if (etClass == null || etClass.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            var now = DateTime.Now;
            var limitResult = await GetCheckClassTimesReservationLimit2(classTimes, request.StudentId, now);
            if (!limitResult.IsCanReservation)
            {
                var err = limitResult.CantReservationErrDesc;
                if (string.IsNullOrEmpty(err))
                {
                    err = "无法预约此课次";
                }
                return ResponseBase.CommonError(err);
            }
            string[] studentIdsClass = null;
            string[] studentIdsTemp = null;
            string[] studentIdsReservation = null;
            if (!string.IsNullOrEmpty(classTimes.StudentIdsClass))
            {
                studentIdsClass = classTimes.StudentIdsClass.Split(',');
            }
            if (!string.IsNullOrEmpty(classTimes.StudentIdsTemp))
            {
                studentIdsTemp = classTimes.StudentIdsTemp.Split(',');
            }
            if (!string.IsNullOrEmpty(classTimes.StudentIdsReservation))
            {
                studentIdsReservation = classTimes.StudentIdsReservation.Split(',');
            }
            var isInClassTimes = ComBusiness3.CheckStudentInClassTimes(studentIdsClass, studentIdsTemp, studentIdsReservation, request.StudentId);
            if (isInClassTimes)
            {
                return ResponseBase.CommonError("您已在此课次中");
            }
            var classTimeStudent = new EtClassTimesStudent()
            {
                IsDeleted = EmIsDeleted.Normal,
                ClassId = classTimes.ClassId,
                ClassOt = classTimes.ClassOt,
                ClassTimesId = classTimes.Id,
                CourseId = request.CourseId,
                Remark = "预约上课",
                RuleId = classTimes.RuleId,
                Status = classTimes.Status,
                StudentId = request.StudentId,
                StudentTryCalssLogId = null,
                StudentType = EmClassStudentType.TempStudent,
                TenantId = classTimes.TenantId,
                IsReservation = EmBool.True
            };
            await _classTimesDAL.AddClassTimesStudent(classTimeStudent);
            if (string.IsNullOrEmpty(classTimes.StudentIdsReservation))
            {
                classTimes.StudentIdsReservation = $",{request.StudentId},";
            }
            else
            {
                classTimes.StudentIdsReservation = $"{classTimes.StudentIdsReservation}{request.StudentId},";
            }
            await _classTimesDAL.EditClassTimes(classTimes);

            await _classTimesDAL.ClassTimesReservationLogAdd(new EtClassTimesReservationLog()
            {
                StudentId = request.StudentId,
                ClassTimesId = classTimes.Id,
                ClassId = classTimes.ClassId,
                CourseId = request.CourseId,
                ClassOt = classTimes.ClassOt,
                IsDeleted = classTimes.IsDeleted,
                StartTime = classTimes.StartTime,
                Week = classTimes.Week,
                RuleId = classTimes.RuleId,
                Status = EmClassTimesReservationLogStatus.Normal,
                EndTime = classTimes.EndTime,
                TenantId = classTimes.TenantId,
                CreateOt = DateTime.Now
            });

            _eventPublisher.Publish(new SyncClassTimesStudentEvent(request.LoginTenantId)
            {
                ClassTimesId = request.ClassTimesId
            });
            _eventPublisher.Publish(new NoticeStudentReservationEvent(request.LoginTenantId)
            {
                ClassTimesStudent = classTimeStudent,
                OpType = NoticeStudentReservationOpType.Success
            });

            await _studentOperationLogDAL.AddStudentLog(request.StudentId, request.LoginTenantId, $"预约上课-班级:{etClass.EtClass.Name},课次:{classTimes.ClassOt.EtmsToDateString()}({EtmsHelper.GetTimeDesc(classTimes.StartTime, classTimes.EndTime)})", EmStudentOperationLogType.StudentReservation);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentReservationCancel(StudentReservationCancelRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var classTimes = await _classTimesDAL.GetClassTimes(request.ClassTimesId);
            if (classTimes == null)
            {
                return ResponseBase.CommonError("课次不存在");
            }
            var classBucket = await _classDAL.GetClassBucket(classTimes.ClassId);
            if (classBucket == null || classBucket.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            if (classBucket.EtClass.Type == EmClassType.OneToMany)
            {
                return await StudentReservationCancel1vN(request, classBucket, classTimes);
            }
            else
            {
                return await StudentReservationCancel1v1(request, classBucket, classTimes);
            }
        }

        public async Task<ResponseBase> StudentReservationCancel1vN(StudentReservationCancelRequest request,
            ClassBucket classBucket, EtClassTimes classTimes)
        {
            var classTimesStudents = await _classTimesDAL.GetClassTimesStudent(request.ClassTimesId);
            var reservationLog = classTimesStudents.FirstOrDefault(p => p.StudentId == request.StudentId && p.IsReservation == EmBool.True);
            if (reservationLog == null)
            {
                return ResponseBase.CommonError("未查找到预约课次");
            }
            var now = DateTime.Now;
            var limitResult = await GetCheckClassTimesReservationLimit2(classTimes, request.StudentId, now);
            if (!limitResult.IsCanCancel)
            {
                return ResponseBase.CommonError("临近上课时间，无法取消预约课次");
            }

            await _classTimesDAL.DelClassTimesStudent(reservationLog.Id);
            classTimesStudents.Remove(reservationLog);
            var surplusReservation = classTimesStudents.Where(p => p.IsReservation == EmBool.True);
            var studentIdsReservation = string.Empty;
            if (surplusReservation != null && surplusReservation.Any())
            {
                studentIdsReservation = EtmsHelper.GetMuIds(surplusReservation.Select(p => p.StudentId));
            }
            classTimes.StudentIdsReservation = studentIdsReservation;
            await _classTimesDAL.EditClassTimes(classTimes);

            await _classTimesDAL.ClassTimesReservationLogSetCancel(request.ClassTimesId, request.StudentId);

            _eventPublisher.Publish(new SyncClassTimesStudentEvent(request.LoginTenantId)
            {
                ClassTimesId = request.ClassTimesId
            });
            _eventPublisher.Publish(new NoticeStudentReservationEvent(request.LoginTenantId)
            {
                ClassTimesStudent = reservationLog,
                OpType = NoticeStudentReservationOpType.Cancel
            });

            await _studentOperationLogDAL.AddStudentLog(request.StudentId, request.LoginTenantId, $"取消约课-班级:{classBucket.EtClass.Name}，课次:{classTimes.ClassOt.EtmsToDateString()}({EtmsHelper.GetTimeDesc(classTimes.StartTime, classTimes.EndTime)})", EmStudentOperationLogType.StudentReservation);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentReservationCancel1v1(StudentReservationCancelRequest request,
            ClassBucket classBucket, EtClassTimes classTimes)
        {
            var now = DateTime.Now;
            var limitResult = await GetCheckClassTimesReservationLimit2(classTimes, request.StudentId, now);
            if (!limitResult.IsCanCancel)
            {
                return ResponseBase.CommonError("临近上课时间，无法取消预约课次");
            }

            await _classTimesDAL.DelClassTimes(classTimes.Id);

            await _classTimesDAL.ClassTimesReservationLogSetCancel(request.ClassTimesId, request.StudentId);

            var tempClassTimesStudent = new EtClassTimesStudent()
            {
                ClassOt = classTimes.ClassOt,
                ClassId = classTimes.ClassId,
                ClassTimesId = classTimes.Id,
                CourseId = EtmsHelper.AnalyzeMuIds(classTimes.CourseList)[0],
                IsDeleted = EmIsDeleted.Normal,
                IsReservation = EmBool.True,
                RuleId = 0,
                Status = EmClassTimesStatus.UnRollcall,
                StudentId = request.StudentId,
                StudentTryCalssLogId = null,
                StudentType = EmClassStudentType.ClassStudent,
                TenantId = request.LoginTenantId
            };
            _eventPublisher.Publish(new NoticeStudentReservationEvent(request.LoginTenantId)
            {
                ClassTimesStudent = tempClassTimesStudent,
                OpType = NoticeStudentReservationOpType.Cancel
            });

            await _studentOperationLogDAL.AddStudentLog(request.StudentId, request.LoginTenantId, $"取消约课-班级:{classBucket.EtClass.Name}，课次:{classTimes.ClassOt.EtmsToDateString()}({EtmsHelper.GetTimeDesc(classTimes.StartTime, classTimes.EndTime)})", EmStudentOperationLogType.StudentReservation);
            return ResponseBase.Success();
        }
    }
}
