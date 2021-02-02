using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Product.Output;
using ETMS.Entity.Dto.Product.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Utility;
using ETMS.IEventProvider;
using ETMS.Event.DataContract;

namespace ETMS.Business
{
    public class GoodsBLL : IGoodsBLL
    {
        private readonly IGoodsDAL _goodsDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IEventPublisher _eventPublisher;

        public GoodsBLL(IGoodsDAL goodsDAL, IUserOperationLogDAL userOperationLogDAL, IEventPublisher eventPublisher)
        {
            this._goodsDAL = goodsDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._eventPublisher = eventPublisher;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _goodsDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> GoodsAdd(GoodsAddRequest request)
        {
            if (await _goodsDAL.ExistGoods(request.Name))
            {
                return ResponseBase.CommonError("已存在相同名称的物品");
            }
            int? limitQuantity = null;
            if (!string.IsNullOrEmpty(request.LimitQuantity))
            {
                limitQuantity = request.LimitQuantity.ToInt();
            }
            var myInventoryQuantity = string.IsNullOrEmpty(request.InventoryQuantity) ? 0 : request.InventoryQuantity.ToInt();
            var entity = new EtGoods()
            {
                InventoryQuantity = myInventoryQuantity,
                IsDeleted = EmIsDeleted.Normal,
                LimitQuantity = limitQuantity,
                Name = request.Name,
                Ot = DateTime.Now,
                Price = request.Price,
                Remark = request.Remark,
                SaleQuantity = 0,
                Status = request.Status,
                TenantId = request.LoginTenantId,
                UserId = request.LoginUserId,
                Points = request.Points.EtmsToPoints()
            };
            await _goodsDAL.AddGoods(entity);
            if (myInventoryQuantity > 0)
            {
                await _goodsDAL.AddGoodsInventoryLog(new EtGoodsInventoryLog()
                {
                    ChangeQuantity = myInventoryQuantity,
                    GoodsId = entity.Id,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = DateTime.Now,
                    Prince = 0,
                    TotalMoney = 0,
                    Remark = "添加物品-初始化库存",
                    TenantId = request.LoginTenantId,
                    UserId = request.LoginUserId,
                    Type = EmGoodsInventoryType.AddGoodsInInventory
                });
            }
            await _userOperationLogDAL.AddUserLog(request, $"添加物品-{request.Name}", EmUserOperationType.GoodsManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> GoodsEdit(GoodsEditRequest request)
        {
            var goods = await _goodsDAL.GetGoods(request.CId);
            if (goods == null)
            {
                return ResponseBase.CommonError("物品不存在");
            }
            if (await _goodsDAL.ExistGoods(request.Name, request.CId))
            {
                return ResponseBase.CommonError("已存在相同名称的物品");
            }
            int? limitQuantity = null;
            if (!string.IsNullOrEmpty(request.LimitQuantity))
            {
                limitQuantity = request.LimitQuantity.ToInt();
            }
            var oldInventoryQuantity = goods.InventoryQuantity;
            var myInventoryQuantity = string.IsNullOrEmpty(request.InventoryQuantity) ? 0 : request.InventoryQuantity.ToInt();

            goods.Name = request.Name;
            goods.Price = request.Price;
            goods.Remark = request.Remark;
            goods.LimitQuantity = limitQuantity;
            goods.Status = request.Status;
            goods.Points = request.Points.EtmsToPoints();
            goods.InventoryQuantity = myInventoryQuantity;
            await _goodsDAL.EditGoods(goods);

            if (myInventoryQuantity != oldInventoryQuantity)
            {
                var dffQuantity = myInventoryQuantity - oldInventoryQuantity;
                var inventoryLog = new EtGoodsInventoryLog()
                {
                    GoodsId = goods.Id,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = DateTime.Now,
                    Prince = 0,
                    TotalMoney = 0,
                    Remark = "编辑物品-修改库存",
                    TenantId = request.LoginTenantId,
                    UserId = request.LoginUserId
                };
                if (dffQuantity > 0)
                {
                    inventoryLog.Type = EmGoodsInventoryType.EditGoodsInInventoryAdd;
                    inventoryLog.ChangeQuantity = dffQuantity;
                }
                else
                {
                    inventoryLog.Type = EmGoodsInventoryType.EditGoodsInInventoryDe;
                    inventoryLog.ChangeQuantity = -dffQuantity;
                }
                await _goodsDAL.AddGoodsInventoryLog(inventoryLog);
            }

            _eventPublisher.Publish(new ResetTenantToDoThingEvent(request.LoginTenantId));
            await _userOperationLogDAL.AddUserLog(request, $"编辑物品-{request.Name}", EmUserOperationType.GoodsManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> GoodsGet(GoodsGetRequest request)
        {
            var goods = await _goodsDAL.GetGoods(request.CId);
            if (goods == null)
            {
                return ResponseBase.CommonError("物品不存在");
            }
            return ResponseBase.Success(new GoodsGetOutput()
            {
                CId = goods.Id,
                LimitQuantity = goods.LimitQuantity == null ? string.Empty : goods.LimitQuantity.Value.ToString(),
                Name = goods.Name,
                Price = goods.Price,
                Remark = goods.Remark,
                Status = goods.Status,
                InventoryQuantity = goods.InventoryQuantity,
                Points = goods.Points
            });
        }

        public async Task<ResponseBase> GoodsDel(GoodsDelRequest request)
        {
            var goods = await _goodsDAL.GetGoods(request.CId);
            if (goods == null)
            {
                return ResponseBase.CommonError("物品不存在");
            }
            if (goods.SaleQuantity > 0 || goods.InventoryQuantity > 0)
            {
                return ResponseBase.CommonError("物品已使用，无法删除");
            }
            await _goodsDAL.DelGoods(request.CId);
            await _userOperationLogDAL.AddUserLog(request, $"删除物品-{goods.Name}", EmUserOperationType.GoodsManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> GoodsStatusChange(GoodsStatusChangeRequest request)
        {
            var goods = await _goodsDAL.GetGoods(request.CId);
            if (goods == null)
            {
                return ResponseBase.CommonError("物品不存在");
            }
            goods.Status = request.NewStatus;
            await _goodsDAL.EditGoods(goods);
            var tag = request.NewStatus == EmGoodsStatus.Enabled ? "启用" : "禁用";
            await _userOperationLogDAL.AddUserLog(request, $"{tag}物品-{goods.Name}", EmUserOperationType.GoodsManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> GoodsGetPaging(GoodsGetPagingRequest request)
        {
            var pagingData = await _goodsDAL.GetPaging(request);
            return ResponseBase.Success(new ResponsePagingDataBase<GoodsGetPagingOutput>(pagingData.Item2, pagingData.Item1.Select(p => new GoodsGetPagingOutput()
            {
                CId = p.Id,
                InventoryQuantity = p.InventoryQuantity,
                Name = p.Name,
                Price = p.Price,
                SaleQuantity = p.SaleQuantity,
                Status = p.Status,
                StatusDesc = EmGoodsStatus.GetGoodsStatusDesc(p.Status),
                LimitQuantityDesc = p.LimitQuantity == null ? "未设置" : p.LimitQuantity.ToString(),
                Points = p.Points
            })));
        }

        public async Task<ResponseBase> GoodsInventoryLogAdd(GoodsInventoryLogAddRequest request)
        {
            var goods = await _goodsDAL.GetGoods(request.GoodsId);
            if (goods == null)
            {
                return ResponseBase.CommonError("物品不存在");
            }
            await _goodsDAL.AddInventoryQuantity(request.GoodsId, request.Quantity);
            await _goodsDAL.AddGoodsInventoryLog(new EtGoodsInventoryLog()
            {
                ChangeQuantity = request.Quantity,
                GoodsId = request.GoodsId,
                IsDeleted = EmIsDeleted.Normal,
                Ot = request.Ot,
                Prince = request.Prince,
                TotalMoney = request.TotalMoney,
                Remark = request.Remark,
                TenantId = request.LoginTenantId,
                UserId = request.LoginUserId,
                Type = EmGoodsInventoryType.InInventory
            });

            _eventPublisher.Publish(new ResetTenantToDoThingEvent(request.LoginTenantId));
            await _userOperationLogDAL.AddUserLog(request, $"物品采购-物品[{goods.Name}],采购数量{request.Quantity}", EmUserOperationType.GoodsManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> GoodsInventoryLogGetPaging(GoodsInventoryLogGetPagingRequest request)
        {
            var pagingData = await _goodsDAL.GetGoodsInventoryLogPaging(request);
            return ResponseBase.Success(new ResponsePagingDataBase<GoodsInventoryLogGetPagingOutput>(pagingData.Item2, pagingData.Item1.Select(p => new GoodsInventoryLogGetPagingOutput()
            {
                ChangeQuantity = EmGoodsInventoryType.GetGoodsInventoryChangeQuantity(p.Type, p.ChangeQuantity),
                GoodsName = p.GoodsName,
                OtDesc = p.Ot.EtmsToDateString(),
                Prince = p.Prince,
                Remark = p.Remark,
                TotalMoney = p.TotalMoney,
                Type = p.Type,
                TypeDesc = EmGoodsInventoryType.GetGoodsInventoryTypeDesc(p.Type),
                UserName = p.UserName
            })));
        }
    }
}
