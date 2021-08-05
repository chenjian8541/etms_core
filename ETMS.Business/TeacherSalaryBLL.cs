using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Teacher.Output;
using ETMS.Entity.Dto.Teacher.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.TeacherSalary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class TeacherSalaryBLL : ITeacherSalaryBLL
    {
        private readonly IAppConfig2BLL _appConfig2BLL;

        private readonly ITeacherSalaryFundsItemsDAL _teacherSalaryFundsItemsDAL;

        private readonly ITeacherSalaryClassDAL _teacherSalaryClassDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IUserDAL _userDAL;

        private readonly IClassDAL _classDAL;

        public TeacherSalaryBLL(IAppConfig2BLL appConfig2BLL, ITeacherSalaryFundsItemsDAL teacherSalaryFundsItemsDAL, ITeacherSalaryClassDAL teacherSalaryClassDAL,
            IUserOperationLogDAL userOperationLogDAL, IUserDAL userDAL, IClassDAL classDAL)
        {
            this._appConfig2BLL = appConfig2BLL;
            this._teacherSalaryFundsItemsDAL = teacherSalaryFundsItemsDAL;
            this._teacherSalaryClassDAL = teacherSalaryClassDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._userDAL = userDAL;
            this._classDAL = classDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._appConfig2BLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _teacherSalaryFundsItemsDAL, _teacherSalaryClassDAL,
                _userOperationLogDAL, _userDAL, _classDAL);
        }

        public async Task<ResponseBase> TeacherSalaryFundsItemsGet(TeacherSalaryFundsItemsGetRequest request)
        {
            var output = new TeacherSalaryFundsItemsGetOutput()
            {
                DefaultItems = new List<TeacherSalaryFundsItemOutput>(),
                CustomItems = new List<TeacherSalaryFundsItemOutput>()
            };
            var fundsItemsDefault = await _appConfig2BLL.GetTeacherSalaryDefaultFundsItems();
            var performanceDefaultItem = fundsItemsDefault.First(p => p.Id == SystemConfig.ComConfig.TeacherSalaryPerformanceDefaultId);
            output.IsOpenClassPerformance = performanceDefaultItem.Status == EmBool.True;
            var orderIndex = 1;
            foreach (var p in fundsItemsDefault)
            {
                if (!request.IsGetDisable && p.Status == EmBool.False) //不展示禁用的项目
                {
                    continue;
                }
                output.DefaultItems.Add(new TeacherSalaryFundsItemOutput()
                {
                    OrderIndex = orderIndex,
                    Name = p.Name,
                    Id = p.Id,
                    Status = p.Status,
                    Type = p.Type,
                    TypeDesc = EmTeacherSalaryFundsItemsType.GetTeacherSalaryFundsItemsTypeDesc(p.Type)
                });
                orderIndex++;
            }

            var fundsItemsCustom = await _teacherSalaryFundsItemsDAL.GetTeacherSalaryFundsItems();
            foreach (var p in fundsItemsCustom)
            {
                output.CustomItems.Add(new TeacherSalaryFundsItemOutput()
                {
                    OrderIndex = orderIndex,
                    Name = p.Name,
                    Id = p.Id,
                    Status = EmBool.True,
                    Type = p.Type,
                    TypeDesc = EmTeacherSalaryFundsItemsType.GetTeacherSalaryFundsItemsTypeDesc(p.Type)
                });
                orderIndex++;
            }

            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> TeacherSalaryFundsItemsAdd(TeacherSalaryFundsItemsAddRequest request)
        {
            await _teacherSalaryFundsItemsDAL.AddTeacherSalaryFundsItems(new EtTeacherSalaryFundsItems()
            {
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name,
                OrderIndex = 0,
                TenantId = request.LoginTenantId,
                Type = request.Type
            });

            await _userOperationLogDAL.AddUserLog(request, $"添加工资条项目-{request.Name}", EmUserOperationType.TeacherSalary);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TeacherSalaryFundsItemsDel(TeacherSalaryFundsItemsDelRequest request)
        {
            await _teacherSalaryFundsItemsDAL.DelTeacherSalaryFundsItems(request.Id);

            await _userOperationLogDAL.AddUserLog(request, $"删除工资条项目-{request.Name}", EmUserOperationType.TeacherSalary);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TeacherSalaryFundsItemsChangeStatus(TeacherSalaryFundsItemsChangeStatusRequest request)
        {
            var fundsItemsDefault = await _appConfig2BLL.GetTeacherSalaryDefaultFundsItems();
            var thisItem = fundsItemsDefault.FirstOrDefault(p => p.Id == request.Id);
            if (thisItem == null)
            {
                return ResponseBase.CommonError("工资条项目不存在");
            }
            thisItem.Status = thisItem.Status == EmBool.True ? EmBool.False : EmBool.True;
            await _appConfig2BLL.SaveTeacherSalaryDefaultFundsItems(request.LoginTenantId, fundsItemsDefault);

            var desc = thisItem.Status == EmBool.True ? "启用" : "禁用";
            await _userOperationLogDAL.AddUserLog(request, $"{desc}工资条项目-{thisItem.Name}", EmUserOperationType.TeacherSalary);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TeacherSalaryClassDayGetPaging(TeacherSalaryClassDayGetPagingRequest request)
        {
            var pagingData = await _teacherSalaryClassDAL.GetTeacherSalaryClassDayPaging(request);
            var output = new List<TeacherSalaryClassDayGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var tempBoxUser = new DataTempBox<EtUser>();
                var tempBoxClass = new DataTempBox<EtClass>();
                foreach (var p in pagingData.Item1)
                {
                    var myUser = await ComBusiness.GetUser(tempBoxUser, _userDAL, p.TeacherId);
                    if (myUser == null)
                    {
                        continue;
                    }
                    var myClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, p.ClassId);
                    output.Add(new TeacherSalaryClassDayGetPagingOutput()
                    {
                        ClassId = p.ClassId,
                        ArrivedAndBeLateCount = p.ArrivedAndBeLateCount,
                        ArrivedCount = p.ArrivedCount,
                        BeLateCount = p.BeLateCount,
                        ClassName = myClass?.Name,
                        DeSum = p.DeSum,
                        Id = p.Id,
                        LeaveCount = p.LeaveCount,
                        MakeUpStudentCount = p.MakeUpStudentCount,
                        NotArrivedCount = p.NotArrivedCount,
                        Ot = p.Ot,
                        StudentClassTimes = p.StudentClassTimes,
                        TeacherClassTimes = p.TeacherClassTimes,
                        TeacherId = p.TeacherId,
                        TeacherName = myUser.Name,
                        TeacherPhone = myUser.Phone,
                        TryCalssStudentCount = p.TryCalssStudentCount
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<TeacherSalaryClassDayGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> TeacherSalaryGlobalRuleGet(TeacherSalaryGlobalRuleGetRequest request)
        {
            var log = await _appConfig2BLL.GetTeacherSalaryGlobalRule();
            return ResponseBase.Success(new TeacherSalaryGlobaleRuleGetOutput()
            {
                GradientCalculateType = log.GradientCalculateType,
                StatisticalRuleType = log.StatisticalRuleType,
                IncludeArrivedMakeUpStudent = log.IncludeArrivedMakeUpStudent,
                IncludeArrivedTryCalssStudent = log.IncludeArrivedTryCalssStudent
            });
        }

        public async Task<ResponseBase> TeacherSalaryPerformanceRuleSave(TeacherSalaryPerformanceRuleSaveRequest request)
        {
            var log = await _appConfig2BLL.GetTeacherSalaryGlobalRule();
            log.GradientCalculateType = request.GradientCalculateType;
            log.StatisticalRuleType = request.StatisticalRuleType;
            await _appConfig2BLL.SaveTeacherSalaryGlobalRule(request.LoginTenantId, log);

            await _userOperationLogDAL.AddUserLog(request, "设置绩效工资统计规则", EmUserOperationType.TeacherSalary);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TeacherSalaryIncludeArrivedRuleSave(TeacherSalaryIncludeArrivedRuleSaveRequest request)
        {
            var log = await _appConfig2BLL.GetTeacherSalaryGlobalRule();
            log.IncludeArrivedMakeUpStudent = request.IncludeArrivedMakeUpStudent;
            log.IncludeArrivedTryCalssStudent = request.IncludeArrivedTryCalssStudent;
            await _appConfig2BLL.SaveTeacherSalaryGlobalRule(request.LoginTenantId, log);

            await _userOperationLogDAL.AddUserLog(request, "到课人次计算规则", EmUserOperationType.TeacherSalary);
            return ResponseBase.Success();
        }
    }
}
