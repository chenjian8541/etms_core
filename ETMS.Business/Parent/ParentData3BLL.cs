using ETMS.Business.Common;
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

        private readonly IStudentAccountRechargeDAL _studentAccountRechargeDAL;

        private readonly IStudentAccountRechargeLogDAL _studentAccountRechargeLogDAL;

        private readonly IAppConfig2BLL _appConfig2BLL;

        private readonly IUserDAL _userDAL;

        private readonly IClassTimesDAL _classTimesDAL;

        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly IClassDAL _classDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IClassRoomDAL _classRoomDAL;

        public ParentData3BLL(IActiveWxMessageDAL activeWxMessageDAL, IStudentDAL studentDAL, IActiveWxMessageParentReadDAL activeWxMessageParentReadDAL,
            IActiveGrowthRecordDAL activeGrowthRecordDAL, ITryCalssApplyLogDAL tryCalssApplyLogDAL, IStudentCheckOnLogDAL studentCheckOnLogDAL,
            IEventPublisher eventPublisher, IStudentAccountRechargeDAL studentAccountRechargeDAL, IStudentAccountRechargeLogDAL studentAccountRechargeLogDAL,
           IAppConfig2BLL appConfig2BLL, IUserDAL userDAL, IClassTimesDAL classTimesDAL, IStudentCourseDAL studentCourseDAL,
           IClassDAL classDAL, ICourseDAL courseDAL, IClassRoomDAL classRoomDAL)
        {
            this._activeWxMessageDAL = activeWxMessageDAL;
            this._studentDAL = studentDAL;
            this._activeWxMessageParentReadDAL = activeWxMessageParentReadDAL;
            this._activeGrowthRecordDAL = activeGrowthRecordDAL;
            this._tryCalssApplyLogDAL = tryCalssApplyLogDAL;
            this._studentCheckOnLogDAL = studentCheckOnLogDAL;
            this._eventPublisher = eventPublisher;
            this._studentAccountRechargeDAL = studentAccountRechargeDAL;
            this._studentAccountRechargeLogDAL = studentAccountRechargeLogDAL;
            this._appConfig2BLL = appConfig2BLL;
            this._userDAL = userDAL;
            this._classTimesDAL = classTimesDAL;
            this._studentCourseDAL = studentCourseDAL;
            this._classDAL = classDAL;
            this._courseDAL = courseDAL;
            this._classRoomDAL = classRoomDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._appConfig2BLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _activeWxMessageDAL, _studentDAL, _activeWxMessageParentReadDAL, _activeGrowthRecordDAL,
                _tryCalssApplyLogDAL, _studentCheckOnLogDAL, _studentAccountRechargeDAL, _studentAccountRechargeLogDAL,
                _userDAL, _classTimesDAL, _studentCourseDAL, _classDAL, _courseDAL, _classRoomDAL);
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
            await _tryCalssApplyLogDAL.AddTryCalssApplyLog(new EtTryCalssApplyLog()
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
                SourceType = EmTryCalssSourceType.WeChat,
                StudentId = null,
                TenantId = request.TenantId,
                TouristName = request.Name,
                TouristRemark = request.Remark
            });

            _eventPublisher.Publish(new ResetTenantToDoThingEvent(log.TenantId));
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> CheckOnLogGet(CheckOnLogGetRequest request)
        {
            var log = await _studentCheckOnLogDAL.GetStudentCheckOnLog(request.StudentCheckOnLogId);
            if (log == null)
            {
                return ResponseBase.CommonError("考勤记录不存在");
            }
            return ResponseBase.Success(new CheckOnLogGetOutput()
            {
                CheckOt = log.CheckOt,
                CheckMediumUrl = AliyunOssUtil.GetAccessUrlHttps(log.CheckMedium),
                CheckType = log.CheckType,
                CheckTypeDesc = EmStudentCheckOnLogCheckType.GetStudentCheckOnLogCheckTypeDesc(log.CheckType)
            });
        }

        public async Task<ResponseBase> StudentAccountRechargeGet(StudentAccountRechargeGetRequest request)
        {
            var accountLog = await _studentAccountRechargeDAL.GetStudentAccountRecharge(request.Id);
            if (accountLog == null)
            {
                return ResponseBase.CommonError("账户不存在");
            }
            return ResponseBase.Success(new StudentAccountRechargeGetOutput()
            {
                BalanceGiveDesc = accountLog.BalanceGive.ToString("F2"),
                Id = accountLog.Id,
                BalanceRealDesc = accountLog.BalanceReal.ToString("F2"),
                BalanceSumDesc = accountLog.BalanceSum.ToString("F2"),
                Ot = accountLog.Ot,
                Phone = accountLog.Phone
            });
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
            var output = new List<StudentReservationTimetableOutput>();
            var studentCourseIds = await _studentCourseDAL.GetStudentCourseId(request.StudentId);
            if (studentCourseIds == null || studentCourseIds.Count == 0)
            {
                return ResponseBase.Success(output);
            }
            request.StudentCourseIds = studentCourseIds; //购买的课程
            var classTimeGroupCount = await _classTimesDAL.ClassTimesClassOtGroupCount(request);
            return ResponseBase.Success(classTimeGroupCount.Select(p => new StudentReservationTimetableOutput()
            {
                ClassTimesCount = p.TotalCount,
                Date = p.ClassOt.EtmsToDateString()
            }));
        }

        private ClassTimesReservationLimit CheckClassTimesReservationLimit(EtClassTimes myClassTimes, long studentId, DateTime now)
        {
            var result = new ClassTimesReservationLimit()
            {
                IsCanReservation = false,
                Status = EmStudentReservationTimetableOutputStatus.Normal
            };
            if (myClassTimes.Status == EmClassTimesStatus.BeRollcall || myClassTimes.ClassOt < now)
            {
                result.Status = EmStudentReservationTimetableOutputStatus.Over;
            }
            else
            {
                if (!string.IsNullOrEmpty(myClassTimes.StudentIdsReservation)
                    && myClassTimes.StudentIdsReservation.IndexOf($",{studentId},") != -1)
                {
                    result.Status = EmStudentReservationTimetableOutputStatus.IsReservationed;
                }
                else
                {
                    result.IsCanReservation = true;
                }
            }

            if (myClassTimes.LimitStudentNums == null)
            {
                result.StudentCountLimitDesc = "-";
                result.StudentCountSurplusDesc = "不限制";
            }
            else
            {
                result.StudentCountLimitDesc = myClassTimes.LimitStudentNums.Value.ToString();
                if (myClassTimes.StudentCount >= myClassTimes.LimitStudentNums &&
                    myClassTimes.LimitStudentNumsType == EmLimitStudentNumsType.NotOverflow)
                {
                    result.IsCanReservation = false;
                }
                if (myClassTimes.LimitStudentNumsType == EmLimitStudentNumsType.CanOverflow)
                {
                    result.StudentCountSurplusDesc = "不限制";
                }
                else
                {
                    if (myClassTimes.StudentCount >= myClassTimes.LimitStudentNums)
                    {
                        result.StudentCountSurplusDesc = "0";
                    }
                    else
                    {
                        result.StudentCountSurplusDesc = (myClassTimes.LimitStudentNums.Value - myClassTimes.StudentCount).ToString();
                    }
                }

            }
            return result;
        }

        public async Task<ResponseBase> StudentReservationTimetableDetail(StudentReservationTimetableDetailRequest request)
        {
            var output = new List<StudentReservationTimetableDetailOutput>();
            var studentCourseIds = await _studentCourseDAL.GetStudentCourseId(request.StudentId);
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
                var now = DateTime.Now.Date;
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
                    var reservationLimit = CheckClassTimesReservationLimit(myClassTimes, request.StudentId, now);
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
                        StudentCountFinish = myClassTimes.StudentCount,
                        StudentCountLimitDesc = reservationLimit.StudentCountLimitDesc
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentReservationDetail(StudentReservationDetailRequest request)
        {
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

            var classTimesCourseIds = EtmsHelper.AnalyzeMuIds(classTimes.CourseList);
            var myCourse = await _studentCourseDAL.GetStudentCourse(request.StudentId);
            var isHasSurplusCourse = false;
            var cantReservationErrDesc = string.Empty;
            foreach (var id in classTimesCourseIds)
            {
                var myCourseDetail = myCourse.Where(p => p.CourseId == id);
                if (myCourseDetail.Any())
                {
                    if (ComBusiness3.CheckStudentCourseHasSurplus(myCourseDetail))
                    {
                        isHasSurplusCourse = true;
                        break;
                    }
                }
            }
            var now = DateTime.Now;
            var reservationLimit = CheckClassTimesReservationLimit(classTimes, request.StudentId, now);
            if (!isHasSurplusCourse)
            {
                cantReservationErrDesc = "学员课程剩余课时不足";
                reservationLimit.IsCanReservation = false;
            }

            var ruleConfig = await _appConfig2BLL.GetClassReservationSetting();
            if (reservationLimit.IsCanReservation)
            {
                //通过约课设置 判断是否可以预约

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
                ClassName = etClass.EtClass.Name,
                CantReservationErrDesc = cantReservationErrDesc,
                ClassContent = classTimes.ClassContent,
                ClassId = classTimes.ClassId,
                ClassOt = classTimes.ClassOt.EtmsToDateString(),
                ClassRoomIdsDesc = classRoomIdsDesc,
                ClassTimesId = classTimes.Id,
                CourseListDesc = await ComBusiness.GetCourseNames(tempBoxCourse, _courseDAL, classTimes.CourseList),
                IsCanReservation = reservationLimit.IsCanReservation,
                StudentCountFinish = classTimes.StudentCount,
                StudentCountLimitDesc = reservationLimit.StudentCountLimitDesc,
                StudentCountSurplusDesc = reservationLimit.StudentCountSurplusDesc,
                TeachersDesc = await ComBusiness.GetParentTeachers(tempBoxUser, _userDAL, classTimes.Teachers, "未安排老师"),
                TimeDesc = $"{EtmsHelper.GetTimeDesc(classTimes.StartTime)}~{EtmsHelper.GetTimeDesc(classTimes.EndTime)}",
                WeekDesc = $"周{EtmsHelper.GetWeekDesc(classTimes.Week)}",

            };

            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentReservationLogGetPaging(StudentReservationLogGetPagingRequest request)
        { return ResponseBase.Success(); }

        public async Task<ResponseBase> StudentReservationLogDetail(StudentReservationLogDetailRequest request)
        { return ResponseBase.Success(); }

        public async Task<ResponseBase> StudentReservationSubmit(StudentReservationSubmitRequest request)
        { return ResponseBase.Success(); }

        public async Task<ResponseBase> StudentReservationCancel(StudentReservationCancelRequest request)
        { return ResponseBase.Success(); }
    }
}
