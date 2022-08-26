using ETMS.Entity.Common;
using ETMS.Entity.Dto.Parent.Output;
using ETMS.Entity.Dto.Parent.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Business.Common;
using ETMS.Entity.Database.Source;
using Microsoft.AspNetCore.Http;
using ETMS.Entity.Config;
using ETMS.LOG;
using Newtonsoft.Json;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;
using ETMS.Event.DataContract;
using ETMS.Entity.Dto.Parent2.Output;
using ETMS.IDataAccess.ShareTemplate;

namespace ETMS.Business
{
    public class ParentDataBLL : IParentDataBLL
    {
        private readonly IStudentLeaveApplyLogDAL _studentLeaveApplyLogDAL;

        private readonly IParentStudentDAL _parentStudentDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IStudentOperationLogDAL _studentOperationLogDAL;

        private readonly IClassTimesDAL _classTimesDAL;

        private readonly IClassRoomDAL _classRoomDAL;

        private readonly IUserDAL _userDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IClassDAL _classDAL;

        private readonly ITenantConfigDAL _tenantConfigDAL;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly IGiftCategoryDAL _giftCategoryDAL;

        private readonly IGiftDAL _giftDAL;

        private readonly IActiveHomeworkDAL _activeHomeworkDAL;

        private readonly IActiveHomeworkDetailDAL _activeHomeworkDetailDAL;

        private readonly IStudentWechatDAL _studentWechatDAL;

        private readonly IActiveGrowthRecordDAL _activeGrowthRecordDAL;

        private readonly IStudentGrowingTagDAL _studentGrowingTagDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly IStudentRelationshipDAL _studentRelationshipDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IClassRecordDAL _classRecordDAL;

        private readonly IShareTemplateUseTypeDAL _shareTemplateUseTypeDAL;

        public ParentDataBLL(IStudentLeaveApplyLogDAL studentLeaveApplyLogDAL, IParentStudentDAL parentStudentDAL, IStudentDAL studentDAL,
            IStudentOperationLogDAL studentOperationLogDAL, IClassTimesDAL classTimesDAL, IClassRoomDAL classRoomDAL, IUserDAL userDAL,
            ICourseDAL courseDAL, IClassDAL classDAL, ITenantConfigDAL tenantConfigDAL, IHttpContextAccessor httpContextAccessor, IAppConfigurtaionServices appConfigurtaionServices,
            IGiftCategoryDAL giftCategoryDAL, IGiftDAL giftDAL, IActiveHomeworkDAL activeHomeworkDAL, IActiveHomeworkDetailDAL activeHomeworkDetailDAL,
           IStudentWechatDAL studentWechatDAL, IActiveGrowthRecordDAL activeGrowthRecordDAL, IStudentGrowingTagDAL studentGrowingTagDAL,
           ISysTenantDAL sysTenantDAL, IStudentRelationshipDAL studentRelationshipDAL, IEventPublisher eventPublisher,
           IClassRecordDAL classRecordDAL, IShareTemplateUseTypeDAL shareTemplateUseTypeDAL)
        {
            this._studentLeaveApplyLogDAL = studentLeaveApplyLogDAL;
            this._parentStudentDAL = parentStudentDAL;
            this._studentDAL = studentDAL;
            this._studentOperationLogDAL = studentOperationLogDAL;
            this._classTimesDAL = classTimesDAL;
            this._classRoomDAL = classRoomDAL;
            this._userDAL = userDAL;
            this._courseDAL = courseDAL;
            this._classDAL = classDAL;
            this._tenantConfigDAL = tenantConfigDAL;
            this._httpContextAccessor = httpContextAccessor;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._giftCategoryDAL = giftCategoryDAL;
            this._giftDAL = giftDAL;
            this._activeHomeworkDAL = activeHomeworkDAL;
            this._activeHomeworkDetailDAL = activeHomeworkDetailDAL;
            this._studentWechatDAL = studentWechatDAL;
            this._activeGrowthRecordDAL = activeGrowthRecordDAL;
            this._studentGrowingTagDAL = studentGrowingTagDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._studentRelationshipDAL = studentRelationshipDAL;
            this._eventPublisher = eventPublisher;
            this._classRecordDAL = classRecordDAL;
            this._shareTemplateUseTypeDAL = shareTemplateUseTypeDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentLeaveApplyLogDAL, _parentStudentDAL, _studentDAL,
                _studentOperationLogDAL, _classTimesDAL, _classRoomDAL, _userDAL, _courseDAL, _classDAL,
                _tenantConfigDAL, _giftCategoryDAL, _giftDAL, _activeHomeworkDAL, _activeHomeworkDetailDAL,
                _studentWechatDAL, _activeGrowthRecordDAL, _studentGrowingTagDAL, _studentRelationshipDAL, _classRecordDAL,
                _shareTemplateUseTypeDAL);
        }

        public async Task<ResponseBase> StudentLeaveApplyAdd(StudentLeaveApplyAddRequest request)
        {
            //判断是否重复请假
            var startFullTime = EtmsHelper.GetTime(request.StartDate, request.StartTime);
            var endFullTime = EtmsHelper.GetTime(request.EndDate, request.EndTime);
            return await StudentLeaveApplyProcess(request.StartDate, request.EndDate,
                request.StartTime, request.EndTime, startFullTime, endFullTime, request);
        }

        public async Task<ResponseBase> StudentLeaveApplyAddClassTimes(StudentLeaveApplyAddClassTimesRequest request)
        {
            var classTimes = await _classTimesDAL.GetClassTimes(request.ClassTimesId);
            if (classTimes == null)
            {
                return ResponseBase.CommonError("课次不存在");
            }
            if (classTimes.Status == EmClassTimesStatus.BeRollcall)
            {
                return ResponseBase.CommonError("该课次已点名");
            }
            if (classTimes.ClassOt < DateTime.Now.Date)
            {
                return ResponseBase.CommonError("请假日期不能小于当前日期");
            }
            var startFullTime = EtmsHelper.GetTime(classTimes.ClassOt, classTimes.StartTime);
            var endFullTime = EtmsHelper.GetTime(classTimes.ClassOt, classTimes.EndTime);
            var myDate = classTimes.ClassOt.Date;
            var leaveRemark = string.Empty;
            var myClassBucket = await _classDAL.GetClassBucket(classTimes.ClassId);
            if (myClassBucket != null && myClassBucket.EtClass != null)
            {
                leaveRemark = $"班级({myClassBucket.EtClass.Name})在{myDate.EtmsToDateString()}(周{EtmsHelper.GetWeekDesc(classTimes.Week)}){EtmsHelper.GetTimeDesc(classTimes.StartTime)}~{EtmsHelper.GetTimeDesc(classTimes.EndTime)}的上课";
            }
            return await StudentLeaveApplyProcess(myDate, myDate, classTimes.StartTime, classTimes.EndTime, startFullTime, endFullTime, request, leaveRemark);
        }

        private async Task<ResponseBase> StudentLeaveApplyProcess(DateTime myStartDate, DateTime myEndDate, int myStartTime, int myEndTime,
            DateTime startFullTime, DateTime endFullTime, StudentLeaveApplyRequest request, string leaveRemark = "")
        {
            var isExistApplyLog = await _studentLeaveApplyLogDAL.ExistStudentLeaveApplyLog(request.StudentId, startFullTime, endFullTime);
            if (isExistApplyLog)
            {
                return ResponseBase.CommonError("此时间段已存在请假申请，请勿重复提交");
            }

            var now = DateTime.Now;
            var config = await _tenantConfigDAL.GetTenantConfig();
            if (config.TenantOtherConfig.StudentLeaveApplyMonthLimitCount > 0 || config.TenantOtherConfig.StudentLeaveApplyMustBeforeHour > 0)
            {
                IEnumerable<EtClassTimes> myClassTimes = null;
                if (config.TenantOtherConfig.StudentLeaveApplyMonthLimitCount > 0) //学员请假次数限制
                {
                    DateTime? startDate = null;
                    DateTime? endDate = null;
                    switch (config.TenantOtherConfig.StudentLeaveApplyMonthLimitType)
                    {
                        case EmStudentLeaveApplyMonthLimitType.Week:
                            var re1 = EtmsHelper2.GetThisWeek(myStartDate);
                            startDate = re1.Item1;
                            endDate = re1.Item2;
                            break;
                        case EmStudentLeaveApplyMonthLimitType.Month:
                            var re2 = EtmsHelper2.GetThisMonth(myStartDate);
                            startDate = re2.Item1;
                            endDate = re2.Item2;
                            break;
                        case EmStudentLeaveApplyMonthLimitType.Year:
                            var re3 = EtmsHelper2.GetThisYear(myStartDate);
                            startDate = re3.Item1;
                            endDate = re3.Item2;
                            break;
                    }
                    if (startDate != null && endDate != null)
                    {
                        var classRecordStudentCourseIsLeaveCount = await _classRecordDAL.GetClassRecordStudentCourseIsLeaveCount(request.StudentId, startDate.Value, endDate.Value);
                        if (classRecordStudentCourseIsLeaveCount.Any())
                        {
                            var overtakeLimitCourse = classRecordStudentCourseIsLeaveCount.Where(p => p.TotalCount >= config.TenantOtherConfig.StudentLeaveApplyMonthLimitCount);
                            if (overtakeLimitCourse.Any())
                            {
                                myClassTimes = await _classTimesDAL.GetStudentClassTimes(request.StudentId, myStartDate, myEndDate);
                                if (myClassTimes.Any())
                                {
                                    var allCourseList = myClassTimes.Select(p => p.CourseList).Distinct();
                                    foreach (var overtakeCourse in overtakeLimitCourse)
                                    {
                                        foreach (var p in allCourseList)
                                        {
                                            if (EtmsHelper.AnalyzeMuIds(p).Exists(j => j == overtakeCourse.CourseId))
                                            {
                                                var myCourse = await _courseDAL.GetCourse(overtakeCourse.CourseId);
                                                if (myCourse != null && myCourse.Item1 != null)
                                                {
                                                    return ResponseBase.CommonError($"课程[{myCourse.Item1.Name}]请假次数已超过最多{config.TenantOtherConfig.StudentLeaveApplyMonthLimitCount}次限制");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (config.TenantOtherConfig.StudentLeaveApplyMustBeforeHour > 0) //发起时间限制
                {
                    if (myClassTimes == null)
                    {
                        myClassTimes = await _classTimesDAL.GetStudentClassTimes(request.StudentId, myStartDate, myEndDate, 20);
                    }
                    if (myClassTimes.Any())
                    {
                        foreach (var itemClassTime in myClassTimes)
                        {
                            if (itemClassTime.ClassOt == myStartDate && itemClassTime.EndTime <= myStartTime) //同一天，课程已结束
                            {
                                continue;
                            }
                            if (itemClassTime.ClassOt == myEndDate && itemClassTime.StartTime >= myEndTime)  //同一天，课程已结束
                            {
                                continue;
                            }
                            var startTime = EtmsHelper.GetTime(itemClassTime.ClassOt, itemClassTime.StartTime);
                            var limitDate = startTime.AddHours(-config.TenantOtherConfig.StudentLeaveApplyMustBeforeHour);
                            if (now <= limitDate)
                            {
                                break;
                            }
                            return ResponseBase.CommonError($"{itemClassTime.ClassOt.EtmsToDateString()}({EtmsHelper.GetTimeDesc(itemClassTime.StartTime)}~{EtmsHelper.GetTimeDesc(itemClassTime.EndTime)})有课，必须提前{config.TenantOtherConfig.StudentLeaveApplyMustBeforeHour}小时请假");
                        }
                    }
                }
            }

            var log = new EtStudentLeaveApplyLog()
            {
                ApplyOt = now,
                EndDate = myEndDate,
                EndTime = myEndTime,
                HandleOt = null,
                HandleRemark = string.Empty,
                HandleStatus = EmStudentLeaveApplyHandleStatus.Unreviewed,
                HandleUser = null,
                IsDeleted = EmIsDeleted.Normal,
                LeaveContent = request.LeaveContent,
                StartDate = myStartDate,
                StartTime = myStartTime,
                StudentId = request.StudentId,
                TenantId = request.LoginTenantId,
                StartFullTime = startFullTime,
                EndFullTime = endFullTime,
                LeaveMedias = EtmsHelper2.GetMediasKeys(request.LeaveMediasKeys),
                LeaveRemark = leaveRemark
            };
            await _studentLeaveApplyLogDAL.AddStudentLeaveApplyLog(log);
            await _studentOperationLogDAL.AddStudentLog(new EtStudentOperationLog()
            {
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"添加请假申请",
                Ot = DateTime.Now,
                Remark = string.Empty,
                StudentId = request.StudentId,
                TenantId = request.LoginTenantId,
                Type = (int)EmStudentOperationLogType.StudentLeaveApply
            });

            _eventPublisher.Publish(new ResetTenantToDoThingEvent(request.LoginTenantId));
            _eventPublisher.Publish(new NoticeUserStudentLeaveApplyEvent(request.LoginTenantId)
            {
                StudentLeaveApplyLog = log
            });

            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentLeaveApplyGet(StudentLeaveApplyGetRequest request)
        {
            var pagingData = await _studentLeaveApplyLogDAL.GetPaging(request);
            var output = new List<StudentLeaveApplyGetOutput>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new StudentLeaveApplyGetOutput()
                {
                    TitleDesc = $"{p.StudentName}的请假",
                    ApplyOt = p.ApplyOt,
                    StartDate = p.StartDate.EtmsToDateString(),
                    StartTime = EtmsHelper.GetTimeDesc(p.StartTime),
                    EndDate = p.EndDate.EtmsToDateString(),
                    EndTime = EtmsHelper.GetTimeDesc(p.EndTime),
                    HandleStatus = p.HandleStatus,
                    LeaveContent = p.LeaveContent,
                    HandleStatusDesc = EmStudentLeaveApplyHandleStatus.GetStudentLeaveApplyHandleStatusDescParent(p.HandleStatus),
                    Id = p.Id,
                    LeaveRemark = p.LeaveRemark
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentLeaveApplyGetOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> StudentListGet(StudentListGetRequest request)
        {
            var myStudents = await _parentStudentDAL.GetParentStudents(request.LoginTenantId, request.LoginPhone);
            var output = new List<StudentListGetOutput>();
            foreach (var p in myStudents)
            {
                output.Add(new StudentListGetOutput()
                {
                    Name = p.Name,
                    StudentId = p.Id,
                    Gender = p.Gender,
                    AvatarKey = p.Avatar,
                    AvatarUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, p.Avatar),
                });
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentListDetailGet(StudentListDetailGetRequest request)
        {
            var myStudents = await _parentStudentDAL.GetParentStudents(request.LoginTenantId, request.LoginPhone);
            var output = new List<StudentListDetailGetOutput>();
            var studentRelationship = await _studentRelationshipDAL.GetAllStudentRelationship();
            foreach (var p in myStudents)
            {
                var student = (await _studentDAL.GetStudent(p.Id)).Student;
                var learningManager = string.Empty;
                if (student.LearningManager != null)
                {
                    var user = await _userDAL.GetUser(student.LearningManager.Value);
                    if (user != null)
                    {
                        learningManager = ComBusiness2.GetParentTeacherName(user);
                    }
                }
                output.Add(new StudentListDetailGetOutput()
                {
                    Id = student.Id,
                    AvatarKey = student.Avatar,
                    AvatarUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, student.Avatar),
                    HomeAddress = student.HomeAddress,
                    Name = student.Name,
                    Phone = student.Phone,
                    PhoneBak = student.PhoneBak,
                    SchoolName = student.SchoolName,
                    Points = student.Points,
                    PhoneRelationship = student.PhoneRelationship,
                    PhoneBakRelationship = student.PhoneBakRelationship,
                    BirthdayDesc = student.Birthday.EtmsToDateString(),
                    Age = student.Age,
                    PhoneBakRelationshipDesc = ComBusiness2.GetStudentRelationshipDesc(studentRelationship, student.PhoneBakRelationship, "备用号码"),
                    PhoneRelationshipDesc = ComBusiness2.GetStudentRelationshipDesc(studentRelationship, student.PhoneRelationship, "手机号码"),
                    LearningManager = learningManager
                });
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentLeaveApplyDetailGet(StudentLeaveApplyDetailGetRequest request)
        {
            var p = await _studentLeaveApplyLogDAL.GetStudentLeaveApplyLog(request.Id);
            var student = await _studentDAL.GetStudent(p.StudentId);
            var handleUserName = string.Empty;
            if (p.HandleStatus != EmStudentLeaveApplyHandleStatus.Unreviewed && p.HandleUser != null)
            {
                var handleUser = await _userDAL.GetUser(p.HandleUser.Value);
                if (handleUser != null)
                {
                    handleUserName = ComBusiness2.GetParentTeacherName(handleUser);
                }
            }
            return ResponseBase.Success(new StudentLeaveApplyDetailGetOutput()
            {
                TitleDesc = $"{student.Student.Name}的请假",
                ApplyOt = p.ApplyOt,
                StartDate = p.StartDate.EtmsToDateString(),
                StartTime = EtmsHelper.GetTimeDesc(p.StartTime),
                EndDate = p.EndDate.EtmsToDateString(),
                EndTime = EtmsHelper.GetTimeDesc(p.EndTime),
                HandleStatus = p.HandleStatus,
                LeaveContent = p.LeaveContent,
                HandleStatusDesc = EmStudentLeaveApplyHandleStatus.GetStudentLeaveApplyHandleStatusDescParent(p.HandleStatus),
                Id = p.Id,
                HandleOt = p.HandleOt.EtmsToMinuteString(),
                LeaveMediasUrl = EtmsHelper2.GetMediasUrl(p.LeaveMedias),
                HandleRemark = p.HandleRemark,
                HandleUserName = handleUserName,
                LeaveRemark = p.LeaveRemark
            });
        }

        public async Task<ResponseBase> StudentLeaveApplyRevoke(StudentLeaveApplyRevokeRequest request)
        {
            var p = await _studentLeaveApplyLogDAL.GetStudentLeaveApplyLog(request.Id);
            if (p.HandleStatus != EmStudentLeaveApplyHandleStatus.Unreviewed)
            {
                return ResponseBase.CommonError("已审批,无法撤销");
            }
            p.HandleStatus = EmStudentLeaveApplyHandleStatus.IsRevoke;
            await _studentLeaveApplyLogDAL.EditStudentLeaveApplyLog(p);

            _eventPublisher.Publish(new ResetTenantToDoThingEvent(request.LoginTenantId));
            _eventPublisher.Publish(new NoticeUserStudentLeaveApplyEvent(request.LoginTenantId)
            {
                StudentLeaveApplyLog = p
            });
            await _studentOperationLogDAL.AddStudentLog(p.StudentId, request.LoginTenantId, "撤销请假申请", EmStudentOperationLogType.StudentLeaveApply);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentClassTimetableCountGet(StudentClassTimetableCountGetRequest request)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            if (config.TenantOtherConfig.IsStudentLimitShowClassTimes && config.TenantOtherConfig.StudentLimitShowClassTimesValue > 0)
            {
                request.LimitDate = DateTime.Now.AddMonths(config.TenantOtherConfig.StudentLimitShowClassTimesValue);
            }
            var classTimeGroupCount = await _classTimesDAL.ClassTimesClassOtGroupCount(request);
            return ResponseBase.Success(classTimeGroupCount.Select(p => new StudentTimetableCountOutput()
            {
                ClassTimesCount = p.TotalCount,
                Date = p.ClassOt.EtmsToDateString()
            }));
        }

        public async Task<ResponseBase> StudentClassTimetableGet(StudentClassTimetableRequest request)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            if (config.TenantOtherConfig.IsStudentLimitShowClassTimes && config.TenantOtherConfig.StudentLimitShowClassTimesValue > 0)
            {
                request.LimitDate = DateTime.Now.AddMonths(config.TenantOtherConfig.StudentLimitShowClassTimesValue);
            }
            var classTimesData = (await _classTimesDAL.GetList(request)).OrderBy(p => p.ClassOt).ThenBy(p => p.StartTime);
            return ResponseBase.Success(await GetStudentClassTimetableOutput(request, classTimesData));
        }

        public async Task<ResponseBase> StudentClassTimetableDetailGet(StudentClassTimetableDetailGetRequest request)
        {
            var classTimes = await _classTimesDAL.GetClassTimes(request.Id);
            var output = await GetStudentClassTimetableOutput(request, new List<EtClassTimes>() { classTimes });
            return ResponseBase.Success(output.First());
        }

        private async Task<List<StudentClassTimetableOutput>> GetStudentClassTimetableOutput(ParentRequestBase request, IEnumerable<EtClassTimes> classTimesData)
        {
            var output = new List<StudentClassTimetableOutput>();
            if (!classTimesData.Any())
            {
                return output;
            }
            var myStudents = await _parentStudentDAL.GetParentStudents(request.LoginTenantId, request.LoginPhone);
            var myStudentCount = myStudents.Count();
            var allClassRoom = await _classRoomDAL.GetAllClassRoom();
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var tempBoxUser = new DataTempBox<EtUser>();
            foreach (var classTimes in classTimesData)
            {
                var classRoomIdsDesc = string.Empty;
                var courseListDesc = string.Empty;
                var courseStyleColor = string.Empty;
                var className = string.Empty;
                var teachersDesc = string.Empty;
                var etClass = await _classDAL.GetClassBucket(classTimes.ClassId);
                var courseInfo = await ComBusiness.GetCourseNameAndColor(tempBoxCourse, _courseDAL, classTimes.CourseList);
                classRoomIdsDesc = ComBusiness.GetDesc(allClassRoom, classTimes.ClassRoomIds);
                className = etClass.EtClass.Name;
                courseListDesc = courseInfo.Item1;
                courseStyleColor = courseInfo.Item2;
                teachersDesc = await ComBusiness.GetParentTeachers(tempBoxUser, _userDAL, classTimes.Teachers);
                var studentName = string.Empty;
                if (myStudentCount == 1)
                {
                    studentName = myStudents.First().Name;
                }
                else
                {
                    var allClassTimesStudent = $"{classTimes.StudentIdsClass}{classTimes.StudentIdsTemp}";
                    var tempStudent = new StringBuilder();
                    foreach (var p in myStudents)
                    {
                        if (allClassTimesStudent.IndexOf($",{p.Id},") != -1)
                        {
                            tempStudent.Append($"{p.Name},");
                        }
                    }
                    studentName = tempStudent.ToString().TrimEnd(',');
                }
                output.Add(new StudentClassTimetableOutput()
                {
                    Id = classTimes.Id,
                    ClassId = classTimes.ClassId,
                    ClassName = className,
                    ClassOt = classTimes.ClassOt.EtmsToDateString(),
                    ClassRoomIds = classTimes.ClassRoomIds,
                    ClassRoomIdsDesc = classRoomIdsDesc,
                    CourseList = classTimes.CourseList,
                    CourseListDesc = courseListDesc,
                    CourseStyleColor = courseStyleColor,
                    EndTime = EtmsHelper.GetTimeDesc(classTimes.EndTime),
                    StartTime = EtmsHelper.GetTimeDesc(classTimes.StartTime),
                    Status = classTimes.Status,
                    Week = classTimes.Week,
                    WeekDesc = $"星期{EtmsHelper.GetWeekDesc(classTimes.Week)}",
                    Teachers = classTimes.Teachers,
                    TeachersDesc = teachersDesc,
                    StudentName = studentName,
                    ClassOtShort = classTimes.ClassOt.EtmsToDateShortString(),
                    ClassContent = classTimes.ClassContent
                });
            }
            return output;
        }

        public async Task<ResponseBase> IndexBannerGet(IndexBannerGetRequest request)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            var outPut = new List<IndexBannerGetOutput>();
            if (config.ParentSetConfig.ParentBanners.Any())
            {
                foreach (var p in config.ParentSetConfig.ParentBanners)
                {
                    if (string.IsNullOrEmpty(p.ImgKey))
                    {
                        continue;
                    }
                    outPut.Add(new IndexBannerGetOutput()
                    {
                        ImgUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, p.ImgKey),
                        LinkUrl = p.UrlKey
                    });
                }
            }
            return ResponseBase.Success(outPut);
        }

        public async Task<ResponseBase> HomeworkUnansweredGetPaging(HomeworkUnansweredGetPagingRequest request)
        {
            var pagingData = await _activeHomeworkDetailDAL.GetPaging(request);
            var output = new List<HomeworkUnansweredGetPagingOutput>();
            var tempBoxUser = new DataTempBox<EtUser>();
            var tempBoxClass = new DataTempBox<EtClass>();
            var tempBoxStudent = new DataTempBox<EtStudent>();
            foreach (var p in pagingData.Item1)
            {
                var myClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, p.ClassId);
                var student = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId);
                if (student == null)
                {
                    continue;
                }
                var overdueStatusOutput = EmOverdueStatusOutput.GetOverdueStatusOutput(p.ExDate, DateTime.Now);
                output.Add(new HomeworkUnansweredGetPagingOutput()
                {
                    ClassId = p.ClassId,
                    ClassName = myClass?.Name,
                    TeacherName = await ComBusiness.GetParentTeacher(tempBoxUser, _userDAL, p.CreateUserId),
                    ExDateDesc = p.ExDate == null ? string.Empty : p.ExDate.EtmsToMinuteString(),
                    OtDesc = p.Ot.EtmsToMinuteString(),
                    Title = p.Title,
                    Type = p.Type,
                    TypeDesc = EmActiveHomeworkType.GetActiveHomeworkTypeDesc(p.Type),
                    AnswerStatus = p.AnswerStatus,
                    ReadStatus = p.ReadStatus,
                    StudentId = p.StudentId,
                    HomeworkDetailId = p.Id,
                    HomeworkId = p.HomeworkId,
                    StudentName = student.Name,
                    AnswerStatusDesc = EmActiveHomeworkDetailAnswerStatus.ActiveHomeworkDetailAnswerStatusDesc(p.AnswerStatus),
                    OverdueStatus = overdueStatusOutput.Item1,
                    OverdueStatusDesc = overdueStatusOutput.Item2
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<HomeworkUnansweredGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> HomeworkAnsweredGetPaging(HomeworkAnsweredGetPagingRequest request)
        {
            var pagingData = await _activeHomeworkDetailDAL.GetPaging(request);
            var output = new List<HomeworkAnsweredGetPagingOutput>();
            var tempBoxUser = new DataTempBox<EtUser>();
            var tempBoxClass = new DataTempBox<EtClass>();
            var tempBoxStudent = new DataTempBox<EtStudent>();
            foreach (var p in pagingData.Item1)
            {
                var myClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, p.ClassId);
                var student = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId);
                if (student == null)
                {
                    continue;
                }
                output.Add(new HomeworkAnsweredGetPagingOutput()
                {
                    ClassId = p.ClassId,
                    ClassName = myClass?.Name,
                    TeacherName = await ComBusiness.GetParentTeacher(tempBoxUser, _userDAL, p.CreateUserId),
                    OtDesc = p.Ot.EtmsToMinuteString(),
                    Title = p.Title,
                    Type = p.Type,
                    TypeDesc = EmActiveHomeworkType.GetActiveHomeworkTypeDesc(p.Type),
                    AnswerStatus = p.AnswerStatus,
                    StudentId = p.StudentId,
                    HomeworkDetailId = p.Id,
                    HomeworkId = p.HomeworkId,
                    StudentName = student.Name,
                    AnswerStatusDesc = EmActiveHomeworkDetailAnswerStatus.ActiveHomeworkDetailAnswerStatusDesc(p.AnswerStatus),
                    AnswerOtDesc = p.AnswerOt.EtmsToMinuteString()
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<HomeworkAnsweredGetPagingOutput>(pagingData.Item2, output));
        }

        private List<string> GetMediasUrl(string workMedias)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(workMedias))
            {
                return result;
            }
            var myMedias = workMedias.Split('|');
            foreach (var p in myMedias)
            {
                if (!string.IsNullOrEmpty(p))
                {
                    result.Add(UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, p));
                }
            }
            return result;
        }

        public async Task<ResponseBase> HomeworkDetailGet(HomeworkDetailGetRequest request)
        {
            var homeworkDetailBucket = await _activeHomeworkDetailDAL.GetActiveHomeworkDetailBucket(request.HomeworkDetailId);
            if (homeworkDetailBucket == null || homeworkDetailBucket.ActiveHomeworkDetail == null)
            {
                return ResponseBase.CommonError("作业不存在");
            }
            var p = homeworkDetailBucket.ActiveHomeworkDetail;
            var classInfo = await _classDAL.GetClassBucket(p.ClassId);
            var tempBoxUser = new DataTempBox<EtUser>();
            var teacher = await ComBusiness.GetUser(tempBoxUser, _userDAL, p.CreateUserId);
            var output = new HomeworkDetailGetOutput()
            {
                ClassId = p.ClassId,
                ClassName = classInfo?.EtClass.Name,
                ExDateDesc = p.ExDate == null ? string.Empty : p.ExDate.EtmsToMinuteString(),
                HomeworkDetailId = p.Id,
                HomeworkId = p.HomeworkId,
                OtDesc = p.Ot.EtmsToMinuteString(),
                ReadStatus = p.ReadStatus,
                TeacherName = ComBusiness2.GetParentTeacherName(teacher),
                TeacherAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, teacher.Avatar),
                Title = p.Title,
                Type = p.Type,
                TypeDesc = EmActiveHomeworkType.GetActiveHomeworkTypeDesc(p.Type),
                WorkContent = p.WorkContent,
                WorkMediasUrl = GetMediasUrl(p.WorkMedias),
                AnswerStatus = p.AnswerStatus,
                AnswerStatusDesc = EmActiveHomeworkDetailAnswerStatus.ActiveHomeworkDetailAnswerStatusDesc(p.AnswerStatus),
                AnswerInfo = null,
                CommentOutputs = await GetCommentOutput(homeworkDetailBucket.ActiveHomeworkDetailComments, tempBoxUser)
            };
            if (p.AnswerStatus == EmActiveHomeworkDetailAnswerStatus.Answered)
            {
                output.AnswerInfo = new HomeworkDetailAnswerInfo()
                {
                    AnswerContent = p.AnswerContent,
                    AnswerMediasUrl = GetMediasUrl(p.AnswerMedias),
                    AnswerOtDesc = p.AnswerOt.EtmsToMinuteString()
                };
            }
            return ResponseBase.Success(output);
        }

        private async Task<List<ParentCommentOutput>> GetCommentOutput(List<EtActiveHomeworkDetailComment> activeHomeworkDetailComments, DataTempBox<EtUser> tempBoxUser)
        {
            var commentOutputs = new List<ParentCommentOutput>();
            if (activeHomeworkDetailComments != null || activeHomeworkDetailComments.Count > 0)
            {
                var first = activeHomeworkDetailComments.Where(j => j.ReplyId == null).OrderBy(j => j.Ot);
                foreach (var myFirstComment in first)
                {
                    var firstrelatedManName = string.Empty;
                    var firstrelatedManAvatar = string.Empty;
                    if (myFirstComment.SourceType == EmActiveCommentSourceType.User)
                    {
                        var myRelatedUser = await ComBusiness.GetUser(tempBoxUser, _userDAL, myFirstComment.SourceId);
                        if (myRelatedUser != null)
                        {
                            firstrelatedManName = ComBusiness2.GetParentTeacherName(myRelatedUser);
                            firstrelatedManAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, myRelatedUser.Avatar);
                        }
                    }
                    else
                    {
                        firstrelatedManName = myFirstComment.Nickname;
                        firstrelatedManAvatar = myFirstComment.Headimgurl;
                    }
                    commentOutputs.Add(new ParentCommentOutput()
                    {
                        CommentContent = myFirstComment.CommentContent,
                        CommentId = myFirstComment.Id,
                        Ot = myFirstComment.Ot.EtmsToMinuteString(),
                        ReplyId = myFirstComment.ReplyId,
                        SourceType = myFirstComment.SourceType,
                        RelatedManAvatar = firstrelatedManAvatar,
                        RelatedManName = firstrelatedManName,
                        OtDesc = EtmsHelper.GetOtFriendlyDesc(myFirstComment.Ot)
                    });
                    var second = activeHomeworkDetailComments.Where(p => p.ReplyId == myFirstComment.Id).OrderBy(j => j.Ot);
                    foreach (var mySecondComment in second)
                    {
                        var secondfirstrelatedManName = string.Empty;
                        var secondfirstrelatedManAvatar = string.Empty;
                        if (mySecondComment.SourceType == EmActiveCommentSourceType.User)
                        {
                            var myRelatedUser = await ComBusiness.GetUser(tempBoxUser, _userDAL, mySecondComment.SourceId);
                            if (myRelatedUser != null)
                            {
                                secondfirstrelatedManName = ComBusiness2.GetParentTeacherName(myRelatedUser);
                                secondfirstrelatedManAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, myRelatedUser.Avatar);
                            }
                        }
                        else
                        {
                            secondfirstrelatedManName = mySecondComment.Nickname;
                            secondfirstrelatedManAvatar = mySecondComment.Headimgurl;
                        }
                        commentOutputs.Add(new ParentCommentOutput()
                        {
                            CommentContent = mySecondComment.CommentContent,
                            CommentId = mySecondComment.Id,
                            Ot = mySecondComment.Ot.EtmsToMinuteString(),
                            OtDesc = EtmsHelper.GetOtFriendlyDesc(mySecondComment.Ot),
                            RelatedManAvatar = secondfirstrelatedManAvatar,
                            RelatedManName = secondfirstrelatedManName,
                            ReplyId = mySecondComment.ReplyId,
                            SourceType = mySecondComment.SourceType,
                            ReplyRelatedManName = firstrelatedManName
                        });
                    }
                }
            }
            return commentOutputs;
        }

        public async Task<ResponseBase> HomeworkDetailSetRead(HomeworkDetailSetReadRequest request)
        {
            var homeworkDetailBucket = await _activeHomeworkDetailDAL.GetActiveHomeworkDetailBucket(request.HomeworkDetailId);
            if (homeworkDetailBucket == null || homeworkDetailBucket.ActiveHomeworkDetail == null)
            {
                return ResponseBase.CommonError("作业不存在");
            }
            if (homeworkDetailBucket.ActiveHomeworkDetail.ReadStatus == EmActiveHomeworkDetailReadStatus.Yes)
            {
                Log.Warn($"[HomeworkDetailSetRead]重复提交设置作业已读请求:HomeworkDetailId:{JsonConvert.SerializeObject(request)}", this.GetType());
                return ResponseBase.Success();
            }
            var activeHomeworkDetail = homeworkDetailBucket.ActiveHomeworkDetail;
            activeHomeworkDetail.ReadStatus = EmActiveHomeworkDetailReadStatus.Yes;
            await _activeHomeworkDetailDAL.EditActiveHomeworkDetail(activeHomeworkDetail);

            if (activeHomeworkDetail.Type == EmActiveHomeworkType.SingleWork)
            {
                var activeHomework = await _activeHomeworkDAL.GetActiveHomework(activeHomeworkDetail.HomeworkId);
                activeHomework.ReadCount += 1;
                await _activeHomeworkDAL.EditActiveHomework(activeHomework);
            }
            else
            {
                _eventPublisher.Publish(new SyncHomeworkReadAndFinishCountEvent(request.LoginTenantId)
                {
                    OpType = SyncHomeworkReadAndFinishCountOpType.Read,
                    HomeworkId = activeHomeworkDetail.HomeworkId,
                    StudentId = activeHomeworkDetail.StudentId
                });
            }

            await _studentOperationLogDAL.AddStudentLog(activeHomeworkDetail.StudentId, request.LoginTenantId, $"阅读课后作业：{activeHomeworkDetail.Title}", EmStudentOperationLogType.Homework);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> HomeworkSubmitAnswer(HomeworkSubmitAnswerRequest request)
        {
            var homeworkDetailBucket = await _activeHomeworkDetailDAL.GetActiveHomeworkDetailBucket(request.HomeworkDetailId);
            if (homeworkDetailBucket == null || homeworkDetailBucket.ActiveHomeworkDetail == null)
            {
                return ResponseBase.CommonError("作业不存在");
            }
            if (homeworkDetailBucket.ActiveHomeworkDetail.AnswerStatus == EmActiveHomeworkDetailAnswerStatus.Answered)
            {
                Log.Warn($"[HomeworkSubmitAnswer]重复提交作业:HomeworkDetailId:{JsonConvert.SerializeObject(request)}", this.GetType());
                return ResponseBase.CommonError("请勿重复提交作业");
            }
            var activeHomeworkDetail = homeworkDetailBucket.ActiveHomeworkDetail;
            activeHomeworkDetail.AnswerStatus = EmActiveHomeworkDetailAnswerStatus.Answered;
            activeHomeworkDetail.AnswerContent = request.AnswerContent;
            activeHomeworkDetail.AnswerMedias = string.Empty;
            if (request.AnswerMediasKeys != null && request.AnswerMediasKeys.Count > 0)
            {
                activeHomeworkDetail.AnswerMedias = string.Join('|', request.AnswerMediasKeys);
            }
            activeHomeworkDetail.AnswerOt = DateTime.Now;
            await _activeHomeworkDetailDAL.EditActiveHomeworkDetail(activeHomeworkDetail);

            if (activeHomeworkDetail.Type == EmActiveHomeworkType.SingleWork)
            {
                var activeHomework = await _activeHomeworkDAL.GetActiveHomework(activeHomeworkDetail.HomeworkId);
                activeHomework.FinishCount += 1;
                await _activeHomeworkDAL.EditActiveHomework(activeHomework);
            }
            else
            {
                _eventPublisher.Publish(new SyncHomeworkReadAndFinishCountEvent(request.LoginTenantId)
                {
                    OpType = SyncHomeworkReadAndFinishCountOpType.Answer,
                    HomeworkId = activeHomeworkDetail.HomeworkId,
                    StudentId = activeHomeworkDetail.StudentId
                });
            }

            _eventPublisher.Publish(new NoticeTeacherOfHomeworkFinishEvent(activeHomeworkDetail.TenantId)
            {
                HomeworkDetailId = activeHomeworkDetail.Id
            });
            await _studentOperationLogDAL.AddStudentLog(activeHomeworkDetail.StudentId, request.LoginTenantId, $"提交课后作业：{activeHomeworkDetail.Title}", EmStudentOperationLogType.Homework);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> HomeworkAddComment(HomeworkAddCommentRequest request)
        {
            var homeworkDetailBucket = await _activeHomeworkDetailDAL.GetActiveHomeworkDetailBucket(request.HomeworkDetailId);
            if (homeworkDetailBucket == null || homeworkDetailBucket.ActiveHomeworkDetail == null)
            {
                return ResponseBase.CommonError("作业不存在");
            }
            var activeHomeworkDetail = homeworkDetailBucket.ActiveHomeworkDetail;
            var myStudentWechat = await _studentWechatDAL.GetStudentWechatByPhone(request.LoginPhone);
            var comment = new EtActiveHomeworkDetailComment()
            {
                CommentContent = request.CommentContent,
                Headimgurl = myStudentWechat?.Headimgurl,
                Nickname = myStudentWechat?.Nickname,
                HomeworkDetailId = request.HomeworkDetailId,
                HomeworkId = activeHomeworkDetail.HomeworkId,
                IsDeleted = EmIsDeleted.Normal,
                Ot = DateTime.Now,
                ReplyId = null,
                SourceId = activeHomeworkDetail.StudentId,
                SourceType = EmActiveCommentSourceType.Student,
                TenantId = request.LoginTenantId
            };
            await _activeHomeworkDetailDAL.AddActiveHomeworkDetailComment(comment);

            await _studentOperationLogDAL.AddStudentLog(activeHomeworkDetail.StudentId, request.LoginTenantId, $"评论课后作业：{activeHomeworkDetail.Title}", EmStudentOperationLogType.Homework);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> HomeworkDelComment(HomeworkDelCommentRequest request)
        {
            var homeworkDetailBucket = await _activeHomeworkDetailDAL.GetActiveHomeworkDetailBucket(request.HomeworkDetailId);
            if (homeworkDetailBucket == null || homeworkDetailBucket.ActiveHomeworkDetail == null)
            {
                return ResponseBase.CommonError("作业不存在");
            }
            var activeHomeworkDetail = homeworkDetailBucket.ActiveHomeworkDetail;
            await _activeHomeworkDetailDAL.DelActiveHomeworkDetailComment(request.HomeworkDetailId, request.CommentId);
            await _studentOperationLogDAL.AddStudentLog(activeHomeworkDetail.StudentId, request.LoginTenantId, "删除课后作业评论", EmStudentOperationLogType.Homework);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> GrowthRecordGetPaging(GrowthRecordGetPagingRequest request)
        {
            var pagingData = await _activeGrowthRecordDAL.GetDetailPaging(request);
            var output = new List<GrowthRecordGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var tempBoxStudent = new DataTempBox<EtStudent>();
                var allstudentGrowingTag = await _studentGrowingTagDAL.GetAllStudentGrowingTag();
                var thisYear = DateTime.Now.Year;
                foreach (var p in pagingData.Item1)
                {
                    var student = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId);
                    if (student == null)
                    {
                        continue;
                    }
                    var growingTagDesc = string.Empty;
                    if (p.SceneType == EmActiveGrowthRecordDetailSceneType.ActiveGrowthRecord)
                    {
                        var myGrowingTag = allstudentGrowingTag.FirstOrDefault(j => j.Id == p.GrowingTag);
                        growingTagDesc = myGrowingTag?.Name;
                    }
                    else
                    {
                        growingTagDesc = "课后点评";
                    }
                    output.Add(new GrowthRecordGetPagingOutput()
                    {
                        Day = p.Ot.Day,
                        Month = p.Ot.Month,
                        IsThisYear = p.Ot.Year == thisYear,
                        FavoriteStatus = p.FavoriteStatus,
                        GrowingTag = p.GrowingTag,
                        GrowingTagDesc = growingTagDesc,
                        GrowthContent = p.GrowthContent,
                        GrowthMediasUrl = GetMediasUrl(p.GrowthMedias),
                        GrowthRecordDetailId = p.Id,
                        GrowthRecordId = p.GrowthRecordId,
                        OtDesc = p.Ot.EtmsToDateString(),
                        StudentId = p.StudentId,
                        StudentName = student.Name,
                        TenantId = p.TenantId,
                        RelatedId = p.RelatedId,
                        SceneType = p.SceneType
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<GrowthRecordGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> GrowthRecordFavoriteGetPaging(GrowthRecordGetPagingRequest request)
        {
            request.IsOnlyGetFavorite = true;
            return await GrowthRecordGetPaging(request);
        }

        public async Task<ResponseBase> GrowthRecordDetailGet(GrowthRecordDetailGetRequest request)
        {
            var growthRecordDetail = await _activeGrowthRecordDAL.GetActiveGrowthRecordDetail(request.GrowthRecordDetailId);
            if (growthRecordDetail == null)
            {
                return ResponseBase.CommonError("成长档案不存在");
            }
            var growthRecordBucket = await _activeGrowthRecordDAL.GetActiveGrowthRecord(growthRecordDetail.GrowthRecordId);
            if (growthRecordBucket == null || growthRecordBucket.ActiveGrowthRecord == null)
            {
                return ResponseBase.CommonError("成长档案不存在");
            }
            var studentBucket = await _studentDAL.GetStudent(growthRecordDetail.StudentId);
            if (studentBucket == null || studentBucket.StudentExtendInfos == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var student = studentBucket.Student;
            var myTenant = await _sysTenantDAL.GetTenant(growthRecordDetail.TenantId);

            var p = growthRecordBucket.ActiveGrowthRecord;
            var allstudentGrowingTag = await _studentGrowingTagDAL.GetAllStudentGrowingTag();
            var myStudentGrowingTag = allstudentGrowingTag.FirstOrDefault(j => j.Id == p.GrowingTag);
            var tempBoxUser = new DataTempBox<EtUser>();
            var output = new GrowthRecordDetailGetOutput()
            {
                TenantId = p.TenantId,
                Day = p.Ot.Day,
                Month = p.Ot.Month,
                IsThisYear = p.Ot.Year == DateTime.Now.Year,
                GrowingTag = p.GrowingTag,
                GrowingTagDesc = myStudentGrowingTag?.Name,
                GrowthContent = p.GrowthContent,
                GrowthMediasUrl = GetMediasUrl(p.GrowthMedias),
                GrowthRecordId = p.Id,
                GrowthRecordDetailId = request.GrowthRecordDetailId,
                OtDesc = p.Ot.EtmsToDateString(),
                CommentOutputs = await GetCommentOutput(growthRecordBucket.Comments, tempBoxUser, request.IsLogin, growthRecordDetail.StudentId),
                StudentName = student.Name,
                StudentAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, student.Avatar),
                TenantName = myTenant.Name
            };
            if (growthRecordDetail.ReadStatus == EmBool.False)
            {
                await _activeGrowthRecordDAL.GrowthRecordAddReadCount(growthRecordDetail.GrowthRecordId);
                await _activeGrowthRecordDAL.SetActiveGrowthRecordIsRead(growthRecordDetail.GrowthRecordId, growthRecordDetail.Id);
            }
            var shareTemplateBucket = await _shareTemplateUseTypeDAL.GetShareTemplate(EmShareTemplateUseType.Growth);
            if (shareTemplateBucket != null)
            {
                output.ShareContent = ShareTemplateHandler.TemplateLinkGrowth(shareTemplateBucket.MyShareTemplateLink,
                    output.StudentName, output.GrowthContent, output.GrowingTagDesc, output.OtDesc);
                output.ShowContent = ShareTemplateHandler.TemplateShowGrowth(shareTemplateBucket.MyShareTemplateShow,
                    output.StudentName, output.GrowthContent, output.GrowingTagDesc, output.OtDesc);
            }
            return ResponseBase.Success(output);
        }

        private async Task<List<ParentCommentOutput>> GetCommentOutput(List<EtActiveGrowthRecordDetailComment> activeHomeworkDetailComments,
            DataTempBox<EtUser> tempBoxUser, bool isLogin, long myStudentId)
        {
            var commentOutputs = new List<ParentCommentOutput>();
            if (activeHomeworkDetailComments != null || activeHomeworkDetailComments.Count > 0)
            {
                var first = activeHomeworkDetailComments.Where(j => j.ReplyId == null).OrderBy(j => j.Ot);
                foreach (var myFirstComment in first)
                {
                    var firstrelatedManName = string.Empty;
                    var firstrelatedManAvatar = string.Empty;
                    var isCanDelete = false;
                    if (myFirstComment.SourceType == EmActiveCommentSourceType.User)
                    {
                        var myRelatedUser = await ComBusiness.GetUser(tempBoxUser, _userDAL, myFirstComment.SourceId);
                        if (myRelatedUser != null)
                        {
                            firstrelatedManName = ComBusiness2.GetParentTeacherName(myRelatedUser);
                            firstrelatedManAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, myRelatedUser.Avatar);
                        }
                    }
                    else
                    {
                        firstrelatedManName = myFirstComment.Nickname;
                        firstrelatedManAvatar = myFirstComment.Headimgurl;
                        if (isLogin && myFirstComment.SourceId == myStudentId)
                        {
                            isCanDelete = true;
                        }
                    }
                    commentOutputs.Add(new ParentCommentOutput()
                    {
                        CommentContent = myFirstComment.CommentContent,
                        CommentId = myFirstComment.Id,
                        Ot = myFirstComment.Ot.EtmsToMinuteString(),
                        ReplyId = myFirstComment.ReplyId,
                        SourceType = myFirstComment.SourceType,
                        RelatedManAvatar = firstrelatedManAvatar,
                        RelatedManName = firstrelatedManName,
                        OtDesc = EtmsHelper.GetOtFriendlyDesc(myFirstComment.Ot),
                        IsCanDelete = isCanDelete
                    });
                    var second = activeHomeworkDetailComments.Where(p => p.ReplyId == myFirstComment.Id).OrderBy(j => j.Ot);
                    foreach (var mySecondComment in second)
                    {
                        var secondfirstrelatedManName = string.Empty;
                        var secondfirstrelatedManAvatar = string.Empty;
                        var secondIsCanDelete = false;
                        if (mySecondComment.SourceType == EmActiveCommentSourceType.User)
                        {
                            var myRelatedUser = await ComBusiness.GetUser(tempBoxUser, _userDAL, mySecondComment.SourceId);
                            if (myRelatedUser != null)
                            {
                                secondfirstrelatedManName = ComBusiness2.GetParentTeacherName(myRelatedUser);
                                secondfirstrelatedManAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, myRelatedUser.Avatar);
                            }
                        }
                        else
                        {
                            secondfirstrelatedManName = mySecondComment.Nickname;
                            secondfirstrelatedManAvatar = mySecondComment.Headimgurl;
                            if (isLogin && myFirstComment.SourceId == myStudentId)
                            {
                                secondIsCanDelete = true;
                            }
                        }
                        commentOutputs.Add(new ParentCommentOutput()
                        {
                            CommentContent = mySecondComment.CommentContent,
                            CommentId = mySecondComment.Id,
                            Ot = mySecondComment.Ot.EtmsToMinuteString(),
                            OtDesc = EtmsHelper.GetOtFriendlyDesc(mySecondComment.Ot),
                            RelatedManAvatar = secondfirstrelatedManAvatar,
                            RelatedManName = secondfirstrelatedManName,
                            ReplyId = mySecondComment.ReplyId,
                            SourceType = mySecondComment.SourceType,
                            ReplyRelatedManName = firstrelatedManName,
                            IsCanDelete = secondIsCanDelete
                        });
                    }
                }
            }
            return commentOutputs;
        }

        public async Task<ResponseBase> GrowthRecordChangeFavorite(GrowthRecordChangeFavoriteRequest request)
        {
            var growthRecordDetail = await _activeGrowthRecordDAL.GetActiveGrowthRecordDetail(request.GrowthRecordDetailId);
            if (growthRecordDetail == null)
            {
                return ResponseBase.CommonError("成长档案不存在");
            }
            await _activeGrowthRecordDAL.SetActiveGrowthRecordDetailNewFavoriteStatus(request.GrowthRecordDetailId, request.NewFavoriteStatus);
            await _activeGrowthRecordDAL.SetActiveGrowthRecordNewFavoriteStatus(growthRecordDetail.GrowthRecordId, request.NewFavoriteStatus);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> GrowthRecordAddComment(GrowthRecordAddCommentRequest request)
        {
            var growthRecordDetail = await _activeGrowthRecordDAL.GetActiveGrowthRecordDetail(request.GrowthRecordDetailId);
            if (growthRecordDetail == null)
            {
                return ResponseBase.CommonError("成长档案不存在");
            }
            var myStudentWechat = await _studentWechatDAL.GetStudentWechatByPhone(request.LoginPhone);
            var comment = new EtActiveGrowthRecordDetailComment()
            {
                CommentContent = request.CommentContent,
                Headimgurl = myStudentWechat?.Headimgurl,
                Nickname = myStudentWechat?.Nickname,
                IsDeleted = EmIsDeleted.Normal,
                Ot = DateTime.Now,
                ReplyId = null,
                SourceType = EmActiveCommentSourceType.Student,
                TenantId = request.LoginTenantId,
                GrowthRecordDetailId = growthRecordDetail.Id,
                GrowthRecordId = growthRecordDetail.GrowthRecordId,
                SourceId = growthRecordDetail.StudentId
            };
            await _activeGrowthRecordDAL.AddActiveGrowthRecordDetailComment(comment);

            _eventPublisher.Publish(new NoticeUserActiveGrowthCommentEvent(request.LoginTenantId)
            {
                ActiveGrowthRecordDetailComment = comment,
                StudentId = growthRecordDetail.StudentId
            });
            await _studentOperationLogDAL.AddStudentLog(growthRecordDetail.StudentId, request.LoginTenantId, $"评论成长档案：{growthRecordDetail.GrowthContent}", EmStudentOperationLogType.GrowthRecord);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> GrowthRecordDelComment(GrowthRecordDelCommentRequest request)
        {
            var growthRecordDetail = await _activeGrowthRecordDAL.GetActiveGrowthRecordDetail(request.GrowthRecordDetailId);
            if (growthRecordDetail == null)
            {
                return ResponseBase.CommonError("成长档案不存在");
            }
            await _activeGrowthRecordDAL.DelActiveGrowthRecordDetailComment(growthRecordDetail.GrowthRecordId, request.CommentId);

            await _studentOperationLogDAL.AddStudentLog(growthRecordDetail.StudentId, request.LoginTenantId, "删除成长档案评论", EmStudentOperationLogType.GrowthRecord);
            return ResponseBase.Success();
        }
    }
}
