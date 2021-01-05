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

namespace ETMS.Business
{
    public class CourseBLL : ICourseBLL
    {
        private readonly ICourseDAL _courseDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IOrderDAL _orderDAL;

        public CourseBLL(ICourseDAL courseDAL, IUserOperationLogDAL userOperationLogDAL, IOrderDAL orderDAL)
        {
            this._courseDAL = courseDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._orderDAL = orderDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _courseDAL, _userOperationLogDAL, _orderDAL);
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
                Status = EmCourseStatus.Enabled,
                CheckPoints = request.CheckPoints.EtmsToPoints()
            };
            await _courseDAL.AddCourse(course, coursePriceRuleInfo.Item1);
            await _userOperationLogDAL.AddUserLog(request, $"添加课程:{request.Name}", EmUserOperationType.CourseManage);
            return ResponseBase.Success();
        }

        private Tuple<List<EtCoursePriceRule>, byte> GetCoursePriceRule(CoursePriceRule coursePriceRule, long courseId, int tenantId)
        {
            var rules = new List<EtCoursePriceRule>();
            var priceTypes = new List<byte>();
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
                        Points = p.Points.EtmsToPoints()
                    });
                    priceTypes.Add(EmCoursePriceType.ClassTimes);
                }
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
                        Points = p.Points.EtmsToPoints()
                    });
                    priceTypes.Add(EmCoursePriceType.Month);
                }
            }
            var totalPriceTypes = priceTypes.Distinct();
            if (totalPriceTypes.Count() > 1)
            {
                return Tuple.Create(rules, EmCoursePriceType.ClassTimesAndMonth);
            }
            return Tuple.Create(rules, totalPriceTypes.First());
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
            await _courseDAL.EditCourse(course, coursePriceRuleInfo.Item1);
            await _userOperationLogDAL.AddUserLog(request, $"编辑课程:{request.Name}", EmUserOperationType.CourseManage);
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
                    ByMonth = new List<CoursePriceRuleOutputItem>()
                }
            };
            if (coursePriceRules != null && coursePriceRules.Any())
            {
                var byClassTimes = coursePriceRules.Where(p => p.PriceType == EmCoursePriceType.ClassTimes);
                if (byClassTimes.Any())
                {
                    courseGetOutput.CoursePriceRules.IsByClassTimes = true;
                    foreach (var p in byClassTimes)
                    {
                        courseGetOutput.CoursePriceRules.ByClassTimes.Add(new CoursePriceRuleOutputItem()
                        {
                            Name = p.Name,
                            Price = p.Price,
                            Quantity = p.Quantity,
                            TotalPrice = p.TotalPrice,
                            Points = p.Points
                        });
                    }
                }
                var byMonth = coursePriceRules.Where(p => p.PriceType == EmCoursePriceType.Month);
                if (byMonth.Any())
                {
                    courseGetOutput.CoursePriceRules.IsByMonth = true;
                    foreach (var p in byMonth)
                    {
                        courseGetOutput.CoursePriceRules.ByMonth.Add(new CoursePriceRuleOutputItem()
                        {
                            Name = p.Name,
                            Price = p.Price,
                            Quantity = p.Quantity,
                            TotalPrice = p.TotalPrice,
                            Points = p.Points
                        });
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
            if (await _courseDAL.IsCanNotDelete(request.CId))
            {
                return ResponseBase.CommonError("此课程已使用，无法删除");
            }
            if (await _orderDAL.ExistProduct(EmOrderProductType.Course, request.CId))
            {
                return ResponseBase.CommonError("此课程已使用，无法删除");
            }
            await _courseDAL.DelCourse(request.CId);
            await _userOperationLogDAL.AddUserLog(request, $"删除课程:{courseInfo.Item1.Name}", EmUserOperationType.CourseManage);
            return ResponseBase.Success();
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
            var tag = request.NewStatus == EmCourseStatus.Enabled ? "启用" : "禁用";
            await _userOperationLogDAL.AddUserLog(request, $"{tag}课程:{courseInfo.Item1.Name}", EmUserOperationType.CourseManage);
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
                    StatusDesc = EmCourseStatus.GetCourseStatusDesc(p.Status),
                    Name = p.Name,
                    PriceType = p.PriceType,
                    PriceTypeDesc = EmCoursePriceType.GetCoursePriceTypeDesc(p.PriceType),
                    Remark = p.Remark,
                    Type = p.Type,
                    TypeDesc = EmCourseType.GetCourseTypeDesc(p.Type),
                    PriceRuleDescs = GetPriceRuleDescs(priceRules),
                    Label = p.Name,
                    Value = p.Id,
                    CheckPoints = p.CheckPoints
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<CourseGetPagingOutput>(pagingData.Item2, courseGetPagingOutput));
        }

        private List<PriceRuleDesc> GetPriceRuleDescs(List<EtCoursePriceRule> priceRules)
        {
            if (priceRules == null || !priceRules.Any())
            {
                return new List<PriceRuleDesc>();
            }
            var myPriceRules = priceRules.OrderBy(p => p.PriceType);
            var roles = new List<PriceRuleDesc>();
            foreach (var p in myPriceRules)
            {
                roles.Add(ComBusiness.GetPriceRuleDesc(p));
            }
            return roles;
        }
    }
}
