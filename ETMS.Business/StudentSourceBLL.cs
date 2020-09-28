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
    public class StudentSourceBLL : IStudentSourceBLL
    {
        private readonly IStudentSourceDAL _studentSourceDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        public StudentSourceBLL(IStudentSourceDAL studentSourceDAL, IUserOperationLogDAL userOperationLogDAL)
        {
            this._studentSourceDAL = studentSourceDAL;
            this._userOperationLogDAL = userOperationLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentSourceDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> StudentSourceAdd(StudentSourceAddRequest request)
        {
            await _studentSourceDAL.AddStudentSource(new EtStudentSource()
            {
                TenantId = request.LoginTenantId,
                Remark = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name
            });
            await _userOperationLogDAL.AddUserLog(request, $"学员来源设置:{request.Name}", EmUserOperationType.StudentSourceSetting);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentSourceGet(StudentSourceGetRequest request)
        {
            var studentSources = await _studentSourceDAL.GetAllStudentSource();
            return ResponseBase.Success(studentSources.Select(p => new StudentSourceViewOutput()
            {
                CId = p.Id,
                Name = p.Name
            }));
        }

        public async Task<ResponseBase> StudentSourceDel(StudentSourceDelRequest request)
        {
            await _studentSourceDAL.DelStudentSource(request.CId);
            await _userOperationLogDAL.AddUserLog(request, $"删除学员来源", EmUserOperationType.StudentSourceSetting);
            return ResponseBase.Success();
        }
    }
}
