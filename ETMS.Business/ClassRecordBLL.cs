using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Educational.Output;
using ETMS.Entity.Dto.Educational.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.Utility;
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

        public ClassRecordBLL(IClassRecordDAL classRecordDAL, IClassRoomDAL classRoomDAL, ICourseDAL courseDAL, IClassDAL classDAL,
            IUserDAL userDAL, IStudentDAL studentDAL, IUserOperationLogDAL userOperationLogDAL, IStudentPointsLogDAL studentPointsLogDAL)
        {
            this._classRecordDAL = classRecordDAL;
            this._classRoomDAL = classRoomDAL;
            this._courseDAL = courseDAL;
            this._classDAL = classDAL;
            this._userDAL = userDAL;
            this._studentDAL = studentDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._studentPointsLogDAL = studentPointsLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _classRecordDAL, this._classRoomDAL, this._courseDAL, _studentDAL, _classDAL, _userDAL, _userOperationLogDAL,
                _studentPointsLogDAL);
        }

        public async Task<ResponseBase> ClassRecordGetPaging(ClassRecordGetPagingRequest request)
        {
            var pagingData = await _classRecordDAL.GetPaging(request);
            var output = new List<ClassRecordGetPagingOutput>();
            var allClassRoom = await _classRoomDAL.GetAllClassRoom();

            var tempBoxCourse = new DataTempBox<EtCourse>();
            var tempBoxUser = new DataTempBox<EtUser>();
            foreach (var classRecord in pagingData.Item1)
            {
                var classRoomIdsDesc = string.Empty;
                var courseListDesc = string.Empty;
                var courseStyleColor = string.Empty;
                var className = string.Empty;
                var teachersDesc = string.Empty;
                var etClass = await _classDAL.GetClassBucket(classRecord.ClassId);
                var courseInfo = await ComBusiness.GetCourseNameAndColor(tempBoxCourse, _courseDAL, classRecord.CourseList);
                classRoomIdsDesc = ComBusiness.GetDesc(allClassRoom, classRecord.ClassRoomIds);
                className = etClass.EtClass.Name;
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
                    DeClassTimes = p.DeClassTimes,
                    DeSum = p.DeSum,
                    DeType = p.DeType,
                    DeTypeDesc = EmDeClassTimesType.GetDeClassTimesTypeDesc(p.DeType),
                    ExceedClassTimes = p.ExceedClassTimes,
                    IsRewardPoints = p.IsRewardPoints,
                    Remark = p.Remark,
                    RewardPoints = p.RewardPoints,
                    StudentCheckStatus = p.StudentCheckStatus,
                    StudentCheckStatusDesc = EmClassStudentCheckStatus.GetClassStudentCheckStatus(p.StudentCheckStatus),
                    StudentName = student.Student.Name,
                    StudentPhone = student.Student.Phone,
                    StudentType = p.StudentType,
                    StudentTypeDesc = EmClassStudentType.GetClassStudentTypeDesc(p.StudentType),
                    DeClassTimesDesc = GetDeClassTimesDesc(p.DeType, p.DeClassTimes, p.ExceedClassTimes),
                    Status = p.Status,
                    StatusDesc = EmClassRecordStatus.GetClassRecordStatusDesc(p.Status),
                });
            }
            return ResponseBase.Success(outPut);
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
                    DeClassTimes = p.DeClassTimes,
                    DeType = p.DeType,
                    DeTypeDesc = EmDeClassTimesType.GetDeClassTimesTypeDesc(p.DeType),
                    ExceedClassTimes = p.ExceedClassTimes,
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
                    DeClassTimesDesc = GetDeClassTimesDesc(p.DeType, p.DeClassTimes, p.ExceedClassTimes),
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
                    DeClassTimes = p.DeClassTimes,
                    DeSum = p.DeSum,
                    DeType = p.DeType,
                    EndTime = p.EndTime,
                    EvaluateTeacherNum = p.EvaluateTeacherNum,
                    ExceedClassTimes = p.ExceedClassTimes,
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
                    DeClassTimesDesc = GetDeClassTimesDesc(p.DeType, p.DeClassTimes, p.ExceedClassTimes)
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentClassRecordGetPagingOutput>(pagingData.Item2, output));
        }

        private string GetDeClassTimesDesc(byte deType, int deClassTimes, int exceedClassTimes)
        {
            if (deType == EmDeClassTimesType.NotDe)
            {
                if (exceedClassTimes > 0)
                {
                    return $"记录超上{exceedClassTimes}课时";
                }
                else
                {
                    return "未扣";
                }
            }
            if (deType == EmDeClassTimesType.Day)
            {
                return "按天自动消耗";
            }
            var desc = new StringBuilder($"{deClassTimes}课时");
            if (exceedClassTimes > 0)
            {
                desc.Append($" (记录超上{exceedClassTimes}课时)");
            }
            return desc.ToString();
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
    }
}
