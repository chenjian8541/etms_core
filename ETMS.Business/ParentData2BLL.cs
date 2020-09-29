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

        public ParentData2BLL(IClassRecordDAL classRecordDAL, IClassDAL classDAL, IUserDAL userDAL, IStudentDAL studentDAL,
            IStudentCourseDAL studentCourseDAL, ICourseDAL courseDAL, IParentStudentDAL parentStudentDAL, IOrderDAL orderDAL,
            ICouponsDAL couponsDAL, ICostDAL costDAL, IGoodsDAL goodsDAL, IIncomeLogDAL incomeLogDAL, IStudentPointsLogDAL studentPointsLogDAL,
            IHttpContextAccessor httpContextAccessor, IAppConfigurtaionServices appConfigurtaionServices, IStudentRelationshipDAL studentRelationshipDAL,
            IClassRoomDAL classRoomDAL)
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
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _classRecordDAL, _classDAL, _userDAL, _studentDAL, _studentCourseDAL,
                _courseDAL, _parentStudentDAL, _orderDAL, _couponsDAL, _costDAL, _goodsDAL, _incomeLogDAL, _studentPointsLogDAL,
                _studentRelationshipDAL, _classRoomDAL);
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
                    CourseDesc = await ComBusiness.GetCourseName(courseTempBox, _courseDAL, p.CourseId)
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ClassRecordGetParentOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> ClassRecordDetailGet(ClassRecordDetailGetRequest request)
        {
            var p = await _classRecordDAL.GetEtClassRecordStudentById(request.Id);
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
                    WeekDesc = $"星期{EtmsHelper.GetWeekDesc(p.Week)}"
                }
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentCourseGet(StudentCourseGetRequest request)
        {
            var output = new List<StudentCourseGetOutput>();
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var allStudent = await _parentStudentDAL.GetParentStudents(request.LoginTenantId, request.LoginPhone);
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
                        var course = await ComBusiness.GetCourseNameAndColor(tempBoxCourse, _courseDAL, courseId);
                        if (course == null)
                        {
                            continue;
                        }
                        var myStudentCourseDetail = new StudentCourseGetOutput()
                        {
                            CourseName = course.Item1,
                            CourseColor = course.Item2,
                            CourseId = courseId,
                            StudentId = studentId,
                            StudentName = studentInfo.Name
                        };
                        var myCourse = studentCourse.Where(p => p.CourseId == courseId);
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
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentOrderGet(StudentOrderGetRequest request)
        {
            var pagingData = await _orderDAL.GetOrderPaging(request);
            var output = new List<StudentOrderGetOutput>();
            foreach (var p in pagingData.Item1)
            {
                var student = await _studentDAL.GetStudent(p.StudentId);
                output.Add(new StudentOrderGetOutput()
                {
                    AptSum = p.AptSum,
                    ArrearsSum = p.ArrearsSum,
                    BuyCost = p.BuyCost,
                    BuyCourse = p.BuyCourse,
                    BuyGoods = p.BuyGoods,
                    No = p.No,
                    OrderType = p.OrderType,
                    OtDesc = p.Ot.EtmsToDateString(),
                    PaySum = p.PaySum,
                    Status = p.Status,
                    StatusDesc = EmOrderStatus.GetOrderStatus(p.Status),
                    StudentId = p.StudentId,
                    StudentName = student.Student.Name,
                    TotalPoints = p.TotalPoints,
                    Id = p.Id
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
            var student = (await _studentDAL.GetStudent(order.StudentId)).Student;
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
                StudentName = student.Name,
                TotalPoints = order.TotalPoints
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
                    case EmOrderProductType.Cost:
                        var myCost = await _costDAL.GetCost(myItem.ProductId);
                        productName = myCost?.Name;
                        break;
                    case EmOrderProductType.Goods:
                        var myGoods = await _goodsDAL.GetGoods(myItem.ProductId);
                        productName = myGoods?.Name;
                        break;
                    case EmOrderProductType.Course:
                        var myCourse = await _courseDAL.GetCourse(myItem.ProductId);
                        productName = myCourse?.Item1.Name;
                        break;
                }
                output.OrderGetDetailProducts.Add(new ParentOrderGetDetailProductInfo()
                {
                    BugUnit = myItem.BugUnit,
                    BuyQuantity = myItem.BuyQuantity,
                    BuyQuantityDesc = ComBusiness.GetBuyQuantityDesc(myItem.BuyQuantity, myItem.BugUnit),
                    DiscountDesc = ComBusiness.GetDiscountDesc(myItem.DiscountValue, myItem.DiscountType),
                    GiveQuantityDesc = ComBusiness.GetGiveQuantityDesc(myItem.GiveQuantity, myItem.GiveUnit),
                    ItemAptSum = myItem.ItemAptSum,
                    ItemSum = myItem.ItemSum,
                    PriceRule = myItem.PriceRule,
                    ProductTypeDesc = EmOrderProductType.GetOrderProductType(myItem.ProductType),
                    ProductName = productName,
                    Id = myItem.Id,
                    ProductType = myItem.ProductType
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

        public async Task<ResponseBase> StudentCouponsGet(StudentCouponsGetRequest request)
        {
            var pagingData = await _couponsDAL.CouponsStudentGetPaging(request);
            return ResponseBase.Success(new ResponsePagingDataBase<StudentCouponsGetOutput>(pagingData.Item2, pagingData.Item1.Select(p =>
            {
                var logStatusDesc = ComBusiness.GetCouponsLogStatusDesc(p);
                return new StudentCouponsGetOutput()
                {
                    Id = p.Id,
                    CouponsMinLimit = p.CouponsMinLimit,
                    CouponsTitle = p.CouponsTitle,
                    CouponsType = p.CouponsType,
                    CouponsValue = p.CouponsValue,
                    CouponsTypeDesc = EmCouponsType.GetCouponsTypeDesc(p.CouponsType),
                    GetTime = p.GetTime,
                    OrderNo = p.OrderNo,
                    LogStatus = logStatusDesc.Item1,
                    LogStatusDesc = logStatusDesc.Item2,
                    EffectiveTimeDesc = ComBusiness.GetCouponsEffectiveTimeDesc(p),
                    StudentName = p.StudentName,
                    CouponsValueDesc = ComBusiness.GetCouponsValueDesc(p.CouponsType, p.CouponsValue),
                    MinLimitDesc = p.CouponsMinLimit == null || p.CouponsMinLimit == 0 ? "无门槛" : $"消费满{p.CouponsMinLimit.Value.ToDecimalDesc()}元可用",
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
    }
}
