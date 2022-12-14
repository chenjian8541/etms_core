using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Student.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IEventProvider;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class StudentTransferCourses : IStudentTransferCourses
    {
        private readonly IStudentDAL _studentDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IGoodsDAL _goodsDAL;

        private readonly ICostDAL _costDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IOrderDAL _orderDAL;

        private readonly IStudentPointsLogDAL _studentPointsLog;

        private readonly IIncomeLogDAL _incomeLogDAL;

        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IClassDAL _classDAL;

        private readonly IStudentPointsLogDAL _studentPointsLogDAL;

        private readonly IStudentAccountRechargeDAL _studentAccountRechargeDAL;

        private readonly IStudentAccountRechargeLogDAL _studentAccountRechargeLogDAL;

        private readonly IStudentAccountRechargeCoreBLL _studentAccountRechargeCoreBLL;

        public StudentTransferCourses(IStudentDAL studentDAL, ICourseDAL courseDAL, IGoodsDAL goodsDAL, ICostDAL costDAL,
            IEventPublisher eventPublisher, IOrderDAL orderDAL, IStudentPointsLogDAL studentPointsLog, IIncomeLogDAL incomeLogDAL,
            IStudentCourseDAL studentCourseDAL, IUserOperationLogDAL userOperationLogDAL, IClassDAL classDAL,
            IStudentPointsLogDAL studentPointsLogDAL, IStudentAccountRechargeDAL studentAccountRechargeDAL,
           IStudentAccountRechargeLogDAL studentAccountRechargeLogDAL, IStudentAccountRechargeCoreBLL studentAccountRechargeCoreBLL)
        {
            this._studentDAL = studentDAL;
            this._courseDAL = courseDAL;
            this._goodsDAL = goodsDAL;
            this._costDAL = costDAL;
            this._eventPublisher = eventPublisher;
            this._orderDAL = orderDAL;
            this._studentPointsLog = studentPointsLog;
            this._incomeLogDAL = incomeLogDAL;
            this._studentCourseDAL = studentCourseDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._classDAL = classDAL;
            this._studentPointsLogDAL = studentPointsLogDAL;
            this._studentAccountRechargeDAL = studentAccountRechargeDAL;
            this._studentAccountRechargeLogDAL = studentAccountRechargeLogDAL;
            this._studentAccountRechargeCoreBLL = studentAccountRechargeCoreBLL;
        }

        public void InitTenantId(int tenantId)
        {
            this._studentAccountRechargeCoreBLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _studentDAL, _courseDAL, _goodsDAL, _costDAL, _incomeLogDAL, _studentCourseDAL, _userOperationLogDAL,
                _orderDAL, _studentPointsLog, _classDAL, _studentPointsLogDAL, _studentAccountRechargeDAL, _studentAccountRechargeLogDAL);
        }

        private EtOrderDetail GetTransferOrderDetailOut(EtOrderDetail sourceOrderDetail, TransferCoursesOut productItem,
            string newNo, DateTime now, long sourceOrderId, string sourceOrderNo, long studentId, int orderType)
        {
            var buyUnit = sourceOrderDetail.BugUnit;
            if (buyUnit == EmCourseUnit.Month)
            {
                buyUnit = EmCourseUnit.Day;
            }
            var buyQuantity = Convert.ToInt32(productItem.ReturnCount);
            if (sourceOrderDetail.ProductType == EmProductType.Course && sourceOrderDetail.BugUnit != EmCourseUnit.ClassTimes)
            {
                buyQuantity = buyQuantity / 30;
            }
            var price = Math.Round(productItem.ReturnSum / productItem.ReturnCount, 2);
            return new EtOrderDetail()
            {
                BugUnit = buyUnit,
                BuyQuantity = -buyQuantity,
                OutQuantity = productItem.ReturnCount,
                DiscountType = EmDiscountType.Nothing,
                DiscountValue = 0,
                GiveQuantity = 0,
                GiveUnit = sourceOrderDetail.GiveUnit,
                InOutType = EmOrderInOutType.Out,
                IsDeleted = EmIsDeleted.Normal,
                ItemAptSum = -productItem.ReturnSum,
                ItemSum = -productItem.ReturnSum,
                OrderId = 0,
                OrderNo = newNo,
                Ot = now,
                Price = -price,
                PriceRule = string.Empty,
                ProductId = sourceOrderDetail.ProductId,
                ProductType = sourceOrderDetail.ProductType,
                Remark = string.Empty,
                Status = EmOrderStatus.Normal,
                TenantId = sourceOrderDetail.TenantId,
                UserId = sourceOrderDetail.UserId,
                OutOrderId = sourceOrderId,
                OutOrderNo = sourceOrderNo,
                StudentId = studentId,
                OrderType = orderType
            };
        }

        public async Task<ResponseBase> TransferCourses(TransferCoursesRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }

            EtStudentAccountRecharge myAccountRecharge = null;
            var isPayAccountRecharge = false;
            var payAccountRechargeReal = 0M;
            var payAccountRechargeGive = 0M;
            if (request.TransferCoursesOrderInfo.PaySum > 0)  //验证充值账户支付/支出
            {
                if (request.TransferCoursesOrderInfo.InOutType == EmOrderInOutType.In)
                {
                    if (request.TransferCoursesOrderInfo.InPayInfo.PayAccountRechargeId != null &&
                        (request.TransferCoursesOrderInfo.InPayInfo.PayAccountRechargeGive > 0
                        || request.TransferCoursesOrderInfo.InPayInfo.PayAccountRechargeReal > 0))
                    {
                        myAccountRecharge = await _studentAccountRechargeDAL.GetStudentAccountRecharge(request.TransferCoursesOrderInfo.InPayInfo.PayAccountRechargeId.Value);
                        if (myAccountRecharge == null)
                        {
                            return ResponseBase.CommonError("充值账户不存在");
                        }
                        if (request.TransferCoursesOrderInfo.InPayInfo.PayAccountRechargeReal > 0
                            && myAccountRecharge.BalanceReal < request.TransferCoursesOrderInfo.InPayInfo.PayAccountRechargeReal)
                        {
                            return ResponseBase.CommonError("充值账户实充余额不足");
                        }
                        if (request.TransferCoursesOrderInfo.InPayInfo.PayAccountRechargeGive > 0
                            && myAccountRecharge.BalanceGive < request.TransferCoursesOrderInfo.InPayInfo.PayAccountRechargeGive)
                        {
                            return ResponseBase.CommonError("充值账户赠送余额不足");
                        }
                        isPayAccountRecharge = true;
                        payAccountRechargeReal = request.TransferCoursesOrderInfo.InPayInfo.PayAccountRechargeReal;
                        payAccountRechargeGive = request.TransferCoursesOrderInfo.InPayInfo.PayAccountRechargeGive;
                    }
                }
                else
                {
                    if (request.TransferCoursesOrderInfo.OutPayInfo.PayStudentAccountRechargeId != null &&
                        request.TransferCoursesOrderInfo.OutPayInfo.PayType == EmPayType.PayAccountRecharge)
                    {
                        myAccountRecharge = await _studentAccountRechargeDAL.GetStudentAccountRecharge(request.TransferCoursesOrderInfo.OutPayInfo.PayStudentAccountRechargeId.Value);
                        if (myAccountRecharge == null)
                        {
                            return ResponseBase.CommonError("充值账户不存在");
                        }
                        isPayAccountRecharge = true;
                        payAccountRechargeReal = request.TransferCoursesOrderInfo.PaySum;
                        payAccountRechargeGive = 0;
                    }
                }
            }

            string newOrderNo;
            if (string.IsNullOrEmpty(request.OrderNo))
            {
                newOrderNo = OrderNumberLib.GetTransferCoursesOrderNumber();
            }
            else
            {
                newOrderNo = request.OrderNo;
            }

            var now = DateTime.Now;
            var processTransferCoursesBuyResult = await ProcessTransferCoursesBuy(request, studentBucket.Student, newOrderNo);
            if (!processTransferCoursesBuyResult.IsResponseSuccess())
            {
                return processTransferCoursesBuyResult;
            }

            var processTransferCoursesOutResult = await ProcessTransferCoursesOut(request, newOrderNo, now);
            if (!processTransferCoursesOutResult.IsResponseSuccess())
            {
                return processTransferCoursesOutResult;
            }

            var processTransferCoursesBuyRes = (ProcessTransferCoursesBuyRes)processTransferCoursesBuyResult.resultData;
            var processTransferCoursesOutRes = (ProcessTransferCoursesOutRes)processTransferCoursesOutResult.resultData;

            var opContent = $"转出课程：{processTransferCoursesOutRes.OutCourseDesc}<br>转入课程：{processTransferCoursesBuyRes.BuyCourse}";
            var allChangedCourseIds = processTransferCoursesBuyRes.ChangeCourseIds;
            allChangedCourseIds.AddRange(processTransferCoursesOutRes.ChangeCourseIds);
            var transferOrder = new EtOrder()
            {
                InOutType = request.TransferCoursesOrderInfo.InOutType,
                AptSum = request.TransferCoursesOrderInfo.PaySum,
                PaySum = request.TransferCoursesOrderInfo.PaySum,
                ArrearsSum = 0,
                BuyCost = string.Empty,
                BuyGoods = string.Empty,
                BuyCourse = opContent,
                CommissionUser = EtmsHelper.GetMuIds(request.TransferCoursesOrderInfo.CommissionUser),
                CouponsIds = string.Empty,
                CouponsStudentGetIds = string.Empty,
                CreateOt = now,
                IsDeleted = EmIsDeleted.Normal,
                No = newOrderNo,
                OrderType = EmOrderType.TransferCourse,
                Ot = request.TransferCoursesOrderInfo.Ot,
                Remark = request.TransferCoursesOrderInfo.Remark,
                Status = EmOrderStatus.Normal,
                StudentId = request.StudentId,
                Sum = request.TransferCoursesOrderInfo.PaySum,
                TenantId = request.LoginTenantId,
                TotalPoints = request.TransferCoursesOrderInfo.ChangePoint,
                UserId = request.LoginUserId,
                UnionTransferOrderIds = EtmsHelper.GetMuIds(processTransferCoursesOutRes.SourceOrderIds)
            };
            if (isPayAccountRecharge)
            {
                transferOrder.PayAccountRechargeReal = payAccountRechargeReal;
                transferOrder.PayAccountRechargeGive = payAccountRechargeGive;
                transferOrder.PayAccountRechargeId = myAccountRecharge.Id;
            }

            var orderDetail = processTransferCoursesOutRes.NewOrderDetailList;
            orderDetail.AddRange(processTransferCoursesBuyRes.OrderDetails);
            var orderId = await _orderDAL.AddOrder(transferOrder, orderDetail);

            await _orderDAL.SetOrderHasIsTransferCourse(processTransferCoursesOutRes.SourceOrderIds);

            if (isPayAccountRecharge)
            {
                //处理充值账户支付(支出)
                await ProcessStudentAccountRechargeChange(request, myAccountRecharge, orderId, newOrderNo, now, request.TransferCoursesOrderInfo.PaySum);
            }

            if (request.TransferCoursesOrderInfo.PaySum > 0)
            {
                //处理收支记录
                await ProcessIncomeLog(request, orderId, newOrderNo, now, transferOrder.Ot);
            }

            _eventPublisher.Publish(new StudentTransferCoursesEvent(request.LoginTenantId)
            {
                StudentCourseDetails = processTransferCoursesBuyRes.StudentCourseDetails,
                OneToOneClassList = processTransferCoursesBuyRes.OneToOneClassLst,
                TransferOrder = transferOrder,
                Request = request,
                UserId = request.LoginUserId,
                ChangeCourseIds = allChangedCourseIds
            });

            //_eventPublisher.Publish(new NoticeStudentCourseSurplusEvent(request.LoginTenantId)
            //{
            //    CourseId = request.CourseId,
            //    StudentId = request.StudentId
            //});
            _eventPublisher.Publish(new StatisticsSalesOrderEvent(request.LoginTenantId)
            {
                Order1 = transferOrder,
                OpType = StatisticsSalesOrderOpType.TransferCourse
            });
            return ResponseBase.Success(orderId);
        }

        private async Task ProcessStudentAccountRechargeChange(TransferCoursesRequest request, EtStudentAccountRecharge myAccountRecharge,
            long orderId, string orderNo, DateTime now, decimal paySum)
        {
            if (request.TransferCoursesOrderInfo.InOutType == EmOrderInOutType.In)
            {
                //充值账户支出
                var inPayInfo = request.TransferCoursesOrderInfo.InPayInfo;
                await _studentAccountRechargeCoreBLL.StudentAccountRechargeChange(new StudentAccountRechargeChangeEvent(request.LoginTenantId)
                {
                    AddBalanceReal = -inPayInfo.PayAccountRechargeReal,
                    AddBalanceGive = -inPayInfo.PayAccountRechargeGive,
                    AddRechargeSum = 0,
                    AddRechargeGiveSum = 0,
                    StudentAccountRechargeId = myAccountRecharge.Id,
                    TryCount = 0
                });
                await _studentAccountRechargeLogDAL.AddStudentAccountRechargeLog(new EtStudentAccountRechargeLog()
                {
                    TenantId = myAccountRecharge.TenantId,
                    CgBalanceGive = inPayInfo.PayAccountRechargeGive,
                    CgBalanceReal = inPayInfo.PayAccountRechargeReal,
                    CgNo = orderNo,
                    CgServiceCharge = 0,
                    CommissionUser = string.Empty,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = now,
                    Phone = myAccountRecharge.Phone,
                    Remark = "转课",
                    RelatedOrderId = orderId,
                    Status = EmStudentAccountRechargeLogStatus.Normal,
                    StudentAccountRechargeId = myAccountRecharge.Id,
                    Type = EmStudentAccountRechargeLogType.Pay,
                    UserId = request.LoginUserId
                });
            }
            else
            {
                //退款至充值账户
                await _studentAccountRechargeCoreBLL.StudentAccountRechargeChange(new StudentAccountRechargeChangeEvent(request.LoginTenantId)
                {
                    AddBalanceReal = paySum,
                    AddBalanceGive = 0,
                    AddRechargeSum = 0,
                    AddRechargeGiveSum = 0,
                    StudentAccountRechargeId = myAccountRecharge.Id,
                    TryCount = 0
                });
                await _studentAccountRechargeLogDAL.AddStudentAccountRechargeLog(new EtStudentAccountRechargeLog()
                {
                    StudentAccountRechargeId = myAccountRecharge.Id,
                    CgBalanceGive = 0,
                    CgBalanceReal = paySum,
                    CgNo = orderNo,
                    CgServiceCharge = 0,
                    CommissionUser = string.Empty,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = now,
                    Phone = myAccountRecharge.Phone,
                    RelatedOrderId = orderId,
                    Remark = "转课",
                    Status = EmStudentAccountRechargeLogStatus.Normal,
                    TenantId = myAccountRecharge.TenantId,
                    Type = EmStudentAccountRechargeLogType.OrderReturn,
                    UserId = request.LoginUserId
                });
            }
        }

        private async Task ProcessIncomeLog(TransferCoursesRequest request, long orderId, string orderNo, DateTime now,
            DateTime orderOt)
        {
            if (request.TransferCoursesOrderInfo.InOutType == EmOrderInOutType.In)
            {
                var inPayInfo = request.TransferCoursesOrderInfo.InPayInfo;
                var incomeLogs = new List<EtIncomeLog>();
                if (inPayInfo.PayWechat > 0)
                {
                    incomeLogs.Add(GetEtIncomeLogIn(EmPayType.WeChat, inPayInfo.PayWechat, now, orderOt, orderNo,
                        request.LoginTenantId, request.LoginUserId, request.TransferCoursesOrderInfo.Remark, orderId));
                }
                if (inPayInfo.PayAlipay > 0)
                {
                    incomeLogs.Add(GetEtIncomeLogIn(EmPayType.Alipay, inPayInfo.PayAlipay, now, orderOt, orderNo,
                        request.LoginTenantId, request.LoginUserId, request.TransferCoursesOrderInfo.Remark, orderId));
                }
                if (inPayInfo.PayCash > 0)
                {
                    incomeLogs.Add(GetEtIncomeLogIn(EmPayType.Cash, inPayInfo.PayCash, now, orderOt, orderNo,
                        request.LoginTenantId, request.LoginUserId, request.TransferCoursesOrderInfo.Remark, orderId));
                }
                if (inPayInfo.PayBank > 0)
                {
                    incomeLogs.Add(GetEtIncomeLogIn(EmPayType.Bank, inPayInfo.PayBank, now, orderOt, orderNo,
                        request.LoginTenantId, request.LoginUserId, request.TransferCoursesOrderInfo.Remark, orderId));
                }
                if (inPayInfo.PayPos > 0)
                {
                    incomeLogs.Add(GetEtIncomeLogIn(EmPayType.Pos, inPayInfo.PayPos, now, orderOt, orderNo,
                        request.LoginTenantId, request.LoginUserId, request.TransferCoursesOrderInfo.Remark, orderId));
                }
                if (inPayInfo.PayOther > 0)
                {
                    incomeLogs.Add(GetEtIncomeLogIn(EmPayType.Other, inPayInfo.PayOther, now, orderOt, orderNo,
                            request.LoginTenantId, request.LoginUserId, request.TransferCoursesOrderInfo.Remark, orderId));
                }
                if (inPayInfo.PayLcsBarcodePay > 0)
                {
                    incomeLogs.Add(GetEtIncomeLogIn(EmPayType.AgtPay, inPayInfo.PayLcsBarcodePay, now, orderOt, orderNo,
                        request.LoginTenantId, request.LoginUserId, request.TransferCoursesOrderInfo.Remark, orderId));
                }

                _incomeLogDAL.AddIncomeLog(incomeLogs);
            }
            else
            {
                var outPayInfo = request.TransferCoursesOrderInfo.OutPayInfo;
                if (outPayInfo.PayType != EmPayType.PayAccountRecharge)
                {
                    await _incomeLogDAL.AddIncomeLog(new EtIncomeLog()
                    {
                        AccountNo = string.Empty,
                        CreateOt = now,
                        IsDeleted = EmIsDeleted.Normal,
                        No = orderNo,
                        OrderId = orderId,
                        Ot = orderOt,
                        PayType = outPayInfo.PayType,
                        ProjectType = EmIncomeLogProjectType.TransferCourse,
                        Remark = request.TransferCoursesOrderInfo.Remark,
                        RepealOt = null,
                        RepealUserId = null,
                        Status = EmIncomeLogStatus.Normal,
                        Sum = request.TransferCoursesOrderInfo.PaySum,
                        TenantId = request.LoginTenantId,
                        UserId = request.LoginUserId,
                        Type = EmIncomeLogType.AccountOut
                    });
                }
            }
        }

        private EtIncomeLog GetEtIncomeLogIn(byte payType, decimal payValue, DateTime createTime, DateTime ot, string no,
            int tenantId, long userId, string remark, long orderId)
        {
            return new EtIncomeLog()
            {
                AccountNo = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                No = no,
                Ot = ot,
                PayType = payType,
                ProjectType = EmIncomeLogProjectType.TransferCourse,
                Remark = remark,
                RepealOt = null,
                OrderId = orderId,
                RepealUserId = null,
                Status = EmIncomeLogStatus.Normal,
                Sum = payValue,
                TenantId = tenantId,
                Type = EmIncomeLogType.AccountIn,
                UserId = userId,
                CreateOt = createTime
            };
        }

        private async Task<ResponseBase> ProcessTransferCoursesBuy(TransferCoursesRequest request,
            EtStudent student, string no)
        {
            var output = new ProcessTransferCoursesBuyRes()
            {
                BuyCourse = new StringBuilder(),
                OneToOneClassLst = new List<OneToOneClass>(),
                OrderDetails = new List<EtOrderDetail>(),
                StudentCourseDetails = new List<EtStudentCourseDetail>(),
                ChangeCourseIds = new List<long>()
            };
            var hisCourseDetail = await _studentCourseDAL.GetStudentCourseDetail(student.Id);
            //课程
            if (request.TransferCoursesBuy != null && request.TransferCoursesBuy.Any())
            {
                byte buyType = EmOrderBuyType.New;
                foreach (var p in request.TransferCoursesBuy)
                {
                    buyType = EmOrderBuyType.New;
                    var course = await _courseDAL.GetCourse(p.CourseId);
                    if (course == null || course.Item1 == null || course.Item2 == null)
                    {
                        return ResponseBase.CommonError("课程不存在");
                    }
                    var priceRule = course.Item2.FirstOrDefault(j => j.Id == p.CoursePriceRuleId);
                    if (priceRule == null)
                    {
                        return ResponseBase.CommonError($"课程[{course.Item1.Name}]定价标准不存在");
                    }
                    if (course.Item1.Type == EmCourseType.OneToOne)
                    {
                        output.OneToOneClassLst.Add(ComBusiness2.GetOneToOneClass(course.Item1, student));
                    }
                    output.StudentCourseDetails.Add(ComBusiness2.GetStudentCourseDetail(course.Item1, priceRule, p, no, request.StudentId, request.LoginTenantId));
                    if (hisCourseDetail != null && hisCourseDetail.Count > 0)
                    {
                        var thisCourse = hisCourseDetail.FirstOrDefault(a => a.CourseId == p.CourseId);
                        if (thisCourse != null)
                        {
                            buyType = EmOrderBuyType.Renew;
                        }
                        else
                        {
                            buyType = EmOrderBuyType.Expand;
                        }
                    }
                    var orderCourseDetailResult = ComBusiness2.GetCourseOrderDetail(course.Item1, priceRule, p, no, request.TransferCoursesOrderInfo.Ot, request.LoginUserId, request.LoginTenantId,
                        buyType, request.StudentId, EmOrderType.TransferCourse);
                    output.OrderDetails.Add(orderCourseDetailResult.Item1);
                    var desc = ComBusiness2.GetBuyCourseDesc(course.Item1.Name, priceRule.PriceUnit, p.BuyQuantity, p.GiveQuantity, p.GiveUnit);
                    output.BuyCourse.Append($"{desc}；");
                    output.ChangeCourseIds.Add(p.CourseId);
                }
            }
            return ResponseBase.Success(output);
        }

        private async Task<ResponseBase> ProcessTransferCoursesOut(TransferCoursesRequest request, string newOrderNo, DateTime now)
        {
            var outCourseBucket = await _courseDAL.GetCourse(request.CourseId);
            if (outCourseBucket == null || outCourseBucket.Item1 == null)
            {
                return ResponseBase.CommonError("课程信息不存在");
            }
            var myOutCourse = outCourseBucket.Item1;
            var sourceStudentCourseDetail = await _studentCourseDAL.GetStudentCourseDetail(request.StudentId, request.CourseId);
            var sourceOrderDetailUpdateEntitys = new List<EtOrderDetail>();
            var sourceStudentCourseDetailUpdateEntitys = new List<EtStudentCourseDetail>();
            var newOrderDetailList = new List<EtOrderDetail>();
            var newOrderOperationLogs = new List<EtOrderOperationLog>();
            var sourceOrderIds = new List<long>();
            var monthToDay = SystemConfig.ComConfig.MonthToDay;
            var desc = new StringBuilder();
            var changeCourseIds = new List<long>();
            foreach (var outOrderDetail in request.TransferCoursesOut)
            {
                //处理原订单和学员剩余课程,创建订单详情
                var mySourceOrderDetail = await _orderDAL.GetOrderDetailById(outOrderDetail.OrderDetailId);
                if (mySourceOrderDetail == null)
                {
                    LOG.Log.Warn("[TransferCourses]课程数据错误", request, this.GetType());
                    return ResponseBase.CommonError("请求数据错误，请重新再试");
                }
                var mySourceStudentCourseDetail = sourceStudentCourseDetail.FirstOrDefault(p => p.OrderId == mySourceOrderDetail.OrderId);
                if (mySourceStudentCourseDetail == null)
                {
                    LOG.Log.Warn("[TransferCourses]课程数据错误", request, this.GetType());
                    return ResponseBase.CommonError("请求数据错误，请重新再试");
                }

                newOrderDetailList.Add(GetTransferOrderDetailOut(mySourceOrderDetail, outOrderDetail, newOrderNo, now, mySourceOrderDetail.OrderId, mySourceOrderDetail.OrderNo,
                    request.StudentId, EmOrderType.TransferCourse));
                var returnCountDesc = outOrderDetail.ReturnCount.EtmsToString();
                newOrderOperationLogs.Add(new EtOrderOperationLog()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    OpType = EmOrderOperationLogType.TransferCourses,
                    OpContent = $"转出课程:{myOutCourse.Name},转出{returnCountDesc}{EmCourseUnit.GetCourseUnitDesc(mySourceStudentCourseDetail.UseUnit)}",
                    OrderId = mySourceOrderDetail.OrderId,
                    OrderNo = mySourceOrderDetail.OrderNo,
                    Ot = now,
                    Remark = request.TransferCoursesOrderInfo.Remark,
                    TenantId = request.LoginTenantId,
                    UserId = request.LoginUserId
                });
                sourceOrderIds.Add(mySourceOrderDetail.OrderId);
                desc.Append($"{returnCountDesc}{EmCourseUnit.GetCourseUnitDesc(mySourceStudentCourseDetail.UseUnit)}");

                if (outOrderDetail.IsAllReturn)
                {
                    mySourceStudentCourseDetail.UseQuantity += outOrderDetail.ReturnCount;
                    mySourceStudentCourseDetail.SurplusQuantity = 0;
                    mySourceStudentCourseDetail.SurplusSmallQuantity = 0;
                    mySourceStudentCourseDetail.Status = EmStudentCourseStatus.EndOfClass;
                    mySourceStudentCourseDetail.EndCourseRemark = "转出课程-结课";
                    mySourceStudentCourseDetail.EndCourseTime = now;
                    mySourceStudentCourseDetail.EndCourseUser = request.LoginUserId;
                }
                else
                {
                    if (mySourceStudentCourseDetail.DeType == EmDeClassTimesType.ClassTimes)
                    {
                        if (mySourceStudentCourseDetail.SurplusQuantity < outOrderDetail.ReturnCount)
                        {
                            return ResponseBase.CommonError($"订单[{outOrderDetail.OrderNo}]剩余课时不足");
                        }
                        mySourceStudentCourseDetail.UseQuantity += outOrderDetail.ReturnCount;
                        mySourceStudentCourseDetail.SurplusQuantity -= outOrderDetail.ReturnCount;
                    }
                    else
                    {
                        //按天
                        var deDay = (int)outOrderDetail.ReturnCount;
                        if (mySourceStudentCourseDetail.StartTime != null && mySourceStudentCourseDetail.EndTime != null)
                        {
                            mySourceStudentCourseDetail.EndTime = mySourceStudentCourseDetail.EndTime.Value.AddDays(-deDay);
                            DateTime firstDate;
                            if (mySourceStudentCourseDetail.StartTime.Value <= now.Date)
                            {
                                firstDate = now.Date;
                            }
                            else
                            {
                                firstDate = mySourceStudentCourseDetail.StartTime.Value;
                            }

                            var dffTime = EtmsHelper.GetDffTimeAboutSurplusQuantity(firstDate, mySourceStudentCourseDetail.EndTime.Value);
                            mySourceStudentCourseDetail.SurplusQuantity = dffTime.Item1;
                            mySourceStudentCourseDetail.SurplusSmallQuantity = dffTime.Item2;
                            mySourceStudentCourseDetail.UseQuantity += deDay;
                        }
                        else
                        {
                            var tatalDay = (int)(mySourceStudentCourseDetail.SurplusQuantity * monthToDay + mySourceStudentCourseDetail.SurplusSmallQuantity); //剩余总天数
                            tatalDay = tatalDay - deDay;
                            if (tatalDay < 0)
                            {
                                mySourceStudentCourseDetail.UseQuantity += outOrderDetail.ReturnCount;
                                mySourceStudentCourseDetail.SurplusQuantity = 0;
                                mySourceStudentCourseDetail.SurplusSmallQuantity = 0;
                                mySourceStudentCourseDetail.Status = EmStudentCourseStatus.EndOfClass;
                                mySourceStudentCourseDetail.EndCourseRemark = "转出课程-结课";
                                mySourceStudentCourseDetail.EndCourseTime = now;
                                mySourceStudentCourseDetail.EndCourseUser = request.LoginUserId;
                            }
                            else
                            {
                                var month = tatalDay / monthToDay;
                                var day = tatalDay % monthToDay;
                                mySourceStudentCourseDetail.UseQuantity += outOrderDetail.ReturnCount;
                                mySourceStudentCourseDetail.SurplusQuantity = month;
                                mySourceStudentCourseDetail.SurplusSmallQuantity = day;
                            }
                        }
                    }
                }
                sourceStudentCourseDetailUpdateEntitys.Add(mySourceStudentCourseDetail);

                mySourceOrderDetail.OutQuantity += (int)outOrderDetail.ReturnCount;
                sourceOrderDetailUpdateEntitys.Add(mySourceOrderDetail);
                changeCourseIds.Add(mySourceStudentCourseDetail.CourseId);
            }
            if (sourceOrderDetailUpdateEntitys.Count > 0)
            {
                await _orderDAL.EditOrderDetail(sourceOrderDetailUpdateEntitys);
            }
            if (sourceStudentCourseDetailUpdateEntitys.Count > 0)
            {
                await _studentCourseDAL.UpdateStudentCourseDetail(sourceStudentCourseDetailUpdateEntitys);
            }
            if (newOrderOperationLogs.Count > 0)
            {
                _orderDAL.AddOrderOperationLog(newOrderOperationLogs);
            }
            return ResponseBase.Success(new ProcessTransferCoursesOutRes()
            {
                NewOrderDetailList = newOrderDetailList,
                OutCourseDesc = $"{myOutCourse.Name}({desc})",
                SourceOrderIds = sourceOrderIds,
                ChangeCourseIds = changeCourseIds
            });
        }

        public async Task StudentTransferCoursesConsumerEvent(StudentTransferCoursesEvent request)
        {
            var orderId = request.TransferOrder.Id;
            //学员课程信息
            if (request.StudentCourseDetails != null && request.StudentCourseDetails.Count > 0)
            {
                foreach (var studentCourse in request.StudentCourseDetails)
                {
                    studentCourse.OrderId = orderId;
                }
                _studentCourseDAL.AddStudentCourseDetail(request.StudentCourseDetails);
                await _studentCourseDAL.ResetStudentCourseNotEnoughRemindInfo(request.TransferOrder.StudentId, request.StudentCourseDetails.Select(p => p.CourseId).ToList());
            }
            _eventPublisher.Publish(new StudentCourseAnalyzeEvent(request.TenantId)
            {
                StudentId = request.TransferOrder.StudentId
            });

            //一对一课程
            if (request.OneToOneClassList != null && request.OneToOneClassList.Any())
            {
                foreach (var myClass in request.OneToOneClassList)
                {
                    var logClass = await _classDAL.GetStudentOneToOneClassNormal(request.TransferOrder.StudentId, myClass.CourseId);
                    if (logClass.Any())
                    {
                        LOG.Log.Info($"[StudentTransferCoursesConsumerEvent]学员存在课程对应的一对一班级,无需创建", this.GetType());
                        continue;
                    }

                    var classId = await _classDAL.AddClass(new EtClass()
                    {
                        DefaultClassTimes = 1,
                        CourseList = $",{myClass.CourseId},",
                        CompleteTime = null,
                        CompleteStatus = EmClassCompleteStatus.UnComplete,
                        ClassCategoryId = null,
                        ClassRoomIds = null,
                        FinishClassTimes = 0,
                        FinishCount = 0,
                        IsDeleted = EmIsDeleted.Normal,
                        IsLeaveCharge = false,
                        IsNotComeCharge = false,
                        LastJobProcessTime = DateTime.Now,
                        LimitStudentNums = 1,
                        Name = myClass.Name,
                        Ot = request.CreateTime,
                        PlanCount = 0,
                        Remark = string.Empty,
                        ScheduleStatus = EmClassScheduleStatus.Unscheduled,
                        StudentNums = myClass.StudentNums,
                        TeacherNum = 0,
                        Teachers = string.Empty,
                        TenantId = request.TenantId,
                        Type = myClass.Type,
                        UserId = request.Request.LoginUserId,
                        StudentIds = $",{string.Join(',', myClass.Students.Select(p => p.StudentId))},",
                        OrderId = orderId
                    });
                    foreach (var student in myClass.Students)
                    {
                        await _classDAL.AddClassStudent(new EtClassStudent()
                        {
                            ClassId = classId,
                            CourseId = student.CourseId,
                            IsDeleted = EmIsDeleted.Normal,
                            Remark = string.Empty,
                            StudentId = student.StudentId,
                            TenantId = request.TenantId,
                            Type = myClass.Type
                        });
                        _eventPublisher.Publish(new SyncStudentStudentClassIdsEvent(request.TenantId, student.StudentId));
                    }
                }
            }

            if (request.TransferOrder.TotalPoints > 0)
            {
                if (request.TransferOrder.InOutType == EmOrderInOutType.In)
                {
                    await _studentDAL.AddPoint(request.TransferOrder.StudentId, request.TransferOrder.TotalPoints);
                }
                else
                {
                    await _studentDAL.DeductionPoint(request.TransferOrder.StudentId, request.TransferOrder.TotalPoints);
                }
                await _studentPointsLogDAL.AddStudentPointsLog(new EtStudentPointsLog()
                {
                    StudentId = request.TransferOrder.StudentId,
                    IsDeleted = EmIsDeleted.Normal,
                    No = request.TransferOrder.No,
                    Ot = request.TransferOrder.CreateOt,
                    Points = request.TransferOrder.TotalPoints,
                    Remark = request.TransferOrder.Remark,
                    TenantId = request.TenantId,
                    Type = request.TransferOrder.InOutType == EmOrderInOutType.In ? EmStudentPointsLogType.TransferCourseAdd : EmStudentPointsLogType.TransferCourseDeduction
                });
            }

            _eventPublisher.Publish(new StatisticsSalesProductEvent(request.TenantId)
            {
                StatisticsDate = request.TransferOrder.Ot
            });
            _eventPublisher.Publish(new StatisticsFinanceIncomeEvent(request.TenantId)
            {
                StatisticsDate = request.TransferOrder.Ot
            });
            _eventPublisher.Publish(new StatisticsSalesCourseEvent(request.TenantId)
            {
                StatisticsDate = request.TransferOrder.Ot
            });
            _eventPublisher.Publish(new SyncStudentStudentCourseIdsEvent(request.TenantId, request.TransferOrder.StudentId));
            _eventPublisher.Publish(new SysTenantStatisticsWeekAndMonthEvent(request.TenantId));

            await _userOperationLogDAL.AddUserLog(request.Request, $"转课-{request.TransferOrder.BuyCourse}", EmUserOperationType.OrderMgr);
            if (request.ChangeCourseIds.Any())
            {
                foreach (var myCourseId in request.ChangeCourseIds)
                {
                    _eventPublisher.Publish(new NoticeStudentCourseSurplusEvent(request.TenantId)
                    {
                        CourseId = myCourseId,
                        StudentId = request.TransferOrder.StudentId
                    });
                }
            }
        }
    }
}
