using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Educational.Output;
using ETMS.Entity.Dto.Educational.Request;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Enum;

namespace ETMS.Business
{
    public class ClassReservationBLL : IClassReservationBLL
    {
        private readonly IAppConfig2BLL _appConfig2BLL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IClassTimesDAL _classTimesDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IClassDAL _classDAL;

        private readonly ICourseDAL _courseDAL;

        public ClassReservationBLL(IAppConfig2BLL appConfig2BLL, IUserOperationLogDAL userOperationLogDAL, IClassTimesDAL classTimesDAL,
           IStudentDAL studentDAL, IClassDAL classDAL, ICourseDAL courseDAL)
        {
            this._appConfig2BLL = appConfig2BLL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._classTimesDAL = classTimesDAL;
            this._studentDAL = studentDAL;
            this._classDAL = classDAL;
            this._courseDAL = courseDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._appConfig2BLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _userOperationLogDAL, _classTimesDAL, _studentDAL, _classDAL, _courseDAL);
        }

        public async Task<ResponseBase> ClassReservationRuleGet(RequestBase request)
        {
            var rule = await _appConfig2BLL.GetClassReservationSetting();
            return ResponseBase.Success(new ClassReservationRuleGetView()
            {
                DeadlineClassReservaLimitDayTimeValue = rule.DeadlineClassReservaLimitDayTimeValue,
                DeadlineClassReservaLimitDayTimeValueDesc = EtmsHelper.GetTimeDesc(rule.DeadlineClassReservaLimitDayTimeValue),
                DeadlineClassReservaLimitType = rule.DeadlineClassReservaLimitType,
                DeadlineClassReservaLimitValue = rule.DeadlineClassReservaLimitValue,
                IsParentShowClassCount = rule.IsParentShowClassCount,
                MaxCountClassReservaLimitType = rule.MaxCountClassReservaLimitType,
                StartClassReservaLimitType = rule.StartClassReservaLimitType,
                StartClassReservaLimitValue = rule.StartClassReservaLimitValue,
                MaxCountClassReservaLimitValue = rule.MaxCountClassReservaLimitValue,
                CancelClassReservaType = rule.CancelClassReservaType,
                CancelClassReservaValue = rule.CancelClassReservaValue
            });
        }

        public async Task<ResponseBase> ClassReservationRuleSave(ClassReservationRuleSaveRequest request)
        {
            var rule = await _appConfig2BLL.GetClassReservationSetting();
            rule.DeadlineClassReservaLimitDayTimeValue = request.DeadlineClassReservaLimitDayTimeValue;
            rule.DeadlineClassReservaLimitType = request.DeadlineClassReservaLimitType;
            rule.DeadlineClassReservaLimitValue = request.DeadlineClassReservaLimitValue;
            rule.IsParentShowClassCount = request.IsParentShowClassCount;
            rule.MaxCountClassReservaLimitType = request.MaxCountClassReservaLimitType;
            rule.StartClassReservaLimitType = request.StartClassReservaLimitType;
            rule.StartClassReservaLimitValue = request.StartClassReservaLimitValue;
            rule.MaxCountClassReservaLimitValue = request.MaxCountClassReservaLimitValue;
            rule.CancelClassReservaType = request.CancelClassReservaType;
            rule.CancelClassReservaValue = request.CancelClassReservaValue;

            await _appConfig2BLL.SaveClassReservationSetting(request.LoginTenantId, rule);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassReservationLogGetPaging(ClassReservationLogGetPagingRequest request)
        {
            var pagingData = await _classTimesDAL.ReservationLogGetPaging(request);
            var output = new List<ClassReservationLogGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var tempBoxCourse = new DataTempBox<EtCourse>();
                var tempBoxClass = new DataTempBox<EtClass>();
                var tempBoxStudent = new DataTempBox<EtStudent>();
                var now = DateTime.Now.Date;
                foreach (var log in pagingData.Item1)
                {
                    var etClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, log.ClassId);
                    var student = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, log.StudentId);
                    if (student == null)
                    {
                        continue;
                    }
                    output.Add(new ClassReservationLogGetPagingOutput()
                    {
                        ClassId = log.ClassId,
                        ClassName = etClass?.Name,
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
                        StudentId = log.StudentId,
                        StudentName = student.Name,
                        StudentPhone = student.Phone,
                        TimeDesc = $"{EtmsHelper.GetTimeDesc(log.StartTime)}~{EtmsHelper.GetTimeDesc(log.EndTime)}",
                        Week = log.Week,
                        WeekDesc = $"周{EtmsHelper.GetWeekDesc(log.Week)}"
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ClassReservationLogGetPagingOutput>(pagingData.Item2, output));
        }
    }
}
