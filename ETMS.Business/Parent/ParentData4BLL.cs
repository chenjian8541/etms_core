using ETMS.Business.BaseBLL;
using ETMS.Business.Common;
using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.CoreBusiness.Request;
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
using ETMS.IBusiness;
using ETMS.IBusiness.Parent;
using ETMS.IBusiness.Wechart;
using ETMS.IDataAccess;
using ETMS.IDataAccess.ElectronicAlbum;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IDataAccess.Lcs;
using ETMS.IDataAccess.MallGoodsDAL;
using ETMS.IEventProvider;
using ETMS.LOG;
using ETMS.Pay.Lcsw;
using ETMS.Utility;
using Microsoft.AspNetCore.Http;
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
        private readonly IMallGoodsDAL _mallGoodsDAL;

        private readonly ITenantLcsPayLogDAL _tenantLcsPayLogDAL;

        private readonly IComponentAccessBLL _componentAccessBLL;

        private readonly IClassDAL _classDAL;

        private readonly IUserDAL _userDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IMallOrderDAL _mallOrderDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IMallPrepayDAL _mallPrepayDAL;

        private IDistributedLockDAL _distributedLockDAL;

        private readonly IAgtPayServiceBLL _agtPayServiceBLL;

        private readonly IClassRecordEvaluateDAL _classRecordEvaluateDAL;

        private readonly IElectronicAlbumDetailDAL _electronicAlbumDetailDAL;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ParentData4BLL(ISysTenantDAL sysTenantDAL, ITenantLcsAccountDAL tenantLcsAccountDAL,
            IMallGoodsDAL mallGoodsDAL, ITenantLcsPayLogDAL tenantLcsPayLogDAL,
            IComponentAccessBLL componentAccessBLL, IClassDAL classDAL, IUserDAL userDAL, IStudentDAL studentDAL,
            IMallOrderDAL mallOrder, IEventPublisher eventPublisher, IMallPrepayDAL mallPrepayDAL,
            IDistributedLockDAL distributedLockDAL, ITenantFubeiAccountDAL tenantFubeiAccountDAL,
            IAgtPayServiceBLL agtPayServiceBLL, IClassRecordEvaluateDAL classRecordEvaluateDAL,
            IElectronicAlbumDetailDAL electronicAlbumDetailDAL, IHttpContextAccessor httpContextAccessor,
            ISysTenantSuixingAccountDAL sysTenantSuixingAccountDAL, ISysTenantSuixingAccount2DAL sysTenantSuixingAccount2DAL)
            : base(tenantLcsAccountDAL, sysTenantDAL, tenantFubeiAccountDAL, sysTenantSuixingAccountDAL, sysTenantSuixingAccount2DAL)
        {
            this._mallGoodsDAL = mallGoodsDAL;
            this._tenantLcsPayLogDAL = tenantLcsPayLogDAL;
            this._componentAccessBLL = componentAccessBLL;
            this._classDAL = classDAL;
            this._userDAL = userDAL;
            this._studentDAL = studentDAL;
            this._mallOrderDAL = mallOrder;
            this._eventPublisher = eventPublisher;
            this._mallPrepayDAL = mallPrepayDAL;
            this._distributedLockDAL = distributedLockDAL;
            this._agtPayServiceBLL = agtPayServiceBLL;
            this._classRecordEvaluateDAL = classRecordEvaluateDAL;
            this._electronicAlbumDetailDAL = electronicAlbumDetailDAL;
            this._httpContextAccessor = httpContextAccessor;
        }

        public void InitTenantId(int tenantId)
        {
            this._agtPayServiceBLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _mallGoodsDAL, _tenantLcsPayLogDAL, _classDAL, _userDAL, _studentDAL,
                _mallOrderDAL, _mallPrepayDAL, _classRecordEvaluateDAL, _electronicAlbumDetailDAL);
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
            var checkTenantLcsAccountResult = await CheckTenantAgtPayAccount(request.LoginTenantId);
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
            var myAgtPayAccount = checkTenantLcsAccountResult.MyAgtPayAccountInfo;
            var now = DateTime.Now;
            var orderNo = OrderNumberLib.EnrolmentOrderNumber2();
            var orderBody = "在线商城_商品购买";
            var payLogId = await _tenantLcsPayLogDAL.AddTenantLcsPayLog(new EtTenantLcsPayLog()
            {
                AgentId = myTenant.AgentId,
                Attach = orderNo,
                AuthNo = string.Empty,
                CreateOt = now,
                DataType = EmTenantLcsPayLogDataType.Prepaid,
                IsDeleted = EmIsDeleted.Normal,
                MerchantName = myAgtPayAccount.MerchantName,
                MerchantNo = myAgtPayAccount.MerchantNo,
                MerchantType = myAgtPayAccount.MerchantType,
                OpenId = request.OpenId,
                OrderBody = orderBody,
                OrderDesc = "在线商城",
                OrderNo = orderNo,
                OrderSource = EmLcsPayLogOrderSource.WeChat,
                OrderType = EmLcsPayLogOrderType.StudentEnrolment,
                OutRefundNo = string.Empty,
                PayFinishDate = null,
                PayFinishOt = null,
                RefundDate = null,
                RefundFee = null,
                RefundOt = null,
                RelationId = request.ParentStudentIds[0],
                Remark = string.Empty,
                Status = EmLcsPayLogStatus.Unpaid,
                TenantId = myTenant.Id,
                TerminalId = myAgtPayAccount.TerminalId,
                TerminalTrace = orderNo,
                TotalFeeDesc = totalMoney.ToString(),
                TotalFeeValue = totalMoney,
                PayType = string.Empty,
                OutTradeNo = string.Empty,
                SubAppid = string.Empty,
                TotalFee = string.Empty,
                AgtPayType = myTenant.AgtPayType
            });
            var unifiedOrderRequest = new UnifiedOrderRequest()
            {
                Now = now,
                OpenId = request.OpenId,
                OrderBody = orderBody,
                OrderNo = orderNo,
                PayLogId = payLogId,
                PayMoney = totalMoney,
                PayMoneyCent = EtmsHelper3.GetCent(totalMoney),
                SubAppid = tenantWechartAuth.AuthorizerAppid,
                IpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString()
            };
            _agtPayServiceBLL.Initialize(checkTenantLcsAccountResult);
            var unifiedOrderResult = await _agtPayServiceBLL.UnifiedOrder(unifiedOrderRequest);
            if (unifiedOrderResult.IsSuccess)
            {
                await _tenantLcsPayLogDAL.UpdateTenantLcsPayLog(payLogId, unifiedOrderResult.OutTradeNo,
                    unifiedOrderResult.PayType, unifiedOrderResult.AppId, unifiedOrderRequest.PayMoneyCent.ToString());
                return ResponseBase.Success(new ParentBuyMallGoodsPrepayOutput()
                {
                    ali_trade_no = unifiedOrderResult.ali_trade_no,
                    appId = unifiedOrderResult.AppId,
                    nonceStr = unifiedOrderResult.NonceStr,
                    orderNo = orderNo,
                    package_str = unifiedOrderResult.Package_str,
                    paySign = unifiedOrderResult.PaySign,
                    signType = unifiedOrderResult.SignType,
                    TenantLcsPayLogId = payLogId,
                    timeStamp = unifiedOrderResult.TimeStamp,
                    token_id = string.Empty
                });
            }
            return ResponseBase.CommonError(unifiedOrderResult.ErrMsg);
        }

        public async Task<ResponseBase> ParentBuyMallGoodsPayInit(ParentBuyMallGoodsSubmitRequest request)
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

            await _mallPrepayDAL.MallPrepayAdd(new EtMallPrepay()
            {
                CreateTime = DateTime.Now,
                IsDeleted = EmIsDeleted.Normal,
                LcsPayLogId = request.TenantLcsPayLogId,
                Status = EmMallPrepayStatus.Untreated,
                TenantId = request.LoginTenantId,
                Type = EmMallPrepayType.StudentEnrolment,
                ReqContent = JsonConvert.SerializeObject(request)
            });

            return ResponseBase.Success();
        }

        private async Task<ResponseBase> ProcessParentBuyMallGoodsSubmit(ParentBuyMallGoodsSubmitRequest request)
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

        private async Task<ResponseBase> HandleParentBuyMallGoodsSubmit(ParentBuyMallGoodsSubmitRequest request)
        {
            var lockKey = new HandleParentBuyMallGoodsSubmitToken(request.LoginTenantId, request.TenantLcsPayLogId);
            if (_distributedLockDAL.LockTake(lockKey))
            {
                try
                {
                    var mallPrepayLog = await _mallPrepayDAL.MallPrepayGet(request.TenantLcsPayLogId);
                    if (mallPrepayLog != null)
                    {
                        if (mallPrepayLog.Status == EmMallPrepayStatus.Finish)
                        {
                            LOG.Log.Warn("[在线商城]订单已处理", request, this.GetType());
                            return ResponseBase.CommonError("订单已处理");
                        }
                    }
                    var lcsPaylog = await _tenantLcsPayLogDAL.GetTenantLcsPayLog(request.TenantLcsPayLogId);
                    if (lcsPaylog == null)
                    {
                        return ResponseBase.CommonError("支付记录不存在");
                    }
                    if (lcsPaylog.Status == EmLcsPayLogStatus.PaySuccess)
                    {
                        LOG.Log.Warn("[在线商城]订单已处理1", request, this.GetType());
                        return ResponseBase.CommonError("订单已处理");
                    }
                    await _mallPrepayDAL.UpdateMallPrepayStatus(request.TenantLcsPayLogId, EmMallPrepayStatus.Finish);
                    return await ProcessParentBuyMallGoodsSubmit(request);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error("[在线商城]订单处理失败", request, ex, this.GetType());
                    return ResponseBase.CommonError("订单处理失败");
                }
                finally
                {
                    _distributedLockDAL.LockRelease(lockKey);
                }
            }
            LOG.Log.Warn("[在线商城]提交已处理的订单", request, this.GetType());
            return ResponseBase.CommonError("请勿重复提交订单");
        }

        public async Task<ResponseBase> ParentBuyMallGoodsSubmit(ParentBuyMallGoodsSubmitRequest request)
        {
            return await HandleParentBuyMallGoodsSubmit(request);
        }

        private async Task OnlyHandleLcsPayLogPaySuccessful(ParentBuyMallGoodsPaySuccessEvent request)
        {
            var lcsPaylog = await _tenantLcsPayLogDAL.GetTenantLcsPayLog(request.LcsPayLogId);
            if (lcsPaylog == null)
            {
                return;
            }

            var now = request.Now;
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
        }

        public async Task ParentBuyMallGoodsPaySuccessConsumerEvent(ParentBuyMallGoodsPaySuccessEvent request)
        {
            var mallPrepayBucket = await _mallPrepayDAL.MallPrepayGetBucket(request.LcsPayLogId);
            if (mallPrepayBucket == null || mallPrepayBucket.MallCartView == null)
            {
                LOG.Log.Fatal("[扫呗支付成功回调]预处理请求未找到", request, this.GetType());
                await OnlyHandleLcsPayLogPaySuccessful(request);
                return;
            }
            var mallPrepay = mallPrepayBucket.MallCartView;
            if (mallPrepay.Status == EmMallPrepayStatus.Finish)
            {
                LOG.Log.Warn("[扫呗支付成功回调]无法处理已完成的支付成功请求", request, this.GetType());
                return;
            }
            await HandleParentBuyMallGoodsSubmit(mallPrepay.Request);
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

        public async Task<ResponseBase> TeacherEvaluateGetPaging(TeacherEvaluateGetPagingRequest request)
        {
            var pagingData = await _classRecordEvaluateDAL.GetEvaluateStudentPaging(request);
            var output = new List<TeacherEvaluateGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var tempBoxStudent = new DataTempBox<EtStudent>();
                var tempBoxClass = new DataTempBox<EtClass>();
                var tempBoxTeacher = new DataTempBox<EtUser>();
                foreach (var p in pagingData.Item1)
                {
                    var myStudent = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId);
                    if (myStudent == null)
                    {
                        continue;
                    }
                    var myUser = await ComBusiness.GetUser(tempBoxTeacher, _userDAL, p.TeacherId);
                    var myClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, p.ClassId);
                    output.Add(new TeacherEvaluateGetPagingOutput()
                    {
                        Ot = p.Ot,
                        TeacherId = p.TeacherId,
                        ClassName = myClass?.Name,
                        ClassOt = p.ClassOt.EtmsToDateString(),
                        ClassId = p.ClassId,
                        StartTime = EtmsHelper.GetTimeDesc(p.StartTime),
                        EndTime = EtmsHelper.GetTimeDesc(p.EndTime),
                        EvaluateContent = p.EvaluateContent,
                        Evaluates = EtmsHelper2.GetMediasUrl(p.EvaluateImg),
                        StudentId = p.StudentId,
                        StudentName = myStudent.Name,
                        Week = p.Week,
                        TeacherName = ComBusiness2.GetParentTeacherName(myUser),
                        Id = p.Id
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<TeacherEvaluateGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> AlbumGetPaging(AlbumGetPagingRequest request)
        {
            var pagingData = await _electronicAlbumDetailDAL.GetPaging(request);
            var output = new List<AlbumGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                foreach (var p in pagingData.Item1)
                {
                    output.Add(new AlbumGetPagingOutput()
                    {
                        CId = p.Id,
                        CoverUrl = AliyunOssUtil.GetAccessUrlHttps(p.CoverKey),
                        Name = p.Name,
                        StudentId = p.StudentId
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<AlbumGetPagingOutput>(pagingData.Item2, output));
        }
    }
}
