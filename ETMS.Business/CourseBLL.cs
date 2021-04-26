using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Product.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Dto.Product.Output;
using ETMS.Utility;
using ETMS.Business.Common;
using ETMS.Entity.Dto.Common.Output;

namespace ETMS.Business
{
    public class CourseBLL : ICourseBLL
    {
        private readonly ICourseDAL _courseDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IOrderDAL _orderDAL;

        private readonly ISuitDAL _suitDAL;

        public CourseBLL(ICourseDAL courseDAL, IUserOperationLogDAL userOperationLogDAL, IOrderDAL orderDAL, ISuitDAL suitDAL)
        {
            this._courseDAL = courseDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._orderDAL = orderDAL;
            this._suitDAL = suitDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _courseDAL, _userOperationLogDAL, _orderDAL, _suitDAL);
        }

        public async Task<ResponseBase> CourseAdd(CourseAddRequest request)
        {
            if (await _courseDAL.ExistCourse(request.Name))
            {
                return ResponseBase.CommonError("已存在相同名称的课程");
            }
            var coursePriceRuleInfo = GetCoursePriceRule(request.CoursePriceRules, 0, request.LoginTenantId);
            var course = new EtCourse()
            {
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name,
                Ot = DateTime.Now,
                Remark = request.Remark,
                StyleColor = request.StyleColor,
                TenantId = request.LoginTenantId,
                Type = request.Type,
                UserId = request.LoginUserId,
                PriceType = coursePriceRuleInfo.Item2,
                Status = EmProductStatus.Enabled,
                CheckPoints = request.CheckPoints.EtmsToPoints(),
                PriceTypeDesc = coursePriceRuleInfo.Item3
            };
            await _courseDAL.AddCourse(course, coursePriceRuleInfo.Item1);
            await _userOperationLogDAL.AddUserLog(request, $"添加课程-{request.Name}", EmUserOperationType.CourseManage);
            return ResponseBase.Success();
        }

        private Tuple<List<EtCoursePriceRule>, byte, string> GetCoursePriceRule(CoursePriceRule coursePriceRule, long courseId, int tenantId)
        {
            var rules = new List<EtCoursePriceRule>();
            var priceTypes = new List<byte>();
            var strPriceTypeDesc = new StringBuilder();
            if (coursePriceRule.IsByClassTimes)
            {
                foreach (var p in coursePriceRule.ByClassTimes)
                {
                    rules.Add(new EtCoursePriceRule()
                    {
                        CourseId = courseId,
                        IsDeleted = EmIsDeleted.Normal,
                        Name = p.Name,
                        Price = p.Price,
                        PriceType = EmCoursePriceType.ClassTimes,
                        PriceUnit = EmCourseUnit.ClassTimes,
                        Quantity = p.Quantity,
                        TotalPrice = p.TotalPrice,
                        TenantId = tenantId,
                        Points = p.Points.EtmsToPoints(),
                        Id = p.Id
                    });
                }
                priceTypes.Add(EmCoursePriceType.ClassTimes);
                strPriceTypeDesc.Append("按课时&");
            }
            if (coursePriceRule.IsByMonth)
            {
                foreach (var p in coursePriceRule.ByMonth)
                {
                    rules.Add(new EtCoursePriceRule()
                    {
                        CourseId = courseId,
                        IsDeleted = EmIsDeleted.Normal,
                        Name = p.Name,
                        Price = p.Price,
                        PriceType = EmCoursePriceType.Month,
                        PriceUnit = EmCourseUnit.Month,
                        Quantity = p.Quantity,
                        TotalPrice = p.TotalPrice,
                        TenantId = tenantId,
                        Points = p.Points.EtmsToPoints(),
                        Id = p.Id
                    });
                }
                priceTypes.Add(EmCoursePriceType.Month);
                strPriceTypeDesc.Append("按月&");
            }
            if (coursePriceRule.IsByDay)
            {
                foreach (var p in coursePriceRule.ByDay)
                {
                    rules.Add(new EtCoursePriceRule()
                    {
                        CourseId = courseId,
                        IsDeleted = EmIsDeleted.Normal,
                        Name = p.Name,
                        Price = p.Price,
                        PriceType = EmCoursePriceType.Day,
                        PriceUnit = EmCourseUnit.Day,
                        Quantity = p.Quantity,
                        TotalPrice = p.TotalPrice,
                        TenantId = tenantId,
                        Points = p.Points.EtmsToPoints(),
                        Id = p.Id
                    });
                }
                priceTypes.Add(EmCoursePriceType.Day);
                strPriceTypeDesc.Append("按天&");
            }
            if (priceTypes.Count() > 1)
            {
                return Tuple.Create(rules, EmCoursePriceType.MultipleWays, strPriceTypeDesc.ToString().TrimEnd('&'));
            }
            return Tuple.Create(rules, priceTypes.First(), strPriceTypeDesc.ToString().TrimEnd('&'));
        }

        public async Task<ResponseBase> CourseEdit(CourseEditRequest request)
        {
            var courseInfo = await _courseDAL.GetCourse(request.CId);
            if (courseInfo == null || courseInfo.Item1 == null)
            {
                return ResponseBase.CommonError("课程不存在");
            }
            if (await _courseDAL.ExistCourse(request.Name, request.CId))
            {
                return ResponseBase.CommonError("已存在相同名称的课程");
            }
            var course = courseInfo.Item1;
            course.Name = request.Name;
            course.Remark = request.Remark;
            course.StyleColor = request.StyleColor;
            course.CheckPoints = request.CheckPoints.EtmsToPoints();
            //course.Type = request.Type;
            var coursePriceRuleInfo = GetCoursePriceRule(request.CoursePriceRules, course.Id, course.TenantId);
            course.PriceType = coursePriceRuleInfo.Item2;
            course.PriceTypeDesc = coursePriceRuleInfo.Item3;
            await _courseDAL.EditCourse(course, coursePriceRuleInfo.Item1);
            await _userOperationLogDAL.AddUserLog(request, $"编辑课程-{request.Name}", EmUserOperationType.CourseManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> CourseGet(CourseGetRequest request)
        {
            var courseInfo = await _courseDAL.GetCourse(request.CId);
            if (courseInfo == null || courseInfo.Item1 == null)
            {
                return ResponseBase.CommonError("课程不存在");
            }
            var course = courseInfo.Item1;
            var coursePriceRules = courseInfo.Item2;
            var courseGetOutput = new CourseGetOutput()
            {
                CId = course.Id,
                Name = course.Name,
                Remark = course.Remark,
                StyleColor = course.StyleColor,
                Type = course.Type,
                Status = course.Status,
                CheckPoints = course.CheckPoints,
                CoursePriceRules = new CoursePriceRuleOutput()
                {
                    ByClassTimes = new List<CoursePriceRuleOutputItem>(),
                    ByMonth = new List<CoursePriceRuleOutputItem>(),
                    ByDay = new List<CoursePriceRuleOutputItem>(),
                    ByClassTimesIsCanModify = true,
                    ByDayIsCanModify = true,
                    ByMonthIsCanModify = true
                }
            };
            var suitUsedPriceRuleIds = await _suitDAL.GetCoursePriceRuleUsed(course.Id);
            if (coursePriceRules != null && coursePriceRules.Any())
            {
                var byClassTimes = coursePriceRules.Where(p => p.PriceType == EmCoursePriceType.ClassTimes);
                if (byClassTimes.Any())
                {
                    courseGetOutput.CoursePriceRules.IsByClassTimes = true;
                    foreach (var p in byClassTimes)
                    {
                        var tempByClassTimes = new CoursePriceRuleOutputItem()
                        {
                            Name = p.Name,
                            Price = p.Price,
                            Quantity = p.Quantity,
                            TotalPrice = p.TotalPrice,
                            Points = p.Points,
                            IsCanModify = true
                        };
                        if (suitUsedPriceRuleIds.Any())
                        {
                            var isHasSuit = suitUsedPriceRuleIds.Exists(j => j == p.Id);
                            if (isHasSuit)
                            {
                                tempByClassTimes.Id = p.Id;   //通过赋值Id，则保存的时候为修改
                                tempByClassTimes.IsCanModify = false;
                                courseGetOutput.CoursePriceRules.ByClassTimesIsCanModify = false;
                            }
                        }
                        courseGetOutput.CoursePriceRules.ByClassTimes.Add(tempByClassTimes);
                    }
                }
                var byMonth = coursePriceRules.Where(p => p.PriceType == EmCoursePriceType.Month);
                if (byMonth.Any())
                {
                    courseGetOutput.CoursePriceRules.IsByMonth = true;
                    foreach (var p in byMonth)
                    {
                        var tempByMonth = new CoursePriceRuleOutputItem()
                        {
                            Name = p.Name,
                            Price = p.Price,
                            Quantity = p.Quantity,
                            TotalPrice = p.TotalPrice,
                            Points = p.Points,
                            IsCanModify = true
                        };
                        if (suitUsedPriceRuleIds.Any())
                        {
                            var isHasSuit = suitUsedPriceRuleIds.Exists(j => j == p.Id);
                            if (isHasSuit)
                            {
                                tempByMonth.Id = p.Id;
                                tempByMonth.IsCanModify = false;
                                courseGetOutput.CoursePriceRules.ByMonthIsCanModify = false;
                            }
                        }
                        courseGetOutput.CoursePriceRules.ByMonth.Add(tempByMonth);
                    }
                }
                var byDay = coursePriceRules.Where(p => p.PriceType == EmCoursePriceType.Day);
                if (byDay.Any())
                {
                    courseGetOutput.CoursePriceRules.IsByDay = true;
                    foreach (var p in byDay)
                    {
                        var tempByDay = new CoursePriceRuleOutputItem()
                        {
                            Name = p.Name,
                            Price = p.Price,
                            Quantity = p.Quantity,
                            TotalPrice = p.TotalPrice,
                            Points = p.Points,
                            IsCanModify = true
                        };
                        if (suitUsedPriceRuleIds.Any())
                        {
                            var isHasSuit = suitUsedPriceRuleIds.Exists(j => j == p.Id);
                            if (isHasSuit)
                            {
                                tempByDay.Id = p.Id;
                                tempByDay.IsCanModify = false;
                                courseGetOutput.CoursePriceRules.ByDayIsCanModify = false;
                            }
                        }
                        courseGetOutput.CoursePriceRules.ByDay.Add(tempByDay);
                    }
                }
            }
            return ResponseBase.Success(courseGetOutput);
        }

        public async Task<ResponseBase> CourseDel(CourseDelRequest request)
        {
            var courseInfo = await _courseDAL.GetCourse(request.CId);
            if (courseInfo == null || courseInfo.Item1 == null)
            {
                return ResponseBase.CommonError("课程不存在");
            }
            var used = await _suitDAL.GetProductSuitUsed(EmProductType.Course, courseInfo.Item1.Id);
            if (used.Any())
            {
                var usedSuit = string.Join(',', used.Select(p => p.Name));
                return ResponseBase.CommonError($"想要删除此课程，请先删除套餐[{usedSuit}]内的此课程");
            }
            if (await _courseDAL.IsCanNotDelete(request.CId))
            {
                return ResponseBase.CommonError("此课程有关联的班级，请先删除所关联的班级");
            }
            if (!request.IsIgnoreCheck)
            {
                if (await _orderDAL.ExistProduct(EmProductType.Course, request.CId))
                {
                    return ResponseBase.Success(new DelOutput(false, true));
                }
            }
            if (request.IsIgnoreCheck)
            {
                await _courseDAL.DelCourseDepth(request.CId);
            }
            else
            {
                await _courseDAL.DelCourse(request.CId);
            }
            await _userOperationLogDAL.AddUserLog(request, $"删除课程-{courseInfo.Item1.Name}", EmUserOperationType.CourseManage);
            return ResponseBase.Success(new DelOutput(true));
        }

        public async Task<ResponseBase> CourseChangeStatus(CourseChangeStatusRequest request)
        {
            var courseInfo = await _courseDAL.GetCourse(request.CId);
            if (courseInfo == null || courseInfo.Item1 == null)
            {
                return ResponseBase.CommonError("课程不存在");
            }
            var course = courseInfo.Item1;
            course.Status = request.NewStatus;
            await _courseDAL.EditCourse(course);
            var tag = request.NewStatus == EmProductStatus.Enabled ? "启用" : "禁用";
            await _userOperationLogDAL.AddUserLog(request, $"{tag}课程-{courseInfo.Item1.Name}", EmUserOperationType.CourseManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> CourseGetPaging(CourseGetPagingRequest request)
        {
            var pagingData = await _courseDAL.GetPaging(request);
            var courseInfo = pagingData.Item1;
            var courseGetPagingOutput = new List<CourseGetPagingOutput>();
            foreach (var p in courseInfo)
            {
                var priceRules = (await _courseDAL.GetCourse(p.Id)).Item2;
                courseGetPagingOutput.Add(new CourseGetPagingOutput()
                {
                    CId = p.Id,
                    Status = p.Status,
                    StatusDesc = EmProductStatus.GetCourseStatusDesc(p.Status),
                    Name = p.Name,
                    PriceType = p.PriceType,
                    PriceTypeDesc = EmCoursePriceType.GetCoursePriceTypeDesc2(p.PriceType, p.PriceTypeDesc),
                    Remark = p.Remark,
                    Type = p.Type,
                    TypeDesc = EmCourseType.GetCourseTypeDesc(p.Type),
                    PriceRuleDescs = ComBusiness3.GetPriceRuleDescs(priceRules),
                    Label = p.Name,
                    Value = p.Id,
                    CheckPoints = p.CheckPoints
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<CourseGetPagingOutput>(pagingData.Item2, courseGetPagingOutput));
        }

        public async Task<ResponseBase> CourseViewGet(CourseViewGetRequest request)
        {
            var courseInfo = await _courseDAL.GetCourse(request.CourseId);
            if (courseInfo == null || courseInfo.Item1 == null)
            {
                return ResponseBase.CommonError("课程不存在");
            }
            var p = courseInfo.Item1;
            var priceRules = courseInfo.Item2;
            return ResponseBase.Success(new CourseGetPagingOutput()
            {
                CId = p.Id,
                Status = p.Status,
                StatusDesc = EmProductStatus.GetCourseStatusDesc(p.Status),
                Name = p.Name,
                PriceType = p.PriceType,
                PriceTypeDesc = EmCoursePriceType.GetCoursePriceTypeDesc2(p.PriceType, p.PriceTypeDesc),
                Remark = p.Remark,
                Type = p.Type,
                TypeDesc = EmCourseType.GetCourseTypeDesc(p.Type),
                PriceRuleDescs = ComBusiness3.GetPriceRuleDescs(priceRules),
                Label = p.Name,
                Value = p.Id,
                CheckPoints = p.CheckPoints
            });
        }
    }
}
