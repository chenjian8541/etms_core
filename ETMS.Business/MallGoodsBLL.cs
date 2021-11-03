using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Product.Output;
using ETMS.Entity.Dto.Product.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.View.Database;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.MallGoodsDAL;
using ETMS.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETMS.Entity.Dto.Common;
using ETMS.Business.Common;
using ETMS.Entity.Config;
using ETMS.Business.BaseBLL;
using ETMS.IDataAccess.EtmsManage;

namespace ETMS.Business
{
    public class MallGoodsBLL : TenantLcsAccountBLL, IMallGoodsBLL
    {
        private readonly IMallGoodsDAL _mallGoodsDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IGoodsDAL _goodsDAL;

        private readonly ICostDAL _costDAL;

        private readonly ISuitDAL _suitDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly ITenantConfig2DAL _tenantConfig2DAL;

        public MallGoodsBLL(IMallGoodsDAL mallGoodsDAL, ICourseDAL courseDAL, IGoodsDAL goodsDAL, ICostDAL costDAL, ISuitDAL suitDAL,
            IUserOperationLogDAL userOperationLogDAL, IAppConfigurtaionServices appConfigurtaionServices,
            ITenantConfig2DAL tenantConfig2DAL, ITenantLcsAccountDAL tenantLcsAccountDAL, ISysTenantDAL sysTenantDAL)
            : base(tenantLcsAccountDAL, sysTenantDAL)
        {
            this._mallGoodsDAL = mallGoodsDAL;
            this._courseDAL = courseDAL;
            this._goodsDAL = goodsDAL;
            this._costDAL = costDAL;
            this._suitDAL = suitDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._tenantConfig2DAL = tenantConfig2DAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _mallGoodsDAL, _courseDAL, _goodsDAL, _costDAL, _suitDAL,
                _userOperationLogDAL, _tenantConfig2DAL);
        }

        private Tuple<List<EtMallCoursePriceRule>, decimal, string> GetCoursePriceRule(CoursePriceRule coursePriceRule,
            long mallGoodsId, long courseId, int tenantId)
        {
            var rules = new List<EtMallCoursePriceRule>();
            if (coursePriceRule.IsByClassTimes)
            {
                foreach (var p in coursePriceRule.ByClassTimes)
                {
                    rules.Add(new EtMallCoursePriceRule()
                    {
                        CourseId = courseId,
                        IsDeleted = EmIsDeleted.Normal,
                        Name = p.Name,
                        Price = p.Price,
                        PriceType = EmCoursePriceType.ClassTimes,
                        PriceUnit = EmCourseUnit.ClassTimes,
                        Quantity = p.Quantity,
                        TotalPrice = p.TotalPrice,
                        TenantId = tenantId,
                        Points = p.Points.EtmsToPoints(),
                        Id = p.Id,
                        ExpiredType = p.ExpiredType,
                        ExpiredValue = p.ExpiredValue,
                        MallGoodsId = mallGoodsId
                    });
                }
            }
            if (coursePriceRule.IsByMonth)
            {
                foreach (var p in coursePriceRule.ByMonth)
                {
                    rules.Add(new EtMallCoursePriceRule()
                    {
                        CourseId = courseId,
                        IsDeleted = EmIsDeleted.Normal,
                        Name = p.Name,
                        Price = p.Price,
                        PriceType = EmCoursePriceType.Month,
                        PriceUnit = EmCourseUnit.Month,
                        Quantity = p.Quantity,
                        TotalPrice = p.TotalPrice,
                        TenantId = tenantId,
                        Points = p.Points.EtmsToPoints(),
                        Id = p.Id,
                        MallGoodsId = mallGoodsId
                    });
                }
            }
            if (coursePriceRule.IsByDay)
            {
                foreach (var p in coursePriceRule.ByDay)
                {
                    rules.Add(new EtMallCoursePriceRule()
                    {
                        CourseId = courseId,
                        IsDeleted = EmIsDeleted.Normal,
                        Name = p.Name,
                        Price = p.Price,
                        PriceType = EmCoursePriceType.Day,
                        PriceUnit = EmCourseUnit.Day,
                        Quantity = p.Quantity,
                        TotalPrice = p.TotalPrice,
                        TenantId = tenantId,
                        Points = p.Points.EtmsToPoints(),
                        Id = p.Id,
                        MallGoodsId = mallGoodsId
                    });
                }
            }
            var minPriceRule = rules.OrderBy(p => p.TotalPrice).FirstOrDefault();
            var minPrice = 0M;
            if (minPriceRule != null)
            {
                minPrice = minPriceRule.TotalPrice;
            }
            if (rules.Count > 1)
            {
                return Tuple.Create(rules, minPrice, $"{minPrice.EtmsToString2()}起");
            }
            return Tuple.Create(rules, minPrice, minPrice.EtmsToString2());
        }

        public async Task<ResponseBase> MallGoodsAdd(MallGoodsAddRequest request)
        {
            if (await _mallGoodsDAL.ExistMlGoods(request.Name))
            {
                return ResponseBase.CommonError("已存在相同展示名称的商品");
            }
            List<EtMallCoursePriceRule> mlCoursePriceRules = null;
            decimal price;
            string priceDesc;
            Tuple<List<EtMallCoursePriceRule>, decimal, string> coursePriceRuleInfo;
            if (request.ProductType == EmProductType.Course)
            {
                coursePriceRuleInfo = GetCoursePriceRule(request.CoursePriceRules, 0, request.RelatedId, request.LoginTenantId);
                if (coursePriceRuleInfo.Item1.Count == 0)
                {
                    return ResponseBase.CommonError("请正确设置售卖价格");
                }
                mlCoursePriceRules = coursePriceRuleInfo.Item1;
                price = coursePriceRuleInfo.Item2;
                priceDesc = coursePriceRuleInfo.Item3;
            }
            else
            {
                price = request.Price;
                priceDesc = request.Price.EtmsToString2();
            }
            var myOrderIndex = await _mallGoodsDAL.GetMaxOrderIndex();
            var entity = new EtMallGoods()
            {
                Name = request.Name,
                Price = price,
                PriceDesc = priceDesc,
                OriginalPrice = request.OriginalPrice,
                OrderIndex = myOrderIndex,
                GsContent = request.GsContent,
                IsDeleted = EmIsDeleted.Normal,
                ProductType = request.ProductType,
                ProductTypeDesc = EmProductType.GetProductType(request.ProductType),
                ImgCover = request.ImgCoverKey,
                ImgDetail = EtmsHelper2.GetImgKeys(request.ImgDetailKeys),
                RelatedId = request.RelatedId,
                TenantId = request.LoginTenantId,
                SpecContent = ComBusiness4.GetSpecContent(request.SpecItems),
                TagContent = ComBusiness4.GetTagContent(request.TagItems),
                RelatedName = request.RelatedName,
                Points = request.Points,
                OriginalPriceDesc = request.OriginalPrice <= 0 ? string.Empty : request.OriginalPrice.EtmsToString2(),
                GId = string.Empty
            };
            await _mallGoodsDAL.AddMallGoods(entity, mlCoursePriceRules);

            await _userOperationLogDAL.AddUserLog(request, $"添加商品-{request.Name}", EmUserOperationType.MallGoodsMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> MallGoodsEdit(MallGoodsEditRequest request)
        {
            var myMallGoodsBucket = await _mallGoodsDAL.GetMallGoods(request.Id);
            if (myMallGoodsBucket == null || myMallGoodsBucket.MallGoods == null)
            {
                return ResponseBase.CommonError("商品不存在");
            }
            if (await _mallGoodsDAL.ExistMlGoods(request.Name, request.Id))
            {
                return ResponseBase.CommonError("已存在相同展示名称的商品");
            }
            var myMallGood = myMallGoodsBucket.MallGoods;
            List<EtMallCoursePriceRule> mlCoursePriceRules = null;
            decimal price;
            string priceDesc;
            Tuple<List<EtMallCoursePriceRule>, decimal, string> coursePriceRuleInfo;
            if (request.ProductType == EmProductType.Course)
            {
                coursePriceRuleInfo = GetCoursePriceRule(request.CoursePriceRules, myMallGood.Id, myMallGood.RelatedId, request.LoginTenantId);
                if (coursePriceRuleInfo.Item1.Count == 0)
                {
                    return ResponseBase.CommonError("请正确设置售卖价格");
                }
                mlCoursePriceRules = coursePriceRuleInfo.Item1;
                price = coursePriceRuleInfo.Item2;
                priceDesc = coursePriceRuleInfo.Item3;
            }
            else
            {
                price = request.Price;
                priceDesc = request.Price.EtmsToString2();
            }
            myMallGood.RelatedName = request.RelatedName;
            myMallGood.Name = request.Name;
            myMallGood.Price = price;
            myMallGood.PriceDesc = priceDesc;
            myMallGood.OriginalPrice = request.OriginalPrice;
            myMallGood.GsContent = request.GsContent;
            myMallGood.ImgCover = request.ImgCoverKey;
            myMallGood.Points = request.Points;
            myMallGood.ImgDetail = EtmsHelper2.GetImgKeys(request.ImgDetailKeys);
            myMallGood.SpecContent = ComBusiness4.GetSpecContent(request.SpecItems);
            myMallGood.TagContent = ComBusiness4.GetTagContent(request.TagItems);
            myMallGood.OriginalPriceDesc = request.OriginalPrice <= 0 ? string.Empty : request.OriginalPrice.EtmsToString2();
            await _mallGoodsDAL.EditMallGoods(myMallGood, mlCoursePriceRules);

            await _userOperationLogDAL.AddUserLog(request, $"编辑商品-{request.Name}", EmUserOperationType.MallGoodsMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> MallGoodsDel(MallGoodsDelRequest request)
        {
            var myMallGoodsBucket = await _mallGoodsDAL.GetMallGoods(request.Id);
            if (myMallGoodsBucket == null || myMallGoodsBucket.MallGoods == null)
            {
                return ResponseBase.CommonError("商品不存在");
            }
            await _mallGoodsDAL.DelMallGoods(request.Id);

            await _userOperationLogDAL.AddUserLog(request, $"移除商品-{myMallGoodsBucket.MallGoods.Name}", EmUserOperationType.MallGoodsMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> MallGoodsChangeOrderIndex(MallGoodsChangeOrderIndexRequest request)
        {
            if (request.Id2 > 0)
            {
                await MallGoodsChangeOrderIndex(request.Id1, request.OrderIndex1, request.Id2, request.OrderIndex2);
            }
            else
            {
                var nearEntity = await _mallGoodsDAL.GetMallGoodsNearOrderIndex(request.OrderIndex1, request.NoId2Type);
                if (nearEntity == null)
                {
                    return ResponseBase.CommonError("操作失败");
                }
                await MallGoodsChangeOrderIndex(request.Id1, request.OrderIndex1, nearEntity.Id, nearEntity.OrderIndex);
            }
            return ResponseBase.Success();
        }

        private async Task MallGoodsChangeOrderIndex(long id1, long orderIndex1, long id2, long orderIndex2)
        {
            if (orderIndex1 == orderIndex2)
            {
                orderIndex2--;
            }
            await _mallGoodsDAL.UpdateOrderIndex(id1, orderIndex2);
            await _mallGoodsDAL.UpdateOrderIndex(id2, orderIndex1);
        }

        public async Task<ResponseBase> MallGoodsGet(MallGoodsGetRequest request)
        {
            var myMallGoodsBucket = await _mallGoodsDAL.GetMallGoods(request.Id);
            if (myMallGoodsBucket == null || myMallGoodsBucket.MallGoods == null)
            {
                return ResponseBase.CommonError("商品不存在");
            }
            var myMallGoods = myMallGoodsBucket.MallGoods;
            var myMallCoursePriceRules = myMallGoodsBucket.MallCoursePriceRules;
            CoursePriceRuleOutput coursePriceRules = null;
            if (myMallGoods.ProductType == EmProductType.Course && myMallCoursePriceRules != null && myMallCoursePriceRules.Count > 0)
            {
                coursePriceRules = new CoursePriceRuleOutput()
                {
                    ByClassTimes = new List<CoursePriceRuleOutputItem>(),
                    ByMonth = new List<CoursePriceRuleOutputItem>(),
                    ByDay = new List<CoursePriceRuleOutputItem>(),
                    ByClassTimesIsCanModify = true,
                    ByDayIsCanModify = true,
                    ByMonthIsCanModify = true,
                };
                var byClassTimes = myMallCoursePriceRules.Where(p => p.PriceType == EmCoursePriceType.ClassTimes);
                if (byClassTimes.Any())
                {
                    coursePriceRules.IsByClassTimes = true;
                    foreach (var p in byClassTimes)
                    {
                        var tempByClassTimes = new CoursePriceRuleOutputItem()
                        {
                            Name = p.Name,
                            Price = p.Price,
                            Quantity = p.Quantity,
                            TotalPrice = p.TotalPrice,
                            Points = p.Points,
                            IsCanModify = true,
                            ExpiredType = p.ExpiredType,
                            ExpiredValue = p.ExpiredValue
                        };
                        coursePriceRules.ByClassTimes.Add(tempByClassTimes);
                    }
                }
                var byMonth = myMallCoursePriceRules.Where(p => p.PriceType == EmCoursePriceType.Month);
                if (byMonth.Any())
                {
                    coursePriceRules.IsByMonth = true;
                    foreach (var p in byMonth)
                    {
                        var tempByMonth = new CoursePriceRuleOutputItem()
                        {
                            Name = p.Name,
                            Price = p.Price,
                            Quantity = p.Quantity,
                            TotalPrice = p.TotalPrice,
                            Points = p.Points,
                            IsCanModify = true
                        };
                        coursePriceRules.ByMonth.Add(tempByMonth);
                    }
                }
                var byDay = myMallCoursePriceRules.Where(p => p.PriceType == EmCoursePriceType.Day);
                if (byDay.Any())
                {
                    coursePriceRules.IsByDay = true;
                    foreach (var p in byDay)
                    {
                        var tempByDay = new CoursePriceRuleOutputItem()
                        {
                            Name = p.Name,
                            Price = p.Price,
                            Quantity = p.Quantity,
                            TotalPrice = p.TotalPrice,
                            Points = p.Points,
                            IsCanModify = true
                        };
                        coursePriceRules.ByDay.Add(tempByDay);
                    }
                }
            }
            var relatedName = string.Empty;
            switch (myMallGoods.ProductType)
            {
                case EmProductType.Course:
                    var myCourse = await _courseDAL.GetCourse(myMallGoods.RelatedId);
                    if (myCourse == null || myCourse.Item1 == null)
                    {
                        return ResponseBase.CommonError("关联商品不存在");
                    }
                    relatedName = myCourse.Item1.Name;
                    break;
                case EmProductType.Goods:
                    var myGoods = await _goodsDAL.GetGoods(myMallGoods.RelatedId);
                    if (myGoods == null)
                    {
                        return ResponseBase.CommonError("关联商品不存在");
                    }
                    relatedName = myGoods.Name;
                    break;
                case EmProductType.Cost:
                    var myCost = await _costDAL.GetCost(myMallGoods.RelatedId);
                    if (myCost == null)
                    {
                        return ResponseBase.CommonError("关联商品不存在");
                    }
                    relatedName = myCost.Name;
                    break;
                case EmProductType.Suit:
                    var mySuit = await _suitDAL.GetSuit(myMallGoods.RelatedId);
                    if (mySuit == null || mySuit.Item1 == null)
                    {
                        return ResponseBase.CommonError("关联商品不存在");
                    }
                    relatedName = mySuit.Item1.Name;
                    break;
            }
            if (coursePriceRules == null)
            {
                coursePriceRules = new CoursePriceRuleOutput()
                {
                    ByClassTimes = new List<CoursePriceRuleOutputItem>(),
                    ByMonth = new List<CoursePriceRuleOutputItem>(),
                    ByDay = new List<CoursePriceRuleOutputItem>(),
                    ByClassTimesIsCanModify = true,
                    ByDayIsCanModify = true,
                    ByMonthIsCanModify = true,
                    IsByClassTimes = false,
                    IsByDay = false,
                    IsByMonth = false
                };
            }
            var output = new MallGoodsGetOutput()
            {
                Id = myMallGoods.Id,
                RelatedName = relatedName,
                Name = myMallGoods.Name,
                GsContent = myMallGoods.GsContent,
                OriginalPrice = myMallGoods.OriginalPrice,
                Price = myMallGoods.Price,
                PriceDesc = myMallGoods.PriceDesc,
                ProductType = myMallGoods.ProductType,
                ProductTypeDesc = myMallGoods.ProductTypeDesc,
                RelatedId = myMallGoods.RelatedId,
                ImgDetail = ComBusiness4.GetImgs(myMallGoods.ImgDetail),
                CoursePriceRules = coursePriceRules,
                SpecItems = ComBusiness4.GetSpecView(myMallGoods.SpecContent),
                TagItems = ComBusiness4.GetTagView(myMallGoods.TagContent),
                OriginalPriceDesc = myMallGoods.OriginalPriceDesc,
                imgCoverKey = myMallGoods.ImgCover,
                imgCoverUrl = AliyunOssUtil.GetAccessUrlHttps(myMallGoods.ImgCover),
                GId = myMallGoods.GId,
                Points = myMallGoods.Points
            };

            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> MallGoodsTagSet(MallGoodsTagSetRequest request)
        {
            var tagContent = ComBusiness4.GetTagContent(request.TagItems);
            await _mallGoodsDAL.UpdateTagContent(request.Ids, tagContent);

            await _userOperationLogDAL.AddUserLog(request, "批量设置活动标签", EmUserOperationType.MallGoodsMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> MallGoodsSaveStatus(MallGoodsSaveStatusRequest request)
        {
            if (request.MallGoodsStatus == EmMallGoodsStatus.Open)
            {
                var checkTenantLcsAccountResult = await CheckTenantLcsAccount(request.LoginTenantId);
                if (!string.IsNullOrEmpty(checkTenantLcsAccountResult.ErrMsg))
                {
                    return ResponseBase.CommonError(checkTenantLcsAccountResult.ErrMsg);
                }
            }
            var tenantConfig = await _tenantConfig2DAL.GetTenantConfig();
            tenantConfig.MallGoodsConfig.MallGoodsStatus = request.MallGoodsStatus;
            await _tenantConfig2DAL.SaveTenantConfig(tenantConfig);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> MallGoodsSaveShareConfig(MallGoodsSaveShareConfigRequest request)
        {
            var tenantConfig = await _tenantConfig2DAL.GetTenantConfig();
            tenantConfig.MallGoodsConfig.Title = request.Title;
            tenantConfig.MallGoodsConfig.HomeShareImgKey = request.HomeShareImgKey;
            await _tenantConfig2DAL.SaveTenantConfig(tenantConfig);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> MallGoodsGetConfig(MallGoodsGetConfigRequest request)
        {
            var config = _appConfigurtaionServices.AppSettings;
            var wxConfig = config.WxConfig;
            var tenantNo = TenantLib.GetTenantEncrypt(request.LoginTenantId);
            var tenantConfig = await _tenantConfig2DAL.GetTenantConfig();
            var tenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            var output = new MallGoodsGetConfigOutput()
            {
                HomeShareUrl = string.Format(wxConfig.WeChatEntranceConfig.MallGoodsHomeUrl, tenantNo),
                DetailShareUrl = string.Format(wxConfig.WeChatEntranceConfig.MallGoodsDetailUrl, tenantNo),
                HomeShareImgUrl = AliyunOssUtil.GetAccessUrlHttps(tenantConfig.MallGoodsConfig.HomeShareImgKey),
                HomeShareImgKey = tenantConfig.MallGoodsConfig.HomeShareImgKey,
                TenantNo = tenantNo,
                MallGoodsStatus = tenantConfig.MallGoodsConfig.MallGoodsStatus,
                Title = tenantConfig.MallGoodsConfig.Title,
                TenantName = tenant.Name
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> MallGoodsGetPaging(MallGoodsGetPagingRequest request)
        {
            var pagingData = await _mallGoodsDAL.GetPagingSimple(request);
            var output = new List<MallGoodsGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var isCanUp = true;
                var isCanDown = true;
                var i = 1;
                var totalPage = EtmsHelper.GetTotalPage(pagingData.Item2, request.PageSize);
                var myCount = pagingData.Item1.Count();
                foreach (var p in pagingData.Item1)
                {
                    isCanUp = true;
                    isCanDown = true;
                    if (request.PageCurrent == 1 && i == 1)
                    {
                        isCanUp = false;
                    }
                    if (request.PageCurrent == totalPage && i == myCount)
                    {
                        isCanDown = false;
                    }
                    i++;

                    List<PriceRuleDesc> myPriceRuleDesc = null;
                    if (p.ProductType == EmProductType.Course)
                    {
                        var myMallGoodsBucket = await _mallGoodsDAL.GetMallGoods(p.Id);
                        if (myMallGoodsBucket == null || myMallGoodsBucket.MallGoods == null)
                        {
                            continue;
                        }
                        myPriceRuleDesc = ComBusiness3.GetPriceRuleDescs(myMallGoodsBucket.MallCoursePriceRules);
                    }
                    output.Add(new MallGoodsGetPagingOutput()
                    {
                        Id = p.Id,
                        ImgCoverUrl = AliyunOssUtil.GetAccessUrlHttps(p.ImgCover),
                        IsCanUp = isCanUp,
                        IsCanDown = isCanDown,
                        Name = p.Name,
                        OrderIndex = p.OrderIndex,
                        OriginalPriceDesc = p.OriginalPriceDesc,
                        Price = p.Price,
                        PriceDesc = p.PriceDesc,
                        ProductType = p.ProductType,
                        ProductTypeDesc = p.ProductTypeDesc,
                        RelatedId = p.RelatedId,
                        RelatedName = p.RelatedName,
                        PriceRuleDescs = myPriceRuleDesc,
                        GId = p.GId,
                        Points = p.Points,
                        IsLoading = false
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<MallGoodsGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> MallGoodsGetPaging2(MallGoodsGetPagingRequest request)
        {
            var pagingData = await _mallGoodsDAL.GetPagingSimple(request);
            var output = new List<MallGoodsGetPaging2Output>();
            if (pagingData.Item1.Any())
            {
                foreach (var p in pagingData.Item1)
                {
                    output.Add(new MallGoodsGetPaging2Output()
                    {
                        Id = p.Id,
                        ImgCoverUrl = AliyunOssUtil.GetAccessUrlHttps(p.ImgCover),
                        Name = p.Name,
                        OrderIndex = p.OrderIndex,
                        OriginalPriceDesc = p.OriginalPriceDesc,
                        Price = p.Price,
                        PriceDesc = p.PriceDesc,
                        ProductType = p.ProductType,
                        ProductTypeDesc = p.ProductTypeDesc,
                        RelatedId = p.RelatedId,
                        RelatedName = p.RelatedName,
                        GId = p.GId,
                        Points = p.Points,
                        TagItems = ComBusiness4.GetTagView(p.TagContent)
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<MallGoodsGetPaging2Output>(pagingData.Item2, output));
        }
    }
}
