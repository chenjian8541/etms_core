using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.BasicData.Output;
using ETMS.Entity.Dto.BasicData.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class ClassSetBLL : IClassSetBLL
    {
        private readonly IClassSetDAL _classSetDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        public ClassSetBLL(IClassSetDAL classSetDAL, IUserOperationLogDAL userOperationLogDAL)
        {
            this._classSetDAL = classSetDAL;
            this._userOperationLogDAL = userOperationLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _classSetDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> ClassSetAdd(ClassSetAddRequest request)
        {
            await _classSetDAL.AddClassSet(new EtClassSet()
            {
                TenantId = request.LoginTenantId,
                Remark = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                StartTime = request.StartTimeDesc.Replace(":", "").ToInt(),
                EndTime = request.EndTimeDesc.Replace(":", "").ToInt()
            });
            await _userOperationLogDAL.AddUserLog(request, $"添加上课时间段-[{request.StartTimeDesc}-{request.EndTimeDesc}]", EmUserOperationType.ClassSetSetting);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassSetGet(ClassSetGetRequest request)
        {
            var classSets = await _classSetDAL.GetAllClassSet();
            return ResponseBase.Success(
               classSets.Select(p => new ClassSetViewOutput()
               {
                   CId = p.Id,
                   StartTimeDesc = EtmsHelper.GetTimeDesc(p.StartTime),
                   EndTimeDesc = EtmsHelper.GetTimeDesc(p.EndTime)
               }));
        }

        public async Task<ResponseBase> ClassSetDel(ClassSetDelRequest request)
        {
            await _classSetDAL.DelClassSet(request.CId);
            await _userOperationLogDAL.AddUserLog(request, "删除上课时间段", EmUserOperationType.ClassSetSetting);
            return ResponseBase.Success();
        }
    }
}
