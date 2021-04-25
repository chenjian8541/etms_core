using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Product.Request;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Enum;
using ETMS.Utility;
using ETMS.Business.Common;
using ETMS.Entity.Dto.Product.Output;

namespace ETMS.Business
{
    public class SuitBLL : ISuitBLL
    {
        private readonly ISuitDAL _suitDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly ICostDAL _costDAL;

        private readonly IGoodsDAL _goodsDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        public SuitBLL(ISuitDAL suitDAL, ICourseDAL courseDAL, ICostDAL costDAL, IGoodsDAL goodsDAL, IUserOperationLogDAL userOperationLogDAL)
        {
            this._suitDAL = suitDAL;
            this._courseDAL = courseDAL;
            this._costDAL = costDAL;
            this._goodsDAL = goodsDAL;
            this._userOperationLogDAL = userOperationLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _suitDAL, _courseDAL, _costDAL, _goodsDAL, _userOperationLogDAL);
        }

        private async Task<Tuple<string, List<EtSuitDetail>>> GetSuitDetail(int tenantId, List<SuitCourseInput> suitCourse,
            List<SuitGoodsInput> suitGoods, List<SuitCostInput> suitCost, long suitId)
        {
            var suitDetail = new List<EtSuitDetail>();
            if (suitCourse != null && suitCourse.Count > 0)
            {
                foreach (var p in suitCourse)
                {
                    var myCourseBucket = await _courseDAL.GetCourse(p.CourseId);
                    if (myCourseBucket == null || myCourseBucket.Item1 == null)
                    {
                        return Tuple.Create("课程不存在", suitDetail);
                    }
                    var myCoursePriceRule = myCourseBucket.Item2.FirstOrDefault(j => j.Id == p.CoursePriceRuleId);
                    if (myCoursePriceRule == null)
                    {
                        return Tuple.Create("课程收费标准不存在", suitDetail);
                    }
                    if (myCoursePriceRule.Quantity > 1 && p.BuyQuantity != myCoursePriceRule.Quantity) //多件一起出售的
                    {
                        return Tuple.Create("购买数量必须依照收费标准", suitDetail);
                    }
                    var priceRuleDesc = ComBusiness.GetPriceRuleDesc(myCoursePriceRule).Desc;
                    var itemSum = myCoursePriceRule.Quantity > 1 ? myCoursePriceRule.TotalPrice : (p.BuyQuantity * myCoursePriceRule.Price).EtmsToRound();
                    suitDetail.Add(new EtSuitDetail()
                    {
                        BuyUnit = myCoursePriceRule.PriceUnit,
                        BuyQuantity = p.BuyQuantity,
                        CoursePriceRuleId = p.CoursePriceRuleId,
                        DiscountValue = p.DiscountValue,
                        DiscountType = p.DiscountType,
                        GiveQuantity = p.GiveQuantity,
                        GiveUnit = p.GiveUnit,
                        IsDeleted = EmIsDeleted.Normal,
                        TenantId = tenantId,
                        ItemAptSum = p.ItemAptSum,
                        ItemSum = itemSum,
                        Price = myCoursePriceRule.Price,
                        PriceRule = priceRuleDesc,
                        ProductId = p.CourseId,
                        ProductType = EmProductType.Course,
                        SuitId = suitId
                    });
                }
            }
            if (suitGoods != null && suitGoods.Count > 0)
            {
                foreach (var p in suitGoods)
                {
                    var myGoods = await _goodsDAL.GetGoods(p.GoodsId);
                    if (myGoods == null)
                    {
                        return Tuple.Create("物品不存在", suitDetail);
                    }
                    var priceRuleDesc = $"{myGoods.Price}元/件";
                    suitDetail.Add(new EtSuitDetail()
                    {
                        BuyQuantity = p.BuyQuantity,
                        BuyUnit = 0,
                        CoursePriceRuleId = null,
                        DiscountType = p.DiscountType,
                        DiscountValue = p.DiscountValue,
                        GiveQuantity = 0,
                        GiveUnit = 0,
                        IsDeleted = EmIsDeleted.Normal,
                        ItemAptSum = p.ItemAptSum,
                        ItemSum = (p.BuyQuantity * myGoods.Price).EtmsToRound(),
                        Price = myGoods.Price,
                        PriceRule = priceRuleDesc,
                        ProductId = p.GoodsId,
                        ProductType = EmProductType.Goods,
                        TenantId = tenantId,
                        SuitId = suitId
                    });
                }
            }
            if (suitCost != null && suitCost.Count > 0)
            {
                foreach (var p in suitCost)
                {
                    var myCost = await _costDAL.GetCost(p.CostId);
                    if (myCost == null)
                    {
                        return Tuple.Create("费用不存在", suitDetail);
                    }
                    var priceRuleDesc = $"{myCost.Price}元/笔";
                    suitDetail.Add(new EtSuitDetail()
                    {
                        BuyQuantity = p.BuyQuantity,
                        BuyUnit = 0,
                        CoursePriceRuleId = null,
                        DiscountType = p.DiscountType,
                        DiscountValue = p.DiscountValue,
                        GiveQuantity = 0,
                        GiveUnit = 0,
                        IsDeleted = EmIsDeleted.Normal,
                        ItemAptSum = p.ItemAptSum,
                        ItemSum = (p.BuyQuantity * myCost.Price).EtmsToRound(),
                        Price = myCost.Price,
                        PriceRule = priceRuleDesc,
                        ProductId = p.CostId,
                        ProductType = EmProductType.Cost,
                        TenantId = tenantId,
                        SuitId = suitId
                    });
                }
            }

            return Tuple.Create(string.Empty, suitDetail);
        }

        public async Task<ResponseBase> SuitAdd(SuitAddRequest request)
        {
            if (await _suitDAL.ExistSuit(request.Name))
            {
                return ResponseBase.CommonError("已存在相同名称的套餐");
            }
            var suitDetailResult = await GetSuitDetail(request.LoginTenantId, request.SuitCourse, request.SuitGoods, request.SuitCost, 0);
            if (!string.IsNullOrEmpty(suitDetailResult.Item1))
            {
                return ResponseBase.CommonError(suitDetailResult.Item1);
            }
            var suitDetail = suitDetailResult.Item2;
            var suitPrice = suitDetail.Sum(p => p.ItemAptSum);
            var suit = new EtSuit()
            {
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name,
                Ot = DateTime.Now,
                Price = suitPrice,
                Remark = request.Remark,
                SalesCount = 0,
                Status = EmProductStatus.Enabled,
                TenantId = request.LoginTenantId,
                UserId = request.LoginUserId,
                Points = request.Points
            };
            await _suitDAL.AddSuit(suit, suitDetail);
            await _userOperationLogDAL.AddUserLog(request, $"添加套餐-{request.Name}", EmUserOperationType.SuitMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> SuitEdit(SuitEditRequest request)
        {
            var suitResult = await _suitDAL.GetSuit(request.Id);
            if (suitResult == null || suitResult.Item1 == null)
            {
                return ResponseBase.CommonError("套餐不存在");
            }
            if (await _suitDAL.ExistSuit(request.Name, request.Id))
            {
                return ResponseBase.CommonError("已存在相同名称的套餐");
            }
            var suitDetailResult = await GetSuitDetail(request.LoginTenantId, request.SuitCourse, request.SuitGoods,
                request.SuitCost, request.Id);
            if (!string.IsNullOrEmpty(suitDetailResult.Item1))
            {
                return ResponseBase.CommonError(suitDetailResult.Item1);
            }
            var suitDetail = suitDetailResult.Item2;
            var suitPrice = suitDetail.Sum(p => p.ItemAptSum);
            var suit = suitResult.Item1;
            suit.Name = request.Name;
            suit.Remark = request.Remark;
            suit.Price = suitPrice;
            suit.Points = request.Points;
            await _suitDAL.EditSuit(suit, suitDetail);

            await _userOperationLogDAL.AddUserLog(request, $"编辑套餐-{request.Name}", EmUserOperationType.SuitMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> SuitGet(SuitGetRequest request)
        {
            var suitResult = await _suitDAL.GetSuit(request.Id);
            if (suitResult == null || suitResult.Item1 == null)
            {
                return ResponseBase.CommonError("套餐不存在");
            }
            var suit = suitResult.Item1;
            var suitDetail = suitResult.Item2;
            var output = new SuitGetOutput()
            {
                Id = suit.Id,
                Name = suit.Name,
                Remark = suit.Remark,
                Points = suit.Points,
                SuitCost = new List<SuitDetailCostItem>(),
                SuitCourse = new List<SuitDetailCourseItem>(),
                SuitGoods = new List<SuitDetailGoodsItem>()
            };
            foreach (var myDetail in suitDetail)
            {
                switch (myDetail.ProductType)
                {
                    case EmProductType.Course:
                        var myCourseResult = await _courseDAL.GetCourse(myDetail.ProductId);
                        if (myCourseResult == null || myCourseResult.Item1 == null)
                        {
                            continue;
                        }
                        var myCoursePriceRule = myCourseResult.Item2.FirstOrDefault(j => j.Id == myDetail.CoursePriceRuleId.Value);
                        if (myCoursePriceRule == null)
                        {
                            continue;
                        }
                        var myCourse = myCourseResult.Item1;
                        var courseGetPagingOutput = new CourseGetPagingOutput()
                        {
                            CId = myCourse.Id,
                            Status = myCourse.Status,
                            Name = myCourse.Name,
                            PriceType = myCourse.PriceType,
                            PriceTypeDesc = EmCoursePriceType.GetCoursePriceTypeDesc2(myCourse.PriceType, myCourse.PriceTypeDesc),
                            Remark = myCourse.Remark,
                            Type = myCourse.Type,
                            TypeDesc = EmCourseType.GetCourseTypeDesc(myCourse.Type),
                            PriceRuleDescs = ComBusiness3.GetPriceRuleDescs(myCourseResult.Item2),
                            Label = myCourse.Name,
                            Value = myCourse.Id,
                            CheckPoints = myCourse.CheckPoints
                        };
                        output.SuitCourse.Add(new SuitDetailCourseItem()
                        {
                            MyData = courseGetPagingOutput,
                            MyCourseId = myCourse.Id,
                            Name = myCourse.Name,
                            PriceRuleDescs = courseGetPagingOutput.PriceRuleDescs,
                            ItemTotalSum = myDetail.ItemSum,
                            MyItemAptSum = myDetail.ItemAptSum,
                            MyBuyQuantity = myDetail.BuyQuantity,
                            MyBuyUnit = myDetail.BuyUnit,
                            MyCoursePriceRuleId = myDetail.CoursePriceRuleId.Value,
                            MyDiscountType = myDetail.DiscountType,
                            MyDiscountValue = myDetail.DiscountValue == 0 ? "" : myDetail.DiscountValue.EtmsToString(),
                            MyGiveQuantity = myDetail.GiveQuantity == 0 ? "" : myDetail.GiveQuantity.ToString(),
                            MyGiveUnit = myDetail.GiveUnit,
                            PriceType = myCoursePriceRule.PriceType,
                            PriceTypeDesc = ComBusiness.GetPriceRuleDesc(myCoursePriceRule).PriceTypeDesc,
                            Quantity = myCoursePriceRule.Quantity,
                            TotalPrice = myCoursePriceRule.TotalPrice
                        });
                        break;
                    case EmProductType.Goods:
                        var myGoods = await _goodsDAL.GetGoods(myDetail.ProductId);
                        if (myGoods == null)
                        {
                            continue;
                        }
                        var goodsView = new GoodsGetPagingOutput()
                        {
                            CId = myGoods.Id,
                            InventoryQuantity = myGoods.InventoryQuantity,
                            Name = myGoods.Name,
                            Price = myGoods.Price,
                            SaleQuantity = myGoods.SaleQuantity,
                            Status = myGoods.Status,
                            Points = myGoods.Points
                        };
                        output.SuitGoods.Add(new SuitDetailGoodsItem()
                        {
                            ItemTotalSum = myDetail.ItemSum,
                            MyBuyQuantity = myDetail.BuyQuantity,
                            MyData = goodsView,
                            MyDiscountType = myDetail.DiscountType,
                            MyDiscountValue = myDetail.DiscountValue == 0 ? "" : myDetail.DiscountValue.EtmsToString(),
                            MyGoodsId = myGoods.Id,
                            MyItemAptSum = myDetail.ItemAptSum,
                            Name = myGoods.Name,
                            Price = myDetail.Price,
                            Quantity = 1
                        });
                        break;
                    case EmProductType.Cost:
                        var myCost = await _costDAL.GetCost(myDetail.ProductId);
                        if (myCost == null)
                        {
                            continue;
                        }
                        var costView = new CostGetPagingOutput()
                        {
                            CId = myCost.Id,
                            Name = myCost.Name,
                            Price = myCost.Price,
                            SaleQuantity = myCost.SaleQuantity,
                            Status = myCost.Status,
                            Remark = myCost.Remark,
                            Points = myCost.Points
                        };
                        output.SuitCost.Add(new SuitDetailCostItem()
                        {
                            MyCostId = myCost.Id,
                            Price = myDetail.Price,
                            ItemTotalSum = myDetail.ItemSum,
                            MyBuyQuantity = myDetail.BuyQuantity,
                            MyDiscountType = myDetail.DiscountType,
                            MyDiscountValue = myDetail.DiscountValue == 0 ? "" : myDetail.DiscountValue.EtmsToString(),
                            MyData = costView,
                            MyItemAptSum = myDetail.ItemAptSum,
                            Name = myCost.Name,
                            Quantity = 1
                        });
                        break;
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> SuitDel(SuitDelRequest request)
        {
            var suitResult = await _suitDAL.GetSuit(request.Id);
            if (suitResult == null || suitResult.Item1 == null)
            {
                return ResponseBase.CommonError("套餐不存在");
            }
            await _suitDAL.DelSuit(request.Id);

            await _userOperationLogDAL.AddUserLog(request, $"删除套餐-{suitResult.Item1.Name}", EmUserOperationType.SuitMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> SuitChangeStatus(SuitChangeStatusRequest request)
        {
            var suitResult = await _suitDAL.GetSuit(request.Id);
            if (suitResult == null || suitResult.Item1 == null)
            {
                return ResponseBase.CommonError("套餐不存在");
            }

            var suit = suitResult.Item1;
            suit.Status = request.NewStatus;
            await _suitDAL.EditSuit(suit);
            var tag = request.NewStatus == EmProductStatus.Enabled ? "启用" : "禁用";
            await _userOperationLogDAL.AddUserLog(request, $"{tag}套餐-{suit.Name}", EmUserOperationType.SuitMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> SuitGetPaging(SuitGetPagingRequest request)
        {
            var output = new List<SuitGetPagingOutput>();
            var pagingData = await _suitDAL.GetPaging(request);
            foreach (var p in pagingData.Item1)
            {
                output.Add(new SuitGetPagingOutput()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Status = p.Status,
                    Points = p.Points
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<SuitGetPagingOutput>(pagingData.Item2, output));
        }
    }
}
