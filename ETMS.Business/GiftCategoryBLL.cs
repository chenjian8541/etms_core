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
    public class GiftCategoryBLL : IGiftCategoryBLL
    {
        private readonly IGiftCategoryDAL _giftCategoryDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        public GiftCategoryBLL(IGiftCategoryDAL giftCategoryDAL, IUserOperationLogDAL userOperationLogDAL)
        {
            this._giftCategoryDAL = giftCategoryDAL;
            this._userOperationLogDAL = userOperationLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _giftCategoryDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> GiftCategoryAdd(GiftCategoryAddRequest request)
        {
            await _giftCategoryDAL.AddGiftCategory(new EtGiftCategory()
            {
                TenantId = request.LoginTenantId,
                Remark = request.Remark,
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name
            });
            await _userOperationLogDAL.AddUserLog(request, $"礼品分类设置:{request.Name}", EmUserOperationType.GiftCategorySetting);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> GiftCategoryGet(GiftCategoryGetRequest request)
        {
            var giftCategorys = await _giftCategoryDAL.GetAllGiftCategory();
            return ResponseBase.Success(giftCategorys.Select(p => new GiftCategoryViewOutput()
            {
                CId = p.Id,
                Label = p.Name,
                Name = p.Name,
                Remark = p.Remark,
                Value = p.Id
            }));
        }

        public async Task<ResponseBase> GiftCategoryDel(GiftCategoryDelRequest request)
        {
            await _giftCategoryDAL.DelGiftCategory(request.CId);
            await _userOperationLogDAL.AddUserLog(request, "删除礼品分类设置", EmUserOperationType.GiftCategorySetting);
            return ResponseBase.Success();
        }
    }
}
