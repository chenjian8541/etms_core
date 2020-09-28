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
    public class StudentTagBLL : IStudentTagBLL
    {
        private readonly IStudentTagDAL _studentTagDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        public StudentTagBLL(IStudentTagDAL studentTagDAL, IUserOperationLogDAL userOperationLogDAL)
        {
            this._studentTagDAL = studentTagDAL;
            this._userOperationLogDAL = userOperationLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentTagDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> StudentTagAdd(StudentTagAddRequest request)
        {
            await _studentTagDAL.AddStudentTag(new EtStudentTag()
            {
                TenantId = request.LoginTenantId,
                Remark = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name,
                DisplayStyle = string.Empty
            });
            await _userOperationLogDAL.AddUserLog(request, $"学员标签设置:{request.Name}", EmUserOperationType.StudentTagSetting);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentTagGet(StudentTagGetRequest request)
        {
            var studentTags = await _studentTagDAL.GetAllStudentTag();
            return ResponseBase.Success(studentTags.Select(p => new StudentTagViewOutput()
            {
                CId = p.Id,
                Name = p.Name
            }));
        }

        public async Task<ResponseBase> StudentTagDel(StudentTagDelRequest request)
        {
            await _studentTagDAL.DelStudentTag(request.CId);
            await _userOperationLogDAL.AddUserLog(request, $"删除学员标签", EmUserOperationType.StudentTagSetting);
            return ResponseBase.Success();
        }
    }
}
