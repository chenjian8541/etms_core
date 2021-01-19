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
    public class ClassCategoryBLL : IClassCategoryBLL
    {
        private readonly IClassCategoryDAL _classCategoryDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        public ClassCategoryBLL(IClassCategoryDAL classCategoryDAL, IUserOperationLogDAL userOperationLogDAL)
        {
            this._classCategoryDAL = classCategoryDAL;
            this._userOperationLogDAL = userOperationLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _classCategoryDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> ClassCategoryAdd(ClassCategoryAddRequest request)
        {
            await _classCategoryDAL.AddClassCategory(new EtClassCategory()
            {
                TenantId = request.LoginTenantId,
                Remark = request.Remark,
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name
            });
            await _userOperationLogDAL.AddUserLog(request, $"班级分类设置-{request.Name}", EmUserOperationType.ClassCategorySetting);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassCategoryGet(ClassCategoryGetRequest request)
        {
            var classCategorys = await _classCategoryDAL.GetAllClassCategory();
            return ResponseBase.Success(classCategorys.Select(p => new ClassCategoryViewOutput()
            {
                CId = p.Id,
                Name = p.Name,
                Remark = p.Remark
            }).ToList());
        }

        public async Task<ResponseBase> ClassCategoryDel(ClassCategoryDelRequest request)
        {
            await _classCategoryDAL.DelClassCategory(request.CId);
            await _userOperationLogDAL.AddUserLog(request, "删除班级分类设置", EmUserOperationType.ClassCategorySetting);
            return ResponseBase.Success();
        }
    }
}
