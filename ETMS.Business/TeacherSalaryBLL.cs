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

        public TeacherSalaryBLL(IAppConfig2BLL appConfig2BLL, ITeacherSalaryFundsItemsDAL teacherSalaryFundsItemsDAL, ITeacherSalaryClassDAL teacherSalaryClassDAL,
            IUserOperationLogDAL userOperationLogDAL, IUserDAL userDAL, IClassDAL classDAL, ITeacherSalaryContractDAL teacherSalaryContractDAL,
            ICourseDAL courseDAL)
        {
            this._appConfig2BLL = appConfig2BLL;
            this._teacherSalaryFundsItemsDAL = teacherSalaryFundsItemsDAL;
            this._teacherSalaryClassDAL = teacherSalaryClassDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._userDAL = userDAL;
            this._classDAL = classDAL;
            this._teacherSalaryContractDAL = teacherSalaryContractDAL;
            this._courseDAL = courseDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._appConfig2BLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _teacherSalaryFundsItemsDAL, _teacherSalaryClassDAL,
                _userOperationLogDAL, _userDAL, _classDAL, _teacherSalaryContractDAL, _courseDAL);
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
                        Property = $"SalaryContract{index}",
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
                        SalaryContractStatus = EmBool.False
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
                            if (teacherSalaryContractSetBucket.TeacherSalaryContractPerformanceSet != null)
                            {
                                item.SetSalaryContract(index, teacherSalaryContractSetBucket.TeacherSalaryContractPerformanceSet.ComputeDesc);
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
            List<EtTeacherSalaryContractPerformanceSetDetail> myTeacherSalaryContractPerformanceSetDetails, byte computeType, byte gradientCalculateType)
        {
            var output = new List<TeacherSalaryContractPerformanceSet>();
            //绩效工资
            var teacherAllClass = await _classDAL.GetClassOfTeacher(teacherId);
            switch (computeType)
            {
                case EmTeacherSalaryComputeType.Class:
                    if (teacherAllClass.Any())
                    {
                        foreach (var myClass in teacherAllClass)
                        {
                            var performanceSetClass = new TeacherSalaryContractPerformanceSet()
                            {
                                ComputeMode = EmTeacherSalaryComputeMode.TeacherClassTimes,
                                RelationExtend = new List<string>(),
                                RelationId = myClass.Id,
                                RelationName = myClass.Name,
                                SetDetails = new List<TeacherSalaryContractPerformanceSetDetail>()
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
                            var performanceSetCourse = new TeacherSalaryContractPerformanceSet()
                            {
                                ComputeMode = EmTeacherSalaryComputeMode.TeacherClassTimes,
                                RelationExtend = new List<string>(),
                                RelationId = id,
                                RelationName = myCourse.Item1.Name,
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
                    output = new List<TeacherSalaryContractPerformanceSet>() {
                        new TeacherSalaryContractPerformanceSet(){
                            ComputeMode =computeModeGlobal,
                            RelationExtend = relationExtend,
                            RelationId = 0,
                            RelationName ="所有班级",
                            SetDetails =setDetailsGlobal,
                            ComputeModeDesc =  EmTeacherSalaryComputeMode.GetTeacherSalaryComputeModeDesc2(computeModeGlobal),
                            ComputeModeUnitDesc = EmTeacherSalaryComputeMode.GetModelUnitDesc(computeModeGlobal)
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
            }

            if (myTeacherSalaryContractPerformanceSet != null)
            {
                output.ComputeType = myTeacherSalaryContractPerformanceSet.ComputeType;
                output.GradientCalculateType = myTeacherSalaryContractPerformanceSet.GradientCalculateType;
            }

            //固定工资
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
                        Value = value
                    });
                }
            }

            //绩效工资
            output.PerformanceSetItems = await GetTeacherSalaryContractPerformanceSet(request.TeacherId, myTeacherSalaryContractPerformanceSetDetails,
                output.ComputeType, output.GradientCalculateType);

            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> TeacherSalaryContractChangeComputeType(TeacherSalaryContractChangeComputeTypeRequest request)
        {
            EtTeacherSalaryContractPerformanceSet myTeacherSalaryContractPerformanceSet = null;
            List<EtTeacherSalaryContractPerformanceSetDetail> myTeacherSalaryContractPerformanceSetDetails = null;
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
            }

            if (myTeacherSalaryContractPerformanceSet != null && myTeacherSalaryContractPerformanceSet.ComputeType != request.NewComputeType)
            {
                myTeacherSalaryContractPerformanceSetDetails = null;
            }

            //绩效工资
            var performanceSetItems = await GetTeacherSalaryContractPerformanceSet(request.TeacherId, myTeacherSalaryContractPerformanceSetDetails,
                request.NewComputeType, request.NewGradientCalculateType);

            return ResponseBase.Success(new TeacherSalaryContractChangeComputeTypeOutput()
            {
                PerformanceSetItems = performanceSetItems
            }); ;
        }

        public async Task<ResponseBase> TeacherSalaryContractSave(TeacherSalaryContractSaveRequest request)
        {
            var config = await GetTeacherSalaryFundsItems(false);
            if (config.IsOpenContractPerformance)
            {
                if (request.ContractPerformanceSet == null || request.PerformanceSetDetails == null || request.PerformanceSetDetails.Count == 0)
                {
                    return ResponseBase.CommonError("请设置绩效工资");
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

            EtTeacherSalaryContractPerformanceSet performanceSet = null;
            List<EtTeacherSalaryContractPerformanceSetDetail> performanceSetDetails = null;
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
                strDesc.Append($"<div class='performance_set_rule'>梯度工资：{EmTeacherSalaryGradientCalculateType.GetTeacherSalaryGradientCalculateTypeDesc(performanceSet.GradientCalculateType)}</div>");
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
                            var desClassResult = ComBusiness4.GetTeacherSalaryContractPerformanceSetDetailDesc(tempClassPerformanceSetDetailEntity);
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
                            var desCourseResult = ComBusiness4.GetTeacherSalaryContractPerformanceSetDetailDesc(tempCoursePerformanceSetDetailEntity);
                            strDesc.Append($"<div class='performance_set_rule_title'>{myCourseResult.Item1.Name}：{desCourseResult.Item2}</div>{desCourseResult.Item1}");
                            performanceSetDetails.AddRange(tempCoursePerformanceSetDetailEntity);
                        }
                        break;
                    case EmTeacherSalaryComputeType.Global:
                        var myGlobalSetDetails = request.PerformanceSetDetails.Where(p => p.RelationId == 0).OrderBy(p => p.MinLimit);
                        if (myGlobalSetDetails.Any())
                        {
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
                            var desGlobalResult = ComBusiness4.GetTeacherSalaryContractPerformanceSetDetailDesc(performanceSetDetails);
                            strDesc.Append($"<div class='performance_set_rule_title'>统一设置：{desGlobalResult.Item2}</div>{desGlobalResult.Item1}");
                        }
                        break;
                }
                performanceSet.ComputeDesc = strDesc.ToString();
            }

            await _teacherSalaryContractDAL.SaveTeacherSalaryContract(request.TeacherId, teacherSalaryContractFixeds, performanceSet, performanceSetDetails);
            return ResponseBase.Success();
        }
    }
}
