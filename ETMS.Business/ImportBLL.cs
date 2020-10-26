﻿using ETMS.Business.Common;
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


        public ImportBLL(IHttpContextAccessor httpContextAccessor, IAppConfigurtaionServices appConfigurtaionServices,
            IStudentSourceDAL studentSourceDAL, IStudentRelationshipDAL studentRelationshipDAL, IGradeDAL gradeDAL, ISysTenantDAL sysTenantDAL,
            IStudentDAL studentDAL, IEventPublisher eventPublisher, IUserOperationLogDAL userOperationLogDAL, ICourseDAL courseDAL, IOrderDAL orderDAL,
            IIncomeLogDAL incomeLogDAL, IStudentCourseDAL studentCourseDAL)
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
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentSourceDAL, _studentRelationshipDAL,
                _gradeDAL, _studentDAL, _userOperationLogDAL, _courseDAL, _orderDAL, _incomeLogDAL, _studentCourseDAL);
        }

        public async Task<ResponseBase> GetImportStudentExcelTemplate(GetImportStudentExcelTemplateRequest request)
        {
            var tenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            var checkImportStudentTemplateFileResult = ExcelLib.CheckImportStudentExcelTemplate(tenant.TenantCode, _appConfigurtaionServices.AppSettings.StaticFilesConfig.ServerPath);
            if (!checkImportStudentTemplateFileResult.IsExist)
            {
                ExcelLib.GenerateImportStudentExcelTemplate(new ImportStudentExcelTemplateRequest()
                {
                    CheckResult = checkImportStudentTemplateFileResult,
                    GradeAll = await _gradeDAL.GetAllGrade(),
                    StudentRelationshipAll = await _studentRelationshipDAL.GetAllStudentRelationship(),
                    StudentSourceAll = await _studentSourceDAL.GetAllStudentSource(),
                });
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

                studentList.Add(new EtStudent()
                {
                    Age = p.Birthday.EtmsGetAge(),
                    Name = p.StudentName,
                    Avatar = string.Empty,
                    Birthday = p.Birthday,
                    CardNo = string.Empty,
                    CreateBy = request.LoginUserId,
                    EndClassOt = null,
                    Gender = gender,
                    GradeId = gradeId,
                    HomeAddress = p.HomeAddress,
                    IntentionLevel = EmStudentIntentionLevel.Middle,
                    IsBindingWechat = EmStudentIsBindingWechat.No,
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
                    TrackUser = null
                });
            }
            if (studentList.Count > 0)
            {
                _studentDAL.AddStudent(studentList);
                SyncStatisticsStudentInfo(new StatisticsStudentCountEvent(request.LoginTenantId)
                {
                    ChangeCount = studentList.Count,
                    OpType = StatisticsStudentOpType.Add,
                    Time = now
                }, request, now.Date, true);
                await _userOperationLogDAL.AddUserLog(request, $"批量导入潜在学员，成功导入了{studentList.Count}位学员", EmUserOperationType.StudentManage);
            }
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
                ExcelLib.GenerateImportCourseTimesExcelTemplate(new ImportCourseHeadDescTimesExcelTemplateRequest()
                {
                    CheckResult = checkImportCourseTimesExcelTemplate,
                    PayTypeAll = EmPayType.GetPayTypeAll()
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
            foreach (var p in request.ImportCourseTimess)
            {
                var student = await _studentDAL.GetStudent(p.StudentName, p.Phone);
                if (student == null)
                {
                    student = new EtStudent()
                    {
                        Age = null,
                        Name = p.StudentName,
                        Avatar = string.Empty,
                        Birthday = null,
                        CardNo = string.Empty,
                        CreateBy = request.LoginUserId,
                        EndClassOt = null,
                        Gender = null,
                        GradeId = null,
                        HomeAddress = string.Empty,
                        IntentionLevel = EmStudentIntentionLevel.Middle,
                        IsBindingWechat = EmStudentIsBindingWechat.No,
                        IsDeleted = EmIsDeleted.Normal,
                        LastJobProcessTime = now,
                        LastTrackTime = null,
                        LearningManager = null,
                        NextTrackTime = null,
                        Ot = now.Date,
                        Phone = p.Phone,
                        PhoneBak = string.Empty,
                        PhoneBakRelationship = null,
                        PhoneRelationship = 0,
                        Points = 0,
                        Remark = remark,
                        SchoolName = string.Empty,
                        SourceId = null,
                        StudentType = EmStudentType.ReadingStudent,
                        Tags = string.Empty,
                        TenantId = request.LoginTenantId,
                        TrackStatus = EmStudentTrackStatus.NotTrack,
                        TrackUser = null
                    };
                    await _studentDAL.AddStudent(student, null);
                    addStudentCount++;
                }
                else
                {
                    await _studentDAL.StudentEnrolmentEventChangeInfo(student.Id, 0, EmStudentType.ReadingStudent);
                }

                var course = await _courseDAL.GetCourse(p.CourseName);
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
                        Type = EmCourseType.OneToMany,
                        UserId = request.LoginUserId,
                        PriceType = EmCoursePriceType.ClassTimes,
                        Status = EmCourseStatus.Enabled
                    };
                    await _courseDAL.AddCourse(course, new List<EtCoursePriceRule>() { priceRule });
                }

                var no = OrderNumberLib.EnrolmentOrderNumber();
                var arrearsSum = p.AptSum - p.PaySum;
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
                    CreateOt = now
                };

                var orderDetail = new EtOrderDetail()
                {
                    BugUnit = EmCourseUnit.ClassTimes,
                    OrderNo = no,
                    Ot = p.OrderOt,
                    Price = price,
                    BuyQuantity = p.BuyQuantity,
                    DiscountType = EmOrderDiscountType.Nothing,
                    DiscountValue = 0,
                    GiveQuantity = p.GiveQuantity,
                    GiveUnit = EmCourseUnit.ClassTimes,
                    IsDeleted = EmIsDeleted.Normal,
                    ItemAptSum = p.AptSum,
                    ItemSum = p.AptSum,
                    PriceRule = string.Empty,
                    ProductId = course.Id,
                    ProductType = EmOrderProductType.Course,
                    Remark = remark,
                    Status = EmOrderStatus.Normal,
                    TenantId = request.LoginTenantId,
                    UserId = request.LoginUserId
                };

                //订单
                var orderId = await _orderDAL.AddOrder(order, new List<EtOrderDetail>() { orderDetail });

                if (p.PaySum > 0)
                {
                    var payType = EmPayType.Cash;
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
                    Price = price,
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

                _eventPublisher.Publish(new StudentCourseAnalyzeEvent(request.LoginTenantId)
                {
                    StudentId = student.Id
                });

                _eventPublisher.Publish(new StatisticsSalesProductEvent(request.LoginTenantId)
                {
                    StatisticsDate = p.OrderOt
                });
                _eventPublisher.Publish(new StatisticsFinanceIncomeEvent(request.LoginTenantId)
                {
                    StatisticsDate = p.OrderOt
                });
                _eventPublisher.Publish(new StatisticsSalesCourseEvent(request.LoginTenantId)
                {
                    StatisticsDate = p.OrderOt
                });
            }
            SyncStatisticsStudentInfo(new StatisticsStudentCountEvent(request.LoginTenantId)
            {
                ChangeCount = addStudentCount,
                OpType = StatisticsStudentOpType.Add,
                Time = now
            }, request, now.Date, true);

            return ResponseBase.Success(new ImportCourseTimesOutput()
            {
                SuccessCount = request.ImportCourseTimess.Count
            });
        }
    }
}
