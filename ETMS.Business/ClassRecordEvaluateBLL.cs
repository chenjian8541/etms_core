using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Interaction.Output;
using ETMS.Entity.Dto.Interaction.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.Utility;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class ClassRecordEvaluateBLL : IClassRecordEvaluateBLL
    {
        private readonly IClassRecordDAL _classRecordDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IUserDAL _userDAL;

        private readonly IClassDAL _classDAL;

        private readonly IClassRecordEvaluateDAL _classRecordEvaluateDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IClassRoomDAL _classRoomDAL;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        public ClassRecordEvaluateBLL(IClassRecordDAL classRecordDAL, IStudentDAL studentDAL, IUserDAL userDAL, IClassDAL classDAL,
            IClassRecordEvaluateDAL classRecordEvaluateDAL, ICourseDAL courseDAL, IClassRoomDAL classRoomDAL,
            IHttpContextAccessor httpContextAccessor, IAppConfigurtaionServices appConfigurtaionServices, IUserOperationLogDAL userOperationLogDAL)
        {
            this._classRecordDAL = classRecordDAL;
            this._studentDAL = studentDAL;
            this._userDAL = userDAL;
            this._classDAL = classDAL;
            this._classRecordEvaluateDAL = classRecordEvaluateDAL;
            this._courseDAL = courseDAL;
            this._classRoomDAL = classRoomDAL;
            this._httpContextAccessor = httpContextAccessor;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._userOperationLogDAL = userOperationLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _classRecordDAL, _studentDAL, _userDAL, _classDAL, _classRecordEvaluateDAL,
                _courseDAL, _classRoomDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> TeacherClassRecordEvaluateGetPaging(TeacherClassRecordEvaluateGetPagingRequest request)
        {
            var pagingData = await _classRecordDAL.GetPaging(request);
            var output = new List<TeacherClassRecordEvaluateGetPagingOutput>();
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var tempBoxUser = new DataTempBox<EtUser>();
            var tempBoxClass = new DataTempBox<EtClass>();
            foreach (var classRecord in pagingData.Item1)
            {
                var courseListDesc = string.Empty;
                var courseStyleColor = string.Empty;
                var className = string.Empty;
                var teachersDesc = string.Empty;
                var etClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, classRecord.ClassId);
                var courseInfo = await ComBusiness.GetCourseNameAndColor(tempBoxCourse, _courseDAL, classRecord.CourseList);
                className = etClass.Name;
                courseListDesc = courseInfo.Item1;
                courseStyleColor = courseInfo.Item2;
                teachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, classRecord.Teachers);
                output.Add(new TeacherClassRecordEvaluateGetPagingOutput()
                {
                    ClassRecordId = classRecord.Id,
                    ClassId = classRecord.ClassId,
                    ClassName = className,
                    ClassOtDesc = classRecord.ClassOt.EtmsToDateString(),
                    ClassTimeDesc = $"{EtmsHelper.GetTimeDesc(classRecord.StartTime)}~{EtmsHelper.GetTimeDesc(classRecord.EndTime)}",
                    CourseListDesc = courseListDesc,
                    WeekDesc = $"周{EtmsHelper.GetWeekDesc(classRecord.Week)}",
                    TeachersDesc = teachersDesc,
                    TotalEvaluateCount = classRecord.EvaluateStudentCount,
                    TotalNeedEvaluateCount = classRecord.AttendNumber
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<TeacherClassRecordEvaluateGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> TeacherClassRecordEvaluateGetDetail(TeacherClassRecordEvaluateGetDetailRequest request)
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
            return ResponseBase.Success(new TeacherClassRecordEvaluateGetDetailOutput()
            {
                ClassRecordId = classRecord.Id,
                ClassId = classRecord.ClassId,
                ClassName = className,
                ClassOtDesc = classRecord.ClassOt.EtmsToDateString(),
                ClassTimeDesc = $"{EtmsHelper.GetTimeDesc(classRecord.StartTime)}~{EtmsHelper.GetTimeDesc(classRecord.EndTime)}",
                ClassRoomIdsDesc = classRoomIdsDesc,
                CourseListDesc = courseListDesc,
                WeekDesc = $"周{EtmsHelper.GetWeekDesc(classRecord.Week)}",
                TeachersDesc = teachersDesc,
                CheckOt = classRecord.CheckOt,
                TotalEvaluateCount = classRecord.EvaluateStudentCount,
                TotalNeedEvaluateCount = classRecord.AttendNumber
            });
        }

        public async Task<ResponseBase> TeacherClassRecordEvaluateStudent(TeacherClassRecordEvaluateStudentRequest request)
        {
            var classRecordStudents = await _classRecordDAL.GetClassRecordStudents(request.ClassRecordId);
            var outPut = new List<TeacherClassRecordEvaluateStudentOutput>();
            foreach (var p in classRecordStudents)
            {
                var student = await _studentDAL.GetStudent(p.StudentId);
                if (student == null || student.Student == null)
                {
                    continue;
                }
                outPut.Add(new TeacherClassRecordEvaluateStudentOutput()
                {
                    StudentCheckStatus = p.StudentCheckStatus,
                    StudentCheckStatusDesc = EmClassStudentCheckStatus.GetClassStudentCheckStatus(p.StudentCheckStatus),
                    StudentName = student.Student.Name,
                    StudentPhone = student.Student.Phone,
                    ClassRecordId = p.ClassRecordId,
                    ClassRecordStudentId = p.Id,
                    EvaluateCount = p.EvaluateCount,
                    EvaluateReadCount = p.EvaluateReadCount,
                    StudentId = p.StudentId,
                    IsCanEvaluate = EmClassStudentCheckStatus.CheckIsCanEvaluate(p.StudentCheckStatus)
                });
            }
            return ResponseBase.Success(outPut);
        }

        public async Task<ResponseBase> TeacherClassRecordEvaluateStudentDetail(TeacherClassRecordEvaluateStudentDetailRequest request)
        {

            var log = await _classRecordEvaluateDAL.GetClassRecordEvaluateStudent(request.ClassRecordId, request.StudentId);
            var output = new List<TeacherClassRecordEvaluateStudentDetailOutput>();
            if (log.Count > 0)
            {
                var tempBoxUser = new DataTempBox<EtUser>();
                foreach (var p in log)
                {
                    var userInfo = await ComBusiness.GetUser(tempBoxUser, _userDAL, p.TeacherId);
                    output.Add(new TeacherClassRecordEvaluateStudentDetailOutput()
                    {
                        EvaluateContent = p.EvaluateContent,
                        EvaluateIsRead = p.IsRead,
                        EvaluateOt = p.Ot,
                        EvaluateUserAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, userInfo.Avatar),
                        EvaluateUserName = userInfo.Name
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> TeacherClassRecordEvaluateSubmit(TeacherClassRecordEvaluateSubmitRequest request)
        {
            var classRecord = await _classRecordDAL.GetClassRecord(request.ClassRecordId);
            if (classRecord == null)
            {
                return ResponseBase.CommonError("点名记录不存在");
            }
            var now = DateTime.Now;
            await _classRecordEvaluateDAL.AddClassRecordEvaluateStudent(new EtClassRecordEvaluateStudent()
            {
                ClassRecordId = classRecord.Id,
                CheckUserId = classRecord.CheckUserId,
                IsDeleted = classRecord.IsDeleted,
                IsRead = false,
                EvaluateContent = request.EvaluateContent,
                Ot = now,
                CheckOt = classRecord.CheckOt,
                ClassId = classRecord.ClassId,
                ClassOt = classRecord.ClassOt,
                EndTime = classRecord.EndTime,
                EvaluateImg = string.Empty,
                StartTime = classRecord.StartTime,
                Status = classRecord.Status,
                StudentId = request.StudentId,
                StudentType = EmClassStudentType.ClassStudent,
                TeacherId = request.LoginUserId,
                Teachers = classRecord.Teachers,
                TenantId = classRecord.TenantId,
                Week = classRecord.Week
            });
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TeacherEvaluateLogGetPaging(TeacherEvaluateLogGetPagingRequest request)
        {
            var pagingData = await _classRecordEvaluateDAL.GetEvaluateStudentPaging(request);
            var output = new List<TeacherEvaluateLogGetPagingOutput>();
            var tempBoxUser = new DataTempBox<EtUser>();
            var tempBoxClass = new DataTempBox<EtClass>();
            var tempStudent = new DataTempBox<EtStudent>();
            foreach (var evaluateStudent in pagingData.Item1)
            {
                var etClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, evaluateStudent.ClassId);
                var className = etClass.Name;
                var teachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, evaluateStudent.Teachers);
                var evaluateUserName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, evaluateStudent.TeacherId);
                var student = await ComBusiness.GetStudent(tempStudent, _studentDAL, evaluateStudent.StudentId);
                if (student == null)
                {
                    continue;
                }
                output.Add(new TeacherEvaluateLogGetPagingOutput()
                {
                    ClassName = className,
                    ClassOtDesc = evaluateStudent.ClassOt.EtmsToDateString(),
                    ClassTimeDesc = $"{EtmsHelper.GetTimeDesc(evaluateStudent.StartTime)}~{EtmsHelper.GetTimeDesc(evaluateStudent.EndTime)}",
                    WeekDesc = $"周{EtmsHelper.GetWeekDesc(evaluateStudent.Week)}",
                    TeachersDesc = teachersDesc,
                    EvaluateContent = evaluateStudent.EvaluateContent,
                    EvaluateOt = evaluateStudent.Ot,
                    EvaluateStudentId = evaluateStudent.StudentId,
                    EvaluateUserName = evaluateUserName,
                    IsRead = evaluateStudent.IsRead,
                    StudentName = student.Name,
                    StudentPhone = student.Phone
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<TeacherEvaluateLogGetPagingOutput>(pagingData.Item2, output));
        }
    }
}
