using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.BasicData.Output;
using ETMS.Entity.Dto.BasicData.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class IncomeProjectTypeBLL : IIncomeProjectTypeBLL
    {
        private readonly IIncomeProjectTypeDAL _incomeProjectTypeDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        public IncomeProjectTypeBLL(IIncomeProjectTypeDAL incomeProjectTypeDAL, IUserOperationLogDAL userOperationLogDAL)
        {
            this._incomeProjectTypeDAL = incomeProjectTypeDAL;
            this._userOperationLogDAL = userOperationLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _incomeProjectTypeDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> IncomeProjectTypeAdd(IncomeProjectTypeAddRequest request)
        {
            await _incomeProjectTypeDAL.AddIncomeProjectType(new EtIncomeProjectType()
            {
                TenantId = request.LoginTenantId,
                Remark = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name
            });
            await _userOperationLogDAL.AddUserLog(request, $"收支项目设置:{request.Name}", EmUserOperationType.IncomeProjectTypeSetting);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> IncomeProjectTypeDel(IncomeProjectTypeDelRequest request)
        {
            await _incomeProjectTypeDAL.DelIncomeProjectType(request.CId);
            await _userOperationLogDAL.AddUserLog(request, $"删除收支项目", EmUserOperationType.IncomeProjectTypeSetting);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> IncomeProjectTypeGet(IncomeProjectTypeGetRequest request)
        {
            var incomeProjectTypes = await _incomeProjectTypeDAL.GetAllIncomeProjectType();
            return ResponseBase.Success(incomeProjectTypes.Select(p => new IncomeProjectTypeViewOutput()
            {
                CId = p.Id,
                Name = p.Name,
                Label = p.Name,
                Value = p.Id
            }));
        }
    }
}
