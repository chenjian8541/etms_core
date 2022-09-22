using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Educational.Output;
using ETMS.Entity.Dto.Educational.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IEventProvider;
using ETMS.Utility;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ETMS.Business
{
    public class ClassRecordBLL : IClassRecordBLL
    {
        private readonly IClassRecordDAL _classRecordDAL;

        private readonly IClassRoomDAL _classRoomDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IClassDAL _classDAL;

        private readonly IUserDAL _userDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IStudentPointsLogDAL _studentPointsLogDAL;

        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IStudentCourseConsumeLogDAL _studentCourseConsumeLogDAL;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly ICommonHandlerBLL _commonHandlerBLL;

        private readonly IClassCategoryDAL _classCategoryDAL;

        public ClassRecordBLL(IClassRecordDAL classRecordDAL, IClassRoomDAL classRoomDAL, ICourseDAL courseDAL, IClassDAL classDAL,
            IUserDAL userDAL, IStudentDAL studentDAL, IUserOperationLogDAL userOperationLogDAL, IStudentPointsLogDAL studentPointsLogDAL,
            IStudentCourseDAL studentCourseDAL, IEventPublisher eventPublisher, IStudentCourseConsumeLogDAL studentCourseConsumeLogDAL,
            IHttpContextAccessor httpContextAccessor, IAppConfigurtaionServices appConfigurtaionServices, ICommonHandlerBLL commonHandlerBLL,
            IClassCategoryDAL classCategoryDAL)
        {
            this._classRecordDAL = classRecordDAL;
            this._classRoomDAL = classRoomDAL;
            this._courseDAL = courseDAL;
            this._classDAL = classDAL;
            this._userDAL = userDAL;
            this._studentDAL = studentDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._studentPointsLogDAL = studentPointsLogDAL;
            this._studentCourseDAL = studentCourseDAL;
            this._eventPublisher = eventPublisher;
            this._studentCourseConsumeLogDAL = studentCourseConsumeLogDAL;
            this._httpContextAccessor = httpContextAccessor;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._commonHandlerBLL = commonHandlerBLL;
            this._classCategoryDAL = classCategoryDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._commonHandlerBLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _classRecordDAL, this._classRoomDAL, this._courseDAL, _studentDAL, _classDAL, _userDAL, _userOperationLogDAL,
                _studentPointsLogDAL, _studentCourseDAL, _studentCourseConsumeLogDAL, _classCategoryDAL);
        }

        public async Task<ResponseBase> ClassRecordGetPaging(ClassRecordGetPagingRequest request)
        {
            var pagingData = await _classRecordDAL.GetPaging(request);
            var output = new List<ClassRecordGetPagingOutput>();
            var allClassRoom = await _classRoomDAL.GetAllClassRoom();
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var tempBoxUser = new DataTempBox<EtUser>();
            var tempBoxClass = new DataTempBox<EtClass>();
            if (pagingData.Item1.Any())
            {
                var allClassCategory = await _classCategoryDAL.GetAllClassCategory();
                foreach (var classRecord in pagingData.Item1)
                {
                    var classRoomIdsDesc = string.Empty;
                    var courseListDesc = string.Empty;
                    var courseStyleColor = string.Empty;
                    var className = string.Empty;
                    var teachersDesc = string.Empty;
                    var classCategoryName = string.Empty;
                    var etClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, classRecord.ClassId);
                    var courseInfo = await ComBusiness.GetCourseNameAndColor(tempBoxCourse, _courseDAL, classRecord.CourseList);
                    classRoomIdsDesc = ComBusiness.GetDesc(allClassRoom, classRecord.ClassRoomIds);
                    className = etClass.Name;
                    courseListDesc = courseInfo.Item1;
                    courseStyleColor = courseInfo.Item2;
                    teachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, classRecord.Teachers);
                    if (classRecord.ClassCategoryId != null)
                    {
                        var myClassCategory = allClassCategory.FirstOrDefault(j => j.Id == classRecord.ClassCategoryId);
                        if (myClassCategory != null)
                        {
                            classCategoryName = myClassCategory.Name;
                        }
                    }
                    output.Add(new ClassRecordGetPagingOutput()
                    {
                        CId = classRecord.Id,
                        ClassContent = classRecord.ClassContent,
                        ClassId = classRecord.ClassId,
                        ClassName = className,
                        ClassOtDesc = classRecord.ClassOt.EtmsToDateString(),
                        ClassTimeDesc = $"{EtmsHelper.GetTimeDesc(classRecord.StartTime)}~{EtmsHelper.GetTimeDesc(classRecord.EndTime)}",
                        ClassRoomIdsDesc = classRoomIdsDesc,
                        CourseListDesc = courseListDesc,
                        Status = classRecord.Status,
                        WeekDesc = $"周{EtmsHelper.GetWeekDesc(classRecord.Week)}",
                        TeachersDesc = teachersDesc,
                        AttendNumber = classRecord.AttendNumber,
                        CheckOt = classRecord.CheckOt,
                        CheckUserId = classRecord.CheckUserId,
                        NeedAttendNumber = classRecord.NeedAttendNumber,
                        ClassTimes = classRecord.ClassTimes.EtmsToString(),
                        DeSum = EmRoleSecrecyType.GetSecrecyValue(request.SecrecyType, request.SecrecyDataBag, classRecord.DeSum.EtmsToString2()),
                        StatusDesc = EmClassRecordStatus.GetClassRecordStatusDesc(classRecord.Status),
                        CheckUserDesc = await ComBusiness.GetUserName(tempBoxUser, _userDAL, classRecord.CheckUserId),
                        ClassCategoryName = classCategoryName,
                        Remark = classRecord.Remark
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ClassRecordGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> ClassRecordGetPagingH5(ClassRecordGetPagingRequest request)
        {
            return ResponseBase.Success(await ClassRecordGetPaging(request));
        }

        public async Task<ResponseBase> ClassRecordGet(ClassRecordGetRequest request)
        {
            var classRecord = await _classRecordDAL.GetClassRecord(request.ClassRecordId);
            if (classRecord == null)
            {
                return ResponseBase.CommonError("上课记录不存在");
            }
            var etClass = await _classDAL.GetClassBucket(classRecord.ClassId);
            if (etClass == null || etClass.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var courseInfo = await ComBusiness.GetCourseNameAndColor(tempBoxCourse, _courseDAL, classRecord.CourseList);
            var classRoomIdsDesc = string.Empty;
            if (!string.IsNullOrEmpty(classRecord.ClassRoomIds))
            {
                var allClassRoom = await _classRoomDAL.GetAllClassRoom();
                classRoomIdsDesc = ComBusiness.GetDesc(allClassRoom, classRecord.ClassRoomIds);
            }
            var myClass = etClass.EtClass;
            var courseListDesc = courseInfo.Item1;
            var tempBoxUser = new DataTempBox<EtUser>();
            var teachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, classRecord.Teachers);
            return ResponseBase.Success(new ClassRecordGet()
            {
                CId = classRecord.Id,
                ClassContent = classRecord.ClassContent,
                ClassId = classRecord.ClassId,
                ClassName = myClass.Name,
                ClassOtDesc = classRecord.ClassOt.EtmsToDateString(),
                ClassTimeDesc = $"{EtmsHelper.GetTimeDesc(classRecord.StartTime)}~{EtmsHelper.GetTimeDesc(classRecord.EndTime)}",
                ClassRoomIdsDesc = classRoomIdsDesc,
                CourseListDesc = courseListDesc,
                Status = classRecord.Status,
                WeekDesc = $"周{EtmsHelper.GetWeekDesc(classRecord.Week)}",
                TeachersDesc = teachersDesc,
                AttendNumber = classRecord.AttendNumber,
                CheckOt = classRecord.CheckOt,
                CheckUserId = classRecord.CheckUserId,
                NeedAttendNumber = classRecord.NeedAttendNumber,
                ClassTimes = classRecord.ClassTimes.EtmsToString(),
                DeSum = EmRoleSecrecyType.GetSecrecyValue(request.SecrecyType, request.SecrecyDataBag, classRecord.DeSum.EtmsToString2()),
                StatusDesc = EmClassRecordStatus.GetClassRecordStatusDesc(classRecord.Status),
                CheckUserDesc = await ComBusiness.GetUserName(tempBoxUser, _userDAL, classRecord.CheckUserId),
                IsLeaveCharge = myClass.IsLeaveCharge,
                IsNotComeCharge = myClass.IsNotComeCharge,
                Remark = classRecord.Remark
            });
        }

        public async Task<ResponseBase> ClassRecordStudentGet(ClassRecordStudentGetRequest request)
        {
            var classRecordStudents = await _classRecordDAL.GetClassRecordStudents(request.ClassRecordId);
            var outPut = new List<ClassRecordStudentGetOutput>();

            var courseTempBox = new DataTempBox<EtCourse>();
            foreach (var p in classRecordStudents)
            {
                var studentBucket = await _studentDAL.GetStudent(p.StudentId);
                if (studentBucket == null || studentBucket.Student == null)
                {
                    continue;
                }
                var deClassTimes = p.DeClassTimes.EtmsToString();
                var myCourse = await ComBusiness.GetCourse(courseTempBox, _courseDAL, p.CourseId);
                var courseDesc = string.Empty;
                var checkPointsDefault = 0;
                if (myCourse != null)
                {
                    courseDesc = myCourse.Name;
                    checkPointsDefault = myCourse.CheckPoints;
                }
                outPut.Add(new ClassRecordStudentGetOutput()
                {
                    CId = p.Id,
                    CourseDesc = courseDesc,
                    CheckPointsDefault = checkPointsDefault,
                    CourseId = p.CourseId,
                    DeClassTimes = deClassTimes,
                    DeSum = EmRoleSecrecyType.GetSecrecyValue(request.SecrecyType, request.SecrecyDataBag, p.DeSum.EtmsToString2()),
                    DeType = p.DeType,
                    DeTypeDesc = EmDeClassTimesType.GetDeClassTimesTypeDesc(p.DeType),
                    ExceedClassTimes = p.ExceedClassTimes.EtmsToString(),
                    IsRewardPoints = p.IsRewardPoints,
                    Remark = p.Remark,
                    RewardPoints = p.RewardPoints,
                    StudentCheckStatus = p.StudentCheckStatus,
                    StudentCheckStatusDesc = EmClassStudentCheckStatus.GetClassStudentCheckStatus(p.StudentCheckStatus),
                    StudentName = studentBucket.Student.Name,
                    StudentPhone = ComBusiness3.PhoneSecrecy(studentBucket.Student.Phone, request.SecrecyType, request.SecrecyDataBag),
                    StudentType = p.StudentType,
                    StudentTypeDesc = EmClassStudentType.GetClassStudentTypeDesc(p.StudentType),
                    DeClassTimesDesc = ComBusiness2.GetDeClassTimesDesc(p.DeType, p.DeClassTimes, p.ExceedClassTimes),
                    Status = p.Status,
                    StatusDesc = EmClassRecordStatus.GetClassRecordStatusDesc(p.Status),
                    ChangeRowState = 0,
                    NewDeClassTimes = deClassTimes,
                    NewRemark = p.Remark,
                    NewStudentCheckStatus = p.StudentCheckStatus,
                    NewRewardPoints = p.RewardPoints,
                    SurplusCourseDesc = p.SurplusCourseDesc,
                    StudentAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, studentBucket.Student.Avatar)
                });
            }
            return ResponseBase.Success(outPut);
        }

        public async Task<ResponseBase> ClassRecordOperationLogGetPaging(ClassRecordOperationLogGetPagingRequest request)
        {
            var pagingData = await _classRecordDAL.GetClassRecordOperationLogPaging(request);
            var output = new List<ClassRecordOperationLogGetOutput>();
            var tempBoxUser = new DataTempBox<EtUser>();
            foreach (var p in pagingData.Item1)
            {
                var userName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, p.UserId);
                output.Add(new ClassRecordOperationLogGetOutput()
                {
                    CId = p.Id,
                    ClassRecordId = p.ClassRecordId,
                    UserId = p.UserId,
                    OpContent = p.OpContent,
                    OpType = p.OpType,
                    OpTypeDesc = EmClassRecordOperationType.GetClassRecordOperationTypeDesc(p.OpType),
                    Ot = p.Ot,
                    Remark = p.Remark,
                    UserName = userName
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ClassRecordOperationLogGetOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> ClassRecordAbsenceLogPaging(ClassRecordAbsenceLogPagingRequest request)
        {
            var pagingData = await _classRecordDAL.GetClassRecordAbsenceLogPaging(request);
            var output = new List<ClassRecordAbsenceLogPagingOutput>();

            var courseTempBox = new DataTempBox<EtCourse>();
            var tempBoxUser = new DataTempBox<EtUser>();
            foreach (var p in pagingData.Item1)
            {
                var className = string.Empty;
                var teachersDesc = string.Empty;
                var etClass = await _classDAL.GetClassBucket(p.ClassId);
                className = etClass.EtClass.Name;
                teachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, p.Teachers);
                output.Add(new ClassRecordAbsenceLogPagingOutput()
                {
                    CId = p.Id,
                    ClassContent = p.ClassContent,
                    ClassId = p.ClassId,
                    ClassName = className,
                    ClassOtDesc = p.ClassOt.EtmsToDateString(),
                    ClassTimeDesc = $"{EtmsHelper.GetTimeDesc(p.StartTime)}~{EtmsHelper.GetTimeDesc(p.EndTime)}",
                    Status = p.Status,
                    TeachersDesc = teachersDesc,
                    CheckOt = p.CheckOt,
                    CheckUserId = p.CheckUserId,
                    StatusDesc = EmClassRecordStatus.GetClassRecordStatusDesc(p.Status),
                    CheckUserName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, p.CheckUserId),
                    CourseDesc = await ComBusiness.GetCourseName(courseTempBox, _courseDAL, p.CourseId),
                    CourseId = p.CourseId,
                    DeClassTimes = p.DeClassTimes.EtmsToString(),
                    DeType = p.DeType,
                    DeTypeDesc = EmDeClassTimesType.GetDeClassTimesTypeDesc(p.DeType),
                    ExceedClassTimes = p.ExceedClassTimes.EtmsToString(),
                    HandleContent = p.HandleContent,
                    HandleOtDesc = p.HandleOt == null ? string.Empty : p.HandleOt.Value.EtmsToString(),
                    HandleStatus = p.HandleStatus,
                    HandleStatusDesc = EmClassRecordAbsenceHandleStatus.GetClassRecordAbsenceHandleStatusDesc(p.HandleStatus),
                    Remark = p.Remark,
                    StudentId = p.StudentId,
                    StudentName = p.StudentName,
                    StudentPhone = ComBusiness3.PhoneSecrecy(p.StudentPhone, request.SecrecyType, request.SecrecyDataBag),
                    StudentType = p.StudentType,
                    StudentTypeDesc = EmClassStudentType.GetClassStudentTypeDesc(p.StudentType),
                    StudentCheckStatus = p.StudentCheckStatus,
                    StudentCheckStatusDesc = EmClassStudentCheckStatus.GetClassStudentCheckStatus(p.StudentCheckStatus),
                    HandleUserName = p.HandleUser == null ? string.Empty : await ComBusiness.GetUserName(tempBoxUser, _userDAL, p.HandleUser.Value),
                    DeClassTimesDesc = ComBusiness2.GetDeClassTimesDesc(p.DeType, p.DeClassTimes, p.ExceedClassTimes),
                    WeekDesc = $"周{EtmsHelper.GetWeekDesc(p.Week)}",
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ClassRecordAbsenceLogPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> ClassRecordAbsenceLogHandle(ClassRecordAbsenceLogHandleRequest request)
        {
            var log = await _classRecordDAL.GetClassRecordAbsenceLog(request.ClassRecordAbsenceLogId);
            if (log == null)
            {
                return ResponseBase.CommonError("缺勤记录不存在");
            }
            if (log.HandleStatus == EmClassRecordAbsenceHandleStatus.MarkFinish)
            {
                return ResponseBase.CommonError("此缺勤记录已标记补课");
            }
            log.HandleContent = request.HandleContent;
            log.HandleOt = DateTime.Now;
            log.HandleUser = request.LoginUserId;
            log.HandleStatus = EmClassRecordAbsenceHandleStatus.MarkFinish;
            await _classRecordDAL.UpdateClassRecordAbsenceLog(log);
            _eventPublisher.Publish(new ResetTenantToDoThingEvent(request.LoginTenantId));

            await _userOperationLogDAL.AddUserLog(request, "标记缺勤补课", EmUserOperationType.ClassRecordManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassRecordPointsApplyHandle(ClassRecordPointsApplyHandleRequest request)
        {
            var processMsg = await ClassRecordPointsApplyHandleProcess(DateTime.Now, request.ClassRecordPointsApplyLogId, request.NewHandleStatus,
                request.HandleContent, request.LoginUserId);
            if (!string.IsNullOrEmpty(processMsg))
            {
                return ResponseBase.CommonError(processMsg);
            }
            await _userOperationLogDAL.AddUserLog(request, "课堂积分奖励审核", EmUserOperationType.ClassRecordManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassRecordPointsApplyHandleBatch(ClassRecordPointsApplyHandleBatchRequest request)
        {
            var now = DateTime.Now;
            foreach (var p in request.ClassRecordPointsApplyLogIds)
            {
                await ClassRecordPointsApplyHandleProcess(now, p, request.NewHandleStatus, request.HandleContent, request.LoginUserId);
            }

            await _userOperationLogDAL.AddUserLog(request, "课堂积分奖励批量审核", EmUserOperationType.ClassRecordManage);
            return ResponseBase.Success();
        }

        private async Task<string> ClassRecordPointsApplyHandleProcess(DateTime now, long classRecordPointsApplyLogId,
            byte newHandleStatus, string handleContent, long userId)
        {
            var log = await _classRecordDAL.GetClassRecordPointsApplyLog(classRecordPointsApplyLogId);
            if (log == null)
            {
                return "申请记录不存在";
            }
            if (log.HandleStatus != EmClassRecordPointsApplyHandleStatus.NotCheckd)
            {
                return "此申请记录已审核";
            }
            log.HandleOt = now;
            log.HandleStatus = newHandleStatus;
            log.HandleUser = userId;
            log.HandleContent = handleContent;
            await _classRecordDAL.EditClassRecordPointsApplyLog(log);
            if (log.HandleStatus == EmClassRecordPointsApplyHandleStatus.CheckPass)
            {
                await _studentPointsLogDAL.AddStudentPointsLog(new EtStudentPointsLog()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    No = string.Empty,
                    Ot = DateTime.Now,
                    Points = log.Points,
                    Remark = log.Remark,
                    StudentId = log.StudentId,
                    TenantId = log.TenantId,
                    Type = EmStudentPointsLogType.ClassReward
                });
                await _studentDAL.AddPoint(log.StudentId, log.Points);
            }
            return string.Empty;
        }

        public async Task<ResponseBase> StudentClassRecordGetPaging(StudentClassRecordGetPagingRequest request)
        {
            var pagingData = await _classRecordDAL.GetClassRecordStudentPaging(request);
            var output = new List<StudentClassRecordGetPagingOutput>();
            var courseTempBox = new DataTempBox<EtCourse>();
            var tempBoxUser = new DataTempBox<EtUser>();
            var tempBoxStudent = new DataTempBox<EtStudent>();
            foreach (var p in pagingData.Item1)
            {
                var etClass = await _classDAL.GetClassBucket(p.ClassId);
                var teachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, p.Teachers);
                var item = new StudentClassRecordGetPagingOutput()
                {
                    CheckOt = p.CheckOt,
                    CheckUserId = p.CheckUserId,
                    CheckUserName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, p.CheckUserId),
                    ClassContent = p.ClassContent,
                    ClassId = p.ClassId,
                    ClassName = etClass.EtClass.Name,
                    ClassOt = p.ClassOt,
                    ClassOtDesc = p.ClassOt.EtmsToDateString(),
                    ClassTimeDesc = $"{EtmsHelper.GetTimeDesc(p.StartTime)}~{EtmsHelper.GetTimeDesc(p.EndTime)}",
                    ClassRecordId = p.ClassRecordId,
                    ClassRoomIds = p.ClassRoomIds,
                    CourseDesc = await ComBusiness.GetCourseName(courseTempBox, _courseDAL, p.CourseId),
                    CourseId = p.CourseId,
                    DeClassTimes = p.DeClassTimes.EtmsToString(),
                    DeSum = EmRoleSecrecyType.GetSecrecyValue(request.SecrecyType, request.SecrecyDataBag, p.DeSum.EtmsToString2()),
                    DeType = p.DeType,
                    EndTime = p.EndTime,
                    EvaluateTeacherNum = p.EvaluateTeacherNum,
                    ExceedClassTimes = p.ExceedClassTimes.EtmsToString(),
                    IsBeEvaluate = p.IsBeEvaluate,
                    IsRewardPoints = p.IsRewardPoints,
                    Remark = p.Remark,
                    RewardPoints = p.RewardPoints,
                    StartTime = p.StartTime,
                    Status = p.Status,
                    StatusDesc = EmClassRecordStatus.GetClassRecordStatusDesc(p.Status),
                    StudentCheckStatus = p.StudentCheckStatus,
                    StudentCheckStatusDesc = EmClassStudentCheckStatus.GetClassStudentCheckStatus(p.StudentCheckStatus),
                    StudentId = p.StudentId,
                    StudentTryCalssLogId = p.StudentTryCalssLogId,
                    StudentType = p.StudentType,
                    StudentTypeDesc = EmClassStudentType.GetClassStudentTypeDesc2(p.StudentType),
                    TeacherNum = p.TeacherNum,
                    Teachers = p.Teachers,
                    TeachersDesc = teachersDesc,
                    Week = p.Week,
                    DeClassTimesDesc = ComBusiness2.GetDeClassTimesDesc(p.DeType, p.DeClassTimes, p.ExceedClassTimes),
                    IsHasExceedClassTimes = p.ExceedClassTimes > 0,
                    WeekDesc = $"周{EtmsHelper.GetWeekDesc(p.Week)}",
                    SurplusCourseDesc = p.SurplusCourseDesc
                };
                if (request.IsGetStudent)
                {
                    var myStudent = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId);
                    if (myStudent != null)
                    {
                        item.StudentName = myStudent.Name;
                        item.StudentPhone = myStudent.Phone;
                    }
                }
                output.Add(item);
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentClassRecordGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> ClassRecordPointsApplyLogPaging(ClassRecordPointsApplyLogPagingRequest request)
        {
            var pagingData = await _classRecordDAL.GetClassRecordPointsApplyLog(request);
            var output = new List<ClassRecordPointsApplyLogPagingOutput>();

            var courseTempBox = new DataTempBox<EtCourse>();
            var tempBoxUser = new DataTempBox<EtUser>();
            foreach (var p in pagingData.Item1)
            {
                var className = string.Empty;
                var teacherDesc = string.Empty;
                var etClass = await _classDAL.GetClassBucket(p.ClassId);
                className = etClass.EtClass.Name;
                teacherDesc = await ComBusiness.GetUserName(tempBoxUser, _userDAL, p.TeacherId);
                output.Add(new ClassRecordPointsApplyLogPagingOutput()
                {
                    CId = p.Id,
                    ClassId = p.ClassId,
                    ClassName = className,
                    ClassOtDesc = p.ClassOt.EtmsToDateString(),
                    ClassTimeDesc = $"{EtmsHelper.GetTimeDesc(p.StartTime)}~{EtmsHelper.GetTimeDesc(p.EndTime)}",
                    Status = p.Status,
                    TeacherDesc = teacherDesc,
                    CheckOt = p.CheckOt,
                    CheckUserId = p.CheckUserId,
                    StatusDesc = EmClassRecordStatus.GetClassRecordStatusDesc(p.Status),
                    CourseDesc = await ComBusiness.GetCourseName(courseTempBox, _courseDAL, p.CourseId),
                    HandleOtDesc = p.HandleOt == null ? string.Empty : p.HandleOt.Value.EtmsToString(),
                    HandleStatus = p.HandleStatus,
                    HandleStatusDesc = EmClassRecordPointsApplyHandleStatus.GetClassRecordPointsApplyHandleStatusDesc(p.HandleStatus),
                    Remark = p.Remark,
                    StudentId = p.StudentId,
                    StudentName = p.StudentName,
                    StudentPhone = ComBusiness3.PhoneSecrecy(p.StudentPhone, request.SecrecyType, request.SecrecyDataBag),
                    StudentType = p.StudentType,
                    StudentTypeDesc = EmClassStudentType.GetClassStudentTypeDesc(p.StudentType),
                    StudentCheckStatus = p.StudentCheckStatus,
                    StudentCheckStatusDesc = EmClassStudentCheckStatus.GetClassStudentCheckStatus(p.StudentCheckStatus),
                    HandleUserName = p.HandleUser == null ? string.Empty : await ComBusiness.GetUserName(tempBoxUser, _userDAL, p.HandleUser.Value),
                    WeekDesc = $"周{EtmsHelper.GetWeekDesc(p.Week)}",
                    ApplyOt = p.ApplyOt,
                    HandleUser = p.HandleUser,
                    HandleOt = p.HandleOt,
                    TeacherId = p.TeacherId,
                    ClassOt = p.ClassOt,
                    ClassRecordId = p.ClassRecordId,
                    ClassRoomIds = p.ClassRoomIds,
                    CourseId = p.CourseId,
                    EndTime = p.EndTime,
                    Points = p.Points,
                    StartTime = p.StartTime,
                    StudentTryCalssLogId = p.StudentTryCalssLogId,
                    Week = p.Week,
                    HandleContent = p.HandleContent
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ClassRecordPointsApplyLogPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> ClassRecordStudentChange(ClassRecordStudentChangeRequest request)
        {
            var p = await _classRecordDAL.GetEtClassRecordStudentById(request.ClassRecordStudentId);
            if (p == null)
            {
                return ResponseBase.CommonError("不存在此点名记录");
            }
            if (p.Status == EmClassRecordStatus.Revoked)
            {
                return ResponseBase.CommonError("点名记录已撤销，无法修改");
            }
            var studentBuck = await _studentDAL.GetStudent(p.StudentId);
            if (studentBuck == null || studentBuck.Student == null)
            {
                return ResponseBase.CommonError("学员信息不存在");
            }

            var oldDeSum = p.DeSum;
            var oldDeClassTimes = p.DeClassTimes;
            var oldCheckStatus = p.StudentCheckStatus;
            var oldRemark = p.Remark;
            var oldPoints = p.RewardPoints;
            var now = DateTime.Now;
            var processReslut = await ClassRecordStudentResetDeClassTimes(p, request, now);
            p = processReslut.Item1;
            await _classRecordDAL.EditClassRecordStudent(processReslut.Item1, processReslut.Item2);

            if (oldPoints != request.NewRewardPoints)
            {
                await ClassRecordStudentResetRewardPoints(p, oldPoints, request.NewRewardPoints, now);
            }

            var addAttendNumber = 0;
            if (EmClassStudentCheckStatus.CheckIsAttend(oldCheckStatus))
            {
                addAttendNumber -= 1;
            }
            if (EmClassStudentCheckStatus.CheckIsAttend(p.StudentCheckStatus))
            {
                addAttendNumber += 1;
            }
            var classRecord = await _classRecordDAL.GetClassRecord(p.ClassRecordId);
            classRecord.DeSum = classRecord.DeSum - oldDeSum + p.DeSum;
            classRecord.AttendNumber += addAttendNumber;
            await _classRecordDAL.EditClassRecord(classRecord);

            await _classRecordDAL.AddClassRecordOperationLog(new EtClassRecordOperationLog()
            {
                ClassId = p.ClassId,
                ClassRecordId = p.ClassRecordId,
                IsDeleted = EmIsDeleted.Normal,
                OpType = EmClassRecordOperationType.ModifyStudentClassRecord,
                Ot = now,
                Remark = string.Empty,
                Status = p.Status,
                TenantId = p.TenantId,
                UserId = request.LoginUserId,
                OpContent = $"修改学员[{studentBuck.Student.Name}]点名信息，状态从：{EmClassStudentCheckStatus.GetClassStudentCheckStatus(oldCheckStatus)}改成{EmClassStudentCheckStatus.GetClassStudentCheckStatus(p.StudentCheckStatus)}，扣减课时从：{oldDeClassTimes.EtmsToString()}改成{p.DeClassTimes.EtmsToString()}，原备注：{oldRemark}"
            });

            //发通知  已在计算剩余课时之后 发送提醒
            //_eventPublisher.Publish(new NoticeStudentCourseSurplusEvent(request.LoginTenantId)
            //{
            //    CourseId = p.CourseId,
            //    StudentId = p.StudentId
            //});

            if (!EtmsHelper2.IsThisMonth(classRecord.ClassOt))
            {
                _eventPublisher.Publish(new StatisticsEducationEvent(request.LoginTenantId)
                {
                    Time = classRecord.ClassOt
                });
            }
            _eventPublisher.Publish(new StatisticsClassEvent(request.LoginTenantId)
            {
                ClassOt = classRecord.ClassOt
            });

            _eventPublisher.Publish(new StatisticsTeacherSalaryClassTimesEvent(request.LoginTenantId)
            {
                ClassRecordId = classRecord.Id
            });
            _eventPublisher.Publish(new StatisticsTeacherSalaryClassDayEvent(request.LoginTenantId)
            {
                Time = classRecord.ClassOt
            });

            await _userOperationLogDAL.AddUserLog(request, "修改点名记录", EmUserOperationType.ClassRecordManage, now);
            return ResponseBase.Success();
        }

        private async Task<Tuple<EtClassRecordStudent, bool>> ClassRecordStudentResetDeClassTimes(EtClassRecordStudent p,
            ClassRecordStudentChangeRequest request, DateTime now)
        {
            p.StudentCheckStatus = request.NewStudentCheckStatus;
            p.Remark = request.NewRemark;
            p.RewardPoints = request.NewRewardPoints;
            p.IsRewardPoints = request.NewRewardPoints > 0;
            if (p.DeClassTimes + p.ExceedClassTimes == request.NewDeClassTimes)
            {
                return Tuple.Create(p, false);
            }
            var studentCourseConsumeLogs = new List<EtStudentCourseConsumeLog>();
            if (p.DeType == EmDeClassTimesType.ClassTimes && p.DeClassTimes > 0)
            {
                if (p.DeStudentCourseDetailId == null)
                {
                    LOG.Log.Error($"[ClassRecordStudentChange]扣减的课时未记录具体的扣减订单:{JsonConvert.SerializeObject(request)}", this.GetType());
                }
                else
                {
                    //原路返还所扣除的课时
                    await _studentCourseDAL.AddClassTimesOfStudentCourseDetail(p.DeStudentCourseDetailId.Value, p.DeClassTimes);

                    //课消记录
                    studentCourseConsumeLogs.Add(new EtStudentCourseConsumeLog()
                    {
                        IsDeleted = EmIsDeleted.Normal,
                        DeClassTimes = p.DeClassTimes,
                        DeClassTimesSmall = 0,
                        CourseId = p.CourseId,
                        DeType = EmDeClassTimesType.ClassTimes,
                        OrderId = 0,
                        OrderNo = string.Empty,
                        Ot = now,
                        SourceType = EmStudentCourseConsumeSourceType.ModifyStudentClassRecordAdd,
                        StudentId = p.StudentId,
                        TenantId = p.TenantId,
                        DeSum = 0,
                        Remark = p.Remark,
                    });

                    await _commonHandlerBLL.AnalyzeStudentCourseDetailRestoreNormalStatus(p.DeStudentCourseDetailId.Value);
                }
            }
            if (p.ExceedClassTimes > 0)
            {
                await _studentCourseDAL.DeExceedTotalClassTimes(p.StudentId, p.CourseId, p.ExceedClassTimes);
            }

            p.DeClassTimes = request.NewDeClassTimes;
            p.StudentCheckStatus = request.NewStudentCheckStatus;
            p.ExceedClassTimes = 0;
            p.Remark = request.NewRemark;
            var deStudentClassTimesResult = await CoreBusiness.DeStudentClassTimes(_studentCourseDAL, new DeStudentClassTimesTempRequest()
            {
                ClassOt = p.ClassOt,
                TenantId = p.TenantId,
                StudentId = p.StudentId,
                DeClassTimes = p.DeClassTimes,
                CourseId = p.CourseId
            });
            _eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(p.TenantId)
            {
                StudentId = p.StudentId,
                CourseId = p.CourseId,
                IsSendNoticeStudent = true
            });

            p.DeSum = deStudentClassTimesResult.DeSum;
            p.DeType = deStudentClassTimesResult.DeType;
            p.ExceedClassTimes = deStudentClassTimesResult.ExceedClassTimes;
            if (deStudentClassTimesResult.DeType != EmDeClassTimesType.ClassTimes)
            {
                p.DeClassTimes = 0;
            }
            else
            {
                p.DeClassTimes = deStudentClassTimesResult.DeClassTimes;
            }
            p.DeStudentCourseDetailId = deStudentClassTimesResult.DeStudentCourseDetailId;
            if (deStudentClassTimesResult.DeType != EmDeClassTimesType.NotDe)
            {
                studentCourseConsumeLogs.Add(new EtStudentCourseConsumeLog()
                {
                    CourseId = p.CourseId,
                    DeClassTimes = deStudentClassTimesResult.DeClassTimes,
                    DeType = deStudentClassTimesResult.DeType,
                    IsDeleted = EmIsDeleted.Normal,
                    OrderId = deStudentClassTimesResult.OrderId,
                    OrderNo = deStudentClassTimesResult.OrderNo,
                    Ot = p.CheckOt,
                    SourceType = EmStudentCourseConsumeSourceType.ModifyStudentClassRecordDe,
                    StudentId = p.StudentId,
                    TenantId = p.TenantId,
                    DeClassTimesSmall = 0,
                    DeSum = deStudentClassTimesResult.DeSum,
                    Remark = request.NewRemark
                });
            }
            if (studentCourseConsumeLogs.Count > 0)
            {
                _studentCourseConsumeLogDAL.AddStudentCourseConsumeLog(studentCourseConsumeLogs); //课消记录
            }
            return Tuple.Create(p, true);
        }

        private async Task ClassRecordStudentResetRewardPoints(EtClassRecordStudent p, int oldPoints, int newPoint, DateTime now)
        {
            var pointsLogs = new List<EtStudentPointsLog>();
            if (oldPoints > 0) //返还之前的积分
            {
                var isProcessOldPoints = false;
                var checkLog = await _classRecordDAL.GetClassRecordPointsApplyLogByClassRecordId(p.ClassRecordId, p.StudentId);
                if (checkLog != null && checkLog.Status == EmClassRecordStatus.Normal)
                {
                    if (checkLog.HandleStatus != EmClassRecordPointsApplyHandleStatus.CheckPass)
                    {
                        isProcessOldPoints = true;
                    }
                    checkLog.Status = EmClassRecordStatus.Revoked;
                    checkLog.Remark = $"{checkLog.Remark} 修改点名记录-撤销原记录赠送的积分";
                    await _classRecordDAL.EditClassRecordPointsApplyLog(checkLog);
                }
                if (!isProcessOldPoints)
                {
                    await _studentDAL.DeductionPoint(p.StudentId, oldPoints);
                    pointsLogs.Add(new EtStudentPointsLog()
                    {
                        StudentId = p.StudentId,
                        IsDeleted = EmIsDeleted.Normal,
                        No = string.Empty,
                        Ot = now,
                        Points = oldPoints,
                        Remark = "修改点名记录-撤销原记录赠送的积分",
                        TenantId = p.TenantId,
                        Type = EmStudentPointsLogType.ModifyStudentClassRecordDe
                    });
                }
            }
            if (newPoint > 0)
            {
                await _studentDAL.AddPoint(p.StudentId, newPoint);
                pointsLogs.Add(new EtStudentPointsLog()
                {
                    StudentId = p.StudentId,
                    IsDeleted = EmIsDeleted.Normal,
                    No = string.Empty,
                    Ot = now,
                    Points = newPoint,
                    Remark = "修改点名记录-赠送学员积分",
                    TenantId = p.TenantId,
                    Type = EmStudentPointsLogType.ModifyStudentClassRecordAdd
                });
            }
            if (pointsLogs.Count > 0)
            {
                _studentPointsLogDAL.AddStudentPointsLog(pointsLogs);
            }
        }
    }
}
