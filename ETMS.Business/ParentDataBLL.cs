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

        public ParentDataBLL(IStudentLeaveApplyLogDAL studentLeaveApplyLogDAL, IParentStudentDAL parentStudentDAL, IStudentDAL studentDAL,
            IStudentOperationLogDAL studentOperationLogDAL, IClassTimesDAL classTimesDAL, IClassRoomDAL classRoomDAL, IUserDAL userDAL,
            ICourseDAL courseDAL, IClassDAL classDAL, ITenantConfigDAL tenantConfigDAL, IHttpContextAccessor httpContextAccessor, IAppConfigurtaionServices appConfigurtaionServices,
            IGiftCategoryDAL giftCategoryDAL, IGiftDAL giftDAL)
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
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentLeaveApplyLogDAL, _parentStudentDAL, _studentDAL,
                _studentOperationLogDAL, _classTimesDAL, _classRoomDAL, _userDAL, _courseDAL, _classDAL,
                _tenantConfigDAL, _giftCategoryDAL, _giftDAL);
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
                    Id = p.Id
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

        public async Task<ResponseBase> StudentLeaveApplyDetailGet(StudentLeaveApplyDetailGetRequest request)
        {
            var p = await _studentLeaveApplyLogDAL.GetStudentLeaveApplyLog(request.Id);
            var student = await _studentDAL.GetStudent(p.StudentId);
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
                HandleOt = p.HandleOt.EtmsToString()
            });
        }

        public async Task<ResponseBase> StudentLeaveApplyRevoke(StudentLeaveApplyRevokeRequest request)
        {
            var p = await _studentLeaveApplyLogDAL.GetStudentLeaveApplyLog(request.Id);
            if (p.HandleStatus != EmStudentLeaveApplyHandleStatus.Unreviewed)
            {
                return ResponseBase.CommonError("无法撤销");
            }
            p.HandleStatus = EmStudentLeaveApplyHandleStatus.IsRevoke;
            await _studentLeaveApplyLogDAL.EditStudentLeaveApplyLog(p);
            await _studentOperationLogDAL.AddStudentLog(p.StudentId, request.LoginTenantId, $"撤销请假申请", EmStudentOperationLogType.StudentLeaveApply);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentClassTimetableGet(StudentClassTimetableRequest request)
        {
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
                teachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, classTimes.Teachers);
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
    }
}
