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

        public StudentContractsBLL(IStudentDAL studentDAL, ICouponsDAL couponsDAL, ICourseDAL courseDAL, IGoodsDAL goodsDAL, ICostDAL costDAL,
            IEventPublisher eventPublisher, IOrderDAL orderDAL, IStudentPointsLogDAL studentPointsLog, IIncomeLogDAL incomeLogDAL,
            IStudentCourseDAL studentCourseDAL, IUserOperationLogDAL userOperationLogDAL, IClassDAL classDAL)
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
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentDAL, _couponsDAL, _courseDAL, _goodsDAL, _costDAL, _incomeLogDAL, _studentCourseDAL, _userOperationLogDAL,
                _orderDAL, _studentPointsLog, _classDAL);
        }

        public async Task<ResponseBase> StudentEnrolment(StudentEnrolmentRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var student = studentBucket.Student;
            var no = OrderNumberLib.EnrolmentOrderNumber();
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
            //课程
            if (request.EnrolmentCourses != null && request.EnrolmentCourses.Any())
            {
                foreach (var p in request.EnrolmentCourses)
                {
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
                        oneToOneClassLst.Add(ComBusiness2.GetOneToOneClass(course.Item1, student));
                    }
                    studentCourseDetails.Add(ComBusiness2.GetStudentCourseDetail(course.Item1, priceRule, p, no, request.StudentId, request.LoginTenantId));
                    var orderCourseDetailResult = ComBusiness2.GetCourseOrderDetail(course.Item1, priceRule, p, no, request.OtherInfo.Ot, request.LoginUserId, request.LoginTenantId);
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
                    var orderGoodsDetailResult = GetGoodsOrderDetail(goods, p, request, no);
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
                    var orderCostDetailResult = GetCostOrderDetail(cost, p, request, no);
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
                CreateOt = now
            };
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
                LoginClientType = request.LoginClientType
            };
            //异步执行
            //_eventPublisher.Publish(studentEnrolmentEvent);
            var orderId = await StudentEnrolmentEvent(studentEnrolmentEvent);
            return ResponseBase.Success(new StudentEnrolmentOutput()
            {
                OrderId = orderId
            });
        }

        private Tuple<EtOrderDetail, string> GetGoodsOrderDetail(EtGoods goods, EnrolmentGoods enrolmentGoods, StudentEnrolmentRequest request, string no)
        {
            var priceRuleDesc = $"{goods.Price}元/件";
            var ruleDesc = goods.Name;
            return new Tuple<EtOrderDetail, string>(new EtOrderDetail()
            {
                OrderNo = no,
                Ot = request.OtherInfo.Ot,
                Price = goods.Price,
                BuyQuantity = enrolmentGoods.BuyQuantity,
                BugUnit = 0,
                DiscountType = enrolmentGoods.DiscountType,
                DiscountValue = enrolmentGoods.DiscountValue,
                GiveQuantity = 0,
                GiveUnit = 0,
                IsDeleted = EmIsDeleted.Normal,
                ItemAptSum = enrolmentGoods.ItemAptSum,
                ItemSum = (enrolmentGoods.BuyQuantity * goods.Price).EtmsToRound(),
                PriceRule = priceRuleDesc,
                ProductId = goods.Id,
                ProductType = EmOrderProductType.Goods,
                Remark = string.Empty,
                Status = EmOrderStatus.Normal,
                TenantId = request.LoginTenantId,
                UserId = request.LoginUserId
            }, ruleDesc);
        }

        private Tuple<EtOrderDetail, string> GetCostOrderDetail(EtCost cost, EnrolmentCost enrolmentCost, StudentEnrolmentRequest request, string no)
        {
            var priceRuleDesc = $"{cost.Price}元/笔";
            var ruleDesc = cost.Name;
            return new Tuple<EtOrderDetail, string>(new EtOrderDetail()
            {
                OrderNo = no,
                Ot = request.OtherInfo.Ot,
                Price = cost.Price,
                BuyQuantity = enrolmentCost.BuyQuantity,
                BugUnit = 0,
                DiscountType = enrolmentCost.DiscountType,
                DiscountValue = enrolmentCost.DiscountValue,
                GiveQuantity = 0,
                GiveUnit = 0,
                IsDeleted = EmIsDeleted.Normal,
                ItemAptSum = enrolmentCost.ItemAptSum,
                ItemSum = (enrolmentCost.BuyQuantity * cost.Price).EtmsToRound(),
                PriceRule = priceRuleDesc,
                ProductId = cost.Id,
                ProductType = EmOrderProductType.Cost,
                Remark = string.Empty,
                Status = EmOrderStatus.Normal,
                TenantId = request.LoginTenantId,
                UserId = request.LoginUserId
            }, ruleDesc);
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

        public async Task<long> StudentEnrolmentEvent(StudentEnrolmentEvent request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.Order.StudentId);
            if (studentBucket.Student.StudentType != EmStudentType.ReadingStudent || request.Order.TotalPoints > 0)
            {
                await _studentDAL.StudentEnrolmentEventChangeInfo(request.Order.StudentId, request.Order.TotalPoints, EmStudentType.ReadingStudent);
                _eventPublisher.Publish(new StatisticsStudentEvent(request.TenantId) { OpType = EmStatisticsStudentType.StudentType });
            }

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

            //订单
            var orderId = await _orderDAL.AddOrder(request.Order, request.OrderDetails);

            //收支明细
            if (request.IncomeLogs.Any())
            {
                foreach (var incomeLog in request.IncomeLogs)
                {
                    incomeLog.OrderId = orderId;
                }
                _incomeLogDAL.AddIncomeLog(request.IncomeLogs);
            }

            //物品销售数量和库存变动记录
            var saleGoods = request.OrderDetails.Where(p => p.ProductType == EmOrderProductType.Goods);
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
            var saleCost = request.OrderDetails.Where(p => p.ProductType == EmOrderProductType.Cost);
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

            //学员课程信息
            if (request.StudentCourseDetails != null && request.StudentCourseDetails.Count > 0)
            {
                foreach (var studentCourse in request.StudentCourseDetails)
                {
                    studentCourse.OrderId = orderId;
                }
                _studentCourseDAL.AddStudentCourseDetail(request.StudentCourseDetails);
                _eventPublisher.Publish(new StudentCourseAnalyzeEvent(request.TenantId)
                {
                    StudentId = request.Order.StudentId
                });
                await _studentCourseDAL.ResetStudentCourseNotEnoughRemindInfo(request.Order.StudentId, request.StudentCourseDetails.Select(p => p.CourseId).ToList());
            }

            //一对一课程
            if (request.OneToOneClassList != null && request.OneToOneClassList.Any())
            {
                foreach (var myClass in request.OneToOneClassList)
                {
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
                        UserId = request.Order.UserId,
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
                            TenantId = request.TenantId
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

            //日志
            var opContent = new StringBuilder();
            if (!string.IsNullOrEmpty(request.Order.BuyCourse))
            {
                opContent.Append($"课程：{request.Order.BuyCourse}；");
            }
            if (!string.IsNullOrEmpty(request.Order.BuyGoods))
            {
                opContent.Append($"<br>物品：{request.Order.BuyGoods}；");
            }
            if (!string.IsNullOrEmpty(request.Order.BuyCost))
            {
                opContent.Append($"<br>费用：{request.Order.BuyCost}；");
            }
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
            return orderId;
        }
    }
}
