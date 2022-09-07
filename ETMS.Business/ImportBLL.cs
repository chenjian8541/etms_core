using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.External.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Event.DataContract;
using ETMS.IEventProvider;
using ETMS.Entity.Dto.External.Output;
using ETMS.Event.DataContract.Statistics;

namespace ETMS.Business
{
    public class ImportBLL : IImportBLL
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly IStudentSourceDAL _studentSourceDAL;

        private readonly IStudentRelationshipDAL _studentRelationshipDAL;

        private readonly IGradeDAL _gradeDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IOrderDAL _orderDAL;

        private readonly IIncomeLogDAL _incomeLogDAL;

        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly IClassDAL _classDAL;

        private readonly ITenantConfigDAL _tenantConfigDAL;

        private readonly IStudentExtendFieldDAL _studentExtendFieldDAL;

        public ImportBLL(IHttpContextAccessor httpContextAccessor, IAppConfigurtaionServices appConfigurtaionServices,
            IStudentSourceDAL studentSourceDAL, IStudentRelationshipDAL studentRelationshipDAL, IGradeDAL gradeDAL, ISysTenantDAL sysTenantDAL,
            IStudentDAL studentDAL, IEventPublisher eventPublisher, IUserOperationLogDAL userOperationLogDAL, ICourseDAL courseDAL, IOrderDAL orderDAL,
            IIncomeLogDAL incomeLogDAL, IStudentCourseDAL studentCourseDAL, IClassDAL classDAL, ITenantConfigDAL tenantConfigDAL,
            IStudentExtendFieldDAL studentExtendFieldDAL)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._studentSourceDAL = studentSourceDAL;
            this._studentRelationshipDAL = studentRelationshipDAL;
            this._gradeDAL = gradeDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._studentDAL = studentDAL;
            this._eventPublisher = eventPublisher;
            this._userOperationLogDAL = userOperationLogDAL;
            this._courseDAL = courseDAL;
            this._orderDAL = orderDAL;
            this._incomeLogDAL = incomeLogDAL;
            this._studentCourseDAL = studentCourseDAL;
            this._classDAL = classDAL;
            this._tenantConfigDAL = tenantConfigDAL;
            this._studentExtendFieldDAL = studentExtendFieldDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentSourceDAL, _studentRelationshipDAL,
                _gradeDAL, _studentDAL, _userOperationLogDAL, _courseDAL, _orderDAL, _incomeLogDAL, _studentCourseDAL, _classDAL,
                _tenantConfigDAL, _studentExtendFieldDAL);
        }

        public async Task<List<EtStudentExtendField>> StudentExtendFieldAllGet()
        {
            return await _studentExtendFieldDAL.GetAllStudentExtendField();
        }

        public async Task<ResponseBase> GetImportStudentExcelTemplate(GetImportStudentExcelTemplateRequest request)
        {
            var tenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            var checkImportStudentTemplateFileResult = ExcelLib.CheckImportStudentExcelTemplate(tenant.TenantCode, _appConfigurtaionServices.AppSettings.StaticFilesConfig.ServerPath);
            if (!checkImportStudentTemplateFileResult.IsExist)
            {
                var studentRelationshipAll = await _studentRelationshipDAL.GetAllStudentRelationship();
                if (studentRelationshipAll.Count > 100)
                {
                    studentRelationshipAll = studentRelationshipAll.Take(100).ToList();
                }
                var studentSourceAll = await _studentSourceDAL.GetAllStudentSource();
                if (studentSourceAll.Count > 100)
                {
                    studentSourceAll = studentSourceAll.Take(100).ToList();
                }
                var studentExtendFieldAll = await _studentExtendFieldDAL.GetAllStudentExtendField();
                if (studentExtendFieldAll.Count > 100)
                {
                    studentExtendFieldAll = studentExtendFieldAll.Take(100).ToList();
                }
                var gradeAll = await _gradeDAL.GetAllGrade();
                if (gradeAll.Count > 100)
                {
                    gradeAll = gradeAll.Take(100).ToList();
                }
                ExcelLib.GenerateImportStudentExcelTemplate(new ImportStudentExcelTemplateRequest()
                {
                    CheckResult = checkImportStudentTemplateFileResult,
                    GradeAll = gradeAll,
                    StudentRelationshipAll = studentRelationshipAll,
                    StudentSourceAll = studentSourceAll,
                    StudentExtendFieldAll = studentExtendFieldAll
                }); ;
            }
            return ResponseBase.Success(UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, checkImportStudentTemplateFileResult.UrlKey));
        }

        public async Task<ResponseBase> ImportStudent(ImportStudentRequest request)
        {
            var msg = request.Validate();
            if (!string.IsNullOrEmpty(msg))
            {
                return ResponseBase.CommonError(msg);
            }
            var studentList = new List<EtStudent>();
            var now = DateTime.Now;
            var gradeAll = await _gradeDAL.GetAllGrade();
            var studentRelationshipAll = await _studentRelationshipDAL.GetAllStudentRelationship();
            var studentSourceAll = await _studentSourceDAL.GetAllStudentSource();
            var pwd = string.Empty;
            var config = await _tenantConfigDAL.GetTenantConfig();
            if (!string.IsNullOrEmpty(config.StudentConfig.InitialPassword))
            {
                pwd = CryptogramHelper.Encrypt3DES(config.StudentConfig.InitialPassword, SystemConfig.CryptogramConfig.Key);
            }
            foreach (var p in request.ImportStudents)
            {
                if (await _studentDAL.ExistStudent(p.StudentName, p.Phone))
                {
                    continue;
                }

                var phoneRelationship = 0L;
                if (!string.IsNullOrEmpty(p.PhoneRelationshipDesc))
                {
                    var myPhoneRelationship = studentRelationshipAll.FirstOrDefault(j => j.Name == p.PhoneRelationshipDesc);
                    if (myPhoneRelationship != null)
                    {
                        phoneRelationship = myPhoneRelationship.Id;
                    }
                }

                byte? gender = null;
                if (!string.IsNullOrEmpty(p.GenderDesc))
                {
                    gender = p.GenderDesc.Trim() == "男" ? EmGender.Man : EmGender.Woman;
                }

                long? gradeId = null;
                if (!string.IsNullOrEmpty(p.GradeDesc))
                {
                    var myGenderDesc = gradeAll.FirstOrDefault(j => j.Name == p.GradeDesc);
                    gradeId = myGenderDesc?.Id;
                }

                long? sourceId = null;
                if (!string.IsNullOrEmpty(p.SourceDesc))
                {
                    var mySourceDesc = studentSourceAll.FirstOrDefault(j => j.Name == p.SourceDesc);
                    sourceId = mySourceDesc?.Id;
                }
                int? birthdayMonth = null;
                int? birthdayDay = null;
                int? birthdayTag = null;
                if (p.Birthday != null)
                {
                    birthdayMonth = p.Birthday.Value.Month;
                    birthdayDay = p.Birthday.Value.Day;
                    birthdayTag = EtmsHelper3.GetBirthdayTag(p.Birthday.Value);
                }
                var myAgeResult = p.Birthday.EtmsGetAge();
                studentList.Add(new EtStudent()
                {
                    BirthdayMonth = birthdayMonth,
                    BirthdayDay = birthdayDay,
                    BirthdayTag = birthdayTag,
                    Age = myAgeResult?.Item1,
                    AgeMonth = myAgeResult?.Item2,
                    Name = p.StudentName,
                    Avatar = string.Empty,
                    Birthday = p.Birthday,
                    CardNo = p.CardNo,
                    CreateBy = request.LoginUserId,
                    EndClassOt = null,
                    Gender = gender,
                    GradeId = gradeId,
                    HomeAddress = p.HomeAddress,
                    IntentionLevel = EmStudentIntentionLevel.Middle,
                    IsBindingWechat = EmIsBindingWechat.No,
                    IsDeleted = EmIsDeleted.Normal,
                    LastJobProcessTime = now,
                    LastTrackTime = null,
                    LearningManager = null,
                    NextTrackTime = null,
                    Ot = now.Date,
                    Phone = p.Phone,
                    PhoneBak = p.PhoneBak,
                    PhoneBakRelationship = null,
                    PhoneRelationship = phoneRelationship,
                    Points = 0,
                    Remark = p.Remark,
                    SchoolName = p.SchoolName,
                    SourceId = sourceId,
                    StudentType = EmStudentType.HiddenStudent,
                    Tags = string.Empty,
                    TenantId = request.LoginTenantId,
                    TrackStatus = EmStudentTrackStatus.NotTrack,
                    TrackUser = null,
                    Password = pwd
                });
            }
            if (studentList.Count > 0)
            {
                _studentDAL.AddStudent(studentList);
                SyncStatisticsStudentInfo(new StatisticsStudentCountEvent(request.LoginTenantId)
                {
                    Time = now
                }, request, now.Date, true);
                var phones = studentList.Select(p => p.Phone).ToList();
                _eventPublisher.Publish(new SyncParentStudentsEvent(request.LoginTenantId)
                {
                    Phones = phones.ToArray()
                });

                if (request.StudentExtendFieldItems != null && request.StudentExtendFieldItems.Count > 0)
                {
                    _eventPublisher.Publish(new ImportExtendFieldExcelEvent(request.LoginTenantId)
                    {
                        Students = studentList,
                        StudentExtendFieldItems = request.StudentExtendFieldItems
                    });
                }
                await _userOperationLogDAL.AddUserLog(request, $"导入潜在学员-成功导入了{studentList.Count}位学员", EmUserOperationType.StudentManage);
            }
            _eventPublisher.Publish(new SysTenantStatistics2Event(request.LoginTenantId));
            return ResponseBase.Success(new ImportStudentOutput()
            {
                SuccessCount = studentList.Count
            });
        }

        private void SyncStatisticsStudentInfo(StatisticsStudentCountEvent studentCountEvent, RequestBase request, DateTime ot, bool isChangeStudentSource)
        {
            if (studentCountEvent != null)
            {
                _eventPublisher.Publish(studentCountEvent);
            }
            if (isChangeStudentSource)
            {
                _eventPublisher.Publish(new StatisticsStudentEvent(request.LoginTenantId) { OpType = EmStatisticsStudentType.StudentSource, StatisticsDate = ot });
            }
            _eventPublisher.Publish(new StatisticsStudentEvent(request.LoginTenantId) { OpType = EmStatisticsStudentType.StudentType, StatisticsDate = ot });
        }

        public async Task<ResponseBase> GetImportCourseTimesExcelTemplate(GetImportCourseTimesExcelTemplateRequest request)
        {
            var tenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            var checkImportCourseTimesExcelTemplate = ExcelLib.CheckImportCourseTimesExcelTemplate(tenant.TenantCode, _appConfigurtaionServices.AppSettings.StaticFilesConfig.ServerPath);
            if (!checkImportCourseTimesExcelTemplate.IsExist)
            {
                var studentRelationshipAll = await _studentRelationshipDAL.GetAllStudentRelationship();
                if (studentRelationshipAll.Count > 100)
                {
                    studentRelationshipAll = studentRelationshipAll.Take(100).ToList();
                }
                var studentSourceAll = await _studentSourceDAL.GetAllStudentSource();
                if (studentSourceAll.Count > 100)
                {
                    studentSourceAll = studentSourceAll.Take(100).ToList();
                }
                var studentExtendFieldAll = await _studentExtendFieldDAL.GetAllStudentExtendField();
                if (studentExtendFieldAll.Count > 100)
                {
                    studentExtendFieldAll = studentExtendFieldAll.Take(100).ToList();
                }
                var gradeAll = await _gradeDAL.GetAllGrade();
                if (gradeAll.Count > 100)
                {
                    gradeAll = gradeAll.Take(100).ToList();
                }
                ExcelLib.GenerateImportCourseTimesExcelTemplate(new ImportCourseHeadDescTimesExcelTemplateRequest()
                {
                    CheckResult = checkImportCourseTimesExcelTemplate,
                    PayTypeAll = EmPayType.GetPayTypeAll(),
                    GradeAll = gradeAll,
                    StudentRelationshipAll = studentRelationshipAll,
                    StudentSourceAll = studentSourceAll,
                    StudentExtendFieldAll = studentExtendFieldAll
                });
            }
            return ResponseBase.Success(UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, checkImportCourseTimesExcelTemplate.UrlKey));
        }

        public async Task<ResponseBase> ImportCourseTimes(ImportCourseTimesRequest request)
        {
            var msg = request.Validate();
            if (!string.IsNullOrEmpty(msg))
            {
                return ResponseBase.CommonError(msg);
            }
            var now = DateTime.Now;
            var remark = $"{now.EtmsToMinuteString()}导入学员课程";
            var addStudentCount = 0;
            var gradeAll = await _gradeDAL.GetAllGrade();
            var studentRelationshipAll = await _studentRelationshipDAL.GetAllStudentRelationship();
            var studentSourceAll = await _studentSourceDAL.GetAllStudentSource();
            var pwd = string.Empty;
            var config = await _tenantConfigDAL.GetTenantConfig();
            if (!string.IsNullOrEmpty(config.StudentConfig.InitialPassword))
            {
                pwd = CryptogramHelper.Encrypt3DES(config.StudentConfig.InitialPassword, SystemConfig.CryptogramConfig.Key);
            }
            if (!config.TenantOtherConfig.IsAllowStudentOverpayment)
            {
                var studentOverpaymentLog = request.ImportCourseTimess.FirstOrDefault(p => p.PaySum > p.AptSum);
                if (studentOverpaymentLog != null)
                {
                    return ResponseBase.CommonError("支付金额应该小于等于应付金额");
                }
            }
            var studentExtendInfos = new List<EtStudentExtendInfo>();
            var studentExtendFieldAll = await _studentExtendFieldDAL.GetAllStudentExtendField();
            foreach (var p in request.ImportCourseTimess)
            {
                var student = await _studentDAL.GetStudent(p.StudentName, p.Phone);
                if (student == null)
                {
                    var phoneRelationship = 0L;
                    if (!string.IsNullOrEmpty(p.PhoneRelationshipDesc))
                    {
                        var myPhoneRelationship = studentRelationshipAll.FirstOrDefault(j => j.Name == p.PhoneRelationshipDesc);
                        if (myPhoneRelationship != null)
                        {
                            phoneRelationship = myPhoneRelationship.Id;
                        }
                    }

                    byte? gender = null;
                    if (!string.IsNullOrEmpty(p.GenderDesc))
                    {
                        gender = p.GenderDesc.Trim() == "男" ? EmGender.Man : EmGender.Woman;
                    }

                    long? gradeId = null;
                    if (!string.IsNullOrEmpty(p.GradeDesc))
                    {
                        var myGenderDesc = gradeAll.FirstOrDefault(j => j.Name == p.GradeDesc);
                        gradeId = myGenderDesc?.Id;
                    }

                    long? sourceId = null;
                    if (!string.IsNullOrEmpty(p.SourceDesc))
                    {
                        var mySourceDesc = studentSourceAll.FirstOrDefault(j => j.Name == p.SourceDesc);
                        sourceId = mySourceDesc?.Id;
                    }
                    int? birthdayMonth = null;
                    int? birthdayDay = null;
                    int? birthdayTag = null;
                    if (p.Birthday != null)
                    {
                        birthdayMonth = p.Birthday.Value.Month;
                        birthdayDay = p.Birthday.Value.Day;
                        birthdayTag = EtmsHelper3.GetBirthdayTag(p.Birthday.Value);
                    }
                    var myAgeResult = p.Birthday.EtmsGetAge();
                    student = new EtStudent()
                    {
                        BirthdayMonth = birthdayMonth,
                        BirthdayDay = birthdayDay,
                        BirthdayTag = birthdayTag,
                        Age = myAgeResult?.Item1,
                        AgeMonth = myAgeResult?.Item2,
                        Name = p.StudentName,
                        Avatar = string.Empty,
                        Birthday = p.Birthday,
                        CardNo = p.CardNo,
                        CreateBy = request.LoginUserId,
                        EndClassOt = null,
                        Gender = gender,
                        GradeId = gradeId,
                        HomeAddress = p.HomeAddress,
                        IntentionLevel = EmStudentIntentionLevel.Middle,
                        IsBindingWechat = EmIsBindingWechat.No,
                        IsDeleted = EmIsDeleted.Normal,
                        LastJobProcessTime = now,
                        LastTrackTime = null,
                        LearningManager = null,
                        NextTrackTime = null,
                        Ot = now.Date,
                        Phone = p.Phone,
                        PhoneBak = p.PhoneBak,
                        PhoneBakRelationship = null,
                        PhoneRelationship = phoneRelationship,
                        Points = 0,
                        Remark = string.IsNullOrEmpty(p.Remark) ? remark : p.Remark,
                        SchoolName = p.SchoolName,
                        SourceId = sourceId,
                        StudentType = EmStudentType.ReadingStudent,
                        Tags = string.Empty,
                        TenantId = request.LoginTenantId,
                        TrackStatus = EmStudentTrackStatus.NotTrack,
                        TrackUser = null,
                        Password = pwd
                    };
                    await _studentDAL.AddStudentNotUpCache(student);
                    addStudentCount++;
                    if (p.ExtendInfoList != null && p.ExtendInfoList.Count > 0 && studentExtendFieldAll != null && studentExtendFieldAll.Count > 0)
                    {
                        foreach (var j in p.ExtendInfoList)
                        {
                            var tempFile = studentExtendFieldAll.FirstOrDefault(a => a.DisplayName == j.DisplayName);
                            if (tempFile != null)
                            {
                                studentExtendInfos.Add(new EtStudentExtendInfo()
                                {
                                    ExtendFieldId = tempFile.Id,
                                    IsDeleted = EmIsDeleted.Normal,
                                    Remark = string.Empty,
                                    StudentId = student.Id,
                                    TenantId = request.LoginTenantId,
                                    Value1 = j.Value
                                });
                            }
                        }
                    }
                }
                else
                {
                    await _studentDAL.StudentEnrolmentEventChangeInfo(student.Id, 0, EmStudentType.ReadingStudent);
                }

                var course = await _courseDAL.GetCourse(p.CourseName, p.CourseType);
                decimal price = 0;
                if (p.AptSum > 0 && p.BuyQuantity > 0)
                {
                    price = Math.Round(p.AptSum / p.BuyQuantity, 2);
                }
                if (course == null)
                {
                    var priceRule = new EtCoursePriceRule()
                    {
                        CourseId = 0,
                        IsDeleted = EmIsDeleted.Normal,
                        Name = "导入时自动生成的价格规则",
                        Price = price,
                        PriceType = EmCoursePriceType.ClassTimes,
                        PriceUnit = EmCourseUnit.ClassTimes,
                        Quantity = 1,
                        TotalPrice = price,
                        TenantId = request.LoginTenantId
                    };
                    course = new EtCourse()
                    {
                        IsDeleted = EmIsDeleted.Normal,
                        Name = p.CourseName,
                        Ot = DateTime.Now,
                        Remark = "导入时自动生成的课程",
                        StyleColor = "#409EFF",
                        TenantId = request.LoginTenantId,
                        Type = p.CourseType,
                        UserId = request.LoginUserId,
                        PriceType = EmCoursePriceType.ClassTimes,
                        Status = EmProductStatus.Enabled
                    };
                    await _courseDAL.AddCourse(course, new List<EtCoursePriceRule>() { priceRule });
                }

                var no = OrderNumberLib.EnrolmentOrderNumber();
                var arrearsSum = p.AptSum - p.PaySum;
                var addToAccountRechargeMoney = 0M;
                if (arrearsSum < 0)
                {
                    arrearsSum = 0;
                    if (config.TenantOtherConfig.IsAllowStudentOverpayment &&
                        config.TenantOtherConfig.StudentOverpaymentProcessType == EmStudentOverpaymentProcessType.GoStudentAccountRecharge)
                    {
                        addToAccountRechargeMoney = p.PaySum - p.AptSum;
                    }
                }
                var status = EmOrderStatus.Normal;
                if (p.PaySum == 0 && p.AptSum > 0)
                {
                    status = EmOrderStatus.Unpaid;
                }
                else if (p.AptSum > p.PaySum)
                {
                    status = EmOrderStatus.MakeUpMoney;
                }
                var order = new EtOrder()
                {
                    TenantId = request.LoginTenantId,
                    UserId = request.LoginUserId,
                    CouponsStudentGetIds = string.Empty,
                    CouponsIds = string.Empty,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = p.OrderOt,
                    Remark = remark,
                    TotalPoints = 0,
                    No = no,
                    StudentId = student.Id,
                    OrderType = EmOrderType.StudentEnrolment,
                    AptSum = p.AptSum,
                    ArrearsSum = arrearsSum,
                    BuyCost = string.Empty,
                    BuyCourse = course.Name,
                    BuyGoods = string.Empty,
                    CommissionUser = string.Empty,
                    PaySum = p.PaySum,
                    Sum = p.AptSum,
                    Status = status,
                    OrderSource = EmOrderSource.OrderImport,
                    CreateOt = now
                };

                var orderDetail = new EtOrderDetail()
                {
                    BugUnit = EmCourseUnit.ClassTimes,
                    OrderNo = no,
                    Ot = p.OrderOt,
                    Price = price,
                    BuyQuantity = p.BuyQuantity,
                    DiscountType = EmDiscountType.Nothing,
                    DiscountValue = 0,
                    GiveQuantity = p.GiveQuantity,
                    GiveUnit = EmCourseUnit.ClassTimes,
                    IsDeleted = EmIsDeleted.Normal,
                    ItemAptSum = p.AptSum,
                    ItemSum = p.AptSum,
                    PriceRule = string.Empty,
                    ProductId = course.Id,
                    ProductType = EmProductType.Course,
                    Remark = remark,
                    Status = EmOrderStatus.Normal,
                    TenantId = request.LoginTenantId,
                    UserId = request.LoginUserId,
                    StudentId = student.Id,
                    OrderType = EmOrderType.StudentEnrolment
                };

                //订单
                var orderId = await _orderDAL.AddOrder(order, new List<EtOrderDetail>() { orderDetail });

                var payType = EmPayType.Cash;
                if (p.PaySum > 0)
                {
                    if (!string.IsNullOrEmpty(p.PayTypeName))
                    {
                        payType = EmPayType.GetPayType(p.PayTypeName);
                    }
                    var incomeLog = new EtIncomeLog()
                    {
                        AccountNo = string.Empty,
                        IsDeleted = EmIsDeleted.Normal,
                        No = no,
                        Ot = p.OrderOt,
                        PayType = payType,
                        ProjectType = EmIncomeLogProjectType.StudentEnrolment,
                        Remark = remark,
                        RepealOt = null,
                        OrderId = orderId,
                        RepealUserId = null,
                        Status = EmIncomeLogStatus.Normal,
                        Sum = p.PaySum,
                        TenantId = request.LoginTenantId,
                        Type = EmIncomeLogType.AccountIn,
                        UserId = request.LoginUserId,
                        CreateOt = now
                    };
                    await _incomeLogDAL.AddIncomeLog(incomeLog);
                }

                var useQuantity = p.BuyQuantity + p.GiveQuantity - p.SurplusQuantity;
                var courseDetailPrice = ComBusiness2.GetOneClassDeSum(p.AptSum, EmDeClassTimesType.ClassTimes,
                    p.BuyQuantity + p.GiveQuantity, 0, null, null);
                var studentCourseDetail = new EtStudentCourseDetail()
                {
                    BugUnit = EmCourseUnit.ClassTimes,
                    BuyQuantity = p.BuyQuantity,
                    CourseId = course.Id,
                    StudentId = student.Id,
                    TenantId = request.LoginTenantId,
                    OrderNo = no,
                    IsDeleted = EmIsDeleted.Normal,
                    DeType = EmDeClassTimesType.ClassTimes,
                    EndCourseRemark = string.Empty,
                    EndCourseTime = null,
                    EndCourseUser = null,
                    GiveQuantity = p.GiveQuantity,
                    GiveUnit = EmCourseUnit.ClassTimes,
                    Price = courseDetailPrice,
                    StartTime = null,
                    EndTime = p.EndTime,
                    Status = EmStudentCourseStatus.Normal,
                    SurplusQuantity = p.SurplusQuantity,
                    SurplusSmallQuantity = 0,
                    TotalMoney = p.AptSum,
                    UseQuantity = useQuantity,
                    UseUnit = EmCourseUnit.ClassTimes,
                    OrderId = orderId
                };

                _studentCourseDAL.AddStudentCourseDetail(new List<EtStudentCourseDetail>() { studentCourseDetail });

                if (course.Type == EmCourseType.OneToOne || p.CourseType == EmCourseType.OneToOne)
                {
                    await AddOneToOneClass(course.Id, course.Name, student.Name, now, request.LoginTenantId, orderId, student.Id, request.LoginUserId);
                }

                if (addToAccountRechargeMoney > 0)
                {
                    _eventPublisher.Publish(new StudentAutoAddAccountRechargeEvent(request.LoginTenantId)
                    {
                        StudentId = order.StudentId,
                        AddMoney = addToAccountRechargeMoney,
                        RechargeLogType = EmStudentAccountRechargeLogType.StudentImportOverpayment,
                        PayType = payType,
                        UserId = order.UserId,
                        OrderNo = order.No,
                        OrderId = order.Id,
                        Remark = order.Remark
                    });
                }

                _eventPublisher.Publish(new StudentCourseAnalyzeEvent(request.LoginTenantId)
                {
                    StudentId = student.Id
                });
                _eventPublisher.Publish(new SyncStudentStudentCourseIdsEvent(request.LoginTenantId, student.Id));
            }

            if (studentExtendInfos.Any())
            {
                _studentDAL.AddStudentExtend(studentExtendInfos);
            }

            SyncStatisticsStudentInfo(new StatisticsStudentCountEvent(request.LoginTenantId)
            {
                Time = now
            }, request, now.Date, true);

            var phones = request.ImportCourseTimess.Select(p => p.Phone).ToList();
            _eventPublisher.Publish(new SyncParentStudentsEvent(request.LoginTenantId)
            {
                Phones = phones.ToArray()
            });

            var allDate = request.ImportCourseTimess.Select(p => p.OrderOt).Distinct();
            foreach (var myDate in allDate)
            {
                _eventPublisher.Publish(new StatisticsSalesProductEvent(request.LoginTenantId)
                {
                    StatisticsDate = myDate
                });
                _eventPublisher.Publish(new StatisticsFinanceIncomeEvent(request.LoginTenantId)
                {
                    StatisticsDate = myDate
                });
                _eventPublisher.Publish(new StatisticsSalesCourseEvent(request.LoginTenantId)
                {
                    StatisticsDate = myDate
                });
            }

            _eventPublisher.Publish(new SysTenantStatistics2Event(request.LoginTenantId));
            _eventPublisher.Publish(new SysTenantStatisticsWeekAndMonthEvent(request.LoginTenantId));
            return ResponseBase.Success(new ImportCourseOutput()
            {
                SuccessCount = request.ImportCourseTimess.Count
            });
        }

        public async Task<ResponseBase> GetImportCourseDayExcelTemplate(GetImportCourseDayExcelTemplateRequest request)
        {
            var tenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            var checkImportCourseDayExcelTemplate = ExcelLib.CheckImportCourseDayExcelTemplate(tenant.TenantCode, _appConfigurtaionServices.AppSettings.StaticFilesConfig.ServerPath);
            if (!checkImportCourseDayExcelTemplate.IsExist)
            {
                var studentRelationshipAll = await _studentRelationshipDAL.GetAllStudentRelationship();
                if (studentRelationshipAll.Count > 100)
                {
                    studentRelationshipAll = studentRelationshipAll.Take(100).ToList();
                }
                var studentSourceAll = await _studentSourceDAL.GetAllStudentSource();
                if (studentSourceAll.Count > 100)
                {
                    studentSourceAll = studentSourceAll.Take(100).ToList();
                }
                var studentExtendFieldAll = await _studentExtendFieldDAL.GetAllStudentExtendField();
                if (studentExtendFieldAll.Count > 100)
                {
                    studentExtendFieldAll = studentExtendFieldAll.Take(100).ToList();
                }
                var gradeAll = await _gradeDAL.GetAllGrade();
                if (gradeAll.Count > 100)
                {
                    gradeAll = gradeAll.Take(100).ToList();
                }
                ExcelLib.GenerateImportCourseDayExcelTemplate(new ImportCourseHeadDescDayExcelTemplateRequest()
                {
                    CheckResult = checkImportCourseDayExcelTemplate,
                    PayTypeAll = EmPayType.GetPayTypeAll(),
                    GradeAll = gradeAll,
                    StudentRelationshipAll = studentRelationshipAll,
                    StudentSourceAll = studentSourceAll,
                    StudentExtendFieldAll = studentExtendFieldAll
                });
            }
            return ResponseBase.Success(UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, checkImportCourseDayExcelTemplate.UrlKey));
        }

        public async Task<ResponseBase> ImportCourseDay(ImportCourseDayRequest request)
        {
            var msg = request.Validate();
            if (!string.IsNullOrEmpty(msg))
            {
                return ResponseBase.CommonError(msg);
            }
            var now = DateTime.Now;
            var remark = $"{now.EtmsToMinuteString()}导入学员课程";
            var addStudentCount = 0;
            var gradeAll = await _gradeDAL.GetAllGrade();
            var studentRelationshipAll = await _studentRelationshipDAL.GetAllStudentRelationship();
            var studentSourceAll = await _studentSourceDAL.GetAllStudentSource();
            var pwd = string.Empty;
            var config = await _tenantConfigDAL.GetTenantConfig();
            if (!string.IsNullOrEmpty(config.StudentConfig.InitialPassword))
            {
                pwd = CryptogramHelper.Encrypt3DES(config.StudentConfig.InitialPassword, SystemConfig.CryptogramConfig.Key);
            }
            if (!config.TenantOtherConfig.IsAllowStudentOverpayment)
            {
                var studentOverpaymentLog = request.ImportCourseDays.FirstOrDefault(p => p.PaySum > p.AptSum);
                if (studentOverpaymentLog != null)
                {
                    return ResponseBase.CommonError("支付金额应该小于等于应付金额");
                }
            }
            var studentExtendInfos = new List<EtStudentExtendInfo>();
            var studentExtendFieldAll = await _studentExtendFieldDAL.GetAllStudentExtendField();
            foreach (var p in request.ImportCourseDays)
            {
                var student = await _studentDAL.GetStudent(p.StudentName, p.Phone);
                if (student == null)
                {
                    var phoneRelationship = 0L;
                    if (!string.IsNullOrEmpty(p.PhoneRelationshipDesc))
                    {
                        var myPhoneRelationship = studentRelationshipAll.FirstOrDefault(j => j.Name == p.PhoneRelationshipDesc);
                        if (myPhoneRelationship != null)
                        {
                            phoneRelationship = myPhoneRelationship.Id;
                        }
                    }

                    byte? gender = null;
                    if (!string.IsNullOrEmpty(p.GenderDesc))
                    {
                        gender = p.GenderDesc.Trim() == "男" ? EmGender.Man : EmGender.Woman;
                    }

                    long? gradeId = null;
                    if (!string.IsNullOrEmpty(p.GradeDesc))
                    {
                        var myGenderDesc = gradeAll.FirstOrDefault(j => j.Name == p.GradeDesc);
                        gradeId = myGenderDesc?.Id;
                    }

                    long? sourceId = null;
                    if (!string.IsNullOrEmpty(p.SourceDesc))
                    {
                        var mySourceDesc = studentSourceAll.FirstOrDefault(j => j.Name == p.SourceDesc);
                        sourceId = mySourceDesc?.Id;
                    }
                    int? birthdayMonth = null;
                    int? birthdayDay = null;
                    int? birthdayTag = null;
                    if (p.Birthday != null)
                    {
                        birthdayMonth = p.Birthday.Value.Month;
                        birthdayDay = p.Birthday.Value.Day;
                        birthdayTag = EtmsHelper3.GetBirthdayTag(p.Birthday.Value);
                    }
                    var myAgeResult = p.Birthday.EtmsGetAge();
                    student = new EtStudent()
                    {
                        BirthdayMonth = birthdayMonth,
                        BirthdayDay = birthdayDay,
                        BirthdayTag = birthdayTag,
                        Age = myAgeResult?.Item1,
                        AgeMonth = myAgeResult?.Item2,
                        Name = p.StudentName,
                        Avatar = string.Empty,
                        Birthday = p.Birthday,
                        CardNo = p.CardNo,
                        CreateBy = request.LoginUserId,
                        EndClassOt = null,
                        Gender = gender,
                        GradeId = gradeId,
                        HomeAddress = p.HomeAddress,
                        IntentionLevel = EmStudentIntentionLevel.Middle,
                        IsBindingWechat = EmIsBindingWechat.No,
                        IsDeleted = EmIsDeleted.Normal,
                        LastJobProcessTime = now,
                        LastTrackTime = null,
                        LearningManager = null,
                        NextTrackTime = null,
                        Ot = now.Date,
                        Phone = p.Phone,
                        PhoneBak = p.PhoneBak,
                        PhoneBakRelationship = null,
                        PhoneRelationship = phoneRelationship,
                        Points = 0,
                        Remark = string.IsNullOrEmpty(p.Remark) ? remark : p.Remark,
                        SchoolName = p.SchoolName,
                        SourceId = sourceId,
                        StudentType = EmStudentType.ReadingStudent,
                        Tags = string.Empty,
                        TenantId = request.LoginTenantId,
                        TrackStatus = EmStudentTrackStatus.NotTrack,
                        TrackUser = null,
                        Password = pwd
                    };
                    await _studentDAL.AddStudentNotUpCache(student);
                    addStudentCount++;
                    if (p.ExtendInfoList != null && p.ExtendInfoList.Count > 0 && studentExtendFieldAll != null && studentExtendFieldAll.Count > 0)
                    {
                        foreach (var j in p.ExtendInfoList)
                        {
                            var tempFile = studentExtendFieldAll.FirstOrDefault(a => a.DisplayName == j.DisplayName);
                            if (tempFile != null)
                            {
                                studentExtendInfos.Add(new EtStudentExtendInfo()
                                {
                                    ExtendFieldId = tempFile.Id,
                                    IsDeleted = EmIsDeleted.Normal,
                                    Remark = string.Empty,
                                    StudentId = student.Id,
                                    TenantId = request.LoginTenantId,
                                    Value1 = j.Value
                                });
                            }
                        }
                    }
                }
                else
                {
                    await _studentDAL.StudentEnrolmentEventChangeInfo(student.Id, 0, EmStudentType.ReadingStudent);
                }

                var course = await _courseDAL.GetCourse(p.CourseName, p.CourseType);
                var dayDeffBuy = EtmsHelper.GetDffTimeAboutBuyQuantity(p.StartTime, p.EndTime);
                var buyUnit = EmCourseUnit.Month;
                var priceUnit = EmCoursePriceType.Month;
                var priceTypeDesc = "按月";
                var buyQuantity = dayDeffBuy.Item1;
                if (dayDeffBuy.Item1 == 0)
                {
                    buyUnit = EmCourseUnit.Day;
                    priceUnit = EmCoursePriceType.Day;
                    priceTypeDesc = "按天";
                    buyQuantity = dayDeffBuy.Item2;
                }
                decimal price = 0;
                if (buyQuantity > 0)
                {
                    price = Math.Round(p.AptSum / buyQuantity, 2);
                }
                if (course == null)
                {
                    var priceRule = new EtCoursePriceRule()
                    {
                        CourseId = 0,
                        IsDeleted = EmIsDeleted.Normal,
                        Name = "导入时自动生成的价格规则",
                        Price = price,
                        PriceType = priceUnit,
                        PriceUnit = buyUnit,
                        Quantity = 1,
                        TotalPrice = price,
                        TenantId = request.LoginTenantId
                    };
                    course = new EtCourse()
                    {
                        IsDeleted = EmIsDeleted.Normal,
                        Name = p.CourseName,
                        Ot = DateTime.Now,
                        Remark = "导入时自动生成的课程",
                        StyleColor = "#409EFF",
                        TenantId = request.LoginTenantId,
                        Type = p.CourseType,
                        UserId = request.LoginUserId,
                        PriceType = priceUnit,
                        PriceTypeDesc = priceTypeDesc,
                        Status = EmProductStatus.Enabled
                    };
                    await _courseDAL.AddCourse(course, new List<EtCoursePriceRule>() { priceRule });
                }

                var no = OrderNumberLib.EnrolmentOrderNumber();
                var arrearsSum = p.AptSum - p.PaySum;
                var addToAccountRechargeMoney = 0M;
                if (arrearsSum < 0)
                {
                    arrearsSum = 0;
                    if (config.TenantOtherConfig.IsAllowStudentOverpayment &&
                        config.TenantOtherConfig.StudentOverpaymentProcessType == EmStudentOverpaymentProcessType.GoStudentAccountRecharge)
                    {
                        addToAccountRechargeMoney = p.PaySum - p.AptSum;
                    }
                }
                var status = EmOrderStatus.Normal;
                if (p.PaySum == 0 && p.AptSum > 0)
                {
                    status = EmOrderStatus.Unpaid;
                }
                else if (p.AptSum > p.PaySum)
                {
                    status = EmOrderStatus.MakeUpMoney;
                }
                var order = new EtOrder()
                {
                    TenantId = request.LoginTenantId,
                    UserId = request.LoginUserId,
                    CouponsStudentGetIds = string.Empty,
                    CouponsIds = string.Empty,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = p.OrderOt,
                    Remark = remark,
                    TotalPoints = 0,
                    No = no,
                    StudentId = student.Id,
                    OrderType = EmOrderType.StudentEnrolment,
                    AptSum = p.AptSum,
                    ArrearsSum = arrearsSum,
                    BuyCost = string.Empty,
                    BuyCourse = course.Name,
                    BuyGoods = string.Empty,
                    CommissionUser = string.Empty,
                    PaySum = p.PaySum,
                    Sum = p.AptSum,
                    Status = status,
                    OrderSource = EmOrderSource.OrderImport,
                    CreateOt = now
                };

                var orderDetail = new EtOrderDetail()
                {
                    BugUnit = buyUnit,
                    OrderNo = no,
                    Ot = p.OrderOt,
                    Price = price,
                    BuyQuantity = buyQuantity,
                    DiscountType = EmDiscountType.Nothing,
                    DiscountValue = 0,
                    GiveQuantity = 0,
                    GiveUnit = EmCourseUnit.Day,
                    IsDeleted = EmIsDeleted.Normal,
                    ItemAptSum = p.AptSum,
                    ItemSum = p.AptSum,
                    PriceRule = string.Empty,
                    ProductId = course.Id,
                    ProductType = EmProductType.Course,
                    Remark = remark,
                    Status = EmOrderStatus.Normal,
                    TenantId = request.LoginTenantId,
                    UserId = request.LoginUserId,
                    StudentId = student.Id,
                    OrderType = EmOrderType.StudentEnrolment
                };

                //订单
                var orderId = await _orderDAL.AddOrder(order, new List<EtOrderDetail>() { orderDetail });

                var payType = EmPayType.Cash;
                if (p.PaySum > 0)
                {
                    if (!string.IsNullOrEmpty(p.PayTypeName))
                    {
                        payType = EmPayType.GetPayType(p.PayTypeName);
                    }
                    var incomeLog = new EtIncomeLog()
                    {
                        AccountNo = string.Empty,
                        IsDeleted = EmIsDeleted.Normal,
                        No = no,
                        Ot = p.OrderOt,
                        PayType = payType,
                        ProjectType = EmIncomeLogProjectType.StudentEnrolment,
                        Remark = remark,
                        RepealOt = null,
                        OrderId = orderId,
                        RepealUserId = null,
                        Status = EmIncomeLogStatus.Normal,
                        Sum = p.PaySum,
                        TenantId = request.LoginTenantId,
                        Type = EmIncomeLogType.AccountIn,
                        UserId = request.LoginUserId,
                        CreateOt = now
                    };
                    await _incomeLogDAL.AddIncomeLog(incomeLog);
                }

                var useQuantity = 0;
                if (p.EndTime > now.Date)
                {
                    useQuantity = (int)(now.Date - p.StartTime).TotalDays;
                }
                else
                {
                    useQuantity = (int)(p.EndTime - p.StartTime).TotalDays;
                }
                var surplusQuantity = 0;
                var surplusSmallQuantity = 0;
                if (p.EndTime > now.Date)
                {
                    var startDate = now.Date;
                    if (p.StartTime > startDate)
                    {
                        startDate = p.StartTime.Date;
                    }
                    var daySurplusQuantityDeff = EtmsHelper.GetDffTimeAboutSurplusQuantity(startDate, p.EndTime);
                    surplusQuantity = daySurplusQuantityDeff.Item1;
                    surplusSmallQuantity = daySurplusQuantityDeff.Item2;
                }
                var buyTotalDays = (int)(p.EndTime.AddDays(1) - p.StartTime).TotalDays;
                var courseDetailPrice = ComBusiness2.GetOneClassDeSum(p.AptSum, EmDeClassTimesType.Day, 0, buyTotalDays,
                    p.StartTime, p.EndTime);
                var studentCourseDetail = new EtStudentCourseDetail()
                {
                    BugUnit = buyUnit,
                    BuyQuantity = buyQuantity,
                    CourseId = course.Id,
                    StudentId = student.Id,
                    TenantId = request.LoginTenantId,
                    OrderNo = no,
                    IsDeleted = EmIsDeleted.Normal,
                    DeType = EmDeClassTimesType.Day,
                    EndCourseRemark = string.Empty,
                    EndCourseTime = null,
                    EndCourseUser = null,
                    GiveQuantity = 0,
                    GiveUnit = EmCourseUnit.Day,
                    Price = courseDetailPrice,
                    StartTime = p.StartTime,
                    EndTime = p.EndTime,
                    Status = EmStudentCourseStatus.Normal,
                    SurplusQuantity = surplusQuantity,
                    SurplusSmallQuantity = surplusSmallQuantity,
                    TotalMoney = p.AptSum,
                    UseQuantity = useQuantity,
                    UseUnit = EmCourseUnit.Day,
                    OrderId = orderId
                };

                _studentCourseDAL.AddStudentCourseDetail(new List<EtStudentCourseDetail>() { studentCourseDetail });

                if (course.Type == EmCourseType.OneToOne || p.CourseType == EmCourseType.OneToOne)
                {
                    await AddOneToOneClass(course.Id, course.Name, student.Name, now, request.LoginTenantId, orderId, student.Id, request.LoginUserId);
                }

                if (addToAccountRechargeMoney > 0)
                {
                    _eventPublisher.Publish(new StudentAutoAddAccountRechargeEvent(request.LoginTenantId)
                    {
                        StudentId = order.StudentId,
                        AddMoney = addToAccountRechargeMoney,
                        RechargeLogType = EmStudentAccountRechargeLogType.StudentImportOverpayment,
                        PayType = payType,
                        UserId = order.UserId,
                        OrderNo = order.No,
                        OrderId = order.Id,
                        Remark = order.Remark
                    });
                }

                _eventPublisher.Publish(new StudentCourseAnalyzeEvent(request.LoginTenantId)
                {
                    StudentId = student.Id
                });
                _eventPublisher.Publish(new SyncStudentStudentCourseIdsEvent(request.LoginTenantId, student.Id));
            }

            if (studentExtendInfos.Any())
            {
                _studentDAL.AddStudentExtend(studentExtendInfos);
            }
            SyncStatisticsStudentInfo(new StatisticsStudentCountEvent(request.LoginTenantId)
            {
                Time = now
            }, request, now.Date, true);

            var phones = request.ImportCourseDays.Select(p => p.Phone).ToList();
            _eventPublisher.Publish(new SyncParentStudentsEvent(request.LoginTenantId)
            {
                Phones = phones.ToArray()
            });

            var allDate = request.ImportCourseDays.Select(p => p.OrderOt).Distinct();
            foreach (var myDate in allDate)
            {
                _eventPublisher.Publish(new StatisticsSalesProductEvent(request.LoginTenantId)
                {
                    StatisticsDate = myDate
                });
                _eventPublisher.Publish(new StatisticsFinanceIncomeEvent(request.LoginTenantId)
                {
                    StatisticsDate = myDate
                });
                _eventPublisher.Publish(new StatisticsSalesCourseEvent(request.LoginTenantId)
                {
                    StatisticsDate = myDate
                });
            }

            _eventPublisher.Publish(new SysTenantStatistics2Event(request.LoginTenantId));
            _eventPublisher.Publish(new SysTenantStatisticsWeekAndMonthEvent(request.LoginTenantId));
            return ResponseBase.Success(new ImportCourseOutput()
            {
                SuccessCount = request.ImportCourseDays.Count
            });
        }

        private async Task AddOneToOneClass(long courseId, string courseName, string studentName,
            DateTime ot, int tenantId, long orderId, long studentId, long userId)
        {
            var classId = await _classDAL.AddClass(new EtClass()
            {
                DefaultClassTimes = 1,
                CourseList = $",{courseId},",
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
                Name = $"{courseName}_{studentName}",
                Ot = ot,
                PlanCount = 0,
                Remark = string.Empty,
                ScheduleStatus = EmClassScheduleStatus.Unscheduled,
                StudentNums = 1,
                TeacherNum = 0,
                Teachers = string.Empty,
                TenantId = tenantId,
                Type = EmClassType.OneToOne,
                UserId = userId,
                StudentIds = $",{studentId},",
                OrderId = orderId
            });
            await _classDAL.AddClassStudent(new EtClassStudent()
            {
                ClassId = classId,
                CourseId = courseId,
                IsDeleted = EmIsDeleted.Normal,
                Remark = string.Empty,
                StudentId = studentId,
                TenantId = tenantId,
                Type = EmClassType.OneToOne
            });
            _eventPublisher.Publish(new SyncStudentStudentClassIdsEvent(tenantId, studentId));
        }
    }
}
