using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Parent.Output;
using ETMS.Entity.Dto.Parent.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using ETMS.Entity.Config;
using ETMS.Entity.Temp;

namespace ETMS.Business
{
    public class ParentData2BLL : IParentData2BLL
    {
        private readonly IClassRecordDAL _classRecordDAL;

        private readonly IClassDAL _classDAL;

        private readonly IUserDAL _userDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IParentStudentDAL _parentStudentDAL;

        private readonly IOrderDAL _orderDAL;

        private readonly ICouponsDAL _couponsDAL;

        private readonly ICostDAL _costDAL;

        private readonly IGoodsDAL _goodsDAL;

        private readonly IIncomeLogDAL _incomeLogDAL;

        private readonly IStudentPointsLogDAL _studentPointsLogDAL;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly IStudentRelationshipDAL _studentRelationshipDAL;

        private readonly IClassRoomDAL _classRoomDAL;

        private readonly IClassRecordEvaluateDAL _classRecordEvaluateDAL;

        private readonly IStudentOperationLogDAL _studentOperationLogDAL;

        private readonly IStudentAccountRechargeDAL _studentAccountRechargeDAL;

        private readonly ITenantConfigDAL _tenantConfigDAL;

        public ParentData2BLL(IClassRecordDAL classRecordDAL, IClassDAL classDAL, IUserDAL userDAL, IStudentDAL studentDAL,
            IStudentCourseDAL studentCourseDAL, ICourseDAL courseDAL, IParentStudentDAL parentStudentDAL, IOrderDAL orderDAL,
            ICouponsDAL couponsDAL, ICostDAL costDAL, IGoodsDAL goodsDAL, IIncomeLogDAL incomeLogDAL, IStudentPointsLogDAL studentPointsLogDAL,
            IHttpContextAccessor httpContextAccessor, IAppConfigurtaionServices appConfigurtaionServices, IStudentRelationshipDAL studentRelationshipDAL,
            IClassRoomDAL classRoomDAL, IClassRecordEvaluateDAL classRecordEvaluateDAL, IStudentOperationLogDAL studentOperationLogDAL,
            IStudentAccountRechargeDAL studentAccountRechargeDAL, ITenantConfigDAL tenantConfigDAL)
        {
            this._classRecordDAL = classRecordDAL;
            this._classDAL = classDAL;
            this._userDAL = userDAL;
            this._studentDAL = studentDAL;
            this._studentCourseDAL = studentCourseDAL;
            this._courseDAL = courseDAL;
            this._parentStudentDAL = parentStudentDAL;
            this._orderDAL = orderDAL;
            this._couponsDAL = couponsDAL;
            this._costDAL = costDAL;
            this._goodsDAL = goodsDAL;
            this._incomeLogDAL = incomeLogDAL;
            this._studentPointsLogDAL = studentPointsLogDAL;
            this._httpContextAccessor = httpContextAccessor;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._studentRelationshipDAL = studentRelationshipDAL;
            this._classRoomDAL = classRoomDAL;
            this._classRecordEvaluateDAL = classRecordEvaluateDAL;
            this._studentOperationLogDAL = studentOperationLogDAL;
            this._studentAccountRechargeDAL = studentAccountRechargeDAL;
            this._tenantConfigDAL = tenantConfigDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _classRecordDAL, _classDAL, _userDAL, _studentDAL, _studentCourseDAL,
                _courseDAL, _parentStudentDAL, _orderDAL, _couponsDAL, _costDAL, _goodsDAL, _incomeLogDAL, _studentPointsLogDAL,
                _studentRelationshipDAL, _classRoomDAL, _classRecordEvaluateDAL, _studentOperationLogDAL, _studentAccountRechargeDAL,
                _tenantConfigDAL);
        }

        private async Task<OrderStudentView> OrderStudentGet(EtOrder order)
        {
            var orderStudentView = new OrderStudentView()
            {
                StudentId = order.StudentId
            };
            if (order.StudentId > 0)
            {
                var studentBucket = await _studentDAL.GetStudent(order.StudentId);
                if (studentBucket != null && studentBucket.Student != null)
                {
                    orderStudentView.StudentPhone = studentBucket.Student.Phone;
                    orderStudentView.StudentName = studentBucket.Student.Name;
                    orderStudentView.StudentCardNo = studentBucket.Student.CardNo;
                    orderStudentView.StudentAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, studentBucket.Student.Avatar);
                    return orderStudentView;
                }
            }
            else
            {
                if (order.StudentAccountRechargeId != null)
                {
                    var accountLog = await _studentAccountRechargeDAL.GetStudentAccountRecharge(order.StudentAccountRechargeId.Value);
                    if (accountLog != null)
                    {
                        orderStudentView.StudentPhone = accountLog.Phone;
                        return orderStudentView;
                    }
                }
            }
            return orderStudentView;
        }

        public async Task<ResponseBase> ClassRecordGet(ClassRecordGetRequest request)
        {
            var pagingData = await _classRecordDAL.GetClassRecordStudentPaging(request);
            var output = new List<ClassRecordGetParentOutput>();
            var tempBoxUser = new DataTempBox<EtUser>();
            var tempBoxStudent = new DataTempBox<EtStudent>();
            var courseTempBox = new DataTempBox<EtCourse>();
            foreach (var p in pagingData.Item1)
            {
                var etClass = await _classDAL.GetClassBucket(p.ClassId);
                var teachersDesc = await ComBusiness.GetParentTeachers(tempBoxUser, _userDAL, p.Teachers);
                var student = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId);
                output.Add(new ClassRecordGetParentOutput()
                {
                    ClassName = etClass.EtClass.Name,
                    ClassOtDesc = p.ClassOt.EtmsToDateString(),
                    StartTime = EtmsHelper.GetTimeDesc(p.StartTime),
                    EndTime = EtmsHelper.GetTimeDesc(p.EndTime),
                    EvaluateTeacherNum = p.EvaluateTeacherNum,
                    IsBeEvaluate = p.IsBeEvaluate,
                    StudentCheckStatus = p.StudentCheckStatus,
                    StudentCheckStatusDesc = EmClassStudentCheckStatus.GetClassStudentCheckStatus(p.StudentCheckStatus),
                    StudentId = p.StudentId,
                    StudentType = p.StudentType,
                    StudentTypeDesc = EmClassStudentType.GetClassStudentTypeDesc(p.StudentType),
                    TeacherNum = p.TeacherNum,
                    TeachersDesc = teachersDesc,
                    Week = p.Week,
                    Id = p.Id,
                    WeekDesc = $"星期{EtmsHelper.GetWeekDesc(p.Week)}",
                    StudentName = student.Name,
                    CourseDesc = await ComBusiness.GetCourseName(courseTempBox, _courseDAL, p.CourseId),
                    DeClassTimesDesc = ComBusiness2.GetDeClassTimesDesc(p.DeType, p.DeClassTimes, p.ExceedClassTimes),
                    IsTeacherEvaluate = p.IsTeacherEvaluate
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ClassRecordGetParentOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> ClassRecordDetailGet(ClassRecordDetailGetRequest request)
        {
            var p = await _classRecordDAL.GetEtClassRecordStudentById(request.Id);
            if (p == null)
            {
                return ResponseBase.CommonError("上课记录不存在");
            }
            var courseTempBox = new DataTempBox<EtCourse>();
            var tempBoxUser = new DataTempBox<EtUser>();
            var etClass = await _classDAL.GetClassBucket(p.ClassId);
            var teachersDesc = await ComBusiness.GetParentTeachers(tempBoxUser, _userDAL, p.Teachers);
            var classRoomIdsDesc = string.Empty;
            if (!string.IsNullOrEmpty(p.ClassRoomIds))
            {
                var allClassRoom = await _classRoomDAL.GetAllClassRoom();
                classRoomIdsDesc = ComBusiness.GetDesc(allClassRoom, p.ClassRoomIds);
            }
            var student = (await _studentDAL.GetStudent(p.StudentId)).Student;
            var output = new ClassRecordDetailGetOutput()
            {
                ClassRecordBascInfo = new ClassRecordBascInfo()
                {
                    ClassContent = p.ClassContent,
                    ClassId = p.ClassId,
                    ClassName = etClass.EtClass.Name,
                    ClassOtDesc = p.ClassOt.EtmsToDateString(),
                    CourseDesc = await ComBusiness.GetCourseName(courseTempBox, _courseDAL, p.CourseId),
                    CourseId = p.CourseId,
                    EndTime = EtmsHelper.GetTimeDesc(p.EndTime),
                    RewardPoints = p.RewardPoints,
                    StartTime = EtmsHelper.GetTimeDesc(p.StartTime),
                    StudentCheckStatus = p.StudentCheckStatus,
                    StudentCheckStatusDesc = EmClassStudentCheckStatus.GetClassStudentCheckStatus(p.StudentCheckStatus),
                    StudentId = p.StudentId,
                    StudentType = p.StudentType,
                    StudentTypeDesc = EmClassStudentType.GetClassStudentTypeDesc(p.StudentType),
                    TeacherNum = p.TeacherNum,
                    TeachersDesc = teachersDesc,
                    Week = p.Week,
                    ClassRoomIdsDesc = classRoomIdsDesc,
                    StudentName = student.Name,
                    WeekDesc = $"星期{EtmsHelper.GetWeekDesc(p.Week)}",
                    DeClassTimesDesc = ComBusiness2.GetDeClassTimesDesc(p.DeType, p.DeClassTimes, p.ExceedClassTimes),
                },
                EvaluateStudentInfos = new List<ClassRecordEvaluateStudentInfo>()
            };
            var classRecordEvaluateStudents = await _classRecordEvaluateDAL.GetClassRecordEvaluateStudent(request.Id);
            if (classRecordEvaluateStudents.Count > 0)
            {
                var isNeedUpdateEvaluateIsRead = false;
                foreach (var classRecordEvaluateStudent in classRecordEvaluateStudents)
                {
                    var teacher = await ComBusiness.GetUser(tempBoxUser, _userDAL, classRecordEvaluateStudent.TeacherId);
                    if (teacher == null)
                    {
                        continue;
                    }
                    output.EvaluateStudentInfos.Add(new ClassRecordEvaluateStudentInfo()
                    {
                        EvaluateContent = classRecordEvaluateStudent.EvaluateContent,
                        EvaluateStudentId = classRecordEvaluateStudent.Id,
                        EvaluateOtDesc = EtmsHelper.GetOtFriendlyDesc(classRecordEvaluateStudent.Ot),
                        TeacherId = classRecordEvaluateStudent.TeacherId,
                        TeacherAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, teacher.Avatar),
                        TeacherName = ComBusiness2.GetParentTeacherName(teacher),
                        EvaluateMedias = ComBusiness3.GetMediasUrl(classRecordEvaluateStudent.EvaluateImg)
                    });
                    if (!classRecordEvaluateStudent.IsRead)
                    {
                        isNeedUpdateEvaluateIsRead = true;
                    }
                }
                if (isNeedUpdateEvaluateIsRead)
                {
                    await _classRecordEvaluateDAL.ClassRecordEvaluateStudentSetRead(request.Id, classRecordEvaluateStudents.Count);
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentCourseGet(StudentCourseGetRequest request)
        {
            var output = new List<StudentCourseGetOutput>();
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var allStudent = await _parentStudentDAL.GetParentStudents(request.LoginTenantId, request.LoginPhone);
            var config = await _tenantConfigDAL.GetTenantConfig();
            var parentIsShowEndOfClass = config.TenantOtherConfig.ParentIsShowEndOfClass;
            foreach (var studentInfo in allStudent)
            {
                var studentId = studentInfo.Id;
                var studentCourse = await _studentCourseDAL.GetStudentCourse(studentId);
                if (studentCourse != null && studentCourse.Any())
                {
                    var studentClass = await _classDAL.GetStudentClass(studentId);
                    var courseIds = studentCourse.Select(p => p.CourseId).Distinct();
                    foreach (var courseId in courseIds)
                    {
                        var course = await ComBusiness.GetCourse(tempBoxCourse, _courseDAL, courseId);
                        if (course == null)
                        {
                            continue;
                        }
                        var myStudentCourseDetail = new StudentCourseGetOutput()
                        {
                            CourseName = course.Name,
                            CourseColor = course.StyleColor,
                            CourseId = courseId,
                            StudentId = studentId,
                            StudentName = studentInfo.Name,
                            Type = course.Type
                        };
                        var myCourse = studentCourse.Where(p => p.CourseId == courseId);
                        if (!parentIsShowEndOfClass)
                        {
                            if (myCourse.First().Status == EmStudentCourseStatus.EndOfClass)
                            {
                                continue;
                            }
                        }
                        foreach (var theCourse in myCourse)
                        {
                            myStudentCourseDetail.Status = theCourse.Status;
                            myStudentCourseDetail.StatusDesc = EmStudentCourseStatus.GetStudentCourseStatusDesc(theCourse.Status);
                            if (theCourse.DeType == EmDeClassTimesType.ClassTimes && theCourse.BuyQuantity > 0)
                            {
                                myStudentCourseDetail.DeTypeClassTimes = new ParentDeTypeClassTimes()
                                {
                                    SurplusQuantityDesc = ComBusiness.GetSurplusQuantityDesc(theCourse.SurplusQuantity, theCourse.SurplusSmallQuantity, theCourse.DeType)
                                };
                            }
                            if (theCourse.DeType == EmDeClassTimesType.Day)
                            {
                                myStudentCourseDetail.DeTypeDay = new ParentDeTypeDay()
                                {
                                    SurplusQuantityDesc = ComBusiness.GetSurplusQuantityDesc(theCourse.SurplusQuantity, theCourse.SurplusSmallQuantity, theCourse.DeType)
                                };
                            }
                        }
                        var myClass = studentClass.Where(p => p.CourseList.IndexOf($",{courseId},") != -1);
                        if (myClass.Any())
                        {
                            myStudentCourseDetail.StudentClass = new List<ParentStudentClass>();
                            foreach (var c in myClass)
                            {
                                myStudentCourseDetail.StudentClass.Add(new ParentStudentClass()
                                {
                                    Id = c.Id,
                                    Name = c.Name
                                });
                            }
                        }
                        output.Add(myStudentCourseDetail);
                    }
                }
            }
            return ResponseBase.Success(output.OrderBy(p => p.Status));
        }

        private void ProcessOrderAccountRechargePay(List<ParentOrderGetDetailIncomeLog> incomeLogs, EtOrder order)
        {
            if (order.PayAccountRechargeId == null || (order.PayAccountRechargeReal == 0 && order.PayAccountRechargeGive == 0))
            {
                return;
            }
            incomeLogs.Insert(0, new ParentOrderGetDetailIncomeLog()
            {
                PayOt = order.Ot.EtmsToDateString(),
                PayType = EmPayType.PayAccountRecharge,
                PayTypeDesc = EmPayType.GetPayType(EmPayType.PayAccountRecharge),
                ProjectType = 0,
                ProjectTypeName = EmOrderType.GetOrderTypeDesc(order.OrderType),
                Sum = order.PayAccountRechargeReal + order.PayAccountRechargeGive,
            });
        }

        public async Task<ResponseBase> StudentOrderGet(StudentOrderGetRequest request)
        {
            var pagingData = await _orderDAL.GetOrderPaging(request);
            var output = new List<StudentOrderGetOutput>();
            foreach (var p in pagingData.Item1)
            {
                var student = await OrderStudentGet(p);
                output.Add(new StudentOrderGetOutput()
                {
                    AptSum = p.AptSum,
                    ArrearsSum = p.ArrearsSum,
                    BuyCost = p.BuyCost,
                    BuyCourse = p.BuyCourse,
                    BuyGoods = p.BuyGoods,
                    BuyOther = p.BuyOther,
                    No = p.No,
                    OrderType = p.OrderType,
                    OtDesc = p.Ot.EtmsToDateString(),
                    PaySum = p.PaySum,
                    Status = p.Status,
                    StatusDesc = EmOrderStatus.GetOrderStatus(p.Status),
                    StudentId = p.StudentId,
                    StudentName = student.StudentName,
                    StudentPhone = student.StudentPhone,
                    StudentAvatar = student.StudentAvatar,
                    TotalPoints = p.TotalPoints,
                    Id = p.Id,
                    InOutType = p.InOutType,
                    OrderTypeDesc = EmOrderType.GetOrderTypeDesc(p.OrderType),
                    TotalPointsDesc = EmOrderInOutType.GetTotalPointsDesc(p.TotalPoints, p.InOutType)
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentOrderGetOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> StudentOrderDetailGet(StudentOrderDetailGetRequest request)
        {
            var order = await _orderDAL.GetOrder(request.OrderId);
            if (order == null)
            {
                return ResponseBase.CommonError("订单不存在");
            }
            var output = new StudentOrderDetailGetOutput();
            var student = await OrderStudentGet(order);
            output.BascInfo = new ParentOrderGetDetailBascInfo()
            {
                ArrearsSum = order.ArrearsSum,
                Id = order.Id,
                AptSum = order.AptSum,
                No = order.No,
                OrderType = order.OrderType,
                OtDesc = order.Ot.EtmsToDateString(),
                PaySum = order.PaySum,
                Status = order.Status,
                StatusDesc = EmOrderStatus.GetOrderStatus(order.Status),
                StudentId = order.StudentId,
                StudentName = student.StudentName,
                StudentPhone = student.StudentPhone,
                StudentAvatar = student.StudentAvatar,
                TotalPoints = order.TotalPoints,
                InOutType = order.InOutType,
                TotalPointsDesc = EmOrderInOutType.GetTotalPointsDesc(order.TotalPoints, order.InOutType),
                UnionOrderId = order.UnionOrderId.ToString(),
                UnionOrderNo = order.UnionOrderNo,
                IsReturn = order.IsReturn,
                IsTransferCourse = order.IsTransferCourse
            };
            if (!string.IsNullOrEmpty(order.CouponsIds) && !string.IsNullOrEmpty(order.CouponsStudentGetIds))
            {
                output.OrderGetDetailCoupons = new List<ParentOrderGetDetailCoupons>();
                var couponsIds = order.CouponsIds.Split(',');
                foreach (var couponsId in couponsIds)
                {
                    if (string.IsNullOrEmpty(couponsId))
                    {
                        continue;
                    }
                    var myCoupons = await _couponsDAL.GetCoupons(couponsId.ToLong());
                    if (myCoupons == null)
                    {
                        continue;
                    }
                    output.OrderGetDetailCoupons.Add(new ParentOrderGetDetailCoupons()
                    {
                        Id = myCoupons.Id,
                        CouponsMinLimit = myCoupons.MinLimit,
                        CouponsTitle = myCoupons.Title,
                        CouponsType = myCoupons.Type,
                        CouponsTypeDesc = EmCouponsType.GetCouponsTypeDesc(myCoupons.Type),
                        CouponsValue = myCoupons.Value,
                        CouponsValueDesc = ComBusiness.GetCouponsValueDesc2(myCoupons.Type, myCoupons.Value),
                        MinLimitDesc = myCoupons.MinLimit == null || myCoupons.MinLimit == 0 ? "无门槛" : $"消费满{myCoupons.MinLimit.Value.ToDecimalDesc()}元可用"
                    });
                }
            }
            var orderDetail = await _orderDAL.GetOrderDetail(request.OrderId);
            output.OrderGetDetailProducts = new List<ParentOrderGetDetailProductInfo>();
            foreach (var myItem in orderDetail)
            {
                var productName = string.Empty;
                switch (myItem.ProductType)
                {
                    case EmProductType.Cost:
                        var myCost = await _costDAL.GetCost(myItem.ProductId);
                        productName = myCost?.Name;
                        break;
                    case EmProductType.Goods:
                        var myGoods = await _goodsDAL.GetGoods(myItem.ProductId);
                        productName = myGoods?.Name;
                        break;
                    case EmProductType.Course:
                        var myCourse = await _courseDAL.GetCourse(myItem.ProductId);
                        productName = myCourse?.Item1.Name;
                        break;
                }
                output.OrderGetDetailProducts.Add(new ParentOrderGetDetailProductInfo()
                {
                    BugUnit = myItem.BugUnit,
                    BuyQuantity = myItem.BuyQuantity,
                    BuyQuantityDesc = ComBusiness.GetBuyQuantityDesc(myItem.BuyQuantity, 0, myItem.BugUnit, myItem.ProductType),
                    DiscountDesc = ComBusiness.GetDiscountDesc(myItem.DiscountValue, myItem.DiscountType),
                    GiveQuantityDesc = ComBusiness.GetGiveQuantityDesc(myItem.GiveQuantity, myItem.GiveUnit),
                    ItemAptSum = myItem.ItemAptSum,
                    ItemSum = myItem.ItemSum,
                    PriceRule = myItem.PriceRule,
                    ProductTypeDesc = EmProductType.GetProductType(myItem.ProductType),
                    ProductName = productName,
                    Id = myItem.Id,
                    ProductType = myItem.ProductType,
                    OutQuantity = myItem.OutQuantity,
                    OutQuantityDesc = ComBusiness.GetOutQuantityDesc(myItem.OutQuantity, myItem.BugUnit, myItem.ProductType)
                });
            }
            var payLog = await _incomeLogDAL.GetIncomeLogByOrderId(request.OrderId);
            output.OrderGetDetailIncomeLogs = new List<ParentOrderGetDetailIncomeLog>();
            if (payLog != null && payLog.Any())
            {
                foreach (var p in payLog)
                {
                    output.OrderGetDetailIncomeLogs.Add(new ParentOrderGetDetailIncomeLog()
                    {
                        PayOt = p.Ot.EtmsToDateString(),
                        PayType = p.PayType,
                        PayTypeDesc = EmPayType.GetPayType(p.PayType),
                        ProjectType = p.ProjectType,
                        ProjectTypeName = EmIncomeLogProjectType.GetIncomeLogProjectType(p.ProjectType),
                        Sum = p.Sum,
                        Id = p.Id
                    });
                }
            }
            ProcessOrderAccountRechargePay(output.OrderGetDetailIncomeLogs, order);

            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentOrderTransferCoursesGetDetail(StudentOrderTransferCoursesGetDetailRequest request)
        {
            var order = await _orderDAL.GetOrder(request.OrderId);
            if (order == null)
            {
                return ResponseBase.CommonError("订单不存在");
            }
            var output = new StudentOrderTransferCoursesGetDetailOutput()
            {
                InList = new List<StudentOrderTransferCoursesGetDetailIn>(),
                OutList = new List<StudentOrderTransferCoursesGetDetailOut>()
            };
            var student = await OrderStudentGet(order);
            output.BascInfo = new StudentOrderTransferCoursesGetDetailBascInfo()
            {
                ArrearsSum = order.ArrearsSum,
                BuyCost = order.BuyCost,
                CId = order.Id,
                AptSum = order.AptSum,
                BuyCourse = order.BuyCourse,
                BuyGoods = order.BuyGoods,
                No = order.No,
                OrderType = order.OrderType,
                OtDesc = order.Ot.EtmsToDateString(),
                PaySum = order.PaySum,
                Remark = order.Remark,
                Status = order.Status,
                StatusDesc = EmOrderStatus.GetOrderStatus(order.Status),
                StudentId = order.StudentId,
                StudentName = student.StudentName,
                StudentPhone = student.StudentPhone,
                StudentAvatar = student.StudentAvatar,
                Sum = order.Sum,
                TotalPoints = order.TotalPoints,
                UserId = order.UserId,
                CreateOt = order.CreateOt,
                InOutType = order.InOutType,
                TotalPointsDesc = EmOrderInOutType.GetTotalPointsDesc(order.TotalPoints, order.InOutType),
                UnionOrderId = order.UnionOrderId.ToString(),
                UnionOrderNo = order.UnionOrderNo,
                IsReturn = order.IsReturn,
                IsTransferCourse = order.IsTransferCourse
            };
            var orderDetail = await _orderDAL.GetOrderDetail(request.OrderId);
            var intDetail = orderDetail.Where(p => p.InOutType == EmOrderInOutType.In);
            var tempBoxCourse = new DataTempBox<EtCourse>();
            foreach (var myItem in intDetail)
            {
                output.InList.Add(new StudentOrderTransferCoursesGetDetailIn()
                {
                    BugUnit = myItem.BugUnit,
                    BuyQuantity = myItem.BuyQuantity,
                    BuyQuantityDesc = ComBusiness.GetBuyQuantityDesc(myItem.BuyQuantity, 0, myItem.BugUnit, myItem.ProductType),
                    DiscountDesc = ComBusiness.GetDiscountDesc(myItem.DiscountValue, myItem.DiscountType),
                    GiveQuantityDesc = ComBusiness.GetGiveQuantityDesc(myItem.GiveQuantity, myItem.GiveUnit),
                    ItemAptSum = Math.Abs(myItem.ItemAptSum),
                    ItemSum = Math.Abs(myItem.ItemSum),
                    PriceRule = myItem.PriceRule,
                    ProductTypeDesc = EmProductType.GetProductType(myItem.ProductType),
                    ProductName = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, myItem.ProductId),
                    CId = myItem.Id,
                    OutQuantity = myItem.OutQuantity,
                    OutQuantityDesc = ComBusiness.GetOutQuantityDesc(myItem.OutQuantity, myItem.BugUnit, myItem.ProductType)
                });
            }
            var outDetail = orderDetail.Where(p => p.InOutType == EmOrderInOutType.Out);
            foreach (var myItem in outDetail)
            {
                output.OutList.Add(new StudentOrderTransferCoursesGetDetailOut()
                {
                    CId = myItem.Id,
                    UnionOrderId = myItem.OutOrderId.Value,
                    UnionOrderNo = myItem.OutOrderNo,
                    ItemAptSum = Math.Abs(myItem.ItemAptSum),
                    OutQuantity = myItem.OutQuantity.EtmsToString(),
                    OutQuantityDesc = ComBusiness.GetOutQuantityDesc(myItem.OutQuantity, myItem.BugUnit, myItem.ProductType),
                    ProductName = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, myItem.ProductId)
                });
            }

            var payLog = await _incomeLogDAL.GetIncomeLogByOrderId(request.OrderId);
            output.OrderGetDetailIncomeLogs = new List<ParentOrderGetDetailIncomeLog>();
            if (payLog != null && payLog.Any())
            {
                foreach (var p in payLog)
                {
                    output.OrderGetDetailIncomeLogs.Add(new ParentOrderGetDetailIncomeLog()
                    {
                        PayOt = p.Ot.EtmsToDateString(),
                        PayType = p.PayType,
                        PayTypeDesc = EmPayType.GetPayType(p.PayType),
                        ProjectType = p.ProjectType,
                        ProjectTypeName = EmIncomeLogProjectType.GetIncomeLogProjectType(p.ProjectType),
                        Sum = p.Sum,
                        Id = p.Id
                    });
                }
            }
            ProcessOrderAccountRechargePay(output.OrderGetDetailIncomeLogs, order);

            output.InSum = output.InList.Sum(j => j.ItemAptSum);
            output.OutSum = output.OutList.Sum(j => j.ItemAptSum);
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentOrderReturnLogGet(StudentOrderReturnLogGetRequest request)
        {
            var returnOrder = await _orderDAL.GetUnionOrderSource(request.OrderId);
            var output = new List<StudentOrderReturnLogGetOutput>();
            if (returnOrder.Count > 0)
            {
                var returnOrderDetail = await _orderDAL.GetOrderDetail(returnOrder.Select(p => p.Id).ToList());
                foreach (var order in returnOrder)
                {
                    var myOutputItem = new StudentOrderReturnLogGetOutput()
                    {
                        AptSum = order.AptSum,
                        CId = order.Id,
                        CreateOt = order.CreateOt,
                        InOutType = order.InOutType,
                        No = order.No,
                        OrderType = order.OrderType,
                        OtDesc = order.Ot.EtmsToDateString(),
                        Status = order.Status,
                        StatusDesc = EmOrderStatus.GetOrderStatus(order.Status),
                        Remark = order.Remark,
                        StudentId = order.StudentId,
                        Sum = order.Sum,
                        TotalPoints = order.TotalPoints,
                        TotalPointsDesc = EmOrderInOutType.GetTotalPointsDesc(order.TotalPoints, order.InOutType),
                        LogDetails = new List<StudentOrderReturnLogDetail>()
                    };
                    var myOrderDetail = returnOrderDetail.Where(p => p.OrderId == order.Id);
                    foreach (var orderDetail in myOrderDetail)
                    {
                        var productName = string.Empty;
                        switch (orderDetail.ProductType)
                        {
                            case EmProductType.Cost:
                                var myCost = await _costDAL.GetCost(orderDetail.ProductId);
                                productName = myCost?.Name;
                                break;
                            case EmProductType.Goods:
                                var myGoods = await _goodsDAL.GetGoods(orderDetail.ProductId);
                                productName = myGoods?.Name;
                                break;
                            case EmProductType.Course:
                                var myCourse = await _courseDAL.GetCourse(orderDetail.ProductId);
                                productName = myCourse?.Item1.Name;
                                break;
                        }
                        myOutputItem.LogDetails.Add(new StudentOrderReturnLogDetail()
                        {
                            CId = orderDetail.Id,
                            ItemAptSum = Math.Abs(orderDetail.ItemAptSum),
                            ItemSum = Math.Abs(orderDetail.ItemSum),
                            OutQuantity = orderDetail.OutQuantity.EtmsToString(),
                            ProductTypeDesc = EmProductType.GetProductType(orderDetail.ProductType),
                            ProductName = productName,
                            OutQuantityDesc = ComBusiness.GetOutQuantityDesc(orderDetail.OutQuantity, orderDetail.BugUnit, orderDetail.ProductType)
                        });
                    }
                    output.Add(myOutputItem);
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentOrderTransferCoursesLogGet(StudentOrderTransferCoursesLogGetRequest request)
        {
            var unionTransferOrder = await _orderDAL.GetUnionTransferOrder(request.OrderId);
            var output = new List<StudentOrderTransferCoursesLogGetOutput>();
            if (unionTransferOrder.Any())
            {
                var tempBoxUser = new DataTempBox<EtUser>();
                var returnOrderDetail = await _orderDAL.GetOrderDetail(unionTransferOrder.Select(p => p.Id).ToList());
                var myTransferOrderDetail = returnOrderDetail.Where(p => p.OutOrderId == request.OrderId);
                foreach (var p in myTransferOrderDetail)
                {
                    var myCourse = await _courseDAL.GetCourse(p.ProductId);
                    if (myCourse == null || myCourse.Item1 == null)
                    {
                        LOG.Log.Error("[OrderTransferCoursesLogGet]课程不存在", request, this.GetType());
                        continue;
                    }
                    output.Add(new StudentOrderTransferCoursesLogGetOutput
                    {
                        ItemAptSum = Math.Abs(p.ItemAptSum),
                        UnionOrderId = p.OrderId,
                        UnionOrderNo = p.OrderNo,
                        OutQuantity = p.OutQuantity.EtmsToString(),
                        OutQuantityDesc = ComBusiness.GetOutQuantityDesc(p.OutQuantity, p.BugUnit, p.ProductType),
                        ProductName = myCourse.Item1.Name,
                        OtDesc = p.Ot.EtmsToDateString()
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentPointsLogGet(StudentPointsLogGetRequest request)
        {
            var pagingData = await _studentPointsLogDAL.GetPaging(request);
            var output = new List<StudentPointsLogGetOutput>();
            var tempBoxStudent = new DataTempBox<EtStudent>();
            foreach (var p in pagingData.Item1)
            {
                var student = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId);
                output.Add(new StudentPointsLogGetOutput()
                {
                    No = p.No,
                    Ot = p.Ot,
                    StudentId = p.StudentId,
                    Type = p.Type,
                    TypeDesc = EmStudentPointsLogType.GetStudentPointsLogType(p.Type),
                    PointsDesc = EmStudentPointsLogType.GetStudentPointsLogChangPointsDesc(p.Type, p.Points),
                    StudentName = student.Name
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentPointsLogGetOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> StudentCouponsNormalGet(StudentCouponsNormalGetRequest request)
        {
            var pagingData = await _couponsDAL.CouponsStudentGetPaging(request);
            return ResponseBase.Success(new ResponsePagingDataBase<StudentCouponsNormalGetOutput>(pagingData.Item2, pagingData.Item1.Select(p =>
            {
                return new StudentCouponsNormalGetOutput()
                {
                    Id = p.Id,
                    CouponsMinLimit = p.CouponsMinLimit,
                    CouponsTitle = p.CouponsTitle,
                    CouponsType = p.CouponsType,
                    CouponsValue = p.CouponsValue,
                    CouponsTypeDesc = EmCouponsType.GetCouponsTypeDesc(p.CouponsType),
                    EffectiveTimeDesc = ComBusiness.GetCouponsEffectiveTimeDesc(p),
                    StudentName = p.StudentName,
                    CouponsValueDesc = ComBusiness.GetCouponsValueDesc(p.CouponsType, p.CouponsValue),
                    MinLimitDesc = p.CouponsMinLimit == null || p.CouponsMinLimit == 0 ? "无门槛" : $"消费满{p.CouponsMinLimit.Value.ToDecimalDesc()}元可用"
                };
            })));
        }

        public async Task<ResponseBase> StudentCouponsUsedGet(StudentCouponsUsedGetRequest request)
        {
            var pagingData = await _couponsDAL.CouponsStudentGetPaging(request);
            return ResponseBase.Success(new ResponsePagingDataBase<StudentCouponsUsedGetOutput>(pagingData.Item2, pagingData.Item1.Select(p =>
            {
                return new StudentCouponsUsedGetOutput()
                {
                    Id = p.Id,
                    CouponsMinLimit = p.CouponsMinLimit,
                    CouponsTitle = p.CouponsTitle,
                    CouponsType = p.CouponsType,
                    CouponsValue = p.CouponsValue,
                    CouponsTypeDesc = EmCouponsType.GetCouponsTypeDesc(p.CouponsType),
                    EffectiveTimeDesc = ComBusiness.GetCouponsEffectiveTimeDesc(p),
                    StudentName = p.StudentName,
                    CouponsValueDesc = ComBusiness.GetCouponsValueDesc(p.CouponsType, p.CouponsValue),
                    MinLimitDesc = p.CouponsMinLimit == null || p.CouponsMinLimit == 0 ? "无门槛" : $"消费满{p.CouponsMinLimit.Value.ToDecimalDesc()}元可用"
                };
            })));
        }

        public async Task<ResponseBase> StudentCouponsExpiredGet(StudentCouponsExpiredGetRequest request)
        {
            var pagingData = await _couponsDAL.CouponsStudentGetPaging(request);
            return ResponseBase.Success(new ResponsePagingDataBase<StudentCouponsExpiredGetOutput>(pagingData.Item2, pagingData.Item1.Select(p =>
            {
                return new StudentCouponsExpiredGetOutput()
                {
                    Id = p.Id,
                    CouponsMinLimit = p.CouponsMinLimit,
                    CouponsTitle = p.CouponsTitle,
                    CouponsType = p.CouponsType,
                    CouponsValue = p.CouponsValue,
                    CouponsTypeDesc = EmCouponsType.GetCouponsTypeDesc(p.CouponsType),
                    EffectiveTimeDesc = ComBusiness.GetCouponsEffectiveTimeDesc(p),
                    StudentName = p.StudentName,
                    CouponsValueDesc = ComBusiness.GetCouponsValueDesc(p.CouponsType, p.CouponsValue),
                    MinLimitDesc = p.CouponsMinLimit == null || p.CouponsMinLimit == 0 ? "无门槛" : $"消费满{p.CouponsMinLimit.Value.ToDecimalDesc()}元可用"
                };
            })));
        }

        public async Task<ResponseBase> StudentDetailInfo(StudentDetailInfoRequest request)
        {
            var student = (await _studentDAL.GetStudent(request.StudentId)).Student;
            var studentRelationship = await _studentRelationshipDAL.GetAllStudentRelationship();
            var learningManager = string.Empty;
            if (student.LearningManager != null)
            {
                var user = await _userDAL.GetUser(student.LearningManager.Value);
                learningManager = ComBusiness2.GetParentTeacherName(user);
            }
            var studentGetOutput = new StudentDetailInfoOutput()
            {
                Id = student.Id,
                AvatarKey = student.Avatar,
                AvatarUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, student.Avatar),
                HomeAddress = student.HomeAddress,
                Name = student.Name,
                Phone = student.Phone,
                PhoneBak = student.PhoneBak,
                SchoolName = student.SchoolName,
                Points = student.Points,
                PhoneRelationship = student.PhoneRelationship,
                PhoneBakRelationship = student.PhoneBakRelationship,
                BirthdayDesc = student.Birthday.EtmsToDateString(),
                Age = student.Age,
                PhoneBakRelationshipDesc = ComBusiness2.GetStudentRelationshipDesc(studentRelationship, student.PhoneBakRelationship, "备用号码"),
                PhoneRelationshipDesc = ComBusiness2.GetStudentRelationshipDesc(studentRelationship, student.PhoneRelationship, "手机号码"),
                LearningManager = learningManager
            };
            return ResponseBase.Success(studentGetOutput);
        }

        public async Task<ResponseBase> EvaluateTeacherGet(EvaluateTeacherGetRequest request)
        {
            var pagingData = await _classRecordDAL.GetClassRecordStudentPaging(request);
            var output = new List<EvaluateTeacherGetOutput>();
            var tempBoxUser = new DataTempBox<EtUser>();
            var tempBoxStudent = new DataTempBox<EtStudent>();
            foreach (var p in pagingData.Item1)
            {
                if (!EmClassStudentCheckStatus.CheckIsCanEvaluate(p.StudentCheckStatus))
                {
                    continue;
                }
                var etClass = await _classDAL.GetClassBucket(p.ClassId);
                var teachersDesc = await ComBusiness.GetParentTeachers(tempBoxUser, _userDAL, p.Teachers);
                var student = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId);
                output.Add(new EvaluateTeacherGetOutput()
                {
                    Id = p.Id,
                    ClassName = etClass.EtClass.Name,
                    ClassOtDesc = p.ClassOt.EtmsToDateString(),
                    StartTime = EtmsHelper.GetTimeDesc(p.StartTime),
                    EndTime = EtmsHelper.GetTimeDesc(p.EndTime),
                    IsBeEvaluate = p.IsBeEvaluate,
                    StudentCheckStatus = p.StudentCheckStatus,
                    StudentCheckStatusDesc = EmClassStudentCheckStatus.GetClassStudentCheckStatus(p.StudentCheckStatus),
                    StudentId = p.StudentId,
                    TeachersDesc = teachersDesc,
                    Week = p.Week,
                    WeekDesc = $"星期{EtmsHelper.GetWeekDesc(p.Week)}",
                    StudentName = student.Name
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<EvaluateTeacherGetOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> EvaluateTeacherGetDetail(EvaluateTeacherGetDetailRequest request)
        {
            var p = await _classRecordDAL.GetEtClassRecordStudentById(request.Id);
            var tempBoxUser = new DataTempBox<EtUser>();
            var etClass = await _classDAL.GetClassBucket(p.ClassId);
            var teachersDesc = await ComBusiness.GetParentTeachers(tempBoxUser, _userDAL, p.Teachers);
            var classRoomIdsDesc = string.Empty;
            if (!string.IsNullOrEmpty(p.ClassRoomIds))
            {
                var allClassRoom = await _classRoomDAL.GetAllClassRoom();
                classRoomIdsDesc = ComBusiness.GetDesc(allClassRoom, p.ClassRoomIds);
            }
            var student = (await _studentDAL.GetStudent(p.StudentId)).Student;
            var output = new EvaluateTeacherGetDetailOutput()
            {
                ClassContent = p.ClassContent,
                ClassId = p.ClassId,
                ClassName = etClass.EtClass.Name,
                ClassOtDesc = p.ClassOt.EtmsToDateString(),
                EndTime = EtmsHelper.GetTimeDesc(p.EndTime),
                RewardPoints = p.RewardPoints,
                StartTime = EtmsHelper.GetTimeDesc(p.StartTime),
                StudentCheckStatus = p.StudentCheckStatus,
                StudentCheckStatusDesc = EmClassStudentCheckStatus.GetClassStudentCheckStatus(p.StudentCheckStatus),
                StudentId = p.StudentId,
                TeachersDesc = teachersDesc,
                Week = p.Week,
                ClassRoomIdsDesc = classRoomIdsDesc,
                StudentName = student.Name,
                WeekDesc = $"星期{EtmsHelper.GetWeekDesc(p.Week)}",
                Id = request.Id,
                EvaluateTeachers = new List<EvaluateTeacherGetDetailTeacherOutput>()
            };
            var classRecordEvaluateTeachers = await _classRecordEvaluateDAL.GetClassRecordEvaluateTeacher(request.Id);
            var teacherIds = p.Teachers.Split(',');
            foreach (var teacherId in teacherIds)
            {
                if (string.IsNullOrEmpty(teacherId))
                {
                    continue;
                }
                var myTeacherId = teacherId.ToLong();
                var myTeacherName = await ComBusiness.GetParentTeacher(tempBoxUser, _userDAL, myTeacherId);
                if (string.IsNullOrEmpty(myTeacherName))
                {
                    continue;
                }
                var myTeacherEvaluate = classRecordEvaluateTeachers.FirstOrDefault(j => j.TeacherId == myTeacherId);
                var myTeacherEvaluateOutput = new EvaluateTeacherGetDetailTeacherOutput()
                {
                    TeacherId = myTeacherId,
                    TeacherName = myTeacherName,
                    StarValue = 0,
                    IsBeEvaluate = false,
                    EvaluateContent = string.Empty
                };
                if (myTeacherEvaluate != null)
                {
                    myTeacherEvaluateOutput.IsBeEvaluate = true;
                    myTeacherEvaluateOutput.EvaluateContent = myTeacherEvaluate.EvaluateContent;
                    myTeacherEvaluateOutput.StarValue = myTeacherEvaluate.StarValue;
                }
                output.EvaluateTeachers.Add(myTeacherEvaluateOutput);
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> EvaluateTeacherSubmit(EvaluateTeacherSubmitRequest request)
        {
            var classRecordStudent = await _classRecordDAL.GetEtClassRecordStudentById(request.Id);
            var now = DateTime.Now;
            await _classRecordEvaluateDAL.AddClassRecordEvaluateTeacher(new EtClassRecordEvaluateTeacher()
            {
                StarValue = request.StarValue,
                EvaluateContent = request.EvaluateContent,
                CheckOt = classRecordStudent.CheckOt,
                CheckUserId = classRecordStudent.CheckUserId,
                ClassId = classRecordStudent.ClassId,
                ClassOt = classRecordStudent.ClassOt,
                ClassRecordId = classRecordStudent.ClassRecordId,
                ClassRecordStudentId = classRecordStudent.Id,
                EndTime = classRecordStudent.EndTime,
                IsDeleted = classRecordStudent.IsDeleted,
                Ot = now,
                StartTime = classRecordStudent.StartTime,
                Status = classRecordStudent.Status,
                StudentId = classRecordStudent.StudentId,
                StudentType = classRecordStudent.StudentType,
                TeacherId = request.TeacherId,
                TenantId = classRecordStudent.TenantId,
                Week = classRecordStudent.Week
            });
            classRecordStudent.IsBeEvaluate = true;
            classRecordStudent.EvaluateTeacherNum += 1;
            await _classRecordDAL.EditClassRecordStudent(classRecordStudent);

            await _studentOperationLogDAL.AddStudentLog(classRecordStudent.StudentId, classRecordStudent.TenantId, $"评价老师:{request.EvaluateContent}", EmStudentOperationLogType.EvaluateTeacher);
            return ResponseBase.Success();
        }
    }
}
