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
    public class SubjectBLL : ISubjectBLL
    {
        private readonly ISubjectDAL _subjectDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        public SubjectBLL(ISubjectDAL subjectDAL, IUserOperationLogDAL userOperationLogDAL)
        {
            this._subjectDAL = subjectDAL;
            this._userOperationLogDAL = userOperationLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _subjectDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> SubjectAdd(SubjectAddRequest request)
        {
            await _subjectDAL.AddSubject(new EtSubject()
            {
                TenantId = request.LoginTenantId,
                Remark = request.Remark,
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name
            });
            await _userOperationLogDAL.AddUserLog(request, $"添加科目-{request.Name}", EmUserOperationType.SubjectSetting);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> SubjectGet(SubjectGetRequest request)
        {
            var subjects = await _subjectDAL.GetAllSubject();
            return ResponseBase.Success(subjects.Select(p => new SubjectViewOutput()
            {
                CId = p.Id,
                Name = p.Name
            }));
        }

        public async Task<ResponseBase> SubjectDel(SubjectDelRequest request)
        {
            await _subjectDAL.DelSubject(request.CId);
            await _userOperationLogDAL.AddUserLog(request, "删除科目", EmUserOperationType.HolidaySetting);
            return ResponseBase.Success();
        }
    }
}
