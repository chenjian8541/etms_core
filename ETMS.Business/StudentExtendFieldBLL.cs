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
    public class StudentExtendFieldBLL : IStudentExtendFieldBLL
    {
        private readonly IStudentExtendFieldDAL _studentExtendFieldDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        public StudentExtendFieldBLL(IStudentExtendFieldDAL studentExtendFieldDAL, IUserOperationLogDAL userOperationLogDAL)
        {
            this._studentExtendFieldDAL = studentExtendFieldDAL;
            this._userOperationLogDAL = userOperationLogDAL;
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
            await _userOperationLogDAL.AddUserLog(request, $"设置学员自定义属性:{request.Name}", EmUserOperationType.StudentExtendFieldSetting);
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
            return ResponseBase.Success();
        }
    }
}
