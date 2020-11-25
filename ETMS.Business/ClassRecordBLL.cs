﻿using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Educational.Output;
using ETMS.Entity.Dto.Educational.Request;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IEventProvider;
using ETMS.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

        public ClassRecordBLL(IClassRecordDAL classRecordDAL, IClassRoomDAL classRoomDAL, ICourseDAL courseDAL, IClassDAL classDAL,
            IUserDAL userDAL, IStudentDAL studentDAL, IUserOperationLogDAL userOperationLogDAL, IStudentPointsLogDAL studentPointsLogDAL,
            IStudentCourseDAL studentCourseDAL, IEventPublisher eventPublisher, IStudentCourseConsumeLogDAL studentCourseConsumeLogDAL)
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
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _classRecordDAL, this._classRoomDAL, this._courseDAL, _studentDAL, _classDAL, _userDAL, _userOperationLogDAL,
                _studentPointsLogDAL, _studentCourseDAL, _studentCourseConsumeLogDAL);
        }

        public async Task<ResponseBase> ClassRecordGetPaging(ClassRecordGetPagingRequest request)
        {
            var pagingData = await _classRecordDAL.GetPaging(request);
            var output = new List<ClassRecordGetPagingOutput>();
            var allClassRoom = await _classRoomDAL.GetAllClassRoom();
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var tempBoxUser = new DataTempBox<EtUser>();
            var tempBoxClass = new DataTempBox<EtClass>();
            foreach (var classRecord in pagingData.Item1)
            {
                var classRoomIdsDesc = string.Empty;
                var courseListDesc = string.Empty;
                var courseStyleColor = string.Empty;
                var className = string.Empty;
                var teachersDesc = string.Empty;
                var etClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, classRecord.ClassId);
                var courseInfo = await ComBusiness.GetCourseNameAndColor(tempBoxCourse, _courseDAL, classRecord.CourseList);
                classRoomIdsDesc = ComBusiness.GetDesc(allClassRoom, classRecord.ClassRoomIds);
                className = etClass.Name;
                courseListDesc = courseInfo.Item1;
                courseStyleColor = courseInfo.Item2;
                teachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, classRecord.Teachers);
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
                    ClassTimes = classRecord.ClassTimes,
                    DeSum = classRecord.DeSum,
                    StatusDesc = EmClassRecordStatus.GetClassRecordStatusDesc(classRecord.Status),
                    CheckUserDesc = await ComBusiness.GetUserName(tempBoxUser, _userDAL, classRecord.CheckUserId)
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ClassRecordGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> ClassRecordGetPagingH5(ClassRecordGetPagingH5Request request)
        {
            var req = new ClassRecordGetPagingRequest()
            {
                LoginTenantId = request.LoginTenantId,
                LoginUserId = request.LoginUserId,
                Ot = request.Ot,
                IpAddress = request.IpAddress,
                PageCurrent = request.PageCurrent,
                PageSize = request.PageSize,
                TeacherId = request.LoginUserId,
                IsDataLimit = true
            };
            return ResponseBase.Success(await ClassRecordGetPaging(req));
        }

        public async Task<ResponseBase> ClassRecordGet(ClassRecordGetRequest request)
        {
            var classRecord = await _classRecordDAL.GetClassRecord(request.ClassRecordId);
            var etClass = await _classDAL.GetClassBucket(classRecord.ClassId);
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var courseInfo = await ComBusiness.GetCourseNameAndColor(tempBoxCourse, _courseDAL, classRecord.CourseList);
            var classRoomIdsDesc = string.Empty;
            if (!string.IsNullOrEmpty(classRecord.ClassRoomIds))
            {
                var allClassRoom = await _classRoomDAL.GetAllClassRoom();
                classRoomIdsDesc = ComBusiness.GetDesc(allClassRoom, classRecord.ClassRoomIds);
            }
            var className = etClass.EtClass.Name;
            var courseListDesc = courseInfo.Item1;
            var tempBoxUser = new DataTempBox<EtUser>();
            var teachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, classRecord.Teachers);
            return ResponseBase.Success(new ClassRecordGet()
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
                ClassTimes = classRecord.ClassTimes,
                DeSum = classRecord.DeSum,
                StatusDesc = EmClassRecordStatus.GetClassRecordStatusDesc(classRecord.Status),
                CheckUserDesc = await ComBusiness.GetUserName(tempBoxUser, _userDAL, classRecord.CheckUserId)
            });
        }

        public async Task<ResponseBase> ClassRecordStudentGet(ClassRecordStudentGetRequest request)
        {
            var classRecordStudents = await _classRecordDAL.GetClassRecordStudents(request.ClassRecordId);
            var outPut = new List<ClassRecordStudentGetOutput>();

            var courseTempBox = new DataTempBox<EtCourse>();
            foreach (var p in classRecordStudents)
            {
                var student = await _studentDAL.GetStudent(p.StudentId);
                outPut.Add(new ClassRecordStudentGetOutput()
                {
                    CId = p.Id,
                    CourseDesc = await ComBusiness.GetCourseName(courseTempBox, _courseDAL, p.CourseId),
                    CourseId = p.CourseId,
                    DeClassTimes = p.DeClassTimes.EtmsToString(),
                    DeSum = p.DeSum,
                    DeType = p.DeType,
                    DeTypeDesc = EmDeClassTimesType.GetDeClassTimesTypeDesc(p.DeType),
                    ExceedClassTimes = p.ExceedClassTimes.EtmsToString(),
                    IsRewardPoints = p.IsRewardPoints,
                    Remark = p.Remark,
                    RewardPoints = p.RewardPoints,
                    StudentCheckStatus = p.StudentCheckStatus,
                    StudentCheckStatusDesc = EmClassStudentCheckStatus.GetClassStudentCheckStatus(p.StudentCheckStatus),
                    StudentName = student.Student.Name,
                    StudentPhone = student.Student.Phone,
                    StudentType = p.StudentType,
                    StudentTypeDesc = EmClassStudentType.GetClassStudentTypeDesc(p.StudentType),
                    DeClassTimesDesc = ComBusiness2.GetDeClassTimesDesc(p.DeType, p.DeClassTimes, p.ExceedClassTimes),
                    Status = p.Status,
                    StatusDesc = EmClassRecordStatus.GetClassRecordStatusDesc(p.Status),
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
                    StudentPhone = p.StudentPhone,
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
            await _userOperationLogDAL.AddUserLog(request, "标记缺勤补课", EmUserOperationType.ClassRecordManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassRecordPointsApplyHandle(ClassRecordPointsApplyHandleRequest request)
        {
            var log = await _classRecordDAL.GetClassRecordPointsApplyLog(request.ClassRecordPointsApplyLogId);
            if (log == null)
            {
                return ResponseBase.CommonError("申请记录不存在");
            }
            if (log.HandleStatus != EmClassRecordPointsApplyHandleStatus.NotCheckd)
            {
                return ResponseBase.CommonError("此申请记录已审核");
            }
            log.HandleOt = DateTime.Now;
            log.HandleStatus = request.NewHandleStatus;
            log.HandleUser = request.LoginUserId;
            log.HandleContent = request.HandleContent;
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
            await _userOperationLogDAL.AddUserLog(request, "课堂积分奖励审核", EmUserOperationType.ClassRecordManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentClassRecordGetPaging(StudentClassRecordGetPagingRequest request)
        {
            var pagingData = await _classRecordDAL.GetClassRecordStudentPaging(request);
            var output = new List<StudentClassRecordGetPagingOutput>();

            var courseTempBox = new DataTempBox<EtCourse>();
            var tempBoxUser = new DataTempBox<EtUser>();
            foreach (var p in pagingData.Item1)
            {
                var etClass = await _classDAL.GetClassBucket(p.ClassId);
                var teachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, p.Teachers);
                output.Add(new StudentClassRecordGetPagingOutput()
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
                    DeSum = p.DeSum,
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
                    StudentTypeDesc = EmClassStudentType.GetClassStudentTypeDesc(p.StudentType),
                    TeacherNum = p.TeacherNum,
                    Teachers = p.Teachers,
                    TeachersDesc = teachersDesc,
                    Week = p.Week,
                    DeClassTimesDesc = ComBusiness2.GetDeClassTimesDesc(p.DeType, p.DeClassTimes, p.ExceedClassTimes)
                });
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
                    StudentPhone = p.StudentPhone,
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
            var oldCheckStatus = p.StudentCheckStatus;
            var now = DateTime.Now;
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
                        TenantId = p.TenantId
                    });
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
            var deStudentClassTimesResult = await CoreBusiness.DeStudentClassTimes(_studentCourseDAL, p);
            _eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(p.TenantId)
            {
                StudentId = p.StudentId,
                CourseId = p.CourseId
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
                    DeClassTimesSmall = 0
                });
            }
            await _classRecordDAL.EditClassRecordStudent(p);

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

            if (studentCourseConsumeLogs.Count > 0)
            {
                _studentCourseConsumeLogDAL.AddStudentCourseConsumeLog(studentCourseConsumeLogs); //课消记录
            }

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
                OpContent = $"修改学员[{studentBuck.Student.Name}]点名信息,到课状态从:{EmClassStudentCheckStatus.GetClassStudentCheckStatus(oldCheckStatus)}改成{EmClassStudentCheckStatus.GetClassStudentCheckStatus(p.StudentCheckStatus)},扣减课时从:{oldDeSum.EtmsToString()}改成:{p.DeClassTimes.EtmsToString()}]"
            });

            //发通知
            _eventPublisher.Publish(new NoticeStudentCourseSurplusEvent(request.LoginTenantId)
            {
                CourseId = p.CourseId,
                StudentId = p.StudentId
            });

            await _userOperationLogDAL.AddUserLog(request, $"修改点名记录", EmUserOperationType.ClassRecordManage, now);
            return ResponseBase.Success();
        }
    }
}
