using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Product.Output;
using ETMS.Entity.Dto.Product.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Utility;

namespace ETMS.Business
{
    public class CostBLL : ICostBLL
    {

        private readonly ICostDAL _costDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly ISuitDAL _suitDAL;

        private readonly IOrderDAL _orderDAL;

        public CostBLL(ICostDAL costDAL, IUserOperationLogDAL userOperationLogDAL, ISuitDAL suitDAL,
            IOrderDAL orderDAL)
        {
            this._costDAL = costDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._suitDAL = suitDAL;
            this._orderDAL = orderDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _costDAL, _userOperationLogDAL, _suitDAL, _orderDAL);
        }

        public async Task<ResponseBase> CostAdd(CostAddRequest request)
        {
            if (await _costDAL.ExistCost(request.Name))
            {
                return ResponseBase.CommonError("已存在相同名称的费用");
            }
            await _costDAL.AddCost(new EtCost()
            {
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name,
                Ot = DateTime.Now,
                Price = request.Price,
                Remark = request.Remark,
                SaleQuantity = 0,
                Status = request.Status,
                TenantId = request.LoginTenantId,
                UserId = request.LoginUserId,
                Points = request.Points.EtmsToPoints()
            });
            await _userOperationLogDAL.AddUserLog(request, $"添加费用-{request.Name}", EmUserOperationType.CostManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> CostEdit(CostEditRequest request)
        {
            var cost = await _costDAL.GetCost(request.CId);
            if (cost == null)
            {
                return ResponseBase.CommonError("费用不存在");
            }
            if (await _costDAL.ExistCost(request.Name, request.CId))
            {
                return ResponseBase.CommonError("已存在相同名称的费用");
            }
            cost.Name = request.Name;
            cost.Price = request.Price;
            cost.Remark = request.Remark;
            cost.Status = request.Status;
            cost.Points = request.Points.EtmsToPoints();
            await _costDAL.EditCost(cost);
            await _userOperationLogDAL.AddUserLog(request, $"编辑费用-{request.Name}", EmUserOperationType.CostManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> CostGet(CostGetRequest request)
        {
            var cost = await _costDAL.GetCost(request.CId);
            if (cost == null)
            {
                return ResponseBase.CommonError("费用不存在");
            }
            var isCanModifyPrice = true;
            var used = await _suitDAL.GetProductSuitUsed(EmProductType.Cost, cost.Id);
            if (used.Any())
            {
                isCanModifyPrice = false;
            }
            return ResponseBase.Success(new CostGetOutput()
            {
                CId = cost.Id,
                Name = cost.Name,
                Price = cost.Price,
                Remark = cost.Remark,
                Status = cost.Status,
                Points = cost.Points,
                IsCanModifyPrice = isCanModifyPrice
            });
        }

        public async Task<ResponseBase> CostDel(CostDelRequest request)
        {
            var cost = await _costDAL.GetCost(request.CId);
            if (cost == null)
            {
                return ResponseBase.CommonError("费用不存在");
            }
            var used = await _suitDAL.GetProductSuitUsed(EmProductType.Cost, cost.Id);
            if (used.Any())
            {
                var usedSuit = string.Join(',', used.Select(p => p.Name));
                return ResponseBase.CommonError($"想要删除此费用，请先删除套餐[{usedSuit}]内的此费用");
            }
            var isUsed = await _orderDAL.ExistOrderProduct(EmProductType.Cost, cost.Id);
            if (isUsed)
            {
                return ResponseBase.CommonError("费用已使用，无法删除");
            }
            await _costDAL.DelCost(request.CId);
            await _userOperationLogDAL.AddUserLog(request, $"删除费用-{cost.Name}", EmUserOperationType.CostManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> CostStatusChange(CostStatusChangeRequest request)
        {
            var cost = await _costDAL.GetCost(request.CId);
            if (cost == null)
            {
                return ResponseBase.CommonError("费用不存在");
            }
            cost.Status = request.NewStatus;
            await _costDAL.EditCost(cost);
            var tag = request.NewStatus == EmGoodsStatus.Enabled ? "启用" : "禁用";
            await _userOperationLogDAL.AddUserLog(request, $"{tag}费用-{cost.Name}", EmUserOperationType.CostManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> CostGetPaging(CostGetPagingRequest request)
        {
            var pagingData = await _costDAL.GetPaging(request);
            return ResponseBase.Success(new ResponsePagingDataBase<CostGetPagingOutput>(pagingData.Item2, pagingData.Item1.Select(p => new CostGetPagingOutput()
            {
                CId = p.Id,
                Name = p.Name,
                Price = p.Price,
                SaleQuantity = p.SaleQuantity,
                Status = p.Status,
                Remark = p.Remark,
                StatusDesc = EmCostStatus.GetCostStatusDesc(p.Status),
                Points = p.Points
            })));
        }
    }
}
