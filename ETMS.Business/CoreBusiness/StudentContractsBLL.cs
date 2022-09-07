using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Student.Request;
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
using ETMS.Entity.Temp;
using ETMS.Entity.Dto.Student.Output;
using ETMS.Event.DataContract.Statistics;

namespace ETMS.Business
{
    public class StudentContractsBLL : IStudentContractsBLL
    {
        private readonly IStudentDAL _studentDAL;

        private readonly ICouponsDAL _couponsDAL;

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

        private readonly IStudentAccountRechargeDAL _studentAccountRechargeDAL;

        private readonly IStudentAccountRechargeLogDAL _studentAccountRechargeLogDAL;

        private readonly IStudentAccountRechargeCoreBLL _studentAccountRechargeCoreBLL;

        private readonly ITenantConfigDAL _tenantConfigDAL;
        public StudentContractsBLL(IStudentDAL studentDAL, ICouponsDAL couponsDAL, ICourseDAL courseDAL, IGoodsDAL goodsDAL, ICostDAL costDAL,
            IEventPublisher eventPublisher, IOrderDAL orderDAL, IStudentPointsLogDAL studentPointsLog, IIncomeLogDAL incomeLogDAL,
            IStudentCourseDAL studentCourseDAL, IUserOperationLogDAL userOperationLogDAL, IClassDAL classDAL,
            IStudentAccountRechargeDAL studentAccountRechargeDAL, IStudentAccountRechargeLogDAL studentAccountRechargeLogDAL,
            IStudentAccountRechargeCoreBLL studentAccountRechargeCoreBLL, ITenantConfigDAL tenantConfigDAL)
        {
            this._studentDAL = studentDAL;
            this._couponsDAL = couponsDAL;
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
            this._studentAccountRechargeDAL = studentAccountRechargeDAL;
            this._studentAccountRechargeLogDAL = studentAccountRechargeLogDAL;
            this._studentAccountRechargeCoreBLL = studentAccountRechargeCoreBLL;
            this._tenantConfigDAL = tenantConfigDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._studentAccountRechargeCoreBLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _studentDAL, _couponsDAL, _courseDAL, _goodsDAL, _costDAL, _incomeLogDAL, _studentCourseDAL, _userOperationLogDAL,
                _orderDAL, _studentPointsLog, _classDAL, _studentAccountRechargeDAL, _studentAccountRechargeLogDAL, _tenantConfigDAL);
        }

        public async Task<ResponseBase> StudentEnrolment(StudentEnrolmentRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            long? payAccountRechargeId = null;
            var payAccountRechargePhone = string.Empty;
            if (request.EnrolmentPayInfo.PayAccountRechargeId != null &&
               (request.EnrolmentPayInfo.PayAccountRechargeReal > 0 || request.EnrolmentPayInfo.PayAccountRechargeGive > 0))
            {
                //验证充值账户抵扣
                var myAccountRecharge = await _studentAccountRechargeDAL.GetStudentAccountRecharge(request.EnrolmentPayInfo.PayAccountRechargeId.Value);
                if (myAccountRecharge == null)
                {
                    return ResponseBase.CommonError("充值账户不存在");
                }
                if (request.EnrolmentPayInfo.PayAccountRechargeReal > 0
                    && myAccountRecharge.BalanceReal < request.EnrolmentPayInfo.PayAccountRechargeReal)
                {
                    return ResponseBase.CommonError("充值账户实充余额不足");
                }
                if (request.EnrolmentPayInfo.PayAccountRechargeGive > 0
                    && myAccountRecharge.BalanceGive < request.EnrolmentPayInfo.PayAccountRechargeGive)
                {
                    return ResponseBase.CommonError("充值账户赠送余额不足");
                }
                payAccountRechargeId = myAccountRecharge.Id;
                payAccountRechargePhone = myAccountRecharge.Phone;
            }

            var student = studentBucket.Student;
            string no;
            if (string.IsNullOrEmpty(request.OrderNo))
            {
                no = OrderNumberLib.EnrolmentOrderNumber();
            }
            else
            {
                no = request.OrderNo;
            }
            var couponsIds = new List<long>();
            //检验优惠券
            if (request.CouponsStudentGetIds != null && request.CouponsStudentGetIds.Any())
            {
                foreach (var couponsStudentGetId in request.CouponsStudentGetIds)
                {
                    var couponsGetLog = await _couponsDAL.CouponsStudentGet(couponsStudentGetId);
                    if (couponsGetLog == null)
                    {
                        return ResponseBase.CommonError("优惠券不存在");
                    }
                    if (couponsGetLog.Status == EmCouponsStudentStatus.Used)
                    {
                        return ResponseBase.CommonError("优惠券已核销");
                    }
                    if (couponsGetLog.LimitUseTime != null && couponsGetLog.LimitUseTime.Value.Date > DateTime.Now)
                    {
                        return ResponseBase.CommonError("此优惠券还未开放使用");
                    }
                    if (couponsGetLog.ExpiredTime != null && DateTime.Now.Date > couponsGetLog.ExpiredTime.Value)
                    {
                        return ResponseBase.CommonError("此优惠券已过期");
                    }
                    var coupons = await _couponsDAL.GetCoupons(couponsGetLog.CouponsId);
                    if (coupons == null)
                    {
                        return ResponseBase.CommonError("优惠券不存在");
                    }
                    couponsIds.Add(coupons.Id);
                }
            }

            decimal sum = 0, aptSum = 0;
            StringBuilder buyCourse = new StringBuilder(), buyGoods = new StringBuilder(), buyCost = new StringBuilder();
            var orderDetails = new List<EtOrderDetail>();
            var studentCourseDetails = new List<EtStudentCourseDetail>();
            var oneToOneClassLst = new List<OneToOneClass>();
            var isHasCourse = false;
            var myCourseDetail = await _studentCourseDAL.GetStudentCourseDetail(request.StudentId);
            byte buyType = EmOrderBuyType.New;

            //课程
            if (request.EnrolmentCourses != null && request.EnrolmentCourses.Any())
            {
                isHasCourse = true;
                foreach (var p in request.EnrolmentCourses)
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
                        oneToOneClassLst.Add(ComBusiness2.GetOneToOneClass(course.Item1, student, p.Teachers));
                    }
                    studentCourseDetails.Add(ComBusiness2.GetStudentCourseDetail(course.Item1, priceRule, p, no, request.StudentId, request.LoginTenantId));
                    if (myCourseDetail != null && myCourseDetail.Count > 0)
                    {
                        var thisCourse = myCourseDetail.FirstOrDefault(a => a.CourseId == p.CourseId);
                        if (thisCourse != null)
                        {
                            buyType = EmOrderBuyType.Renew;
                        }
                        else
                        {
                            buyType = EmOrderBuyType.Expand;
                        }
                    }
                    var orderCourseDetailResult = ComBusiness2.GetCourseOrderDetail(course.Item1, priceRule, p, no, request.OtherInfo.Ot, request.LoginUserId, request.LoginTenantId,
                        buyType, request.StudentId, EmOrderType.StudentEnrolment);
                    orderDetails.Add(orderCourseDetailResult.Item1);
                    var desc = ComBusiness2.GetBuyCourseDesc(course.Item1.Name, priceRule.PriceUnit, p.BuyQuantity, p.GiveQuantity, p.GiveUnit);
                    buyCourse.Append($"{desc}；");
                    sum += orderCourseDetailResult.Item1.ItemSum;
                    aptSum += orderCourseDetailResult.Item1.ItemAptSum;
                }
            }
            //物品
            if (request.EnrolmentGoodss != null && request.EnrolmentGoodss.Any())
            {
                foreach (var p in request.EnrolmentGoodss)
                {
                    var goods = await _goodsDAL.GetGoods(p.GoodsId);
                    if (goods == null)
                    {
                        return ResponseBase.CommonError("物品不存在");
                    }
                    var orderGoodsDetailResult = ComBusiness4.GetGoodsOrderDetail(goods, p, no,
                        request.OtherInfo.Ot, request.LoginTenantId, request.LoginUserId,
                        request.StudentId, EmOrderType.StudentEnrolment);
                    orderDetails.Add(orderGoodsDetailResult.Item1);
                    var desc = ComBusiness2.GetBuyGoodsDesc(goods.Name, p.BuyQuantity);
                    buyGoods.Append($"{desc}；");
                    sum += orderGoodsDetailResult.Item1.ItemSum;
                    aptSum += orderGoodsDetailResult.Item1.ItemAptSum;
                }
            }
            //费用
            if (request.EnrolmentCosts != null && request.EnrolmentCosts.Any())
            {
                foreach (var p in request.EnrolmentCosts)
                {
                    var cost = await _costDAL.GetCost(p.CostId);
                    if (cost == null)
                    {
                        return ResponseBase.CommonError("费用不存在");
                    }
                    var orderCostDetailResult = ComBusiness4.GetCostOrderDetail(cost, p, no,
                        request.OtherInfo.Ot, request.LoginTenantId, request.LoginUserId,
                        request.StudentId, EmOrderType.StudentEnrolment);
                    orderDetails.Add(orderCostDetailResult.Item1);
                    var desc = ComBusiness2.GetBuyCostDesc(cost.Name, p.BuyQuantity);
                    buyCost.Append($"{desc}；");
                    sum += orderCostDetailResult.Item1.ItemSum;
                    aptSum += orderCostDetailResult.Item1.ItemAptSum;
                }
            }

            //支付信息
            decimal paySum = 0;
            var now = DateTime.Now;
            var incomeLogs = new List<EtIncomeLog>();
            if (payAccountRechargeId > 0)
            {
                paySum += request.EnrolmentPayInfo.PayAccountRechargeReal + request.EnrolmentPayInfo.PayAccountRechargeGive;
            }
            if (request.EnrolmentPayInfo.PayWechat > 0)
            {
                paySum += request.EnrolmentPayInfo.PayWechat;
                incomeLogs.Add(GetEtIncomeLog(EmPayType.WeChat, request.EnrolmentPayInfo.PayWechat, now, request.OtherInfo.Ot, no, request));
            }
            if (request.EnrolmentPayInfo.PayAlipay > 0)
            {
                paySum += request.EnrolmentPayInfo.PayAlipay;
                incomeLogs.Add(GetEtIncomeLog(EmPayType.Alipay, request.EnrolmentPayInfo.PayAlipay, now, request.OtherInfo.Ot, no, request));
            }
            if (request.EnrolmentPayInfo.PayCash > 0)
            {
                paySum += request.EnrolmentPayInfo.PayCash;
                incomeLogs.Add(GetEtIncomeLog(EmPayType.Cash, request.EnrolmentPayInfo.PayCash, now, request.OtherInfo.Ot, no, request));
            }
            if (request.EnrolmentPayInfo.PayBank > 0)
            {
                paySum += request.EnrolmentPayInfo.PayBank;
                incomeLogs.Add(GetEtIncomeLog(EmPayType.Bank, request.EnrolmentPayInfo.PayBank, now, request.OtherInfo.Ot, no, request));
            }
            if (request.EnrolmentPayInfo.PayPos > 0)
            {
                paySum += request.EnrolmentPayInfo.PayPos;
                incomeLogs.Add(GetEtIncomeLog(EmPayType.Pos, request.EnrolmentPayInfo.PayPos, now, request.OtherInfo.Ot, no, request));
            }
            if (request.EnrolmentPayInfo.PayOther > 0)
            {
                paySum += request.EnrolmentPayInfo.PayOther;
                incomeLogs.Add(GetEtIncomeLog(EmPayType.Other, request.EnrolmentPayInfo.PayOther, now, request.OtherInfo.Ot, no, request));
            }
            if (request.EnrolmentPayInfo.PayLcsBarcodePay > 0)
            {
                paySum += request.EnrolmentPayInfo.PayLcsBarcodePay;
                incomeLogs.Add(GetEtIncomeLog(EmPayType.AgtPay, request.EnrolmentPayInfo.PayLcsBarcodePay, now, request.OtherInfo.Ot, no, request));
            }

            var arrearsSum = aptSum - paySum;
            var status = EmOrderStatus.Normal;
            if (paySum == 0 && aptSum > 0)
            {
                status = EmOrderStatus.Unpaid;
            }
            else if (aptSum > paySum)
            {
                status = EmOrderStatus.MakeUpMoney;
            }
            var addToAccountRechargeMoney = 0M;
            if (arrearsSum < 0)
            {
                arrearsSum = 0;
                var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
                if (!tenantConfig.TenantOtherConfig.IsAllowStudentOverpayment)
                {
                    return ResponseBase.CommonError("支付金额应该小于等于应付金额");
                }
                if (tenantConfig.TenantOtherConfig.StudentOverpaymentProcessType == EmStudentOverpaymentProcessType.GoStudentAccountRecharge)
                {
                    addToAccountRechargeMoney = paySum - aptSum;
                }
            }
            var order = new EtOrder()
            {
                TenantId = request.LoginTenantId,
                UserId = request.LoginUserId,
                CouponsStudentGetIds = EtmsHelper.GetMuIds(request.CouponsStudentGetIds),
                CouponsIds = EtmsHelper.GetMuIds(couponsIds),
                IsDeleted = EmIsDeleted.Normal,
                Ot = request.OtherInfo.Ot,
                Remark = request.OtherInfo.Remark,
                TotalPoints = request.OtherInfo.TotalPoints,
                No = no,
                StudentId = request.StudentId,
                OrderType = EmOrderType.StudentEnrolment,
                AptSum = aptSum,
                ArrearsSum = arrearsSum,
                BuyCost = EtmsHelper.DescPrefix(buyCost.ToString().TrimEnd('；'), "费用"),
                BuyCourse = EtmsHelper.DescPrefix(buyCourse.ToString().TrimEnd('；'), "课程"),
                BuyGoods = EtmsHelper.DescPrefix(buyGoods.ToString().TrimEnd('；'), "物品"),
                CommissionUser = EtmsHelper.GetMuIds(request.OtherInfo.CommissionUser),
                PaySum = paySum,
                Sum = sum,
                Status = status,
                CreateOt = now,
                PayAccountRechargeGive = request.EnrolmentPayInfo.PayAccountRechargeGive,
                PayAccountRechargeReal = request.EnrolmentPayInfo.PayAccountRechargeReal,
                PayAccountRechargeId = payAccountRechargeId
            };

            if (myCourseDetail != null && myCourseDetail.Count > 0)
            {
                order.BuyType = EmOrderBuyType.Renew;
            }

            //订单
            var orderId = await _orderDAL.AddOrder(order, orderDetails);
            //收支明细
            if (incomeLogs.Any())
            {
                foreach (var incomeLog in incomeLogs)
                {
                    incomeLog.OrderId = orderId;
                }
                _incomeLogDAL.AddIncomeLog(incomeLogs);
            }
            //充值账户
            if (order.PayAccountRechargeId != null)
            {
                await _studentAccountRechargeCoreBLL.StudentAccountRechargeChange(new StudentAccountRechargeChangeEvent(order.TenantId)
                {
                    AddBalanceReal = -order.PayAccountRechargeReal,
                    AddBalanceGive = -order.PayAccountRechargeGive,
                    AddRechargeSum = 0,
                    AddRechargeGiveSum = 0,
                    StudentAccountRechargeId = order.PayAccountRechargeId.Value,
                    TryCount = 0
                });
                await _studentAccountRechargeLogDAL.AddStudentAccountRechargeLog(new EtStudentAccountRechargeLog()
                {
                    TenantId = order.TenantId,
                    CgBalanceGive = order.PayAccountRechargeGive,
                    CgBalanceReal = order.PayAccountRechargeReal,
                    CgNo = order.No,
                    CgServiceCharge = 0,
                    CommissionUser = string.Empty,
                    IsDeleted = order.IsDeleted,
                    Ot = order.CreateOt,
                    Phone = payAccountRechargePhone,
                    Remark = order.Remark,
                    RelatedOrderId = order.Id,
                    Status = EmStudentAccountRechargeLogStatus.Normal,
                    StudentAccountRechargeId = order.PayAccountRechargeId.Value,
                    Type = EmStudentAccountRechargeLogType.Pay,
                    UserId = order.UserId
                });
            }
            //学员课程信息
            if (studentCourseDetails != null && studentCourseDetails.Count > 0)
            {
                foreach (var studentCourse in studentCourseDetails)
                {
                    studentCourse.OrderId = orderId;
                }
                _studentCourseDAL.AddStudentCourseDetail(studentCourseDetails);
                _eventPublisher.Publish(new StudentCourseAnalyzeEvent(order.TenantId)
                {
                    StudentId = order.StudentId
                });
                await _studentCourseDAL.ResetStudentCourseNotEnoughRemindInfo(order.StudentId, studentCourseDetails.Select(p => p.CourseId).ToList());
            }
            //学员
            if (studentBucket.Student.StudentType != EmStudentType.ReadingStudent || order.TotalPoints > 0)
            {
                byte? newStudentType = null;
                if (isHasCourse && studentBucket.Student.StudentType != EmStudentType.ReadingStudent)
                {
                    newStudentType = EmStudentType.ReadingStudent;
                }

                await _studentDAL.StudentEnrolmentEventChangeInfo(order.StudentId, order.TotalPoints, newStudentType);
                _eventPublisher.Publish(new StatisticsStudentEvent(order.TenantId) { OpType = EmStatisticsStudentType.StudentType });
            }

            var studentEnrolmentEvent = new StudentEnrolmentEvent(request.LoginTenantId)
            {
                UserId = request.LoginUserId,
                Order = order,
                OrderDetails = orderDetails,
                StudentCourseDetails = studentCourseDetails,
                IncomeLogs = incomeLogs,
                CreateTime = now,
                OneToOneClassList = oneToOneClassLst,
                CouponsStudentGetIds = request.CouponsStudentGetIds,
                LoginClientType = request.LoginClientType,
                AddToAccountRechargeMoney = addToAccountRechargeMoney
            };

            //异步执行
            _eventPublisher.Publish(studentEnrolmentEvent);

            if (student.RecommendStudentId != null)
            {
                _eventPublisher.Publish(new StudentRecommendRewardEvent(request.LoginTenantId)
                {
                    Student = student,
                    Order = order,
                    Type = StudentRecommendRewardType.Buy
                });
            }

            _eventPublisher.Publish(new StatisticsSalesOrderEvent(request.LoginTenantId)
            {
                Order1 = order,
                OpType = StatisticsSalesOrderOpType.StudentEnrolment
            });

            return ResponseBase.Success(new StudentEnrolmentOutput()
            {
                OrderId = orderId
            });
        }

        private EtIncomeLog GetEtIncomeLog(byte payType, decimal payValue, DateTime createTime, DateTime ot, string no, StudentEnrolmentRequest request)
        {
            return new EtIncomeLog()
            {
                AccountNo = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                No = no,
                Ot = ot,
                PayType = payType,
                ProjectType = EmIncomeLogProjectType.StudentEnrolment,
                Remark = string.Empty,
                RepealOt = null,
                OrderId = null,
                RepealUserId = null,
                Status = EmIncomeLogStatus.Normal,
                Sum = payValue,
                TenantId = request.LoginTenantId,
                Type = EmIncomeLogType.AccountIn,
                UserId = request.LoginUserId,
                CreateOt = createTime
            };
        }

        public async Task StudentEnrolmentEvent(StudentEnrolmentEvent request)
        {
            var orderId = request.Order.Id;

            //积分变动
            if (request.Order.TotalPoints > 0)
            {
                await _studentPointsLog.AddStudentPointsLog(new EtStudentPointsLog()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    No = request.Order.No,
                    Ot = request.CreateTime,
                    Points = request.Order.TotalPoints,
                    Remark = string.Empty,
                    StudentId = request.Order.StudentId,
                    TenantId = request.Order.TenantId,
                    Type = EmStudentPointsLogType.StudentEnrolment
                });
            }

            //物品销售数量和库存变动记录
            var saleGoods = request.OrderDetails.Where(p => p.ProductType == EmProductType.Goods);
            if (saleGoods.Any())
            {
                foreach (var goodsLog in saleGoods)
                {
                    await _goodsDAL.SubtractInventoryAndAddSaleQuantity(goodsLog.ProductId, goodsLog.BuyQuantity);
                    await _goodsDAL.AddGoodsInventoryLog(new EtGoodsInventoryLog()
                    {
                        ChangeQuantity = goodsLog.BuyQuantity,
                        GoodsId = goodsLog.ProductId,
                        IsDeleted = EmIsDeleted.Normal,
                        Ot = request.CreateTime,
                        Prince = goodsLog.Price,
                        Remark = string.Empty,
                        TenantId = request.TenantId,
                        TotalMoney = goodsLog.ItemAptSum,
                        Type = EmGoodsInventoryType.Sale,
                        UserId = request.Order.UserId
                    });
                }
            }

            //费用销售数量
            var saleCost = request.OrderDetails.Where(p => p.ProductType == EmProductType.Cost);
            if (saleCost.Any())
            {
                foreach (var costLog in saleCost)
                {
                    await _costDAL.AddSaleQuantity(costLog.ProductId, costLog.BuyQuantity);
                }
            }

            //优惠券
            if (request.CouponsStudentGetIds != null && request.CouponsStudentGetIds.Any())
            {
                foreach (var couponsStudentGetId in request.CouponsStudentGetIds)
                {
                    var studentCoupons = await _couponsDAL.CouponsStudentGet(couponsStudentGetId);
                    await _couponsDAL.ChangeCouponsStudentGetStatus(studentCoupons.Id, EmCouponsStudentStatus.Used);
                    await _couponsDAL.AddCouponsUseCount(studentCoupons.CouponsId, 1);
                    await _couponsDAL.AddCouponsStudentUse(new EtCouponsStudentUse()
                    {
                        CouponsId = studentCoupons.CouponsId,
                        IsDeleted = EmIsDeleted.Normal,
                        OrderId = orderId,
                        OrderNo = request.Order.No,
                        Ot = DateTime.Now,
                        Remark = "",
                        StudentId = request.Order.StudentId,
                        TenantId = request.Order.TenantId
                    });
                }
            }

            //一对一课程
            if (request.OneToOneClassList != null && request.OneToOneClassList.Any())
            {
                foreach (var myClass in request.OneToOneClassList)
                {
                    var logClass = await _classDAL.GetStudentOneToOneClassNormal(request.Order.StudentId, myClass.CourseId);
                    if (logClass.Any())
                    {
                        LOG.Log.Info($"[StudentEnrolmentEvent]学员存在课程对应的一对一班级,无需创建", this.GetType());
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
                        TeacherNum = myClass.TeacherNum,
                        Teachers = myClass.Teachers,
                        TenantId = request.TenantId,
                        Type = myClass.Type,
                        UserId = request.Order.UserId,
                        StudentIds = $",{string.Join(',', myClass.Students.Select(p => p.StudentId))},",
                        OrderId = orderId,
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
                        _eventPublisher.Publish(new SyncStudentClassInfoEvent(request.TenantId)
                        {
                            StudentId = student.StudentId
                        });
                    }
                }
            }
            _eventPublisher.Publish(new StatisticsSalesProductEvent(request.TenantId)
            {
                StatisticsDate = request.Order.Ot
            });
            _eventPublisher.Publish(new StatisticsFinanceIncomeEvent(request.TenantId)
            {
                StatisticsDate = request.Order.Ot
            });
            _eventPublisher.Publish(new StatisticsSalesCourseEvent(request.TenantId)
            {
                StatisticsDate = request.Order.Ot
            });
            _eventPublisher.Publish(new NoticeStudentContractsEvent(request.TenantId)
            {
                Order = request.Order,
                OrderDetails = request.OrderDetails
            });
            _eventPublisher.Publish(new ResetTenantToDoThingEvent(request.TenantId));
            _eventPublisher.Publish(new SyncStudentStudentCourseIdsEvent(request.TenantId, request.Order.StudentId));
            _eventPublisher.Publish(new SysTenantStatisticsWeekAndMonthEvent(request.TenantId));
            //日志
            var studentBucket = await _studentDAL.GetStudent(request.Order.StudentId);
            var mystudent = studentBucket.Student;

            var opContent = new StringBuilder();
            opContent.Append($"学员：{mystudent.Name}");
            if (!string.IsNullOrEmpty(request.Order.BuyCourse))
            {
                opContent.Append($"<br>课程：{request.Order.BuyCourse}；");
            }
            if (!string.IsNullOrEmpty(request.Order.BuyGoods))
            {
                opContent.Append($"<br>物品：{request.Order.BuyGoods}；");
            }
            if (!string.IsNullOrEmpty(request.Order.BuyCost))
            {
                opContent.Append($"<br>费用：{request.Order.BuyCost}；");
            }

            if (!request.IsMallOrder)
            {
                await _userOperationLogDAL.AddUserLog(new EtUserOperationLog()
                {
                    IpAddress = string.Empty,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = request.CreateTime,
                    Remark = string.Empty,
                    TenantId = request.TenantId,
                    UserId = request.UserId,
                    Type = (int)EmUserOperationType.StudentEnrolment,
                    OpContent = opContent.ToString(),
                    ClientType = request.LoginClientType
                });
            }
            if (request.AddToAccountRechargeMoney > 0)
            {
                var paytype = EmPayType.Cash;
                if (request.IncomeLogs != null && request.IncomeLogs.Any())
                {
                    paytype = request.IncomeLogs.First().PayType;
                }
                _eventPublisher.Publish(new StudentAutoAddAccountRechargeEvent(request.TenantId)
                {
                    StudentId = request.Order.StudentId,
                    AddMoney = request.AddToAccountRechargeMoney,
                    RechargeLogType = EmStudentAccountRechargeLogType.StudentContractsOverpayment,
                    PayType = paytype,
                    UserId = request.Order.UserId,
                    OrderNo = request.Order.No,
                    OrderId = request.Order.Id,
                    Remark = request.Order.Remark
                });
            }
        }
    }
}
