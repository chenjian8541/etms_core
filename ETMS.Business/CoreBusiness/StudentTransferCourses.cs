using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Student.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
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

        public StudentTransferCourses(IStudentDAL studentDAL, ICourseDAL courseDAL, IGoodsDAL goodsDAL, ICostDAL costDAL,
            IEventPublisher eventPublisher, IOrderDAL orderDAL, IStudentPointsLogDAL studentPointsLog, IIncomeLogDAL incomeLogDAL,
            IStudentCourseDAL studentCourseDAL, IUserOperationLogDAL userOperationLogDAL, IClassDAL classDAL)
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
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentDAL, _courseDAL, _goodsDAL, _costDAL, _incomeLogDAL, _studentCourseDAL, _userOperationLogDAL,
                _orderDAL, _studentPointsLog, _classDAL);
        }

        private EtOrderDetail GetTransferOrderDetailOut(EtOrderDetail sourceOrderDetail, TransferCoursesOut productItem,
            string newNo, DateTime now, long sourceOrderId, string sourceOrderNo)
        {
            var buyUnit = sourceOrderDetail.BugUnit;
            if (buyUnit == EmCourseUnit.Month)
            {
                buyUnit = EmCourseUnit.Day;
            }
            var buyQuantity = Convert.ToInt32(productItem.ReturnCount);
            if (sourceOrderDetail.ProductType == EmOrderProductType.Course && sourceOrderDetail.BugUnit != EmCourseUnit.ClassTimes)
            {
                buyQuantity = buyQuantity / 30;
            }
            var price = Math.Round(productItem.ReturnSum / productItem.ReturnCount, 2);
            return new EtOrderDetail()
            {
                BugUnit = buyUnit,
                BuyQuantity = -buyQuantity,
                OutQuantity = productItem.ReturnCount,
                DiscountType = EmOrderDiscountType.Nothing,
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
                OutOrderNo = sourceOrderNo
            };
        }

        public async Task<ResponseBase> TransferCourses(TransferCoursesRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var newOrderNo = OrderNumberLib.GetTransferCoursesOrderNumber();
            var now = DateTime.Now;
            return ResponseBase.Success();
        }

        private async Task<ResponseBase> ProcessTransferCoursesBuy(TransferCoursesRequest request,
            EtStudent student, string no, DateTime now)
        {
             decimal sum = 0, aptSum = 0;
            StringBuilder buyCourse = new StringBuilder(), buyGoods = new StringBuilder(), buyCost = new StringBuilder();
            var orderDetails = new List<EtOrderDetail>();
            var studentCourseDetails = new List<EtStudentCourseDetail>();
            var oneToOneClassLst = new List<OneToOneClass>();
            //课程
            if (request.TransferCoursesBuy != null && request.TransferCoursesBuy.Any())
            {
                foreach (var p in request.TransferCoursesBuy)
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
                    var orderCourseDetailResult = ComBusiness2.GetCourseOrderDetail(course.Item1, priceRule, p, no, request.TransferCoursesOrderInfo.Ot, request.LoginUserId, request.LoginTenantId);
                    orderDetails.Add(orderCourseDetailResult.Item1);
                    buyCourse.Append($"{orderCourseDetailResult.Item2}；");
                    sum += orderCourseDetailResult.Item1.ItemSum;
                    aptSum += orderCourseDetailResult.Item1.ItemAptSum;
                }
            }
            return ResponseBase.Success();
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
            var monthToDay = SystemConfig.ComConfig.MonthToDay;
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

                newOrderDetailList.Add(GetTransferOrderDetailOut(mySourceOrderDetail, outOrderDetail, newOrderNo, now, mySourceOrderDetail.OrderId, mySourceOrderDetail.OrderNo));
                newOrderOperationLogs.Add(new EtOrderOperationLog()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    OpType = EmOrderOperationLogType.TransferCourses,
                    OpContent = $"转出课程:{myOutCourse.Name},转出{outOrderDetail.ReturnCount}{EmCourseUnit.GetCourseUnitDesc(mySourceStudentCourseDetail.UseUnit)}",
                    OrderId = mySourceOrderDetail.OrderId,
                    OrderNo = mySourceOrderDetail.OrderNo,
                    Ot = now,
                    Remark = request.TransferCoursesOrderInfo.Remark,
                    TenantId = request.LoginTenantId,
                    UserId = request.LoginUserId
                });

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

                            var dffTime = EtmsHelper.GetDffTime(firstDate, mySourceStudentCourseDetail.EndTime.Value);
                            mySourceStudentCourseDetail.SurplusQuantity = dffTime.Item1;
                            mySourceStudentCourseDetail.SurplusSmallQuantity = dffTime.Item2;
                            mySourceStudentCourseDetail.UseQuantity += deDay;
                        }
                        else
                        {
                            var tatalDay = mySourceStudentCourseDetail.SurplusQuantity * monthToDay + mySourceStudentCourseDetail.SurplusSmallQuantity; //剩余总天数
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
            return ResponseBase.Success(newOrderDetailList);
        }
    }
}
