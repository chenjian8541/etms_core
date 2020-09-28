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
    public class StudentRelationshipBLL : IStudentRelationshipBLL
    {
        private readonly IStudentRelationshipDAL _studentRelationshipDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        public StudentRelationshipBLL(IStudentRelationshipDAL studentRelationshipDAL, IUserOperationLogDAL userOperationLogDAL)
        {
            this._studentRelationshipDAL = studentRelationshipDAL;
            this._userOperationLogDAL = userOperationLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentRelationshipDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> StudentRelationshipAdd(StudentRelationshipAddRequest request)
        {
            await _studentRelationshipDAL.AddStudentRelationship(new EtStudentRelationship()
            {
                TenantId = request.LoginTenantId,
                Remark = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name
            });
            await _userOperationLogDAL.AddUserLog(request, $"家长关系设置:{request.Name}", EmUserOperationType.StudentRelationshipSetting);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentRelationshipGet(StudentRelationshipGetRequest request)
        {
            var studentRelationships = await _studentRelationshipDAL.GetAllStudentRelationship();
            return ResponseBase.Success(studentRelationships.Select(p => new StudentRelationshipViewOutput()
            {
                CId = p.Id,
                Name = p.Name
            }));
        }

        public async Task<ResponseBase> StudentRelationshipDel(StudentRelationshipDelRequest request)
        {
            await _studentRelationshipDAL.DelStudentRelationship(request.CId);
            await _userOperationLogDAL.AddUserLog(request, "删除家长关系设置", EmUserOperationType.StudentRelationshipSetting);
            return ResponseBase.Success();
        }
    }
}
