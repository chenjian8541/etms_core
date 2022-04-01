﻿using ETMS.Business.Common;
using ETMS.Business.EtmsManage.Common;
using ETMS.Entity.Alien.Dto.Tenant.Output;
using ETMS.Entity.Alien.Dto.Tenant.Request;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.IBusiness.Alien;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Alien
{
    public class AlienTenantBLL : IAlienTenantBLL
    {
        private readonly ISysTenantOperationLogDAL _sysTenantOperationLogDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IUserDAL _userDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IClassDAL _classDAL;

        public AlienTenantBLL(ISysTenantOperationLogDAL sysTenantOperationLogDAL, ISysTenantDAL sysTenantDAL,
            IStudentDAL studentDAL, IUserDAL userDAL, ICourseDAL courseDAL, IClassDAL classDAL)
        {
            this._sysTenantOperationLogDAL = sysTenantOperationLogDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._studentDAL = studentDAL;
            this._userDAL = userDAL;
            this._courseDAL = courseDAL;
            this._classDAL = classDAL;
        }

        public void InitHeadId(int headId)
        {
        }

        public void InitTenant(int tenantId)
        {
            this.InitTenantDataAccess(tenantId, _studentDAL, _userDAL, _courseDAL, _classDAL);
        }

        public async Task<ResponseBase> TenantOperationLogPaging(TenantOperationLogPagingRequest request)
        {
            var output = new List<TenantOperationLogPagingOutput>();
            var pagingData = await _sysTenantOperationLogDAL.GetPaging(request);
            if (pagingData.Item1.Any())
            {
                var tempBoxTenant = new AgentDataTempBox<SysTenant>();
                foreach (var p in pagingData.Item1)
                {
                    var tenant = await AgentComBusiness.GetTenant(tempBoxTenant, _sysTenantDAL, p.TenantId);
                    output.Add(new TenantOperationLogPagingOutput()
                    {
                        ClientType = p.ClientType,
                        IpAddress = p.IpAddress,
                        OpContent = p.OpContent,
                        Ot = p.Ot,
                        Type = p.Type,
                        ClientTypeDesc = EmUserOperationLogClientType.GetClientTypeDesc(p.ClientType),
                        TypeDesc = EnumDataLib.GetUserOperationTypeDesc.FirstOrDefault(j => j.Value == p.Type)?.Label,
                        TenantName = tenant?.Name
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<TenantOperationLogPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> StudentGetPaging(AlStudentGetPagingRequest request)
        {
            _studentDAL.InitTenantId(request.TenantId.Value);
            var pagingData = await _studentDAL.GetStudentPaging(request);
            return ResponseBase.Success(new ResponsePagingDataBase<StudentGetPagingOutput>(pagingData.Item2, pagingData.Item1.Select(student => new StudentGetPagingOutput()
            {
                CId = student.Id,
                CardNo = student.CardNo,
                Gender = student.Gender,
                IsBindingWechat = student.IsBindingWechat,
                StudentType = student.StudentType,
                HomeAddress = student.HomeAddress,
                Name = student.Name,
                Phone = student.Phone,
                SchoolName = student.SchoolName,
                Points = student.Points,
                Remark = student.Remark,
                BirthdayDesc = student.Birthday.EtmsToDateString(),
                Age = student.Age,
                AgeMonth = student.AgeMonth,
                EndClassOtDesc = student.EndClassOt == null ? string.Empty : student.EndClassOt.EtmsToDateString(),
                GenderDesc = EmGender.GetGenderDesc(student.Gender),
                StudentTypeDesc = EmStudentType.GetStudentTypeDesc(student.StudentType),
                OtDesc = student.Ot.EtmsToDateString(),
                Label = student.Name,
                Value = student.Id
            })));
        }

        public async Task<ResponseBase> ClassGetPaging(AlClassGetPagingRequest request)
        {
            this.InitTenant(request.TenantId.Value);
            var pagingData = await _classDAL.GetPaging(request);
            var output = new List<ClassGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var tempBoxCourse = new DataTempBox<EtCourse>();
                var tempBoxUser = new DataTempBox<EtUser>();
                foreach (var p in pagingData.Item1)
                {
                    //EtStudent student = null;
                    //if (p.Type == EmClassType.OneToOne)
                    //{
                    //var classBucket = await _classDAL.GetClassBucket(p.Id);
                    //if (classBucket.EtClassStudents != null && classBucket.EtClassStudents.Any())
                    //{
                    //    var studentBucket = await _studentDAL.GetStudent(classBucket.EtClassStudents.First().StudentId);
                    //    student = studentBucket?.Student;
                    //}
                    //}
                    var limitStudentNumsDesc = string.Empty;
                    if (p.Type == EmClassType.OneToMany)
                    {
                        limitStudentNumsDesc = EmLimitStudentNumsType.GetLimitStudentNumsDesc(p.StudentNums, p.LimitStudentNums, p.LimitStudentNumsType);
                    }
                    output.Add(new ClassGetPagingOutput()
                    {
                        CId = p.Id,
                        Type = p.Type,
                        CourseList = p.CourseList,
                        CompleteStatus = p.CompleteStatus,
                        FinishCount = p.FinishCount,
                        FinishClassTimes = p.FinishClassTimes,
                        DefaultClassTimes = p.DefaultClassTimes.EtmsToString(),
                        IsLeaveCharge = p.IsLeaveCharge,
                        LimitStudentNums = p.LimitStudentNums,
                        LimitStudentNumsDesc = limitStudentNumsDesc,
                        IsNotComeCharge = p.IsNotComeCharge,
                        Name = p.Name,
                        PlanCount = p.PlanCount,
                        Remark = p.Remark,
                        ScheduleStatus = p.ScheduleStatus,
                        TeacherNum = p.TeacherNum,
                        StudentNums = p.StudentNums,
                        CompleteStatusDesc = EmClassCompleteStatus.GetClassCompleteStatusDesc(p.CompleteStatus),
                        CompleteTimeDesc = p.CompleteTime.EtmsToDateString(),
                        //OneToOneStudentName = student?.Name,
                        //OneToOneStudentPhone = student?.Phone,
                        ScheduleStatusDesc = EmClassScheduleStatus.GetClassScheduleStatusDesc(p.ScheduleStatus),
                        TeachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, p.Teachers),
                        CourseDesc = await ComBusiness.GetCourseNames(tempBoxCourse, _courseDAL, p.CourseList),
                        TypeDesc = EmClassType.GetClassTypeDesc(p.Type),
                        Label = p.Name,
                        Value = p.Id,
                        LimitStudentNumsType = p.LimitStudentNumsType,
                        IsCanOnlineSelClass = p.IsCanOnlineSelClass
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ClassGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> CourseGetPaging(AlCourseGetPagingRequest request)
        {
            this.InitTenant(request.TenantId.Value);
            var pagingData = await _courseDAL.GetPaging(request);
            var courseInfo = pagingData.Item1;
            var courseGetPagingOutput = new List<CourseGetPagingOutput>();
            foreach (var p in courseInfo)
            {
                var priceRules = (await _courseDAL.GetCourse(p.Id)).Item2;
                courseGetPagingOutput.Add(new CourseGetPagingOutput()
                {
                    CId = p.Id,
                    Status = p.Status,
                    StatusDesc = EmProductStatus.GetCourseStatusDesc(p.Status),
                    Name = p.Name,
                    PriceType = p.PriceType,
                    PriceTypeDesc = EmCoursePriceType.GetCoursePriceTypeDesc2(p.PriceType, p.PriceTypeDesc),
                    Remark = p.Remark,
                    Type = p.Type,
                    TypeDesc = EmCourseType.GetCourseTypeDesc(p.Type),
                    PriceRuleDescs = ComBusiness3.GetPriceRuleDescs(priceRules),
                    Label = p.Name,
                    Value = p.Id,
                    CheckPoints = p.CheckPoints,
                    StudentCheckDeClassTimes = p.StudentCheckDeClassTimes
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<CourseGetPagingOutput>(pagingData.Item2, courseGetPagingOutput));
        }
    }
}
