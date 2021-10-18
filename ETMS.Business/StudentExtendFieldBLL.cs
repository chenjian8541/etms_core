using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.BasicData.Output;
using ETMS.Entity.Dto.BasicData.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class StudentExtendFieldBLL : IStudentExtendFieldBLL
    {
        private readonly IStudentExtendFieldDAL _studentExtendFieldDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        public StudentExtendFieldBLL(IStudentExtendFieldDAL studentExtendFieldDAL, IUserOperationLogDAL userOperationLogDAL,
            ISysTenantDAL sysTenantDAL, IAppConfigurtaionServices appConfigurtaionServices)
        {
            this._studentExtendFieldDAL = studentExtendFieldDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._appConfigurtaionServices = appConfigurtaionServices;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentExtendFieldDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> StudentExtendFieldAdd(StudentExtendFieldAddRequest request)
        {
            await _studentExtendFieldDAL.AddStudentExtendField(new EtStudentExtendField()
            {
                UserId = request.LoginUserId,
                TenantId = request.LoginTenantId,
                Remark = string.Empty,
                DataExtend = string.Empty,
                DataType = EmStudentExtendFieldDataType.Text,
                DisplayName = request.Name,
                Ot = DateTime.Now,
                IsDeleted = EmIsDeleted.Normal
            });
            await _userOperationLogDAL.AddUserLog(request, $"添加学员自定义属性-{request.Name}", EmUserOperationType.StudentExtendFieldSetting);
            await DelExcelTemplate(request.LoginTenantId);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentExtendFieldGet(StudentExtendFieldGetRequest request)
        {
            var holidays = await _studentExtendFieldDAL.GetAllStudentExtendField();
            return ResponseBase.Success(holidays.Select(p => new StudentExtendFieldViewOutput()
            {
                CId = p.Id,
                Name = p.DisplayName
            }));
        }

        public async Task<ResponseBase> StudentExtendFieldDel(StudentExtendFieldDelRequest request)
        {
            await _studentExtendFieldDAL.DelStudentExtendField(request.CId);
            await _userOperationLogDAL.AddUserLog(request, "删除学员自定义属性", EmUserOperationType.StudentExtendFieldSetting);
            await DelExcelTemplate(request.LoginTenantId);
            return ResponseBase.Success();
        }

        private async Task DelExcelTemplate(int tenantId)
        {
            var mTenant = await _sysTenantDAL.GetTenant(tenantId);
            ExcelLib.DelHisExcelTemplate(mTenant.TenantCode, _appConfigurtaionServices.AppSettings.StaticFilesConfig.ServerPath);
        }
    }
}
