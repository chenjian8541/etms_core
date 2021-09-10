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
    public class TryCalssLogBLL : ITryCalssLogBLL
    {
        private readonly ITryCalssLogDAL _tryCalssLogDAL;

        private readonly IUserDAL _userDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IClassDAL _classDAL;

        public TryCalssLogBLL(ITryCalssLogDAL tryCalssLogDAL, IUserDAL userDAL, IStudentDAL studentDAL, ICourseDAL courseDAL, IClassDAL classDAL)
        {
            this._tryCalssLogDAL = tryCalssLogDAL;
            this._userDAL = userDAL;
            this._studentDAL = studentDAL;
            this._courseDAL = courseDAL;
            this._classDAL = classDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _tryCalssLogDAL, _userDAL, _studentDAL, _courseDAL, _classDAL);
        }

        public async Task<ResponseBase> TryCalssLogGetPaging(TryCalssLogGetPagingRequest request)
        {
            var pagingData = await _tryCalssLogDAL.GetPaging(request);
            var output = new List<TryCalssLogGetPagingOutput>();
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var tempBoxUser = new DataTempBox<EtUser>();
            var tempBoxStudent = new DataTempBox<EtStudent>();
            var tempBoxClass = new DataTempBox<EtClass>();
            foreach (var p in pagingData.Item1)
            {
                var student = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId);
                if (student == null)
                {
                    continue;
                }
                var courseName = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, p.CourseId);
                var teachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, p.Teachers);
                var myClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, p.ClassId);
                var status = p.Status;
                var statusDesc = string.Empty;
                if (p.Status == EmTryCalssLogStatus.IsBooked)
                {
                    if (p.ClassOt == null)
                    {
                        status = EmTryCalssLogStatus.IsCancel;
                    }
                    else if (p.ClassOt < DateTime.Now.Date)
                    {
                        status = EmTryCalssLogStatus.IsExpired;
                    }
                }
                statusDesc = EmTryCalssLogStatus.GetTryCalssLogStatus(status);
                var classOtDesc = string.Empty;
                if (p.ClassOt != null && p.StartTime != null)
                {
                    classOtDesc = $"{p.ClassOt.EtmsToDateString()}(周{EtmsHelper.GetWeekDesc(p.Week.Value)}) {EtmsHelper.GetTimeDesc(p.StartTime.Value)}~{EtmsHelper.GetTimeDesc(p.EndTime.Value)}";
                }
                output.Add(new TryCalssLogGetPagingOutput()
                {
                    ClassOtDesc = classOtDesc,
                    ClassContent = p.ClassContent,
                    ClassDesc = myClass?.Name,
                    ClassId = p.ClassId,
                    ClassTimesId = p.ClassTimesId,
                    CourseDesc = courseName,
                    CourseId = p.CourseId,
                    EndTime = p.EndTime,
                    Id = p.Id,
                    LearningManagerName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, student.LearningManager),
                    TrackUserName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, student.TrackUser),
                    StartTime = p.StartTime,
                    StudentId = p.StudentId,
                    Teachers = p.Teachers,
                    TeachersDesc = teachersDesc,
                    Status = status,
                    UserId = p.UserId,
                    Week = p.Week,
                    StatusDesc = statusDesc,
                    StudentName = student.Name,
                    StudentPhone = ComBusiness3.PhoneSecrecy(student.Phone, request.SecrecyType)
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<TryCalssLogGetPagingOutput>(pagingData.Item2, output));
        }
    }
}
