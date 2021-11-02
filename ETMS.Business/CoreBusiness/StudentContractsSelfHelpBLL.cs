using ETMS.Business.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Student.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.MallGoodsDAL;
using ETMS.IEventProvider;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class StudentContractsSelfHelpBLL : IStudentContractsSelfHelpBLL
    {
        private readonly IStudentDAL _studentDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IGoodsDAL _goodsDAL;

        private readonly ICostDAL _costDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IOrderDAL _orderDAL;

        private readonly IStudentPointsLogDAL _studentPointsLog;

        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly IClassDAL _classDAL;

        private readonly ISuitDAL _suitDAL;

        private readonly IUserDAL _userDAL;

        private readonly IMallOrderDAL _mallOrderDAL;

        private readonly IIncomeLogDAL _incomeLogDAL;

        public StudentContractsSelfHelpBLL(IStudentDAL studentDAL, ICourseDAL courseDAL, IGoodsDAL goodsDAL, ICostDAL costDAL,
          IEventPublisher eventPublisher, IOrderDAL orderDAL, IStudentPointsLogDAL studentPointsLog,
          IStudentCourseDAL studentCourseDAL, IClassDAL classDAL, ISuitDAL suitDAL, IUserDAL userDAL, IMallOrderDAL mallOrderDAL,
          IIncomeLogDAL incomeLogDAL)
        {
            this._studentDAL = studentDAL;
            this._courseDAL = courseDAL;
            this._goodsDAL = goodsDAL;
            this._costDAL = costDAL;
            this._eventPublisher = eventPublisher;
            this._orderDAL = orderDAL;
            this._studentPointsLog = studentPointsLog;
            this._studentCourseDAL = studentCourseDAL;
            this._classDAL = classDAL;
            this._suitDAL = suitDAL;
            this._userDAL = userDAL;
            this._mallOrderDAL = mallOrderDAL;
            this._incomeLogDAL = incomeLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentDAL, _courseDAL, _goodsDAL, _costDAL, _studentCourseDAL,
                _orderDAL, _studentPointsLog, _classDAL, _suitDAL, _userDAL, _mallOrderDAL, _incomeLogDAL);
        }

        private StringBuilder buyCourse = new StringBuilder();
        private StringBuilder buyGoods = new StringBuilder();
        private StringBuilder buyCost = new StringBuilder();
        private List<EtOrderDetail> orderDetails = new List<EtOrderDetail>();
        private List<EtStudentCourseDetail> studentCourseDetails = new List<EtStudentCourseDetail>();
        private List<OneToOneClass> oneToOneClassLst = new List<OneToOneClass>();
        private EtMallOrder mallOrder;
        private ParentBuyMallGoodsSubmitEvent _request;
        long studentId;
        string no;
        int tenantId;
        DateTime ot;
        long userId = 0;

        private void StructureCourse<T>(EtCourse course, T priceRule, EnrolmentCourse consumeDetail) where T : BaseCoursePrice
        {
            studentCourseDetails.Add(ComBusiness2.GetStudentCourseDetail(course, priceRule, consumeDetail, no, studentId, tenantId));
            var orderCourseDetailResult = ComBusiness2.GetCourseOrderDetail(course, priceRule, consumeDetail, no, ot, userId, tenantId);
            orderDetails.Add(orderCourseDetailResult.Item1);
            var desc = ComBusiness2.GetBuyCourseDesc(course.Name, priceRule.PriceUnit, consumeDetail.BuyQuantity, consumeDetail.GiveQuantity, consumeDetail.GiveUnit);
            buyCourse.Append($"{desc}；");
        }

        private async Task StructureCourse(EtSuitDetail suitDetail)
        {
            var myCoursrBucket = await _courseDAL.GetCourse(suitDetail.ProductId);
            if (myCoursrBucket == null || myCoursrBucket.Item1 == null || myCoursrBucket.Item2 == null)
            {
                LOG.Log.Error($"[在线商城]套餐中的课程不存在", _request, this.GetType());
                return;
            }
            var course = myCoursrBucket.Item1;
            var courseRules = myCoursrBucket.Item2;
            var priceRule = courseRules.FirstOrDefault(p => p.Id == suitDetail.CoursePriceRuleId);
            if (priceRule == null)
            {
                LOG.Log.Error($"[在线商城]套餐中的课程收费方式不存在", _request, this.GetType());
                return;
            }
            if (course.Type == EmCourseType.OneToOne)
            {
                oneToOneClassLst.Add(ComBusiness2.GetOneToOneClass(course, _request.MyStudent));
            }
            var startDate = mallOrder.CreateOt;
            List<string> erangeOt = null;
            string exOt = null;
            DateTime extDate;
            var buyCount = suitDetail.BuyQuantity;
            switch (priceRule.PriceType)
            {
                case EmCoursePriceType.ClassTimes:
                    if (priceRule.ExpiredType != null && priceRule.ExpiredValue != null && priceRule.ExpiredValue.Value > 0)
                    {
                        switch (priceRule.ExpiredType)
                        {
                            case EmCoursePriceRuleExpiredType.Day:
                                exOt = startDate.AddDays(priceRule.ExpiredValue.Value).EtmsToDateString();
                                break;
                            case EmCoursePriceRuleExpiredType.Month:
                                exOt = startDate.AddMonths(priceRule.ExpiredValue.Value).EtmsToDateString();
                                break;
                            case EmCoursePriceRuleExpiredType.Year:
                                exOt = startDate.AddYears(priceRule.ExpiredValue.Value).EtmsToDateString();
                                break;
                        }
                    }
                    break;
                case EmCoursePriceType.Month:
                    if (priceRule.Quantity > 1)
                    {
                        extDate = startDate.AddMonths(priceRule.Quantity);
                    }
                    else
                    {
                        extDate = startDate.AddMonths(buyCount);
                    }
                    if (suitDetail.GiveQuantity > 0)
                    {
                        if (suitDetail.GiveUnit == EmCourseUnit.Day)
                        {
                            extDate = extDate.AddDays(suitDetail.GiveQuantity);
                        }
                        else if (suitDetail.GiveUnit == EmCourseUnit.Month)
                        {
                            extDate = extDate.AddMonths(suitDetail.GiveQuantity);
                        }
                    }
                    erangeOt = new List<string>() {
                      startDate.EtmsToDateString(),
                      extDate.EtmsToDateString()
                    };
                    break;
                case EmCoursePriceType.Day:
                    if (priceRule.Quantity > 1)
                    {
                        extDate = startDate.AddDays(priceRule.Quantity);
                    }
                    else
                    {
                        extDate = startDate.AddDays(buyCount);
                    }
                    if (suitDetail.GiveQuantity > 0)
                    {
                        if (suitDetail.GiveUnit == EmCourseUnit.Day)
                        {
                            extDate = extDate.AddDays(suitDetail.GiveQuantity);
                        }
                        else if (suitDetail.GiveUnit == EmCourseUnit.Month)
                        {
                            extDate = extDate.AddMonths(suitDetail.GiveQuantity);
                        }
                    }
                    erangeOt = new List<string>() {
                      startDate.EtmsToDateString(),
                      extDate.EtmsToDateString()
                    };
                    break;
            }
            var consumeDetail = new EnrolmentCourse()
            {
                BuyQuantity = suitDetail.BuyQuantity,
                CourseId = course.Id,
                CoursePriceRuleId = priceRule.Id,
                DiscountType = suitDetail.DiscountType,
                DiscountValue = suitDetail.DiscountValue,
                GiveQuantity = suitDetail.GiveQuantity,
                GiveUnit = suitDetail.GiveUnit,
                ItemAptSum = suitDetail.ItemAptSum,
                ErangeOt = erangeOt,
                ExOt = exOt
            };
            studentCourseDetails.Add(ComBusiness2.GetStudentCourseDetail(course, priceRule, consumeDetail, no, studentId, tenantId));
            var orderCourseDetailResult = ComBusiness2.GetCourseOrderDetail(course, priceRule, consumeDetail, no, ot, userId, tenantId);
            orderDetails.Add(orderCourseDetailResult.Item1);
            var desc = ComBusiness2.GetBuyCourseDesc(course.Name, priceRule.PriceUnit, consumeDetail.BuyQuantity, consumeDetail.GiveQuantity, consumeDetail.GiveUnit);
            buyCourse.Append($"{desc}；");
        }

        private void StructureGoods(EtGoods goods, EnrolmentGoods consumeDetail)
        {
            var orderGoodsDetailResult = ComBusiness4.GetGoodsOrderDetail(goods, consumeDetail, no, ot, tenantId, userId);
            orderDetails.Add(orderGoodsDetailResult.Item1);
            var desc = ComBusiness2.GetBuyGoodsDesc(goods.Name, mallOrder.BuyCount);
            buyGoods.Append($"{desc}；");
        }

        private async Task StructureGoods(EtSuitDetail suitDetail)
        {
            var goods = await _goodsDAL.GetGoods(suitDetail.ProductId);
            if (goods == null)
            {
                LOG.Log.Error($"[在线商城]套餐中的商品不存在", _request, this.GetType());
                return;
            }
            var consumeDetail = new EnrolmentGoods()
            {
                BuyQuantity = suitDetail.BuyQuantity,
                DiscountType = suitDetail.DiscountType,
                DiscountValue = suitDetail.DiscountValue,
                GoodsId = goods.Id,
                ItemAptSum = suitDetail.ItemAptSum
            };
            var orderGoodsDetailResult = ComBusiness4.GetGoodsOrderDetail(goods, consumeDetail, no, ot, tenantId, userId);
            orderDetails.Add(orderGoodsDetailResult.Item1);
            var desc = ComBusiness2.GetBuyGoodsDesc(goods.Name, mallOrder.BuyCount);
            buyGoods.Append($"{desc}；");
        }

        private void StructureCost(EtCost cost, EnrolmentCost consumeDetail)
        {
            var orderCostDetailResult = ComBusiness4.GetCostOrderDetail(cost, consumeDetail, no, ot, tenantId, userId);
            orderDetails.Add(orderCostDetailResult.Item1);
            var desc = ComBusiness2.GetBuyCostDesc(cost.Name, mallOrder.BuyCount);
            buyCost.Append($"{desc}；");
        }

        private async Task StructureCost(EtSuitDetail suitDetail)
        {
            var cost = await _costDAL.GetCost(suitDetail.ProductId);
            if (cost == null)
            {
                LOG.Log.Error($"[在线商城]套餐中的费用不存在", _request, this.GetType());
                return;
            }
            var consumeDetail = new EnrolmentCost()
            {
                BuyQuantity = suitDetail.BuyQuantity,
                CostId = cost.Id,
                DiscountType = suitDetail.DiscountType,
                DiscountValue = suitDetail.DiscountValue,
                ItemAptSum = suitDetail.ItemAptSum
            };
            var orderCostDetailResult = ComBusiness4.GetCostOrderDetail(cost, consumeDetail, no, ot, tenantId, userId);
            orderDetails.Add(orderCostDetailResult.Item1);
            var desc = ComBusiness2.GetBuyCostDesc(cost.Name, mallOrder.BuyCount);
            buyCost.Append($"{desc}；");
        }

        private EnrolmentCourse GetEnrolmentCourse(EtCourse course, EtMallCoursePriceRule rule, int buyCount, decimal totalMoney)
        {
            var startDate = mallOrder.CreateOt;
            var courseUnit = EmCourseUnit.ClassTimes;
            List<string> erangeOt = null;
            string exOt = null;
            DateTime extDate;
            switch (rule.PriceType)
            {
                case EmCoursePriceType.ClassTimes:
                    courseUnit = EmCourseUnit.ClassTimes;
                    if (rule.ExpiredType != null && rule.ExpiredValue != null && rule.ExpiredValue.Value > 0)
                    {
                        switch (rule.ExpiredType)
                        {
                            case EmCoursePriceRuleExpiredType.Day:
                                exOt = startDate.AddDays(rule.ExpiredValue.Value).EtmsToDateString();
                                break;
                            case EmCoursePriceRuleExpiredType.Month:
                                exOt = startDate.AddMonths(rule.ExpiredValue.Value).EtmsToDateString();
                                break;
                            case EmCoursePriceRuleExpiredType.Year:
                                exOt = startDate.AddYears(rule.ExpiredValue.Value).EtmsToDateString();
                                break;
                        }
                    }
                    break;
                case EmCoursePriceType.Month:
                    courseUnit = EmCourseUnit.Day;
                    if (rule.Quantity > 1)
                    {
                        extDate = startDate.AddMonths(rule.Quantity);
                    }
                    else
                    {
                        extDate = startDate.AddMonths(buyCount);
                    }
                    erangeOt = new List<string>() {
                      startDate.EtmsToDateString(),
                      extDate.EtmsToDateString()
                    };
                    break;
                case EmCoursePriceType.Day:
                    courseUnit = EmCourseUnit.Day;
                    if (rule.Quantity > 1)
                    {
                        extDate = startDate.AddDays(rule.Quantity);
                    }
                    else
                    {
                        extDate = startDate.AddDays(buyCount);
                    }
                    erangeOt = new List<string>() {
                      startDate.EtmsToDateString(),
                      extDate.EtmsToDateString()
                    };
                    break;
            }
            return new EnrolmentCourse()
            {
                BuyQuantity = buyCount,
                CourseId = course.Id,
                CoursePriceRuleId = 0,
                DiscountType = EmDiscountType.Nothing,
                DiscountValue = 0,
                GiveUnit = courseUnit,
                GiveQuantity = 0,
                ItemAptSum = totalMoney,
                ErangeOt = erangeOt,
                ExOt = exOt
            };
        }

        private EnrolmentGoods GetEnrolmentGoods(EtGoods goods)
        {
            return new EnrolmentGoods()
            {
                BuyQuantity = mallOrder.BuyCount,
                DiscountType = EmDiscountType.Nothing,
                GoodsId = goods.Id,
                ItemAptSum = mallOrder.AptSum,
                DiscountValue = 0
            };
        }

        private EnrolmentCost GetEnrolmentCost(EtCost cost)
        {
            return new EnrolmentCost()
            {
                BuyQuantity = mallOrder.BuyCount,
                DiscountType = EmDiscountType.Nothing,
                CostId = cost.Id,
                DiscountValue = 0,
                ItemAptSum = mallOrder.AptSum
            };
        }

        public async Task ParentBuyMallGoodsSubmitConsumerEvent(ParentBuyMallGoodsSubmitEvent request)
        {
            var myuser = await _userDAL.GetAdminUser();
            userId = myuser.Id;
            studentId = request.MyStudent.Id;
            no = request.MallOrder.OrderNo;
            tenantId = request.TenantId;
            ot = request.MallOrder.CreateOt;
            mallOrder = request.MallOrder;
            this._request = request;
            switch (request.MallOrder.ProductType)
            {
                case EmProductType.Course:
                    var courseBucket = await _courseDAL.GetCourse(mallOrder.RelatedId);
                    if (courseBucket == null || courseBucket.Item1 == null || courseBucket.Item2 == null)
                    {
                        LOG.Log.Error("[在线商城]关联的课程不存在", request, this.GetType());
                        return;
                    }
                    var course = courseBucket.Item1;
                    var priceRule = request.CoursePriceRule;
                    if (course.Type == EmCourseType.OneToOne)
                    {
                        oneToOneClassLst.Add(ComBusiness2.GetOneToOneClass(course, request.MyStudent));
                    }
                    var consumeDetail1 = GetEnrolmentCourse(course, priceRule, mallOrder.BuyCount, mallOrder.AptSum);
                    StructureCourse(course, priceRule, consumeDetail1);
                    break;
                case EmProductType.Goods:
                    var goods = await _goodsDAL.GetGoods(mallOrder.RelatedId);
                    if (goods == null)
                    {
                        LOG.Log.Error("[在线商城]关联的物品不存在", request, this.GetType());
                        return;
                    }
                    var consumeDetail12 = GetEnrolmentGoods(goods);
                    StructureGoods(goods, consumeDetail12);
                    break;
                case EmProductType.Cost:
                    var cost = await _costDAL.GetCost(mallOrder.RelatedId);
                    if (cost == null)
                    {
                        LOG.Log.Error("[在线商城]关联的费用不存在", request, this.GetType());
                        return;
                    }
                    var consumeDetail13 = GetEnrolmentCost(cost);
                    StructureCost(cost, consumeDetail13);
                    break;
                case EmProductType.Suit:
                    var suitBucket = await _suitDAL.GetSuit(mallOrder.RelatedId);
                    if (suitBucket == null || suitBucket.Item1 == null || suitBucket.Item2 == null || suitBucket.Item2.Count == 0)
                    {
                        LOG.Log.Error("[在线商城]关联的套餐不存在", request, this.GetType());
                        return;
                    }
                    var allProduct = suitBucket.Item2;
                    foreach (var itemProduct in allProduct)
                    {
                        switch (itemProduct.ProductType)
                        {
                            case EmProductType.Course:
                                await StructureCourse(itemProduct);
                                break;
                            case EmProductType.Goods:
                                await StructureGoods(itemProduct);
                                break;
                            case EmProductType.Cost:
                                await StructureCost(itemProduct);
                                break;
                        }
                    }
                    break;
            }
            var status = EmOrderStatus.Normal;
            var order = new EtOrder()
            {
                TenantId = tenantId,
                UserId = userId,
                CouponsStudentGetIds = string.Empty,
                CouponsIds = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                Ot = mallOrder.CreateOt,
                Remark = request.MallOrder.Remark,
                TotalPoints = mallOrder.TotalPoints,
                No = no,
                StudentId = mallOrder.StudentId,
                OrderType = EmOrderType.StudentEnrolment,
                AptSum = mallOrder.AptSum,
                ArrearsSum = 0,
                BuyCost = EtmsHelper.DescPrefix(buyCost.ToString().TrimEnd('；'), "费用"),
                BuyCourse = EtmsHelper.DescPrefix(buyCourse.ToString().TrimEnd('；'), "课程"),
                BuyGoods = EtmsHelper.DescPrefix(buyGoods.ToString().TrimEnd('；'), "物品"),
                CommissionUser = string.Empty,
                PaySum = mallOrder.PaySum,
                Sum = mallOrder.AptSum,
                Status = status,
                CreateOt = mallOrder.CreateTime,
                PayAccountRechargeGive = 0,
                PayAccountRechargeReal = 0,
                PayAccountRechargeId = null,
                OrderSource = EmOrderSource.MallGoodsOrder
            };

            var myCourse = await _studentCourseDAL.GetStudentCourse(mallOrder.StudentId);
            if (myCourse != null && myCourse.Count > 0)
            {
                order.BuyType = EmOrderBuyType.Renew;
            }
            var orderId = await _orderDAL.AddOrder(order, orderDetails);

            await _mallOrderDAL.SetMallOrderOrderId(request.MallOrder.Id, orderId);
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
            if (request.MyStudent.StudentType != EmStudentType.ReadingStudent || order.TotalPoints > 0)
            {
                await _studentDAL.StudentEnrolmentEventChangeInfo(order.StudentId, order.TotalPoints, EmStudentType.ReadingStudent);
                _eventPublisher.Publish(new StatisticsStudentEvent(order.TenantId) { OpType = EmStatisticsStudentType.StudentType });
            }

            var incomeLogs = new List<EtIncomeLog>();
            incomeLogs.Add(new EtIncomeLog()
            {
                AccountNo = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                No = no,
                Ot = ot,
                PayType = EmPayType.WeChat,
                ProjectType = EmIncomeLogProjectType.StudentEnrolment,
                Remark = "在线商城",
                RepealOt = null,
                OrderId = orderId,
                RepealUserId = null,
                Status = EmIncomeLogStatus.Normal,
                Sum = order.PaySum,
                TenantId = tenantId,
                Type = EmIncomeLogType.AccountIn,
                UserId = userId,
                CreateOt = ot
            });
            _incomeLogDAL.AddIncomeLog(incomeLogs);
            var studentEnrolmentEvent = new StudentEnrolmentEvent(request.TenantId)
            {
                UserId = userId,
                Order = order,
                OrderDetails = orderDetails,
                StudentCourseDetails = studentCourseDetails,
                IncomeLogs = incomeLogs,
                CreateTime = mallOrder.CreateTime,
                OneToOneClassList = oneToOneClassLst,
                CouponsStudentGetIds = null,
                LoginClientType = request.LoginClientType
            };

            //异步执行
            _eventPublisher.Publish(studentEnrolmentEvent);

            if (request.MyStudent.RecommendStudentId != null)
            {
                _eventPublisher.Publish(new StudentRecommendRewardEvent(request.TenantId)
                {
                    Student = request.MyStudent,
                    Order = order,
                    Type = StudentRecommendRewardType.Buy
                });
            }
        }
    }
}
