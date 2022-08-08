using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Interaction.Output;
using ETMS.Entity.Dto.Interaction.Request;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IEventProvider;
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

        private readonly IEventPublisher _eventPublisher;

        private readonly IActiveGrowthRecordDAL _activeGrowthRecordDAL;

        public ClassRecordEvaluateBLL(IClassRecordDAL classRecordDAL, IStudentDAL studentDAL, IUserDAL userDAL, IClassDAL classDAL,
            IClassRecordEvaluateDAL classRecordEvaluateDAL, ICourseDAL courseDAL, IClassRoomDAL classRoomDAL,
            IHttpContextAccessor httpContextAccessor, IAppConfigurtaionServices appConfigurtaionServices, IUserOperationLogDAL userOperationLogDAL,
            IEventPublisher eventPublisher, IActiveGrowthRecordDAL activeGrowthRecordDAL)
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
            this._eventPublisher = eventPublisher;
            this._activeGrowthRecordDAL = activeGrowthRecordDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _classRecordDAL, _studentDAL, _userDAL, _classDAL, _classRecordEvaluateDAL,
                _courseDAL, _classRoomDAL, _userOperationLogDAL, _activeGrowthRecordDAL);
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
                    StudentPhone = ComBusiness3.PhoneSecrecy(student.Student.Phone, request.SecrecyType, request.SecrecyDataBag),
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
            var log = await _classRecordEvaluateDAL.GetClassRecordEvaluateStudent(request.ClassRecordStudentId);
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
                        EvaluateUserName = userInfo.Name,
                        EvaluateMedias = ComBusiness3.GetMediasUrl(p.EvaluateImg),
                        Id = p.Id
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> TeacherClassRecordEvaluateSubmit(TeacherClassRecordEvaluateSubmitRequest request)
        {
            var classRecordStudentLog = await _classRecordDAL.GetEtClassRecordStudentById(request.ClassRecordStudentId);
            if (classRecordStudentLog == null)
            {
                return ResponseBase.CommonError("点名记录不存在");
            }
            var now = DateTime.Now;
            var evaluateMedias = string.Empty;
            if (request.EvaluateMediasKeys != null && request.EvaluateMediasKeys.Count > 0)
            {
                evaluateMedias = string.Join('|', request.EvaluateMediasKeys);
            }
            var newEntity = new EtClassRecordEvaluateStudent()
            {
                ClassRecordId = classRecordStudentLog.Id,
                CheckUserId = classRecordStudentLog.CheckUserId,
                IsDeleted = classRecordStudentLog.IsDeleted,
                IsRead = false,
                EvaluateContent = request.EvaluateContent,
                Ot = now,
                CheckOt = classRecordStudentLog.CheckOt,
                ClassId = classRecordStudentLog.ClassId,
                ClassOt = classRecordStudentLog.ClassOt,
                EndTime = classRecordStudentLog.EndTime,
                EvaluateImg = evaluateMedias,
                StartTime = classRecordStudentLog.StartTime,
                Status = classRecordStudentLog.Status,
                StudentId = classRecordStudentLog.StudentId,
                StudentType = EmClassStudentType.ClassStudent,
                TeacherId = request.LoginUserId,
                Teachers = classRecordStudentLog.Teachers,
                TenantId = classRecordStudentLog.TenantId,
                Week = classRecordStudentLog.Week,
                ClassRecordStudentId = request.ClassRecordStudentId,
                CourseId = classRecordStudentLog.CourseId
            };
            await _classRecordEvaluateDAL.AddClassRecordEvaluateStudent(newEntity);

            if (classRecordStudentLog.EvaluateCount == 0) //之前未评价过
            {
                await _classRecordDAL.ClassRecordAddEvaluateStudentCount(classRecordStudentLog.ClassRecordId, 1);
            }
            classRecordStudentLog.EvaluateCount += 1;
            classRecordStudentLog.IsTeacherEvaluate = EmBool.True;
            await _classRecordDAL.EditClassRecordStudent(classRecordStudentLog);

            _eventPublisher.Publish(new NoticeStudentsOfStudentEvaluateEvent(request.LoginTenantId)
            {
                ClassRecordStudentId = request.ClassRecordStudentId,
                EvaluateLogId = newEntity.Id
            });

            //课后点评在老师端成长档案中显示
            await _activeGrowthRecordDAL.AddActiveGrowthRecordDetail(new EtActiveGrowthRecordDetail()
            {
                CreateUserId = request.LoginUserId,
                FavoriteStatus = EmActiveGrowthRecordDetailFavoriteStatus.No,
                GrowingTag = EmActiveGrowthRecordGrowingTagDefault.ClassRecordEvaluateStudent,
                GrowthContent = newEntity.EvaluateContent,
                GrowthMedias = newEntity.EvaluateImg,
                GrowthRecordId = 0,
                RelatedId = newEntity.Id,
                IsDeleted = EmIsDeleted.Normal,
                Ot = now,
                ReadStatus = 0,
                SceneType = EmActiveGrowthRecordDetailSceneType.EvaluateStudent,
                SendType = EmActiveGrowthRecordSendType.Yes,
                StudentId = newEntity.StudentId,
                TenantId = request.LoginTenantId
            });

            await _userOperationLogDAL.AddUserLog(request, $"点评学员-{request.EvaluateContent}", EmUserOperationType.ClassEvaluate, now);
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
                var evaluateUser = await ComBusiness.GetUser(tempBoxUser, _userDAL, evaluateStudent.TeacherId);
                var evaluateUserName = string.Empty;
                var evaluateUserAvatar = string.Empty;
                if (evaluateUser != null)
                {
                    evaluateUserName = evaluateUser.Name;
                    evaluateUserAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, evaluateUser.Avatar);
                }
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
                    EvaluateStudentRecordId = evaluateStudent.Id,
                    EvaluateUserName = evaluateUserName,
                    IsRead = evaluateStudent.IsRead,
                    StudentName = student.Name,
                    StudentPhone = ComBusiness3.PhoneSecrecy(student.Phone, request.SecrecyType, request.SecrecyDataBag),
                    EvaluateMedias = ComBusiness3.GetMediasUrl(evaluateStudent.EvaluateImg),
                    EvaluateUserAvatar = evaluateUserAvatar
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<TeacherEvaluateLogGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> StudentEvaluateLogGetPaging(StudentEvaluateLogGetPagingRequest request)
        {
            var pagingData = await _classRecordEvaluateDAL.GetEvaluateTeacherPaging(request);
            var output = new List<StudentEvaluateLogGetPagingOutput>();
            var tempBoxUser = new DataTempBox<EtUser>();
            var tempBoxClass = new DataTempBox<EtClass>();
            var tempStudent = new DataTempBox<EtStudent>();
            foreach (var evaluateTeacher in pagingData.Item1)
            {
                var etClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, evaluateTeacher.ClassId);
                var className = etClass.Name;
                var teacherName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, evaluateTeacher.TeacherId);
                var student = await ComBusiness.GetStudent(tempStudent, _studentDAL, evaluateTeacher.StudentId);
                if (student == null)
                {
                    continue;
                }
                output.Add(new StudentEvaluateLogGetPagingOutput()
                {
                    ClassName = className,
                    ClassOtDesc = evaluateTeacher.ClassOt.EtmsToDateString(),
                    ClassTimeDesc = $"{EtmsHelper.GetTimeDesc(evaluateTeacher.StartTime)}~{EtmsHelper.GetTimeDesc(evaluateTeacher.EndTime)}",
                    WeekDesc = $"周{EtmsHelper.GetWeekDesc(evaluateTeacher.Week)}",
                    StudentName = student.Name,
                    StudentPhone = ComBusiness3.PhoneSecrecy(student.Phone, request.SecrecyType, request.SecrecyDataBag),
                    EvaluateTeacherRecordId = evaluateTeacher.Id,
                    Ot = evaluateTeacher.Ot,
                    StarValue = evaluateTeacher.StarValue,
                    TeacherName = teacherName,
                    EvaluateContent = evaluateTeacher.EvaluateContent
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentEvaluateLogGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> TeacherClassRecordEvaluateDel(TeacherClassRecordEvaluateDelRequest request)
        {
            var log = await _classRecordEvaluateDAL.ClassRecordEvaluateStudentGet(request.Id);
            if (log == null)
            {
                return ResponseBase.CommonError("点评记录不存在");
            }

            await _classRecordEvaluateDAL.ClassRecordEvaluateStudentDel(request.Id);
            await _classRecordDAL.ClassRecordStudentDeEvaluateCount(log.ClassRecordStudentId, 1);
            await _activeGrowthRecordDAL.DelActiveGrowthRecordDetailAboutRelatedInfo(EmActiveGrowthRecordDetailSceneType.EvaluateStudent
                , log.Id, log.StudentId);

            AliyunOssUtil.DeleteObject2(log.EvaluateImg);
            await _userOperationLogDAL.AddUserLog(request, "删除课后点评", EmUserOperationType.ClassEvaluate);
            return ResponseBase.Success();
        }
    }
}
