using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Teacher.Output;
using ETMS.Entity.Dto.Teacher.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.TeacherSalary;
using ETMS.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETMS.Entity.Extensions;
using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Entity.Temp;
using ETMS.IEventProvider;
using ETMS.Event.DataContract;

namespace ETMS.Business
{
    public class TeacherSalaryBLL : ITeacherSalaryBLL
    {
        private readonly IAppConfig2BLL _appConfig2BLL;

        private readonly ITeacherSalaryFundsItemsDAL _teacherSalaryFundsItemsDAL;

        private readonly ITeacherSalaryClassDAL _teacherSalaryClassDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IUserDAL _userDAL;

        private readonly IClassDAL _classDAL;

        private readonly ITeacherSalaryContractDAL _teacherSalaryContractDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly ITeacherSalaryPayrollDAL _teacherSalaryPayrollDAL;

        private IDistributedLockDAL _distributedLockDAL;

        private readonly IIncomeLogDAL _incomeLogDAL;

        private readonly IEventPublisher _eventPublisher;

        public TeacherSalaryBLL(IAppConfig2BLL appConfig2BLL, ITeacherSalaryFundsItemsDAL teacherSalaryFundsItemsDAL, ITeacherSalaryClassDAL teacherSalaryClassDAL,
            IUserOperationLogDAL userOperationLogDAL, IUserDAL userDAL, IClassDAL classDAL, ITeacherSalaryContractDAL teacherSalaryContractDAL,
            ICourseDAL courseDAL, ITeacherSalaryPayrollDAL teacherSalaryPayrollDAL, IDistributedLockDAL distributedLockDAL, IIncomeLogDAL incomeLogDAL,
            IEventPublisher eventPublisher)
        {
            this._appConfig2BLL = appConfig2BLL;
            this._teacherSalaryFundsItemsDAL = teacherSalaryFundsItemsDAL;
            this._teacherSalaryClassDAL = teacherSalaryClassDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._userDAL = userDAL;
            this._classDAL = classDAL;
            this._teacherSalaryContractDAL = teacherSalaryContractDAL;
            this._courseDAL = courseDAL;
            this._teacherSalaryPayrollDAL = teacherSalaryPayrollDAL;
            this._distributedLockDAL = distributedLockDAL;
            this._incomeLogDAL = incomeLogDAL;
            this._eventPublisher = eventPublisher;
        }

        public void InitTenantId(int tenantId)
        {
            this._appConfig2BLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _teacherSalaryFundsItemsDAL, _teacherSalaryClassDAL,
                _userOperationLogDAL, _userDAL, _classDAL, _teacherSalaryContractDAL, _courseDAL, _teacherSalaryPayrollDAL, _incomeLogDAL);
        }

        private async Task<TeacherSalaryFundsItemsGetOutput> GetTeacherSalaryFundsItems(bool isGetDisable)
        {
            var output = new TeacherSalaryFundsItemsGetOutput()
            {
                DefaultItems = new List<TeacherSalaryFundsItemOutput>(),
                CustomItems = new List<TeacherSalaryFundsItemOutput>()
            };
            var fundsItemsDefault = await _appConfig2BLL.GetTeacherSalaryDefaultFundsItems();
            var performanceDefaultItem = fundsItemsDefault.First(p => p.Id == SystemConfig.ComConfig.TeacherSalaryPerformanceDefaultId);
            output.IsOpenContractPerformance = performanceDefaultItem.Status == EmBool.True;
            var orderIndex = 1;
            foreach (var p in fundsItemsDefault)
            {
                if (!isGetDisable && p.Status == EmBool.False) //不展示禁用的项目
                {
                    continue;
                }
                output.DefaultItems.Add(new TeacherSalaryFundsItemOutput()
                {
                    OrderIndex = orderIndex,
                    Name = p.Name,
                    Id = p.Id,
                    Status = p.Status,
                    Type = p.Type,
                    TypeDesc = EmTeacherSalaryFundsItemsType.GetTeacherSalaryFundsItemsTypeDesc(p.Type)
                });
                orderIndex++;
            }

            var fundsItemsCustom = await _teacherSalaryFundsItemsDAL.GetTeacherSalaryFundsItems();
            foreach (var p in fundsItemsCustom)
            {
                output.CustomItems.Add(new TeacherSalaryFundsItemOutput()
                {
                    OrderIndex = orderIndex,
                    Name = p.Name,
                    Id = p.Id,
                    Status = EmBool.True,
                    Type = p.Type,
                    TypeDesc = EmTeacherSalaryFundsItemsType.GetTeacherSalaryFundsItemsTypeDesc(p.Type)
                });
                orderIndex++;
            }

            return output;
        }

        public async Task<ResponseBase> TeacherSalaryFundsItemsGet(TeacherSalaryFundsItemsGetRequest request)
        {
            return ResponseBase.Success(await GetTeacherSalaryFundsItems(request.IsGetDisable));
        }

        public async Task<ResponseBase> TeacherSalaryFundsItemsAdd(TeacherSalaryFundsItemsAddRequest request)
        {
            await _teacherSalaryFundsItemsDAL.AddTeacherSalaryFundsItems(new EtTeacherSalaryFundsItems()
            {
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name,
                OrderIndex = 0,
                TenantId = request.LoginTenantId,
                Type = request.Type
            });

            await _userOperationLogDAL.AddUserLog(request, $"添加工资条项目-{request.Name}", EmUserOperationType.TeacherSalary);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TeacherSalaryFundsItemsDel(TeacherSalaryFundsItemsDelRequest request)
        {
            await _teacherSalaryFundsItemsDAL.DelTeacherSalaryFundsItems(request.Id);

            await _userOperationLogDAL.AddUserLog(request, $"删除工资条项目-{request.Name}", EmUserOperationType.TeacherSalary);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TeacherSalaryFundsItemsChangeStatus(TeacherSalaryFundsItemsChangeStatusRequest request)
        {
            var fundsItemsDefault = await _appConfig2BLL.GetTeacherSalaryDefaultFundsItems();
            var thisItem = fundsItemsDefault.FirstOrDefault(p => p.Id == request.Id);
            if (thisItem == null)
            {
                return ResponseBase.CommonError("工资条项目不存在");
            }
            thisItem.Status = thisItem.Status == EmBool.True ? EmBool.False : EmBool.True;
            await _appConfig2BLL.SaveTeacherSalaryDefaultFundsItems(request.LoginTenantId, fundsItemsDefault);

            var desc = thisItem.Status == EmBool.True ? "启用" : "禁用";
            await _userOperationLogDAL.AddUserLog(request, $"{desc}工资条项目-{thisItem.Name}", EmUserOperationType.TeacherSalary);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TeacherSalaryClassDayGetPaging(TeacherSalaryClassDayGetPagingRequest request)
        {
            var pagingData = await _teacherSalaryClassDAL.GetTeacherSalaryClassDayPaging(request);
            var output = new List<TeacherSalaryClassDayGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var tempBoxUser = new DataTempBox<EtUser>();
                var tempBoxClass = new DataTempBox<EtClass>();
                foreach (var p in pagingData.Item1)
                {
                    var myUser = await ComBusiness.GetUser(tempBoxUser, _userDAL, p.TeacherId);
                    if (myUser == null)
                    {
                        continue;
                    }
                    var myClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, p.ClassId);
                    output.Add(new TeacherSalaryClassDayGetPagingOutput()
                    {
                        ClassId = p.ClassId,
                        ArrivedAndBeLateCount = p.TotalArrivedAndBeLateCount,
                        ArrivedCount = p.TotalArrivedCount,
                        BeLateCount = p.TotalBeLateCount,
                        ClassName = myClass?.Name,
                        DeSum = p.TotalDeSum,
                        LeaveCount = p.TotalLeaveCount,
                        MakeUpStudentCount = p.TotalMakeUpStudentCount,
                        NotArrivedCount = p.TotalNotArrivedCount,
                        StudentClassTimes = p.TotalStudentClassTimes.EtmsToString(),
                        TeacherClassTimes = p.TotalTeacherClassTimes.EtmsToString(),
                        TeacherId = p.TeacherId,
                        TeacherName = myUser.Name,
                        TeacherPhone = myUser.Phone,
                        TryCalssStudentCount = p.TotalTryCalssStudentCount
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<TeacherSalaryClassDayGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> TeacherSalaryGlobalRuleGet(TeacherSalaryGlobalRuleGetRequest request)
        {
            var log = await _appConfig2BLL.GetTeacherSalaryGlobalRule();
            return ResponseBase.Success(new TeacherSalaryGlobaleRuleGetOutput()
            {
                GradientCalculateType = log.GradientCalculateType,
                StatisticalRuleType = log.StatisticalRuleType,
                IncludeArrivedMakeUpStudent = log.IncludeArrivedMakeUpStudent,
                IncludeArrivedTryCalssStudent = log.IncludeArrivedTryCalssStudent
            });
        }

        public async Task<ResponseBase> TeacherSalaryPerformanceRuleSave(TeacherSalaryPerformanceRuleSaveRequest request)
        {
            var log = await _appConfig2BLL.GetTeacherSalaryGlobalRule();
            log.GradientCalculateType = request.GradientCalculateType;
            log.StatisticalRuleType = request.StatisticalRuleType;
            await _appConfig2BLL.SaveTeacherSalaryGlobalRule(request.LoginTenantId, log);

            await _userOperationLogDAL.AddUserLog(request, "设置绩效工资统计规则", EmUserOperationType.TeacherSalary);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TeacherSalaryIncludeArrivedRuleSave(TeacherSalaryIncludeArrivedRuleSaveRequest request)
        {
            var log = await _appConfig2BLL.GetTeacherSalaryGlobalRule();
            log.IncludeArrivedMakeUpStudent = request.IncludeArrivedMakeUpStudent;
            log.IncludeArrivedTryCalssStudent = request.IncludeArrivedTryCalssStudent;
            await _appConfig2BLL.SaveTeacherSalaryGlobalRule(request.LoginTenantId, log);

            await _userOperationLogDAL.AddUserLog(request, "到课人次计算规则", EmUserOperationType.TeacherSalary);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TeacherSalaryContractGetPaging(TeacherSalaryContractGetPagingRequest request)
        {
            var config = await GetTeacherSalaryFundsItems(false);
            var outputTableHeadInfo = new List<PagingTableHeadOutput>();
            var allTeacherSalaryFundsItemOutput = new List<TeacherSalaryFundsItemOutput>();
            if (config.DefaultItems != null && config.DefaultItems.Count > 0)
            {
                allTeacherSalaryFundsItemOutput.AddRange(config.DefaultItems);
            }
            if (config.CustomItems != null && config.CustomItems.Count > 0)
            {
                allTeacherSalaryFundsItemOutput.AddRange(config.CustomItems);
            }
            if (allTeacherSalaryFundsItemOutput.Count > 0)
            {
                var index = 0;
                foreach (var p in allTeacherSalaryFundsItemOutput)
                {
                    outputTableHeadInfo.Add(new PagingTableHeadOutput()
                    {
                        Index = index,
                        Label = p.Name,
                        OtherInfo = p.Type,
                        Id = p.Id,
                        Property = $"salaryContract{index}",
                        Type = p.Id == SystemConfig.ComConfig.TeacherSalaryPerformanceDefaultId ? PagingTableHeadType.Html : PagingTableHeadType.Text
                    });
                    index++;
                }
            }

            var pagingData = await _userDAL.GetUserSimplePaging(request);
            var outputItem = new List<TeacherSalaryContractGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                int index;
                foreach (var p in pagingData.Item1)
                {
                    var item = new TeacherSalaryContractGetPagingOutput()
                    {
                        Id = p.Id,
                        IsTeacher = p.IsTeacher,
                        IsTeacherDesc = p.IsTeacher ? "是" : "否",
                        Name = p.Name,
                        Phone = p.Phone,
                        SalaryContractStatus = EmBool.False,
                        IsSetContractPerformance = false
                    };
                    var teacherSalaryContractSetBucket = await _teacherSalaryContractDAL.GetTeacherSalaryContract(p.Id);
                    if (teacherSalaryContractSetBucket == null || teacherSalaryContractSetBucket.TeacherSalaryContractFixeds == null ||
                        teacherSalaryContractSetBucket.TeacherSalaryContractFixeds.Count == 0)
                    {
                        outputItem.Add(item);
                        continue;
                    }
                    item.SalaryContractStatus = EmBool.True;
                    index = 0;
                    foreach (var myFundsItem in allTeacherSalaryFundsItemOutput)
                    {
                        if (myFundsItem.Id == SystemConfig.ComConfig.TeacherSalaryPerformanceDefaultId) //绩效工资
                        {

                            if (teacherSalaryContractSetBucket.TeacherSalaryContractPerformanceSet != null &&
                                teacherSalaryContractSetBucket.TeacherSalaryContractPerformanceSetDetails != null &&
                                teacherSalaryContractSetBucket.TeacherSalaryContractPerformanceSetDetails.Count > 0)
                            {
                                item.SetSalaryContract(index, teacherSalaryContractSetBucket.TeacherSalaryContractPerformanceSet.ComputeDesc);
                                item.IsSetContractPerformance = true;
                            }
                            else
                            {
                                item.SetSalaryContract(index, "<span class='tag_color_red'>未设置</span>");
                            }
                        }
                        else
                        {
                            var thisContractFixed = teacherSalaryContractSetBucket.TeacherSalaryContractFixeds.FirstOrDefault(j => j.FundsItemsId == myFundsItem.Id);
                            if (thisContractFixed != null)
                            {
                                item.SetSalaryContract(index, thisContractFixed.AmountValue.EtmsToString2());
                            }
                        }
                        index++;
                    }
                    outputItem.Add(item);
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase2<TeacherSalaryContractGetPagingOutput,
                PagingTableHeadOutput>(pagingData.Item2, outputItem, outputTableHeadInfo));
        }

        private async Task<List<TeacherSalaryContractPerformanceSet>> GetTeacherSalaryContractPerformanceSet(long teacherId,
            List<EtTeacherSalaryContractPerformanceSetDetail> myTeacherSalaryContractPerformanceSetDetails,
            List<EtTeacherSalaryContractPerformanceLessonBasc> myTeacherSalaryContractPerformanceLessonBasc,
            byte computeType, byte gradientCalculateType)
        {
            var output = new List<TeacherSalaryContractPerformanceSet>();
            //绩效工资
            var teacherAllClass = await _classDAL.GetClassOfTeacher(teacherId);
            decimal bascValue = 0M;
            switch (computeType)
            {
                case EmTeacherSalaryComputeType.Class:
                    if (teacherAllClass.Any())
                    {
                        foreach (var myClass in teacherAllClass)
                        {
                            bascValue = 0;
                            if (myTeacherSalaryContractPerformanceLessonBasc != null)
                            {
                                var myClassLessonBasc = myTeacherSalaryContractPerformanceLessonBasc.FirstOrDefault(p => p.RelationId == myClass.Id);
                                if (myClassLessonBasc != null)
                                {
                                    bascValue = myClassLessonBasc.ComputeValue;
                                }
                            }
                            var performanceSetClass = new TeacherSalaryContractPerformanceSet()
                            {
                                ComputeMode = EmTeacherSalaryComputeMode.TeacherClassTimes,
                                RelationExtend = new List<string>(),
                                RelationId = myClass.Id,
                                RelationName = myClass.Name,
                                SetDetails = new List<TeacherSalaryContractPerformanceSetDetail>(),
                                LessonBascValue = bascValue
                            };
                            if (myTeacherSalaryContractPerformanceSetDetails != null)
                            {
                                var myClassSetDetailList = myTeacherSalaryContractPerformanceSetDetails.Where(p => p.RelationId == myClass.Id).OrderBy(p => p.MinLimit);
                                if (myClassSetDetailList.Any())
                                {
                                    var myTempSetDetailsClassFirst = myClassSetDetailList.First();
                                    performanceSetClass.ComputeMode = myTempSetDetailsClassFirst.ComputeMode;
                                    if (gradientCalculateType == EmTeacherSalaryGradientCalculateType.None)
                                    {
                                        performanceSetClass.SetDetails.Add(new TeacherSalaryContractPerformanceSetDetail()
                                        {
                                            MinLimit = myTempSetDetailsClassFirst.MinLimit,
                                            MaxLimit = myTempSetDetailsClassFirst.MaxLimit,
                                            ComputeValue = myTempSetDetailsClassFirst.ComputeValue
                                        });
                                    }
                                    else
                                    {
                                        foreach (var myClassSetDetail in myClassSetDetailList)
                                        {
                                            performanceSetClass.SetDetails.Add(new TeacherSalaryContractPerformanceSetDetail()
                                            {
                                                MinLimit = myClassSetDetail.MinLimit,
                                                MaxLimit = myClassSetDetail.MaxLimit,
                                                ComputeValue = myClassSetDetail.ComputeValue
                                            });
                                        }
                                    }
                                }
                            }
                            if (performanceSetClass.SetDetails.Count == 0)
                            {
                                performanceSetClass.SetDetails.Add(new TeacherSalaryContractPerformanceSetDetail()
                                {
                                    MaxLimit = null,
                                    MinLimit = null,
                                    ComputeValue = 0
                                });
                            }
                            performanceSetClass.ComputeModeUnitDesc = EmTeacherSalaryComputeMode.GetModelUnitDesc(performanceSetClass.ComputeMode);
                            performanceSetClass.ComputeModeDesc = EmTeacherSalaryComputeMode.GetTeacherSalaryComputeModeDesc2(performanceSetClass.ComputeMode);
                            performanceSetClass.ComputeValueMaxLength = EmTeacherSalaryComputeMode.GetSalaryComputeModeValueMaxLength(performanceSetClass.ComputeMode);
                            performanceSetClass.ComputeModeHint = EmTeacherSalaryComputeMode.GetSalaryComputeModeHint(performanceSetClass.ComputeMode);
                            output.Add(performanceSetClass);
                        }
                    }
                    break;
                case EmTeacherSalaryComputeType.Course:
                    if (teacherAllClass.Any())
                    {
                        var allCourse = string.Join(',', teacherAllClass.Select(p => p.CourseList));
                        var allCourseId = EtmsHelper.AnalyzeMuIds(allCourse).Distinct();
                        foreach (var id in allCourseId)
                        {
                            var myCourse = await _courseDAL.GetCourse(id);
                            if (myCourse == null || myCourse.Item1 == null)
                            {
                                continue;
                            }
                            bascValue = 0;
                            if (myTeacherSalaryContractPerformanceLessonBasc != null)
                            {
                                var myCourseLessonBasc = myTeacherSalaryContractPerformanceLessonBasc.FirstOrDefault(p => p.RelationId == id);
                                if (myCourseLessonBasc != null)
                                {
                                    bascValue = myCourseLessonBasc.ComputeValue;
                                }
                            }
                            var performanceSetCourse = new TeacherSalaryContractPerformanceSet()
                            {
                                ComputeMode = EmTeacherSalaryComputeMode.TeacherClassTimes,
                                RelationExtend = new List<string>(),
                                RelationId = id,
                                RelationName = myCourse.Item1.Name,
                                LessonBascValue = bascValue,
                                SetDetails = new List<TeacherSalaryContractPerformanceSetDetail>()
                            };
                            var myCourseClass = teacherAllClass.Where(p => p.CourseList.IndexOf($",{id},") != -1);
                            if (myCourseClass.Any())
                            {
                                foreach (var myCourseClassItem in myCourseClass)
                                {
                                    performanceSetCourse.RelationExtend.Add(myCourseClassItem.Name);
                                }
                            }
                            if (myTeacherSalaryContractPerformanceSetDetails != null)
                            {
                                var myCourseSetDetailList = myTeacherSalaryContractPerformanceSetDetails.Where(p => p.RelationId == id).OrderBy(p => p.MinLimit);
                                if (myCourseSetDetailList.Any())
                                {
                                    var myTempSetDetailsCourseFirst = myCourseSetDetailList.First();
                                    performanceSetCourse.ComputeMode = myTempSetDetailsCourseFirst.ComputeMode;
                                    if (gradientCalculateType == EmTeacherSalaryGradientCalculateType.None)
                                    {
                                        performanceSetCourse.SetDetails.Add(new TeacherSalaryContractPerformanceSetDetail()
                                        {
                                            MinLimit = myTempSetDetailsCourseFirst.MinLimit,
                                            MaxLimit = myTempSetDetailsCourseFirst.MaxLimit,
                                            ComputeValue = myTempSetDetailsCourseFirst.ComputeValue
                                        });
                                    }
                                    else
                                    {
                                        foreach (var myCourseSetDetail in myCourseSetDetailList)
                                        {
                                            performanceSetCourse.SetDetails.Add(new TeacherSalaryContractPerformanceSetDetail()
                                            {
                                                MinLimit = myCourseSetDetail.MinLimit,
                                                MaxLimit = myCourseSetDetail.MaxLimit,
                                                ComputeValue = myCourseSetDetail.ComputeValue
                                            });
                                        }
                                    }
                                }
                            }
                            if (performanceSetCourse.SetDetails.Count == 0)
                            {
                                performanceSetCourse.SetDetails.Add(new TeacherSalaryContractPerformanceSetDetail()
                                {
                                    MaxLimit = null,
                                    MinLimit = null,
                                    ComputeValue = 0
                                });
                            }
                            performanceSetCourse.ComputeModeUnitDesc = EmTeacherSalaryComputeMode.GetModelUnitDesc(performanceSetCourse.ComputeMode);
                            performanceSetCourse.ComputeModeDesc = EmTeacherSalaryComputeMode.GetTeacherSalaryComputeModeDesc2(performanceSetCourse.ComputeMode);
                            performanceSetCourse.ComputeValueMaxLength = EmTeacherSalaryComputeMode.GetSalaryComputeModeValueMaxLength(performanceSetCourse.ComputeMode);
                            performanceSetCourse.ComputeModeHint = EmTeacherSalaryComputeMode.GetSalaryComputeModeHint(performanceSetCourse.ComputeMode);
                            output.Add(performanceSetCourse);
                        }
                    }
                    break;
                case EmTeacherSalaryComputeType.Global:
                    var computeModeGlobal = EmTeacherSalaryComputeMode.TeacherClassTimes;
                    var setDetailsGlobal = new List<TeacherSalaryContractPerformanceSetDetail>();
                    if (myTeacherSalaryContractPerformanceSetDetails != null)
                    {
                        var myTempSetDetailsGlobalFirst = myTeacherSalaryContractPerformanceSetDetails.First();
                        computeModeGlobal = myTempSetDetailsGlobalFirst.ComputeMode;
                        if (gradientCalculateType == EmTeacherSalaryGradientCalculateType.None)
                        {
                            setDetailsGlobal.Add(new TeacherSalaryContractPerformanceSetDetail()
                            {
                                MinLimit = myTempSetDetailsGlobalFirst.MinLimit,
                                MaxLimit = myTempSetDetailsGlobalFirst.MaxLimit,
                                ComputeValue = myTempSetDetailsGlobalFirst.ComputeValue
                            });
                        }
                        else
                        {
                            var performanceSetDetailsOrder = myTeacherSalaryContractPerformanceSetDetails.OrderBy(p => p.MinLimit);
                            foreach (var myGlobalSetDetail in performanceSetDetailsOrder)
                            {
                                setDetailsGlobal.Add(new TeacherSalaryContractPerformanceSetDetail()
                                {
                                    MinLimit = myGlobalSetDetail.MinLimit,
                                    MaxLimit = myGlobalSetDetail.MaxLimit,
                                    ComputeValue = myGlobalSetDetail.ComputeValue
                                });
                            }
                        }
                    }
                    var relationExtend = new List<string>();
                    if (teacherAllClass.Any())
                    {
                        foreach (var p in teacherAllClass)
                        {
                            relationExtend.Add(p.Name);
                        }
                    }
                    if (setDetailsGlobal.Count == 0)
                    {
                        setDetailsGlobal.Add(new TeacherSalaryContractPerformanceSetDetail()
                        {
                            ComputeValue = 0,
                            MaxLimit = null,
                            MinLimit = null
                        });
                    }
                    bascValue = 0;
                    if (myTeacherSalaryContractPerformanceLessonBasc != null)
                    {
                        var myGlobalLessonBasc = myTeacherSalaryContractPerformanceLessonBasc.FirstOrDefault(p => p.RelationId == 0);
                        if (myGlobalLessonBasc != null)
                        {
                            bascValue = myGlobalLessonBasc.ComputeValue;
                        }
                    }
                    output = new List<TeacherSalaryContractPerformanceSet>() {
                        new TeacherSalaryContractPerformanceSet(){
                            ComputeMode =computeModeGlobal,
                            RelationExtend = relationExtend,
                            RelationId = 0,
                            RelationName ="所有班级",
                            LessonBascValue =bascValue,
                            SetDetails =setDetailsGlobal,
                            ComputeModeDesc =  EmTeacherSalaryComputeMode.GetTeacherSalaryComputeModeDesc2(computeModeGlobal),
                            ComputeModeUnitDesc = EmTeacherSalaryComputeMode.GetModelUnitDesc(computeModeGlobal),
                            ComputeValueMaxLength = EmTeacherSalaryComputeMode.GetSalaryComputeModeValueMaxLength(computeModeGlobal),
                            ComputeModeHint = EmTeacherSalaryComputeMode.GetSalaryComputeModeHint(computeModeGlobal)
                        }};
                    break;
            }

            return output;
        }

        public async Task<ResponseBase> TeacherSalaryContractGetDetail(TeacherSalaryContractGetDetailRequest request)
        {
            var user = await _userDAL.GetUser(request.TeacherId);
            if (user == null)
            {
                return ResponseBase.CommonError("员工不存在");
            }
            var config = await GetTeacherSalaryFundsItems(false);
            var globalConfig = await _appConfig2BLL.GetTeacherSalaryGlobalRule();
            var output = new TeacherSalaryContractGetDetailOutput()
            {
                ComputeType = EmTeacherSalaryComputeType.Class,
                IsOpenClassPerformance = config.IsOpenContractPerformance,
                GradientCalculateType = globalConfig.GradientCalculateType,
                FixedItems = new List<TeacherSalaryContractFixedItem>(),
                PerformanceSetItems = new List<TeacherSalaryContractPerformanceSet>(),
                TeacherId = user.Id,
                TeacherName = user.Name,
                TeacherPhone = user.Phone
            };
            List<EtTeacherSalaryContractFixed> myTeacherSalaryContractFixeds = null;
            EtTeacherSalaryContractPerformanceSet myTeacherSalaryContractPerformanceSet = null;
            List<EtTeacherSalaryContractPerformanceSetDetail> myTeacherSalaryContractPerformanceSetDetails = null;
            List<EtTeacherSalaryContractPerformanceLessonBasc> myTeacherSalaryContractPerformanceLessonBasc = null;
            var myTeacherSalaryContractSetBucket = await _teacherSalaryContractDAL.GetTeacherSalaryContract(request.TeacherId);
            if (myTeacherSalaryContractSetBucket != null)
            {
                if (myTeacherSalaryContractSetBucket.TeacherSalaryContractFixeds != null &&
                    myTeacherSalaryContractSetBucket.TeacherSalaryContractFixeds.Any())
                {
                    myTeacherSalaryContractFixeds = myTeacherSalaryContractSetBucket.TeacherSalaryContractFixeds;
                }
                if (myTeacherSalaryContractSetBucket.TeacherSalaryContractPerformanceSet != null)
                {
                    myTeacherSalaryContractPerformanceSet = myTeacherSalaryContractSetBucket.TeacherSalaryContractPerformanceSet;
                }
                if (myTeacherSalaryContractSetBucket.TeacherSalaryContractPerformanceSetDetails != null &&
                    myTeacherSalaryContractSetBucket.TeacherSalaryContractPerformanceSetDetails.Any())
                {
                    myTeacherSalaryContractPerformanceSetDetails = myTeacherSalaryContractSetBucket.TeacherSalaryContractPerformanceSetDetails;
                }
                if (myTeacherSalaryContractSetBucket.EtTeacherSalaryContractPerformanceLessonBascs != null
                    && myTeacherSalaryContractSetBucket.EtTeacherSalaryContractPerformanceLessonBascs.Any())
                {
                    myTeacherSalaryContractPerformanceLessonBasc = myTeacherSalaryContractSetBucket.EtTeacherSalaryContractPerformanceLessonBascs;
                }
            }

            if (myTeacherSalaryContractPerformanceSet != null)
            {
                output.ComputeType = myTeacherSalaryContractPerformanceSet.ComputeType;
                output.GradientCalculateType = myTeacherSalaryContractPerformanceSet.GradientCalculateType;
            }

            //工资条项目
            var allTeacherSalaryFundsItem = new List<TeacherSalaryFundsItemOutput>();
            if (config.DefaultItems != null && config.DefaultItems.Count > 0)
            {
                allTeacherSalaryFundsItem.AddRange(config.DefaultItems);
            }
            if (config.CustomItems != null && config.CustomItems.Count > 0)
            {
                allTeacherSalaryFundsItem.AddRange(config.CustomItems);
            }
            if (allTeacherSalaryFundsItem.Count > 0)
            {
                var fixedFundsItem = allTeacherSalaryFundsItem.Where(p => p.Id != SystemConfig.ComConfig.TeacherSalaryPerformanceDefaultId).OrderBy(p => p.Id);
                foreach (var myixedFundsItem in fixedFundsItem)
                {
                    var value = 0M;
                    if (myTeacherSalaryContractFixeds != null)
                    {
                        var myFixedItem = myTeacherSalaryContractFixeds.FirstOrDefault(p => p.FundsItemsId == myixedFundsItem.Id);
                        if (myFixedItem != null)
                        {
                            value = myFixedItem.AmountValue;
                        }
                    }
                    output.FixedItems.Add(new TeacherSalaryContractFixedItem()
                    {
                        Id = myixedFundsItem.Id,
                        Name = myixedFundsItem.Name,
                        Value = value,
                        Type = myixedFundsItem.Type
                    });
                }
            }

            //绩效工资
            output.PerformanceSetItems = await GetTeacherSalaryContractPerformanceSet(request.TeacherId,
                myTeacherSalaryContractPerformanceSetDetails, myTeacherSalaryContractPerformanceLessonBasc,
                output.ComputeType, output.GradientCalculateType);

            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> TeacherSalaryContractChangeComputeType(TeacherSalaryContractChangeComputeTypeRequest request)
        {
            EtTeacherSalaryContractPerformanceSet myTeacherSalaryContractPerformanceSet = null;
            List<EtTeacherSalaryContractPerformanceSetDetail> myTeacherSalaryContractPerformanceSetDetails = null;
            List<EtTeacherSalaryContractPerformanceLessonBasc> myTeacherSalaryContractPerformanceLessonBasc = null;
            var myTeacherSalaryContractSetBucket = await _teacherSalaryContractDAL.GetTeacherSalaryContract(request.TeacherId);
            if (myTeacherSalaryContractSetBucket != null)
            {
                if (myTeacherSalaryContractSetBucket.TeacherSalaryContractPerformanceSet != null)
                {
                    myTeacherSalaryContractPerformanceSet = myTeacherSalaryContractSetBucket.TeacherSalaryContractPerformanceSet;
                }
                if (myTeacherSalaryContractSetBucket.TeacherSalaryContractPerformanceSetDetails != null &&
                    myTeacherSalaryContractSetBucket.TeacherSalaryContractPerformanceSetDetails.Any())
                {
                    myTeacherSalaryContractPerformanceSetDetails = myTeacherSalaryContractSetBucket.TeacherSalaryContractPerformanceSetDetails;
                }
                if (myTeacherSalaryContractSetBucket.EtTeacherSalaryContractPerformanceLessonBascs != null
                    && myTeacherSalaryContractSetBucket.EtTeacherSalaryContractPerformanceLessonBascs.Any())
                {
                    myTeacherSalaryContractPerformanceLessonBasc = myTeacherSalaryContractSetBucket.EtTeacherSalaryContractPerformanceLessonBascs;
                }
            }

            if (myTeacherSalaryContractPerformanceSet != null && myTeacherSalaryContractPerformanceSet.ComputeType != request.NewComputeType)
            {
                myTeacherSalaryContractPerformanceSetDetails = null;
            }

            //绩效工资
            var performanceSetItems = await GetTeacherSalaryContractPerformanceSet(request.TeacherId,
                myTeacherSalaryContractPerformanceSetDetails, myTeacherSalaryContractPerformanceLessonBasc,
                request.NewComputeType, request.NewGradientCalculateType);

            return ResponseBase.Success(new TeacherSalaryContractChangeComputeTypeOutput()
            {
                PerformanceSetItems = performanceSetItems
            }); ;
        }

        public async Task<ResponseBase> TeacherSalaryContractSave(TeacherSalaryContractSaveRequest request)
        {
            var teacher = await _userDAL.GetUser(request.TeacherId);
            if (teacher == null)
            {
                return ResponseBase.CommonError("员工不存在");
            }

            var config = await GetTeacherSalaryFundsItems(false);
            if (config.IsOpenContractPerformance)
            {
                if (request.ContractPerformanceSet == null)
                {
                    return ResponseBase.CommonError("请设置绩效工资");
                }
                if (request.PerformanceSetDetails != null && request.PerformanceSetDetails.Count > 0)
                {
                    var tempStudentDeSum = request.PerformanceSetDetails.FirstOrDefault(p => p.ComputeMode == EmTeacherSalaryComputeMode.StudentDeSum && p.ComputeValue >= 100);
                    if (tempStudentDeSum != null)
                    {
                        return ResponseBase.CommonError("按课消金额 计算值应该小于100");
                    }
                }
            }

            var teacherSalaryContractFixeds = new List<EtTeacherSalaryContractFixed>();
            foreach (var reqFixed in request.ContractFixeds)
            {
                teacherSalaryContractFixeds.Add(new EtTeacherSalaryContractFixed()
                {
                    AmountValue = reqFixed.AmountValue,
                    FundsItemsId = reqFixed.FundsItemsId,
                    IsDeleted = EmIsDeleted.Normal,
                    TeacherId = request.TeacherId,
                    TenantId = request.LoginTenantId
                });
            }

            if (request.PerformanceLessonBascs == null)
            {
                request.PerformanceLessonBascs = new List<PerformanceLessonBasc>();
            }
            EtTeacherSalaryContractPerformanceSet performanceSet = null;
            var performanceSetDetails = new List<EtTeacherSalaryContractPerformanceSetDetail>();
            var performanceLessonBascs = new List<EtTeacherSalaryContractPerformanceLessonBasc>();
            if (config.IsOpenContractPerformance)
            {
                performanceSet = new EtTeacherSalaryContractPerformanceSet()
                {
                    ComputeDesc = string.Empty,
                    ComputeType = request.ContractPerformanceSet.ComputeType,
                    GradientCalculateType = request.ContractPerformanceSet.GradientCalculateType,
                    IsDeleted = EmIsDeleted.Normal,
                    TeacherId = request.TeacherId,
                    TenantId = request.LoginTenantId
                };
                var strDesc = new StringBuilder();
                strDesc.Append($"<div class='performance_set_rule'>结算方式：{EmTeacherSalaryComputeType.GetTeacherSalaryComputeTypeDesc(performanceSet.ComputeType)} </div>");
                strDesc.Append($"<div class='performance_set_rule'>梯度计算：{EmTeacherSalaryGradientCalculateType.GetTeacherSalaryGradientCalculateTypeDesc(performanceSet.GradientCalculateType)}</div>");
                var bascValue = 0M;
                switch (performanceSet.ComputeType)
                {
                    case EmTeacherSalaryComputeType.Class:
                        var allClassIds = request.PerformanceSetDetails.Select(p => p.RelationId).Distinct();
                        foreach (var classId in allClassIds)
                        {
                            var myClassBucket = await _classDAL.GetClassBucket(classId);
                            if (myClassBucket == null || myClassBucket.EtClass == null)
                            {
                                continue;
                            }
                            bascValue = 0;
                            var myClassLessonBascs = request.PerformanceLessonBascs.FirstOrDefault(p => p.RelationId == classId);
                            if (myClassLessonBascs != null)
                            {
                                bascValue = myClassLessonBascs.ComputeValue;
                                performanceLessonBascs.Add(new EtTeacherSalaryContractPerformanceLessonBasc()
                                {
                                    ComputeType = performanceSet.ComputeType,
                                    ComputeValue = myClassLessonBascs.ComputeValue,
                                    IsDeleted = EmIsDeleted.Normal,
                                    RelationId = classId,
                                    TeacherId = request.TeacherId,
                                    TenantId = request.LoginTenantId
                                });
                            }
                            var myClassSetDetails = request.PerformanceSetDetails.Where(p => p.RelationId == classId).OrderBy(p => p.MinLimit);
                            var tempClassPerformanceSetDetailEntity = new List<EtTeacherSalaryContractPerformanceSetDetail>();
                            foreach (var p in myClassSetDetails)
                            {
                                tempClassPerformanceSetDetailEntity.Add(new EtTeacherSalaryContractPerformanceSetDetail()
                                {
                                    ComputeMode = p.ComputeMode,
                                    ComputeType = performanceSet.ComputeType,
                                    ComputeValue = p.ComputeValue,
                                    IsDeleted = EmIsDeleted.Normal,
                                    MaxLimit = p.MaxLimit,
                                    MinLimit = p.MinLimit,
                                    RelationId = classId,
                                    TeacherId = request.TeacherId,
                                    TenantId = request.LoginTenantId
                                });
                            }
                            var desClassResult = ComBusiness4.GetTeacherSalaryContractPerformanceSetDetailDesc(tempClassPerformanceSetDetailEntity, bascValue);
                            strDesc.Append($"<div class='performance_set_rule_title'>{myClassBucket.EtClass.Name}：{desClassResult.Item2}</div>{desClassResult.Item1}");
                            performanceSetDetails.AddRange(tempClassPerformanceSetDetailEntity);
                        }
                        break;
                    case EmTeacherSalaryComputeType.Course:
                        var allCourseIds = request.PerformanceSetDetails.Select(p => p.RelationId).Distinct();
                        foreach (var courseId in allCourseIds)
                        {
                            var myCourseResult = await _courseDAL.GetCourse(courseId);
                            if (myCourseResult == null || myCourseResult.Item1 == null)
                            {
                                continue;
                            }
                            bascValue = 0;
                            var myCourseLessonBascs = request.PerformanceLessonBascs.FirstOrDefault(p => p.RelationId == courseId);
                            if (myCourseLessonBascs != null)
                            {
                                bascValue = myCourseLessonBascs.ComputeValue;
                                performanceLessonBascs.Add(new EtTeacherSalaryContractPerformanceLessonBasc()
                                {
                                    ComputeType = performanceSet.ComputeType,
                                    ComputeValue = myCourseLessonBascs.ComputeValue,
                                    IsDeleted = EmIsDeleted.Normal,
                                    RelationId = courseId,
                                    TeacherId = request.TeacherId,
                                    TenantId = request.LoginTenantId
                                });
                            }
                            var myCourseDetails = request.PerformanceSetDetails.Where(p => p.RelationId == courseId).OrderBy(p => p.MinLimit);
                            var tempCoursePerformanceSetDetailEntity = new List<EtTeacherSalaryContractPerformanceSetDetail>();
                            foreach (var p in myCourseDetails)
                            {
                                tempCoursePerformanceSetDetailEntity.Add(new EtTeacherSalaryContractPerformanceSetDetail()
                                {
                                    ComputeMode = p.ComputeMode,
                                    ComputeType = performanceSet.ComputeType,
                                    ComputeValue = p.ComputeValue,
                                    IsDeleted = EmIsDeleted.Normal,
                                    MaxLimit = p.MaxLimit,
                                    MinLimit = p.MinLimit,
                                    RelationId = courseId,
                                    TeacherId = request.TeacherId,
                                    TenantId = request.LoginTenantId
                                });
                            }
                            var desCourseResult = ComBusiness4.GetTeacherSalaryContractPerformanceSetDetailDesc(tempCoursePerformanceSetDetailEntity, bascValue);
                            strDesc.Append($"<div class='performance_set_rule_title'>{myCourseResult.Item1.Name}：{desCourseResult.Item2}</div>{desCourseResult.Item1}");
                            performanceSetDetails.AddRange(tempCoursePerformanceSetDetailEntity);
                        }
                        break;
                    case EmTeacherSalaryComputeType.Global:
                        var myGlobalSetDetails = request.PerformanceSetDetails.Where(p => p.RelationId == 0).OrderBy(p => p.MinLimit);
                        if (myGlobalSetDetails.Any())
                        {
                            bascValue = 0;
                            var myGlobalLessonBascs = request.PerformanceLessonBascs.FirstOrDefault(p => p.RelationId == 0);
                            if (myGlobalLessonBascs != null)
                            {
                                bascValue = myGlobalLessonBascs.ComputeValue;
                                performanceLessonBascs.Add(new EtTeacherSalaryContractPerformanceLessonBasc()
                                {
                                    ComputeType = performanceSet.ComputeType,
                                    ComputeValue = myGlobalLessonBascs.ComputeValue,
                                    IsDeleted = EmIsDeleted.Normal,
                                    RelationId = 0,
                                    TeacherId = request.TeacherId,
                                    TenantId = request.LoginTenantId
                                });
                            }
                            foreach (var p in myGlobalSetDetails)
                            {
                                performanceSetDetails.Add(new EtTeacherSalaryContractPerformanceSetDetail()
                                {
                                    ComputeValue = p.ComputeValue,
                                    ComputeMode = p.ComputeMode,
                                    ComputeType = performanceSet.ComputeType,
                                    IsDeleted = EmIsDeleted.Normal,
                                    MaxLimit = p.MaxLimit,
                                    MinLimit = p.MinLimit,
                                    RelationId = 0,
                                    TeacherId = request.TeacherId,
                                    TenantId = request.LoginTenantId
                                });
                            }
                            var desGlobalResult = ComBusiness4.GetTeacherSalaryContractPerformanceSetDetailDesc(performanceSetDetails, bascValue);
                            strDesc.Append($"<div class='performance_set_rule_title'>统一设置：{desGlobalResult.Item2}</div>{desGlobalResult.Item1}");
                        }
                        break;
                }
                performanceSet.ComputeDesc = strDesc.ToString();
            }

            await _teacherSalaryContractDAL.SaveTeacherSalaryContract(request.TeacherId, teacherSalaryContractFixeds, performanceSet,
                performanceSetDetails, performanceLessonBascs);

            await _userOperationLogDAL.AddUserLog(request, $"设置员工[{teacher.Name}]绩效工资", EmUserOperationType.TeacherSalary);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TeacherSalaryContractClear(TeacherSalaryContractClearRequest request)
        {
            var teacher = await _userDAL.GetUser(request.TeacherId);
            if (teacher == null)
            {
                return ResponseBase.CommonError("员工不存在");
            }

            await _teacherSalaryContractDAL.ClearTeacherSalaryContractPerformance(request.TeacherId);

            await _userOperationLogDAL.AddUserLog(request, $"清除员工[{teacher.Name}]绩效工资设置信息", EmUserOperationType.TeacherSalary);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TeacherSalaryPayrollGoSettlement(TeacherSalaryPayrollGoSettlementRequest request)
        {
            var lockKey = new TeacherSalaryPayrollGoSettlementToken(request.LoginTenantId);
            if (_distributedLockDAL.LockTake(lockKey))
            {
                try
                {
                    return await this.TeacherSalaryPayrollGoSettlementProcess(request);
                }
                finally
                {
                    _distributedLockDAL.LockRelease(lockKey);
                }
            }
            else
            {
                return ResponseBase.CommonError("系统正在处理工资结算，请稍后再试");
            }
        }

        private async Task<ResponseBase> TeacherSalaryPayrollGoSettlementProcess(TeacherSalaryPayrollGoSettlementRequest request)
        {
            if (await _teacherSalaryPayrollDAL.ExistName(request.Name))
            {
                return ResponseBase.CommonError("结算名称已存在");
            }
            var config = await GetTeacherSalaryFundsItems(false);
            var isOpenContractPerformance = config.IsOpenContractPerformance;
            IEnumerable<EtTeacherSalaryClassTimes> teacherSalaryClassTimesList = null;
            IEnumerable<EtTeacherSalaryClassTimes2> teacherSalaryClassTimesList2 = null;
            if (isOpenContractPerformance)
            {
                teacherSalaryClassTimesList = await _teacherSalaryClassDAL.GetTeacherSalaryClassTimes(request.UserIds,
                    request.StartOt.Value, request.EndOt.Value);
                teacherSalaryClassTimesList2 = await _teacherSalaryClassDAL.GetTeacherSalaryClassTimes2(request.UserIds,
                    request.StartOt.Value, request.EndOt.Value);
            }
            var allTeacherSalaryFundsItem = new List<TeacherSalaryFundsItemOutput>();
            if (config.DefaultItems != null && config.DefaultItems.Count > 0)
            {
                allTeacherSalaryFundsItem.AddRange(config.DefaultItems);
            }
            if (config.CustomItems != null && config.CustomItems.Count > 0)
            {
                allTeacherSalaryFundsItem.AddRange(config.CustomItems);
            }
            var globalConfig = await _appConfig2BLL.GetTeacherSalaryGlobalRule();
            var processHandler = new UserSalarySettlementHandler(_userDAL, _teacherSalaryPayrollDAL, _teacherSalaryContractDAL,
                isOpenContractPerformance, allTeacherSalaryFundsItem, globalConfig, teacherSalaryClassTimesList, teacherSalaryClassTimesList2);
            return await processHandler.Process(request);
        }

        public async Task<ResponseBase> TeacherSalaryPayrollGetPaging(TeacherSalaryPayrollGetPagingRequest request)
        {
            var pagingData = await _teacherSalaryPayrollDAL.GetSalaryPayrollPaging(request);
            var output = new List<TeacherSalaryPayrollGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var tempBoxUser = new DataTempBox<EtUser>();
                foreach (var p in pagingData.Item1)
                {
                    var opUser = await ComBusiness.GetUser(tempBoxUser, _userDAL, p.OpUserId);
                    var userIds = EtmsHelper.AnalyzeMuIds(p.UserIds);
                    var userNames = new List<string>();
                    for (var i = 0; i < userIds.Count; i++)
                    {
                        if (userNames.Count > 2)
                        {
                            break;
                        }
                        var myUser = await ComBusiness.GetUser(tempBoxUser, _userDAL, userIds[i]);
                        if (myUser != null)
                        {
                            userNames.Add(myUser.Name);
                        }
                    }
                    string strUserNameDesc;
                    if (userIds.Count > 3)
                    {
                        strUserNameDesc = $"{string.Join('、', userNames)}等{p.UserCount}名员工";
                    }
                    else
                    {
                        strUserNameDesc = string.Join('、', userNames);
                    }
                    var item = new TeacherSalaryPayrollGetPagingOutput()
                    {
                        CId = p.Id,
                        DateDesc = $"{p.StartDate.EtmsToDateString()}至{p.EndDate.EtmsToDateString()}",
                        Name = p.Name,
                        OpUserId = p.OpUserId,
                        OpUserName = opUser?.Name,
                        OtDesc = p.Ot.EtmsToDateString(),
                        PaySum = p.PaySum,
                        PayDateDesc = p.PayDate.EtmsToDateString(),
                        Status = p.Status,
                        StatusDesc = EmTeacherSalaryPayrollStatus.GetTeacherSalaryPayrollStatusDesc(p.Status),
                        UserCount = p.UserCount,
                        UserNameDesc = strUserNameDesc
                    };
                    output.Add(item);
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<TeacherSalaryPayrollGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> TeacherSalaryPayrollGet(TeacherSalaryPayrollGetRequest request)
        {
            var salaryPayrollBucket = await _teacherSalaryPayrollDAL.GetTeacherSalaryPayrollBucket(request.CId);
            if (salaryPayrollBucket == null || salaryPayrollBucket.TeacherSalaryPayroll == null)
            {
                return ResponseBase.CommonError("未找到此工资条");
            }
            var teacherSalaryPayroll = salaryPayrollBucket.TeacherSalaryPayroll;
            var teacherSalaryPayrollUsers = salaryPayrollBucket.TeacherSalaryPayrollUsers;
            var teacherSalaryPayrollUserDetails = salaryPayrollBucket.TeacherSalaryPayrollUserDetails;
            var teacherSalaryPayrollUserPerformances = salaryPayrollBucket.TeacherSalaryPayrollUserPerformances;
            var userOp = await _userDAL.GetUser(teacherSalaryPayroll.OpUserId);
            var bascInfo = new TeacherSalaryPayrollInfo()
            {
                CId = teacherSalaryPayroll.Id,
                DateDesc = $"{teacherSalaryPayroll.StartDate.EtmsToDateString()}至{teacherSalaryPayroll.EndDate.EtmsToDateString()}",
                Name = teacherSalaryPayroll.Name,
                OpUserId = teacherSalaryPayroll.OpUserId,
                OpUserName = userOp?.Name,
                OtDesc = teacherSalaryPayroll.Ot.EtmsToDateString(),
                PayDateDesc = teacherSalaryPayroll.PayDate.EtmsToDateString(),
                PaySum = teacherSalaryPayroll.PaySum,
                Status = teacherSalaryPayroll.Status,
                StatusDesc = EmTeacherSalaryPayrollStatus.GetTeacherSalaryPayrollStatusDesc(teacherSalaryPayroll.Status),
                UserCount = teacherSalaryPayroll.UserCount
            };
            var payrollUserTableHeads = new List<PagingTableHeadOutput>();
            var firstUserSalaryDetail = teacherSalaryPayrollUserDetails.Where(p => p.UserId == teacherSalaryPayrollUsers[0].UserId).OrderBy(p => p.OrderIndex);
            var index = 0;
            foreach (var p in firstUserSalaryDetail)
            {
                payrollUserTableHeads.Add(new PagingTableHeadOutput()
                {
                    Index = index,
                    Label = p.FundsItemsName,
                    Type = p.FundsItemsType,
                    Id = p.FundsItemsId,
                    Property = $"salaryContract{index}",
                    OtherInfo = p.Id == SystemConfig.ComConfig.TeacherSalaryPerformanceDefaultId
                });
                index++;
            }

            var tempBoxClass = new DataTempBox<EtClass>();
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var payrollUserList = new List<TeacherSalaryPayrollUserInfo>();
            foreach (var myUserItem in teacherSalaryPayrollUsers)
            {
                var myUser = await _userDAL.GetUser(myUserItem.UserId);
                if (myUser == null)
                {
                    continue;
                }
                var item = new TeacherSalaryPayrollUserInfo()
                {
                    UserId = myUserItem.UserId,
                    CId = myUserItem.Id,
                    UserName = myUser.Name,
                    UserPhone = myUser.Phone,
                    IsTeacher = myUser.IsTeacher,
                    IsTeacherDesc = myUser.IsTeacher ? "是" : "否",
                    PayItemSum = myUserItem.PayItemSum
                };
                foreach (var myFundsItem in payrollUserTableHeads)
                {
                    var mySalaryItem = teacherSalaryPayrollUserDetails.FirstOrDefault(p => p.FundsItemsId == myFundsItem.Id && p.TeacherSalaryPayrollUserId == myUserItem.Id);
                    if (mySalaryItem != null)
                    {
                        item.SetSalaryContract(myFundsItem.Index, mySalaryItem.AmountSum.EtmsToString2());
                    }
                }
                payrollUserList.Add(item);
            }

            return ResponseBase.Success(new TeacherSalaryPayrollGetOutput()
            {
                BascInfo = bascInfo,
                PayrollUserTableHeads = payrollUserTableHeads,
                PayrollUserList = payrollUserList
            });
        }

        public async Task<ResponseBase> TeacherSalaryPayrollUserGet(TeacherSalaryPayrollUserGetRequest request)
        {
            var salaryPayrollBucket = await _teacherSalaryPayrollDAL.GetTeacherSalaryPayrollBucket(request.TeacherSalaryPayrollIdId);
            if (salaryPayrollBucket == null || salaryPayrollBucket.TeacherSalaryPayroll == null)
            {
                return ResponseBase.CommonError("未找到此工资条");
            }
            var teacherSalaryPayroll = salaryPayrollBucket.TeacherSalaryPayroll;
            var teacherSalaryPayrollUsers = salaryPayrollBucket.TeacherSalaryPayrollUsers;
            var teacherSalaryPayrollUserDetails = salaryPayrollBucket.TeacherSalaryPayrollUserDetails;
            var teacherSalaryPayrollUserPerformances = salaryPayrollBucket.TeacherSalaryPayrollUserPerformances;
            var myPayrollUser = teacherSalaryPayrollUsers.FirstOrDefault(p => p.Id == request.PayrollUserId);
            if (myPayrollUser == null)
            {
                return ResponseBase.CommonError("未找到员工工资信息");
            }
            var user = await _userDAL.GetUser(myPayrollUser.UserId);
            if (user == null)
            {
                return ResponseBase.CommonError("员工不存在");
            }
            var baseInfo = new TeacherSalaryPayrollUserBaseInfo()
            {
                ComputeType = myPayrollUser.ComputeType,
                ComputeTypeDesc = EmTeacherSalaryComputeType.GetTeacherSalaryComputeTypeDesc(myPayrollUser.ComputeType),
                ComputeTypeDescTag = EmTeacherSalaryComputeType.GetComputeTypeDescTag(myPayrollUser.ComputeType),
                GradientCalculateType = myPayrollUser.GradientCalculateType,
                GradientCalculateTypeDesc = EmTeacherSalaryGradientCalculateType.GetTeacherSalaryGradientCalculateTypeDesc(myPayrollUser.GradientCalculateType),
                StatisticalRuleType = myPayrollUser.StatisticalRuleType,
                StatisticalRuleTypeDesc = EmTeacherSalaryStatisticalRuleType.GetTeacherSalaryStatisticalRuleType(myPayrollUser.StatisticalRuleType),
                PerformanceSetDesc = myPayrollUser.PerformanceSetDesc,
                IncludeArrivedMakeUpStudent = myPayrollUser.IncludeArrivedMakeUpStudent,
                IncludeArrivedTryCalssStudent = myPayrollUser.IncludeArrivedTryCalssStudent,
                DateDesc = $"{teacherSalaryPayroll.StartDate.EtmsToDateString()}至{teacherSalaryPayroll.EndDate.EtmsToDateString()}",
                Name = teacherSalaryPayroll.Name,
                Status = teacherSalaryPayroll.Status,
                PayrollUserId = request.PayrollUserId,
                TeacherSalaryPayrollIdId = request.TeacherSalaryPayrollIdId,
                PayItemSum = myPayrollUser.PayItemSum,
                UserName = user.Name,
                IsOpenClassPerformance = false
            };
            var fixedSalarys = new List<TeacherSalaryPayrollUserFixedSalary>();
            var mySalaryPayrollUserDetails = teacherSalaryPayrollUserDetails.Where(p => p.TeacherSalaryPayrollUserId == request.PayrollUserId).OrderBy(p => p.OrderIndex);
            foreach (var p in mySalaryPayrollUserDetails)
            {
                if (p.FundsItemsId == SystemConfig.ComConfig.TeacherSalaryPerformanceDefaultId)
                {
                    baseInfo.IsOpenClassPerformance = true;
                }
                else
                {
                    fixedSalarys.Add(new TeacherSalaryPayrollUserFixedSalary()
                    {
                        FundsItemsId = p.FundsItemsId,
                        FundsItemsType = p.FundsItemsType,
                        FundsItemsName = p.FundsItemsName,
                        AmountSum = p.AmountSum,
                        Id = p.Id
                    });
                }
            }
            List<UserPerformances> performanceSalarys = null;
            if (baseInfo.IsOpenClassPerformance)
            {
                performanceSalarys = new List<UserPerformances>();
                if (teacherSalaryPayrollUserPerformances != null && teacherSalaryPayrollUserPerformances.Any())
                {
                    var myUserPerformances = teacherSalaryPayrollUserPerformances.Where(p => p.TeacherSalaryPayrollUserId == request.PayrollUserId);
                    var tempBoxClass = new DataTempBox<EtClass>();
                    var tempBoxCourse = new DataTempBox<EtCourse>();
                    if (myUserPerformances.Any())
                    {
                        foreach (var p in myUserPerformances)
                        {
                            string relationDesc = null;
                            switch (myPayrollUser.ComputeType)
                            {
                                case EmTeacherSalaryComputeType.Class:
                                    var myClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, p.RelationId);
                                    relationDesc = myClass?.Name;
                                    break;
                                case EmTeacherSalaryComputeType.Course:
                                    var myCourse = await ComBusiness.GetCourse(tempBoxCourse, _courseDAL, p.RelationId);
                                    relationDesc = myCourse?.Name;
                                    break;
                                case EmTeacherSalaryComputeType.Global:
                                    relationDesc = "统一设置";
                                    break;
                            }
                            performanceSalarys.Add(new UserPerformances()
                            {
                                CId = p.Id,
                                ComputeDesc = p.ComputeDesc,
                                ComputeMode = p.ComputeMode,
                                ComputeModeDesc = EmTeacherSalaryComputeMode.GetTeacherSalaryComputeModeDesc(p.ComputeMode),
                                ComputeRelationValue = p.ComputeRelationValue,
                                ComputeRelationValueDesc = EmTeacherSalaryComputeMode.GetComputeRelationValueDesc(p.ComputeMode, p.ComputeRelationValue),
                                ComputeSum = p.ComputeSum,
                                ComputeType = p.ComputeType,
                                RelationDesc = relationDesc,
                                RelationId = p.RelationId,
                                SubmitSum = p.SubmitSum,
                                TeacherSalaryPayrollId = p.TeacherSalaryPayrollId,
                                TeacherSalaryPayrollUserId = p.TeacherSalaryPayrollUserId
                            });
                        }
                    }
                }
            }

            return ResponseBase.Success(new TeacherSalaryPayrollUserGetOutput()
            {
                BaseInfo = baseInfo,
                FixedSalarys = fixedSalarys,
                PerformanceSalarys = performanceSalarys
            });
        }

        public async Task<ResponseBase> TeacherSalaryPayrollUserModify(TeacherSalaryPayrollUserModifyRequest request)
        {
            var salaryPayrollBucket = await _teacherSalaryPayrollDAL.GetTeacherSalaryPayrollBucket(request.TeacherSalaryPayrollIdId);
            if (salaryPayrollBucket == null || salaryPayrollBucket.TeacherSalaryPayroll == null)
            {
                return ResponseBase.CommonError("未找到此工资条");
            }
            var teacherSalaryPayroll = salaryPayrollBucket.TeacherSalaryPayroll;
            var teacherSalaryPayrollUsers = salaryPayrollBucket.TeacherSalaryPayrollUsers;
            var teacherSalaryPayrollUserDetails = salaryPayrollBucket.TeacherSalaryPayrollUserDetails;
            var teacherSalaryPayrollUserPerformances = salaryPayrollBucket.TeacherSalaryPayrollUserPerformances;
            var myPayrollUser = teacherSalaryPayrollUsers.FirstOrDefault(p => p.Id == request.PayrollUserId);
            if (myPayrollUser == null)
            {
                return ResponseBase.CommonError("未找到员工工资信息");
            }

            //绩效工资
            var newTotalPerformanceSum = 0M;
            var updateTeacherSalaryPayrollUserPerformances = new List<TeacherSalaryUpdatePayValue>();
            if (request.PerformanceSalaryList != null && request.PerformanceSalaryList.Any())
            {
                var myUserPerformances = teacherSalaryPayrollUserPerformances.Where(p => p.TeacherSalaryPayrollUserId == request.PayrollUserId);
                foreach (var p in myUserPerformances)
                {
                    foreach (var j in request.PerformanceSalaryList)
                    {
                        if (p.Id == j.Id)
                        {
                            if (p.SubmitSum != j.NewValue)
                            {
                                p.SubmitSum = j.NewValue;
                                updateTeacherSalaryPayrollUserPerformances.Add(new TeacherSalaryUpdatePayValue()
                                {
                                    Id = p.Id,
                                    NewValue = j.NewValue
                                });
                            }
                            break;
                        }
                    }
                    newTotalPerformanceSum += p.SubmitSum;
                }
            }

            var updateTeacherSalaryPayrollUserDetails = new List<TeacherSalaryUpdatePayValue>();
            var myUserDetails = teacherSalaryPayrollUserDetails.Where(p => p.TeacherSalaryPayrollUserId == request.PayrollUserId);
            var newTotalPayItemSum = 0M;
            foreach (var p in myUserDetails)
            {
                if (p.FundsItemsId == SystemConfig.ComConfig.TeacherSalaryPerformanceDefaultId)
                {
                    if (p.AmountSum != newTotalPerformanceSum)
                    {
                        p.AmountSum = newTotalPerformanceSum;
                        updateTeacherSalaryPayrollUserDetails.Add(new TeacherSalaryUpdatePayValue()
                        {
                            Id = p.Id,
                            NewValue = newTotalPerformanceSum
                        });
                    }
                }
                else
                {
                    foreach (var j in request.FixedSalaryList)
                    {
                        if (p.Id == j.Id)
                        {
                            if (p.AmountSum != j.NewValue)
                            {
                                p.AmountSum = j.NewValue;
                                updateTeacherSalaryPayrollUserDetails.Add(new TeacherSalaryUpdatePayValue()
                                {
                                    Id = p.Id,
                                    NewValue = j.NewValue
                                });
                            }
                            break;
                        }
                    }
                }
                if (p.FundsItemsType == EmTeacherSalaryFundsItemsType.Add)
                {
                    newTotalPayItemSum += p.AmountSum;
                }
                else
                {
                    newTotalPayItemSum -= p.AmountSum;
                }
            }

            if (updateTeacherSalaryPayrollUserPerformances.Count == 0 && updateTeacherSalaryPayrollUserDetails.Count == 0) //未做任何改变
            {
                return ResponseBase.Success();
            }

            TeacherSalaryUpdatePayValue updateTeacherSalaryPayroll = null;
            TeacherSalaryUpdatePayValue updateTeacherSalaryPayrollUser = null;
            if (newTotalPayItemSum != myPayrollUser.PayItemSum)
            {
                updateTeacherSalaryPayrollUser = new TeacherSalaryUpdatePayValue()
                {
                    Id = myPayrollUser.Id,
                    NewValue = newTotalPayItemSum
                };
                updateTeacherSalaryPayroll = new TeacherSalaryUpdatePayValue()
                {
                    Id = teacherSalaryPayroll.Id,
                    NewValue = teacherSalaryPayroll.PaySum - myPayrollUser.PayItemSum + newTotalPayItemSum
                };
            }

            await _teacherSalaryPayrollDAL.UpdatePayValue(teacherSalaryPayroll.Id, updateTeacherSalaryPayroll, updateTeacherSalaryPayrollUser, updateTeacherSalaryPayrollUserDetails,
                updateTeacherSalaryPayrollUserPerformances);

            await _userOperationLogDAL.AddUserLog(request, $"修改工资条-{teacherSalaryPayroll.Name}", EmUserOperationType.TeacherSalary);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TeacherSalaryPayrollSetOK(TeacherSalaryPayrollSetOKRequest request)
        {
            var salaryPayrollBucket = await _teacherSalaryPayrollDAL.GetTeacherSalaryPayrollBucket(request.CId);
            if (salaryPayrollBucket == null || salaryPayrollBucket.TeacherSalaryPayroll == null)
            {
                return ResponseBase.CommonError("未找到此工资条");
            }
            var teacherSalaryPayroll = salaryPayrollBucket.TeacherSalaryPayroll;
            if (teacherSalaryPayroll.Status == EmTeacherSalaryPayrollStatus.IsOK)
            {
                return ResponseBase.CommonError("工资条已确认，无法执行此操作");
            }
            if (teacherSalaryPayroll.Status == EmTeacherSalaryPayrollStatus.Repeal)
            {
                return ResponseBase.CommonError("工资条已作废，无法执行此操作");
            }
            if (request.PayDate.Value <= teacherSalaryPayroll.StartDate)
            {
                return ResponseBase.CommonError("发薪日期不能小于结算开始日期");
            }

            await _teacherSalaryPayrollDAL.SetTeacherSalaryPayStatusIsOK(request.CId, request.PayDate.Value);

            //生成一笔支出记录
            var desc = $"工资条确认结算-{teacherSalaryPayroll.Name}";
            await _incomeLogDAL.AddIncomeLog(new EtIncomeLog()
            {
                AccountNo = string.Empty,
                CreateOt = DateTime.Now,
                IsDeleted = EmIsDeleted.Normal,
                No = string.Empty,
                Ot = request.PayDate.Value,
                PayType = request.PayType,
                ProjectType = EmIncomeLogProjectType.TeacherSalary,
                Type = EmIncomeLogType.AccountOut,
                Remark = desc,
                RepealOt = null,
                RepealUserId = null,
                Status = EmIncomeLogStatus.Normal,
                Sum = teacherSalaryPayroll.PaySum,
                TenantId = request.LoginTenantId,
                UserId = request.LoginUserId,
                OrderId = null,
                RelationId = teacherSalaryPayroll.Id
            });

            _eventPublisher.Publish(new StatisticsFinanceIncomeEvent(request.LoginTenantId)
            {
                StatisticsDate = request.PayDate.Value
            });
            _eventPublisher.Publish(new NoticeTeacherSalaryEvent(request.LoginTenantId)
            {
                PayrollId = teacherSalaryPayroll.Id
            });
            _eventPublisher.Publish(new StatisticsTeacherSalaryMonthEvent(request.LoginTenantId)
            {
                PayrollId = teacherSalaryPayroll.Id
            });

            await _userOperationLogDAL.AddUserLog(request, desc, EmUserOperationType.TeacherSalary);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TeacherSalaryPayrollDel(TeacherSalaryPayrollDelRequest request)
        {
            var salaryPayrollBucket = await _teacherSalaryPayrollDAL.GetTeacherSalaryPayrollBucket(request.CId);
            if (salaryPayrollBucket == null || salaryPayrollBucket.TeacherSalaryPayroll == null)
            {
                return ResponseBase.CommonError("未找到此工资条");
            }
            var teacherSalaryPayroll = salaryPayrollBucket.TeacherSalaryPayroll;
            if (teacherSalaryPayroll.Status == EmTeacherSalaryPayrollStatus.IsOK)
            {
                return ResponseBase.CommonError("工资条已确认，无法执行此操作");
            }
            //if (teacherSalaryPayroll.Status == EmTeacherSalaryPayrollStatus.Repeal)
            //{
            //    return ResponseBase.CommonError("工资条已作废，无法执行此操作");
            //}
            await _teacherSalaryPayrollDAL.DelTeacherSalaryPay(request.CId);

            await _userOperationLogDAL.AddUserLog(request, $"删除工资条-{teacherSalaryPayroll.Name}", EmUserOperationType.TeacherSalary);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TeacherSalaryPayrollRepeal(TeacherSalaryPayrollRepealRequest request)
        {
            var salaryPayrollBucket = await _teacherSalaryPayrollDAL.GetTeacherSalaryPayrollBucket(request.CId);
            if (salaryPayrollBucket == null || salaryPayrollBucket.TeacherSalaryPayroll == null)
            {
                return ResponseBase.CommonError("未找到此工资条");
            }
            var teacherSalaryPayroll = salaryPayrollBucket.TeacherSalaryPayroll;
            if (teacherSalaryPayroll.Status == EmTeacherSalaryPayrollStatus.NotSure)
            {
                return ResponseBase.CommonError("未确认的工资条，无法作废");
            }
            if (teacherSalaryPayroll.Status == EmTeacherSalaryPayrollStatus.Repeal)
            {
                return ResponseBase.CommonError("工资条已作废，无法执行此操作");
            }
            var now = DateTime.Now;
            await _teacherSalaryPayrollDAL.SetTeacherSalaryPayStatus(request.CId, EmTeacherSalaryPayrollStatus.Repeal);
            await _incomeLogDAL.RepealIncomeLog(EmIncomeLogProjectType.TeacherSalary, request.CId, request.LoginUserId, now);

            _eventPublisher.Publish(new StatisticsFinanceIncomeEvent(request.LoginTenantId)
            {
                StatisticsDate = teacherSalaryPayroll.PayDate.Value
            });
            _eventPublisher.Publish(new NoticeTeacherSalaryEvent(request.LoginTenantId)
            {
                PayrollId = teacherSalaryPayroll.Id
            });
            _eventPublisher.Publish(new StatisticsTeacherSalaryMonthEvent(request.LoginTenantId)
            {
                PayrollId = teacherSalaryPayroll.Id
            });

            await _userOperationLogDAL.AddUserLog(request, $"工资条作废-{teacherSalaryPayroll.Name}", EmUserOperationType.TeacherSalary);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TeacherSalaryUserPerformanceDetailGetPaging(TeacherSalaryUserPerformanceDetailGetPagingRequest request)
        {
            if (request.UserPerformanceId == null)
            {
                return ResponseBase.Success(new ResponsePagingDataBase<TeacherSalaryUserPerformanceDetailGetPagingOutput>(0, new List<TeacherSalaryUserPerformanceDetailGetPagingOutput>()));
            }
            var pagingData = await _teacherSalaryPayrollDAL.GetUserPerformanceDetailPaging(request);
            var output = new List<TeacherSalaryUserPerformanceDetailGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var tempBoxClass = new DataTempBox<EtClass>();
                var tempBoxCourse = new DataTempBox<EtCourse>();
                foreach (var p in pagingData.Item1)
                {
                    string className = null;
                    string courseName = null;
                    var myClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, p.ClassId);
                    if (myClass != null)
                    {
                        className = myClass.Name;
                    }
                    var myCourse = await ComBusiness.GetCourse(tempBoxCourse, _courseDAL, p.CourseId);
                    if (myCourse != null)
                    {
                        courseName = myCourse.Name;
                    }
                    output.Add(new TeacherSalaryUserPerformanceDetailGetPagingOutput()
                    {
                        ArrivedAndBeLateCount = p.ArrivedAndBeLateCount,
                        ArrivedCount = p.ArrivedCount,
                        BeLateCount = p.BeLateCount,
                        CId = p.Id,
                        ClassOtDesc = p.ClassOt.EtmsToDateString(),
                        ClassRecordId = p.ClassRecordId,
                        ClassTimeDesc = $"{EtmsHelper.GetTimeDesc(p.StartTime)}~{EtmsHelper.GetTimeDesc(p.EndTime)}",
                        ComputeMode = p.ComputeMode,
                        ComputeType = p.ComputeType,
                        ComputeSum = p.ComputeSum,
                        DeSum = p.DeSum,
                        LeaveCount = p.LeaveCount,
                        MakeUpEffectiveCount = p.MakeUpEffectiveCount,
                        MakeUpStudentCount = p.MakeUpStudentCount,
                        NotArrivedCount = p.NotArrivedCount,
                        TeacherClassTimes = p.TeacherClassTimes,
                        StudentClassTimes = p.StudentClassTimes,
                        TryCalssEffectiveCount = p.TryCalssEffectiveCount,
                        TryCalssStudentCount = p.TryCalssStudentCount,
                        WeekDesc = $"周{EtmsHelper.GetWeekDesc(p.Week)}",
                        ClassName = className,
                        CourseName = courseName
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<TeacherSalaryUserPerformanceDetailGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> TeacherSalaryUserGetH5(TeacherSalaryUserGetH5Request request)
        {
            int year;
            int month;
            if (request.Year != null && request.Month != null)
            {
                year = request.Year.Value;
                month = request.Month.Value;
            }
            else
            {
                year = DateTime.Now.Year;
                month = DateTime.Now.Month;
            }
            var myData = await _teacherSalaryPayrollDAL.GetValidSalaryPayrollUser(request.LoginUserId, year, month);
            var totalSalary = 0M;
            var salaryDetails = new List<TeacherSalaryUserGetH5UserDetail>();
            if (myData.Any())
            {
                var tempData = myData.OrderByDescending(p => p.PayDate);
                foreach (var p in tempData)
                {
                    totalSalary += p.PayItemSum;
                    salaryDetails.Add(new TeacherSalaryUserGetH5UserDetail()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        OtDesc = $"{p.StartDate.EtmsToDateString()}至{p.EndDate.EtmsToDateString()}",
                        PayDateDesc = p.PayDate.EtmsToDateString(),
                        PayItemSum = p.PayItemSum.ToString("F2")
                    });
                }
            }
            var output = new TeacherSalaryUserGetH5Output()
            {
                Year = year,
                Month = month,
                TotalSalary = totalSalary.ToString("F2"),
                SalaryDetails = salaryDetails
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> TeacherSalaryUserDetailGetH5(TeacherSalaryUserDetailGetH5Request request)
        {
            var myTeacherSalaryPayrollUser = await _teacherSalaryPayrollDAL.GetTeacherSalaryPayrollUser(request.Id);
            if (myTeacherSalaryPayrollUser == null)
            {
                return ResponseBase.CommonError("未找到此工资条");
            }
            if (myTeacherSalaryPayrollUser.UserId != request.LoginUserId)
            {
                return ResponseBase.CommonError("您没有权限查看此工资条");
            }
            var salaryPayrollBucket = await _teacherSalaryPayrollDAL.GetTeacherSalaryPayrollBucket(myTeacherSalaryPayrollUser.TeacherSalaryPayrollId);
            if (salaryPayrollBucket == null || salaryPayrollBucket.TeacherSalaryPayroll == null)
            {
                return ResponseBase.CommonError("未找到此工资条");
            }
            var teacherSalaryPayroll = salaryPayrollBucket.TeacherSalaryPayroll;

            var output = new TeacherSalaryUserDetailGetH5Output()
            {
                Name = teacherSalaryPayroll.Name,
                PayDateDesc = teacherSalaryPayroll.PayDate.EtmsToDateString(),
                OtDesc = $"{teacherSalaryPayroll.StartDate.EtmsToDateString()}至{teacherSalaryPayroll.EndDate.EtmsToDateString()}",
                Items = new List<TeacherSalaryUserDetailGetH5Item>()
            };
            var myTeacherSalaryPayrollUserDetails = salaryPayrollBucket.TeacherSalaryPayrollUserDetails.Where(p => p.TeacherSalaryPayrollUserId == request.Id).OrderBy(p => p.OrderIndex);
            if (myTeacherSalaryPayrollUserDetails.Any())
            {
                foreach (var p in myTeacherSalaryPayrollUserDetails)
                {
                    output.Items.Add(new TeacherSalaryUserDetailGetH5Item()
                    {
                        Type = p.FundsItemsType,
                        Name = p.FundsItemsName,
                        AmountSum = p.AmountSum.ToString("F2")
                    });
                }
            }

            return ResponseBase.Success(output);
        }
    }
}
