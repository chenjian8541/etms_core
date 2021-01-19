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
    public class StudentGrowingTagBLL : IStudentGrowingTagBLL
    {
        private readonly IStudentGrowingTagDAL _studentGrowingTagDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        public StudentGrowingTagBLL(IStudentGrowingTagDAL studentGrowingTagDAL, IUserOperationLogDAL userOperationLogDAL)
        {
            this._studentGrowingTagDAL = studentGrowingTagDAL;
            this._userOperationLogDAL = userOperationLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentGrowingTagDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> StudentGrowingTagAdd(StudentGrowingTagAddRequest request)
        {
            await _studentGrowingTagDAL.AddStudentGrowingTag(new EtStudentGrowingTag()
            {
                TenantId = request.LoginTenantId,
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name,
                Remark = string.Empty
            });
            await _userOperationLogDAL.AddUserLog(request, $"添加成长档案类型-{request.Name}", EmUserOperationType.StudentGrowingTagSetting);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentGrowingTagGet(StudentGrowingTagGetRequest request)
        {
            var subjects = await _studentGrowingTagDAL.GetAllStudentGrowingTag();
            return ResponseBase.Success(subjects.Select(p => new StudentGrowingTagViewOutput()
            {
                CId = p.Id,
                Name = p.Name
            }));
        }

        public async Task<ResponseBase> StudentGrowingTagDel(StudentGrowingTagDelRequest request)
        {
            await _studentGrowingTagDAL.DelStudentGrowingTag(request.CId);
            await _userOperationLogDAL.AddUserLog(request, "删除成长档案类型", EmUserOperationType.StudentGrowingTagSetting);
            return ResponseBase.Success();
        }
    }
}
