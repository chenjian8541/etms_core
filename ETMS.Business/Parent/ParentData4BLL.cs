﻿using ETMS.Business.BaseBLL;
using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Parent3.Request;
using ETMS.Entity.Dto.Parent4.Output;
using ETMS.Entity.Dto.Parent4.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.Pay.Lcsw.Dto.Request;
using ETMS.Entity.Temp;
using ETMS.Entity.View;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
using ETMS.IBusiness.Parent;
using ETMS.IBusiness.Wechart;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IDataAccess.Lcs;
using ETMS.IDataAccess.MallGoodsDAL;
using ETMS.IEventProvider;
using ETMS.LOG;
using ETMS.Pay.Lcsw;
using ETMS.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Parent
{
    public class ParentData4BLL : TenantLcsAccountBLL, IParentData4BLL
    {
        private readonly IPayLcswService _payLcswService;

        private readonly IMallGoodsDAL _mallGoodsDAL;

        private readonly ITenantLcsPayLogDAL _tenantLcsPayLogDAL;

        private readonly IComponentAccessBLL _componentAccessBLL;

        private readonly IClassDAL _classDAL;

        private readonly IUserDAL _userDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IMallOrderDAL _mallOrderDAL;

        private readonly IEventPublisher _eventPublisher;

        public ParentData4BLL(ISysTenantDAL sysTenantDAL, ITenantLcsAccountDAL tenantLcsAccountDAL,
            IPayLcswService payLcswService, IMallGoodsDAL mallGoodsDAL, ITenantLcsPayLogDAL tenantLcsPayLogDAL,
            IComponentAccessBLL componentAccessBLL, IClassDAL classDAL, IUserDAL userDAL, IStudentDAL studentDAL,
            IMallOrderDAL mallOrder, IEventPublisher eventPublisher) : base(tenantLcsAccountDAL, sysTenantDAL)
        {
            this._payLcswService = payLcswService;
            this._mallGoodsDAL = mallGoodsDAL;
            this._tenantLcsPayLogDAL = tenantLcsPayLogDAL;
            this._componentAccessBLL = componentAccessBLL;
            this._classDAL = classDAL;
            this._userDAL = userDAL;
            this._studentDAL = studentDAL;
            this._mallOrderDAL = mallOrder;
            this._eventPublisher = eventPublisher;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _mallGoodsDAL, _tenantLcsPayLogDAL, _classDAL, _userDAL, _studentDAL, _mallOrderDAL);
        }

        public async Task<ResponseBase> ClassCanChooseGet(ClassCanChooseGetRequest request)
        {
            var output = new List<ClassCanChooseGetOutput>();
            var classList = await _classDAL.GetStudentClassCanChoose(request.StudentId, request.CourseId);
            if (classList == null || !classList.Any())
            {
                return ResponseBase.Success(output);
            }
            var tempBoxUser = new DataTempBox<EtUser>();
            foreach (var p in classList)
            {
                var isCanChoose = true;
                if (p.LimitStudentNums != null && p.LimitStudentNumsType == EmLimitStudentNumsType.NotOverflow)
                {
                    if (p.StudentNums >= p.LimitStudentNums.Value)
                    {
                        isCanChoose = false;
                    }
                }
                output.Add(new ClassCanChooseGetOutput()
                {
                    Id = p.Id,
                    IsCanChoose = isCanChoose,
                    Name = p.Name,
                    LimitStudentNumsDesc = EmLimitStudentNumsType.GetLimitStudentNumsDesc(p.StudentNums, p.LimitStudentNums, p.LimitStudentNumsType),
                    TeachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, p.Teachers)
                });
            }
            return ResponseBase.Success(output);
        }

        private CheckBuyMallGoodsResult CheckBuyMallGoods(EtMallGoods mallGoods, List<EtMallCoursePriceRule> mallCoursePriceRules, int buyCount,
            long? coursePriceRuleId, bool isGetComplex = false)
        {
            var totalPoint = 0;
            string strRuleDesc = null;
            if (mallGoods.ProductType == EmProductType.Course)
            {
                if (coursePriceRuleId == null || coursePriceRuleId.Value <= 0)
                {
                    return new CheckBuyMallGoodsResult("请选择课程收费方式");
                }
                if (mallCoursePriceRules == null || mallCoursePriceRules.Count == 0)
                {
                    return new CheckBuyMallGoodsResult("课程未设置收费方式");
                }
                var priceRule = mallCoursePriceRules.FirstOrDefault(p => p.Id == coursePriceRuleId);
                if (priceRule == null)
                {
                    return new CheckBuyMallGoodsResult("课程收费方式错误");
                }
                int buyQuantity;
                decimal itemSum;
                if (priceRule.Quantity > 1)
                {
                    buyQuantity = 1;
                    itemSum = priceRule.TotalPrice;
                }
                else
                {
                    buyQuantity = buyCount;
                    itemSum = (buyCount * priceRule.Price).EtmsToRound();
                }
                if (isGetComplex)
                {
                    if (priceRule.Points > 0)
                    {
                        totalPoint = buyQuantity * priceRule.Points;
                    }
                    strRuleDesc = ComBusiness.GetPriceRuleDesc(priceRule).Desc;
                }
                return new CheckBuyMallGoodsResult()
                {
                    TotalMoney = itemSum,
                    BuyCount = buyQuantity,
                    CoursePriceRule = priceRule,
                    TotalPoint = totalPoint,
                    PriceRuleDesc = strRuleDesc,
                    ErrMsg = string.Empty
                };
            }
            if (isGetComplex)
            {
                if (mallGoods.Points > 0)
                {
                    totalPoint = buyCount * mallGoods.Points;
                }
                strRuleDesc = $"{mallGoods.Price.EtmsToString2()}/{EmProductType.GetProductUnitDesc(mallGoods.ProductType)}";
            }
            return new CheckBuyMallGoodsResult()
            {
                TotalMoney = (buyCount * mallGoods.Price).EtmsToRound(),
                BuyCount = buyCount,
                TotalPoint = totalPoint,
                PriceRuleDesc = strRuleDesc,
                ErrMsg = string.Empty
            };
        }

        public async Task<ResponseBase> ParentBuyMallGoodsPrepay(ParentBuyMallGoodsPrepayRequest request)
        {
            var checkTenantLcsAccountResult = await CheckTenantLcsAccount(request.LoginTenantId);
            if (!string.IsNullOrEmpty(checkTenantLcsAccountResult.ErrMsg))
            {
                return ResponseBase.CommonError(checkTenantLcsAccountResult.ErrMsg);
            }
            var mallGoodsBucket = await _mallGoodsDAL.GetMallGoods(request.Id);
            if (mallGoodsBucket == null || mallGoodsBucket.MallGoods == null)
            {
                return ResponseBase.CommonError("商品不存在");
            }
            var tenantWechartAuth = await _componentAccessBLL.GetTenantWechartAuth(request.LoginTenantId);
            if (tenantWechartAuth == null)
            {
                Log.Error($"[ParentBuyMallGoodsPrepay]未找到机构授权信息,tenantId:{request.LoginTenantId}", this.GetType());
                return ResponseBase.CommonError("机构绑定的微信公众号无权限");
            }
            var myMallGoods = mallGoodsBucket.MallGoods;
            var myMallCoursePriceRules = mallGoodsBucket.MallCoursePriceRules;
            var checkBuyMallGoodsResult = CheckBuyMallGoods(myMallGoods, myMallCoursePriceRules, request.BuyCount, request.CoursePriceRuleId);
            if (!string.IsNullOrEmpty(checkBuyMallGoodsResult.ErrMsg))
            {
                return ResponseBase.CommonError(checkBuyMallGoodsResult.ErrMsg);
            }
            var totalMoney = checkBuyMallGoodsResult.TotalMoney;
            //var myCoursePriceRule = checkBuyMallGoodsResult.CoursePriceRule;

            var myTenant = checkTenantLcsAccountResult.MyTenant;
            var myLcsAccount = checkTenantLcsAccountResult.MyLcsAccount;
            var now = DateTime.Now;
            var orderNo = OrderNumberLib.EnrolmentOrderNumber2();
            var unifiedOrderRequest = new RequestUnifiedOrder()
            {
                access_token = myLcsAccount.AccessToken,
                terminal_id = myLcsAccount.TerminalId,
                terminal_time = ComBusiness4.GetLcsTerminalTime(now),
                attach = myTenant.Id.ToString(),
                merchant_no = myLcsAccount.MerchantNo,
                open_id = request.OpenId,
                order_body = "在线商城_商品购买",
                payType = "010",
                terminal_trace = orderNo,
                notify_url = SysWebApiAddressConfig.LcsPayJspayCallbackUrl,
                total_fee = EtmsHelper3.GetCent(totalMoney).ToString(),
                sub_appid = tenantWechartAuth.AuthorizerAppid
            };
            var unifiedOrderResult = _payLcswService.UnifiedOrder(unifiedOrderRequest);
            if (unifiedOrderResult.IsSuccess())
            {
                var payLogId = await _tenantLcsPayLogDAL.AddTenantLcsPayLog(new EtTenantLcsPayLog()
                {
                    AgentId = myTenant.AgentId,
                    Attach = orderNo,
                    AuthNo = string.Empty,
                    CreateOt = now,
                    DataType = EmTenantLcsPayLogDataType.Prepaid,
                    IsDeleted = EmIsDeleted.Normal,
                    MerchantName = myLcsAccount.MerchantName,
                    MerchantNo = myLcsAccount.MerchantNo,
                    MerchantType = myLcsAccount.MerchantType,
                    OpenId = request.OpenId,
                    OrderBody = unifiedOrderRequest.order_body,
                    OrderDesc = "在线商城",
                    OrderNo = orderNo,
                    OrderSource = EmLcsPayLogOrderSource.WeChat,
                    OrderType = EmLcsPayLogOrderType.StudentEnrolment,
                    OutRefundNo = string.Empty,
                    OutTradeNo = unifiedOrderResult.out_trade_no,
                    PayFinishDate = null,
                    PayFinishOt = null,
                    PayType = unifiedOrderResult.pay_type,
                    RefundDate = null,
                    RefundFee = null,
                    RefundOt = null,
                    RelationId = request.ParentStudentIds[0],
                    Remark = string.Empty,
                    Status = EmLcsPayLogStatus.Unpaid,
                    SubAppid = unifiedOrderResult.appId,
                    TenantId = myTenant.Id,
                    TerminalId = myLcsAccount.TerminalId,
                    TerminalTrace = orderNo,
                    TotalFee = unifiedOrderResult.total_fee,
                    TotalFeeDesc = totalMoney.ToString(),
                    TotalFeeValue = totalMoney
                });
                return ResponseBase.Success(new ParentBuyMallGoodsPrepayOutput()
                {
                    ali_trade_no = unifiedOrderResult.ali_trade_no,
                    appId = unifiedOrderResult.appId,
                    nonceStr = unifiedOrderResult.nonceStr,
                    orderNo = orderNo,
                    package_str = unifiedOrderResult.package_str,
                    paySign = unifiedOrderResult.paySign,
                    signType = unifiedOrderResult.signType,
                    TenantLcsPayLogId = payLogId,
                    timeStamp = unifiedOrderResult.timeStamp,
                    token_id = unifiedOrderResult.token_id
                });
            }
            return ResponseBase.CommonError($"生成预支付订单失败:{unifiedOrderResult.return_msg}");
        }

        public async Task<ResponseBase> ParentBuyMallGoodsSubmit(ParentBuyMallGoodsSubmitRequest request)
        {
            var mallGoodsBucket = await _mallGoodsDAL.GetMallGoods(request.Id);
            if (mallGoodsBucket == null || mallGoodsBucket.MallGoods == null)
            {
                return ResponseBase.CommonError("商品不存在");
            }
            var myMallGoods = mallGoodsBucket.MallGoods;
            var myMallCoursePriceRules = mallGoodsBucket.MallCoursePriceRules;
            var checkBuyMallGoodsResult = CheckBuyMallGoods(myMallGoods, myMallCoursePriceRules, request.BuyCount,
                request.CoursePriceRuleId, true);
            if (!string.IsNullOrEmpty(checkBuyMallGoodsResult.ErrMsg))
            {
                return ResponseBase.CommonError(checkBuyMallGoodsResult.ErrMsg);
            }

            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var lcsPaylog = await _tenantLcsPayLogDAL.GetTenantLcsPayLog(request.TenantLcsPayLogId);
            if (lcsPaylog == null)
            {
                return ResponseBase.CommonError("支付记录不存在");
            }

            var now = DateTime.Now;
            if (lcsPaylog.Status != EmLcsPayLogStatus.PaySuccess)
            {
                lcsPaylog.Status = EmLcsPayLogStatus.PaySuccess;
                lcsPaylog.PayFinishOt = now;
                lcsPaylog.PayFinishDate = now.Date;
                lcsPaylog.DataType = EmTenantLcsPayLogDataType.Normal;
                await _tenantLcsPayLogDAL.EditTenantLcsPayLog(lcsPaylog);
                _eventPublisher.Publish(new StatisticsLcsPayEvent(lcsPaylog.TenantId)
                {
                    StatisticsDate = now.Date
                });
            }
            var goodsSpecContent = string.Empty;
            if (request.SpecItems != null && request.SpecItems.Any())
            {
                goodsSpecContent = JsonConvert.SerializeObject(request.SpecItems);
            }
            var mallOrderEntity = new EtMallOrder()
            {
                OrderNo = lcsPaylog.OrderNo,
                AptSum = checkBuyMallGoodsResult.TotalMoney,
                BuyCount = checkBuyMallGoodsResult.BuyCount,
                CreateOt = now.Date,
                CreateTime = now,
                GoodsName = myMallGoods.Name,
                ImgCover = myMallGoods.ImgCover,
                IsDeleted = EmIsDeleted.Normal,
                LcsPayLogId = lcsPaylog.Id,
                MallGoodsId = myMallGoods.Id,
                PaySum = lcsPaylog.TotalFeeValue,
                ProductType = myMallGoods.ProductType,
                ProductTypeDesc = myMallGoods.ProductTypeDesc,
                RelatedId = myMallGoods.RelatedId,
                Remark = request.Remark,
                Status = EmMallOrderStatus.Normal,
                StudentId = request.StudentId,
                TenantId = request.LoginTenantId,
                PriceRuleDesc = checkBuyMallGoodsResult.PriceRuleDesc,
                TotalPoints = checkBuyMallGoodsResult.TotalPoint,
                GoodsSpecContent = goodsSpecContent
            };
            var mallOrderId = await _mallOrderDAL.AddMallOrder(mallOrderEntity);

            _eventPublisher.Publish(new ParentBuyMallGoodsSubmitEvent(request.LoginTenantId)
            {
                MallOrder = mallOrderEntity,
                MyMallGoodsBucket = mallGoodsBucket,
                MyTenantLcsPayLog = lcsPaylog,
                MyStudent = studentBucket.Student,
                CoursePriceRule = checkBuyMallGoodsResult.CoursePriceRule,
                ClassId = request.ClassId
            });
            return ResponseBase.Success(new ParentBuyMallGoodsSubmitOutput()
            {
                MallOrderId = mallOrderId,
                OrderNo = mallOrderEntity.OrderNo
            });
        }

        public async Task<ResponseBase> MallGoodsGetPaging(MallGoodsGetPagingRequest request)
        {
            var pagingData = await _mallOrderDAL.GetPaging(request);
            var output = new List<MallGoodsGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var tempBoxStudent = new DataTempBox<EtStudent>();
                foreach (var p in pagingData.Item1)
                {
                    var myStudent = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId);
                    if (myStudent == null)
                    {
                        continue;
                    }
                    List<ParentBuyMallGoodsSubmitSpecItem> goodsSpecContent = null;
                    if (p.ProductType == EmProductType.Goods)
                    {
                        if (!string.IsNullOrEmpty(p.GoodsSpecContent))
                        {
                            goodsSpecContent = JsonConvert.DeserializeObject<List<ParentBuyMallGoodsSubmitSpecItem>>(p.GoodsSpecContent);
                        }
                    }
                    output.Add(new MallGoodsGetPagingOutput()
                    {
                        StudentId = p.StudentId,
                        AptSum = p.AptSum,
                        BuyCount = p.BuyCount,
                        CId = p.Id,
                        CreateTime = p.CreateTime,
                        GoodsName = p.GoodsName,
                        GoodsSpecContent = goodsSpecContent,
                        ProductType = p.ProductType,
                        ImgCoverUrl = AliyunOssUtil.GetAccessUrlHttps(p.ImgCover),
                        LcsPayLogId = p.LcsPayLogId,
                        MallGoodsId = p.MallGoodsId,
                        OrderNo = p.OrderNo,
                        PaySum = p.PaySum,
                        PriceRuleDesc = p.PriceRuleDesc,
                        ProductTypeDesc = p.ProductTypeDesc,
                        RelatedId = p.RelatedId,
                        Remark = p.Remark,
                        Status = p.Status,
                        StatusDesc = EmMallOrderStatus.GetMallOrderStatusDesc(p.Status),
                        StudentName = myStudent.Name,
                        StudentPhone = myStudent.Phone,
                        TotalPoints = p.TotalPoints,
                        OrderId = p.OrderId
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<MallGoodsGetPagingOutput>(pagingData.Item2, output));
        }
    }
}
