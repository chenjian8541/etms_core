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
    public class GradeBLL : IGradeBLL
    {
        private readonly IGradeDAL _gradeDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        public GradeBLL(IGradeDAL gradeDAL, IUserOperationLogDAL userOperationLogDAL)
        {
            this._gradeDAL = gradeDAL;
            this._userOperationLogDAL = userOperationLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _gradeDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> GradeAdd(GradeAddRequest request)
        {
            await _gradeDAL.AddGrade(new EtGrade()
            {
                TenantId = request.LoginTenantId,
                Remark = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name
            });
            await _userOperationLogDAL.AddUserLog(request, $"添加年级-{request.Name}", EmUserOperationType.GradeSetting);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> GradeGet(GradeGetRequest request)
        {
            var grades = await _gradeDAL.GetAllGrade();
            return ResponseBase.Success(grades.Select(p => new GradeViewOutput()
            {
                CId = p.Id,
                Name = p.Name
            }));
        }

        public async Task<ResponseBase> GradeDel(GradeDelRequest request)
        {
            await _gradeDAL.DelGrade(request.CId);
            await _userOperationLogDAL.AddUserLog(request, "删除年级", EmUserOperationType.GradeSetting);
            return ResponseBase.Success();
        }
    }
}
