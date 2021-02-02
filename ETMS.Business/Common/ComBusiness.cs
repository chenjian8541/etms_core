using ETMS.Authority;
using ETMS.DataAccess.Lib;
using ETMS.Entity.Config.Menu;
using ETMS.Entity.Config.Router;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Common.Request;
using ETMS.Entity.Dto.Product.Output;
using ETMS.Entity.Dto.User.Output;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Common
{
    public class ComBusiness
    {
        internal static PriceRuleDesc GetPriceRuleDesc(EtCoursePriceRule p)
        {
            var rule = new CoursePriceRuleOut()
            {
                CId = p.Id,
                Name = p.Name,
                Price = p.Price,
                PriceType = p.PriceType,
                PriceUnit = p.PriceUnit,
                Quantity = p.Quantity,
                TotalPrice = p.TotalPrice,
                Points = p.Points
            };
            var givePointsDesc = string.Empty;
            if (p.Points > 0)
            {
                givePointsDesc = $",赠送{p.Points}积分";
            }
            var priceTypeDesc = p.PriceType == EmCoursePriceType.ClassTimes ? "课时" : "月";
            if (p.Quantity == 1)
            {
                return new PriceRuleDesc()
                {
                    PriceType = p.PriceType,
                    Desc = $"{p.Name}({p.TotalPrice.ToDecimalDesc()}元/{(p.PriceType == EmCoursePriceType.ClassTimes ? "课时" : "月")}){givePointsDesc}",
                    RuleValue = rule,
                    PriceTypeDesc = priceTypeDesc,
                    CId = p.Id
                };
            }
            return new PriceRuleDesc()
            {
                PriceType = p.PriceType,
                Desc = $"{p.Name}({p.TotalPrice.ToDecimalDesc()}元{p.Quantity}{(p.PriceType == EmCoursePriceType.ClassTimes ? "课时" : "个月")}){givePointsDesc}",
                RuleValue = rule,
                PriceTypeDesc = priceTypeDesc,
                CId = p.Id
            };
        }

        internal static string GetDesc<T>(List<T> entitys, string strIds) where T : Entity<long>, IHasName
        {
            if (string.IsNullOrEmpty(strIds))
            {
                return string.Empty;
            }
            var ids = strIds.Split(',');
            var strDesc = new StringBuilder();
            foreach (var id in ids)
            {
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }
                var myEntity = entitys.FirstOrDefault(p => p.Id == id.ToLong());
                if (myEntity == null)
                {
                    continue;
                }
                strDesc.Append($"{myEntity.Name},");
            }
            return strDesc.ToString().TrimEnd(',');
        }

        internal static string GetStudentCourseDesc(List<EtStudentCourse> studentCourse)
        {
            var courseSurplusDesc = new StringBuilder();
            if (studentCourse != null && studentCourse.Any())
            {
                var timesStudentCourse = studentCourse.FirstOrDefault(p => p.DeType == EmDeClassTimesType.ClassTimes);
                var dayStudentCourse = studentCourse.FirstOrDefault(p => p.DeType == EmDeClassTimesType.Day);
                if (timesStudentCourse != null)
                {
                    courseSurplusDesc.Append($"{timesStudentCourse.SurplusQuantity.EtmsToString()}课时 ");
                }
                if (dayStudentCourse != null)
                {
                    if (dayStudentCourse.SurplusQuantity > 0)
                    {
                        courseSurplusDesc.Append($"{dayStudentCourse.SurplusQuantity.EtmsToString()}个月 ");
                    }
                    if (dayStudentCourse.SurplusSmallQuantity > 0)
                    {
                        courseSurplusDesc.Append($"{dayStudentCourse.SurplusSmallQuantity.EtmsToString()}天");
                    }
                }
            }
            if (courseSurplusDesc.Length == 0)
            {
                return "0课时";
            }
            return courseSurplusDesc.ToString().TrimEnd();
        }

        internal static async Task<EtCourse> GetCourse(DataTempBox<EtCourse> tempBox, ICourseDAL courseDAL, long courseId)
        {
            var course = await tempBox.GetData(courseId, async () =>
            {
                var temp = await courseDAL.GetCourse(courseId);
                return temp?.Item1;
            });
            return course;
        }

        internal static async Task<string> GetCourseName(DataTempBox<EtCourse> tempBox, ICourseDAL courseDAL, long courseId)
        {
            var course = await tempBox.GetData(courseId, async () =>
            {
                var temp = await courseDAL.GetCourse(courseId);
                return temp?.Item1;
            });
            return course?.Name;
        }

        internal static async Task<string> GetCourseNames(DataTempBox<EtCourse> tempBox, ICourseDAL courseDAL, string courseList)
        {
            if (string.IsNullOrEmpty(courseList))
            {
                return string.Empty;
            }
            var courseIds = courseList.Split(',');
            var courseDesc = new StringBuilder();
            foreach (var id in courseIds)
            {
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }
                var tempId = id.ToLong();
                var course = await tempBox.GetData(tempId, async () =>
                {
                    var temp = await courseDAL.GetCourse(tempId);
                    return temp?.Item1;
                });
                if (course != null)
                {
                    courseDesc.Append($"{course.Name},");
                }
            }
            return courseDesc.ToString().TrimEnd(',');
        }

        internal static async Task<Tuple<string, string>> GetCourseNameAndColor(DataTempBox<EtCourse> tempBox, ICourseDAL courseDAL, long courseId)
        {
            var course = await tempBox.GetData(courseId, async () =>
            {
                var temp = await courseDAL.GetCourse(courseId);
                return temp?.Item1;
            });
            if (course == null)
            {
                return null;
            }
            return Tuple.Create(course.Name, course.StyleColor);
        }

        internal static async Task<Tuple<string, string>> GetCourseNameAndColor(DataTempBox<EtCourse> tempBox, ICourseDAL courseDAL, string courseList)
        {
            if (string.IsNullOrEmpty(courseList))
            {
                return Tuple.Create(string.Empty, string.Empty);
            }
            var courseIds = courseList.Split(',');
            var courseDesc = new StringBuilder();
            var courseStyleColor = string.Empty;
            foreach (var id in courseIds)
            {
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }
                var tempId = id.ToLong();
                var course = await tempBox.GetData(tempId, async () =>
                {
                    var temp = await courseDAL.GetCourse(tempId);
                    return temp?.Item1;
                });
                if (course != null)
                {
                    courseDesc.Append($"{course.Name},");
                    if (courseStyleColor == string.Empty)
                    {
                        courseStyleColor = course.StyleColor;
                    }
                }
            }
            return Tuple.Create(courseDesc.ToString().TrimEnd(','), courseStyleColor);
        }

        internal static async Task<List<MultiSelectValueRequest>> GetCourseMultiSelectValue(DataTempBox<EtCourse> tempBox, ICourseDAL courseDAL, string courseList)
        {
            var courseDesc = new List<MultiSelectValueRequest>();
            if (string.IsNullOrEmpty(courseList))
            {
                return courseDesc;
            }
            var courseIds = courseList.Split(',');
            foreach (var id in courseIds)
            {
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }
                var tempId = id.ToLong();
                var course = await tempBox.GetData(tempId, async () =>
                {
                    var temp = await courseDAL.GetCourse(tempId);
                    return temp?.Item1;
                });
                if (course != null)
                {
                    courseDesc.Add(new MultiSelectValueRequest()
                    {
                        Label = course.Name,
                        Value = tempId
                    });
                }
            }
            return courseDesc;
        }

        internal static async Task<EtUser> GetUser(DataTempBox<EtUser> tempBox, IUserDAL userDAL, long? userId)
        {
            if (userId == null)
            {
                return null;
            }
            var user = await tempBox.GetData(userId.Value, async () =>
            {
                return await userDAL.GetUser(userId.Value);
            });
            return user;
        }

        internal static async Task<string> GetUserName(DataTempBox<EtUser> tempBox, IUserDAL userDAL, long? userId)
        {
            if (userId == null)
            {
                return string.Empty;
            }
            var user = await tempBox.GetData(userId.Value, async () =>
            {
                return await userDAL.GetUser(userId.Value);
            });
            return user?.Name;
        }

        internal static async Task<string> GetUserNames(DataTempBox<EtUser> tempBox, IUserDAL userDAL, string users)
        {
            if (string.IsNullOrEmpty(users))
            {
                return string.Empty;
            }
            var teacherIds = users.Split(',');
            var teacherDesc = new StringBuilder();
            foreach (var id in teacherIds)
            {
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }
                var tempId = id.ToLong();
                var user = await tempBox.GetData(tempId, async () =>
                {
                    return await userDAL.GetUser(tempId);
                });
                if (user != null)
                {
                    teacherDesc.Append($"{user.Name},");
                }
            }
            return teacherDesc.ToString().TrimEnd(',');
        }

        internal static async Task<string> GetParentTeachers(DataTempBox<EtUser> tempBox, IUserDAL userDAL, string users)
        {
            if (string.IsNullOrEmpty(users))
            {
                return string.Empty;
            }
            var teacherIds = users.Split(',');
            var teacherDesc = new StringBuilder();
            foreach (var id in teacherIds)
            {
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }
                var tempId = id.ToLong();
                var user = await tempBox.GetData(tempId, async () =>
                {
                    return await userDAL.GetUser(tempId);
                });
                if (user != null)
                {
                    if (string.IsNullOrEmpty(user.NickName))
                    {
                        teacherDesc.Append($"{user.Name},");
                    }
                    else
                    {
                        teacherDesc.Append($"{user.NickName},");
                    }
                }
            }
            return teacherDesc.ToString().TrimEnd(',');
        }

        internal static async Task<string> GetParentTeacher(DataTempBox<EtUser> tempBox, IUserDAL userDAL, long userId)
        {
            var user = await tempBox.GetData(userId, async () =>
            {
                return await userDAL.GetUser(userId);
            });
            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.NickName))
                {
                    return user.NickName;
                }
                return user.Name;
            }
            return string.Empty;
        }

        internal static async Task<EtCoupons> GetCoupons(DataTempBox<EtCoupons> tempBox, ICouponsDAL couponsDAL, long couponsId)
        {
            var myChoupons = await tempBox.GetData(couponsId, async () =>
            {
                return await couponsDAL.GetCoupons(couponsId);
            });
            return myChoupons;
        }

        internal static async Task<EtStudent> GetStudent(DataTempBox<EtStudent> tempBox, IStudentDAL studentDAL, long studentId)
        {
            var student = await tempBox.GetData(studentId, async () =>
            {
                var myStudent = await studentDAL.GetStudent(studentId);
                return myStudent?.Student;
            });
            return student;
        }

        internal static async Task<string> GetStudentNames(DataTempBox<EtStudent> tempBox, IStudentDAL studentDAL, string studentIds)
        {
            if (string.IsNullOrEmpty(studentIds))
            {
                return string.Empty;
            }
            var studentSplitIds = studentIds.Split(',');
            var studentNameDesc = new StringBuilder();
            foreach (var id in studentSplitIds)
            {
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }
                var tempId = id.ToLong();
                var myStudent = await tempBox.GetData(tempId, async () =>
                {
                    var studentBucket = await studentDAL.GetStudent(tempId);
                    if (studentBucket == null || studentBucket.Student == null)
                    {
                        return null;
                    }
                    return studentBucket.Student;
                });
                if (myStudent != null)
                {
                    studentNameDesc.Append($"{myStudent.Name},");
                }
            }
            return studentNameDesc.ToString().TrimEnd(',');
        }

        internal static async Task<EtClass> GetClass(DataTempBox<EtClass> tempBox, IClassDAL classDAL, long classId)
        {
            return await tempBox.GetData(classId, async () =>
            {
                var classBucket = await classDAL.GetClassBucket(classId);
                return classBucket?.EtClass;
            });
        }

        internal static async Task<string> GetClassNames(DataTempBox<EtClass> tempBox, IClassDAL classDAL, string classIds)
        {
            if (string.IsNullOrEmpty(classIds))
            {
                return string.Empty;
            }
            var classSplitIds = classIds.Split(',');
            var classNameDesc = new StringBuilder();
            foreach (var id in classSplitIds)
            {
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }
                var tempId = id.ToLong();
                var myClass = await tempBox.GetData(tempId, async () =>
                {
                    var classBucket = await classDAL.GetClassBucket(tempId);
                    if (classBucket == null || classBucket.EtClass == null)
                    {
                        return null;
                    }
                    return classBucket.EtClass;
                });
                if (myClass != null)
                {
                    classNameDesc.Append($"{myClass.Name},");
                }
            }
            return classNameDesc.ToString().TrimEnd(',');
        }

        internal static async Task<List<MultiSelectValueRequest>> GetUserMultiSelectValue(DataTempBox<EtUser> tempBox, IUserDAL userDAL, string users)
        {
            var teacherDesc = new List<MultiSelectValueRequest>();
            if (string.IsNullOrEmpty(users))
            {
                return teacherDesc;
            }
            var teacherIds = users.Split(',');
            foreach (var id in teacherIds)
            {
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }
                var tempId = id.ToLong();
                var user = await tempBox.GetData(tempId, async () =>
                {
                    return await userDAL.GetUser(tempId);
                });
                if (user != null)
                {
                    teacherDesc.Add(new MultiSelectValueRequest()
                    {
                        Value = tempId,
                        Label = user.Name
                    });
                }
            }
            return teacherDesc;
        }

        internal static string GetClassRoomDesc(List<EtClassRoom> classRooms, string strClassRoomIds)
        {
            if (string.IsNullOrEmpty(strClassRoomIds))
            {
                return string.Empty;
            }
            var classRoomIds = strClassRoomIds.Split(',');
            var classRoomDesc = new StringBuilder();
            foreach (var id in classRoomIds)
            {
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }
                var room = classRooms.FirstOrDefault(p => p.Id == id.ToLong());
                if (room != null)
                {
                    classRoomDesc.Append($"{room.Name},");
                }
            }
            return classRoomDesc.ToString().TrimEnd(',');
        }

        internal static string GetClassCategoryDesc(List<EtClassCategory> classCategories, long? classCategoryId)
        {
            if (classCategoryId == null)
            {
                return string.Empty;
            }
            var c = classCategories.FirstOrDefault(p => p.Id == classCategoryId.Value);
            return c?.Name;
        }

        internal static string GetSurplusQuantityDesc(decimal surplusQuantity, decimal surplusSmallQuantity, byte deType)
        {
            if (surplusQuantity == 0 && surplusSmallQuantity == 0)
            {
                return string.Empty;
            }
            if (deType == EmDeClassTimesType.ClassTimes)
            {
                return $"{surplusQuantity.EtmsToString()}课时";
            }
            var strDesc = new StringBuilder();
            if (surplusQuantity > 0)
            {
                strDesc.Append($"{surplusQuantity.EtmsToString()}个月");
            }
            if (surplusSmallQuantity > 0)
            {
                strDesc.Append($"{surplusSmallQuantity.EtmsToString()}天");
            }
            return strDesc.ToString();
        }

        internal static Tuple<decimal, decimal> GetDeClassTimes(decimal beforeSurplusQuantity, decimal beforeSurplusSmallQuantity,
            decimal afterSurplusQuantity,
            decimal afterSurplusSmallQuantity)
        {
            var deClassTimes = afterSurplusQuantity - beforeSurplusQuantity;
            var deClassTimesSmall = afterSurplusSmallQuantity - beforeSurplusSmallQuantity;
            return Tuple.Create(deClassTimes, deClassTimesSmall);
        }

        internal static string GetExpirationDate(EtStudentCourseDetail detail)
        {
            if (detail.DeType == EmDeClassTimesType.ClassTimes)
            {
                if (detail.EndTime == null)
                {
                    return "永久有效";
                }
                return $"{detail.EndTime.EtmsToDateString()} 到期";
            }
            if (detail.StartTime == null || detail.EndTime == null)
            {
                return "请设置课程起止日期";
            }
            return $"{detail.StartTime.EtmsToDateString()} 到 {detail.EndTime.EtmsToDateString()}";
        }

        internal static string GetUseQuantityDesc(decimal useQuantity, byte useUnit)
        {
            if (useQuantity == 0)
            {
                return string.Empty;
            }
            return useUnit == EmCourseUnit.ClassTimes ? $"{useQuantity.EtmsToString()}课时" : $"{useQuantity.EtmsToString()}天";
        }

        internal static string GetGiveQuantityDesc(int giveQuantity, int giveSmallQuantity, byte deType)
        {
            if (giveQuantity == 0 && giveSmallQuantity == 0)
            {
                return string.Empty;
            }
            if (deType == EmDeClassTimesType.ClassTimes)
            {
                return $"{giveQuantity}课时";
            }
            var strDesc = new StringBuilder();
            if (giveQuantity > 0)
            {
                strDesc.Append($"{giveQuantity}个月");
            }
            if (giveSmallQuantity > 0)
            {
                strDesc.Append($"{giveSmallQuantity}天");
            }
            return strDesc.ToString();
        }

        internal static string GetGiveQuantityDesc(int giveQuantity, byte giveUnit)
        {
            if (giveQuantity == 0)
            {
                return string.Empty;
            }
            switch (giveUnit)
            {
                case EmCourseUnit.ClassTimes:
                    return $"{giveQuantity}课时";
                case EmCourseUnit.Day:
                    return $"{giveQuantity}天";
                case EmCourseUnit.Month:
                    return $"{giveQuantity}月";
            }
            return string.Empty;
        }

        internal static string GetBuyQuantityDesc(int buyQuantity, byte bugUnit, byte productType)
        {
            if (buyQuantity == 0)
            {
                return string.Empty;
            }
            if (productType == EmOrderProductType.Goods)
            {
                return $"{buyQuantity}件";
            }
            if (productType == EmOrderProductType.Cost)
            {
                return $"{buyQuantity}笔";
            }
            return bugUnit == EmCourseUnit.ClassTimes ? $"{buyQuantity}课时" : $"{buyQuantity}个月";
        }

        internal static string GetOutQuantityDesc(decimal outQuantity, byte bugUnit, byte productType)
        {
            var tempOutQuantity = outQuantity.EtmsToString();
            if (tempOutQuantity == "0")
            {
                return string.Empty;
            }
            if (productType == EmOrderProductType.Goods)
            {
                return $"{tempOutQuantity}件";
            }
            if (productType == EmOrderProductType.Cost)
            {
                return $"{tempOutQuantity}笔";
            }
            return bugUnit == EmCourseUnit.ClassTimes ? $"{tempOutQuantity}课时" : $"{tempOutQuantity}天";
        }

        /// <summary>
        /// 获取产品的剩余数量描述，不包括课程
        /// </summary>
        /// <param name="surplusQuantity"></param>
        /// <param name="bugUnit"></param>
        /// <param name="productType"></param>
        /// <returns></returns>
        internal static string GetProductSurplusQuantityDesc(int surplusQuantity, byte bugUnit, byte productType)
        {
            if (surplusQuantity == 0)
            {
                return string.Empty;
            }
            if (productType == EmOrderProductType.Goods)
            {
                return $"{surplusQuantity}件";
            }
            if (productType == EmOrderProductType.Cost)
            {
                return $"{surplusQuantity}笔";
            }
            return bugUnit == EmCourseUnit.ClassTimes ? $"{surplusQuantity}课时" : $"{surplusQuantity}天";
        }

        /// <summary>
        /// 获取单价描述，不包括课程
        /// </summary>
        /// <param name="price"></param>
        /// <param name="productType"></param>
        /// <returns></returns>
        internal static string GetProductPriceDesc(decimal price, byte productType)
        {
            if (productType == EmOrderProductType.Goods)
            {
                return $"{price.ToString("F2")}元/件";
            }
            if (productType == EmOrderProductType.Cost)
            {
                return $"{price.ToString("F2")}元/笔";
            }
            return string.Empty;
        }

        internal static string GetDiscountDesc(decimal discountValue, byte discountType)
        {
            if (discountValue == 0)
            {
                return string.Empty;
            }
            switch (discountType)
            {
                case EmOrderDiscountType.Nothing:
                    return string.Empty;
                case EmOrderDiscountType.DeductionMoney:
                    return $"直减{discountValue}";
                case EmOrderDiscountType.Discount:
                    return $"折扣系数{discountValue}";
            }
            return string.Empty;
        }

        internal static List<RouteConfig> GetRouteConfigs(List<RouteConfig> pageRoute, string roleAuthorityValueMenu, bool isAdmin)
        {
            var pageWeight = roleAuthorityValueMenu.Split('|')[2].ToBigInteger();
            var authorityCorePage = new AuthorityCore(pageWeight);
            PageRouteHandle(pageRoute, authorityCorePage, isAdmin);
            return pageRoute;
        }

        private static void PageRouteHandle(List<RouteConfig> pageRoute, AuthorityCore authorityCorePage, bool isAdmin)
        {
            foreach (var p in pageRoute)
            {
                if (p.Id == 0 || !authorityCorePage.Validation(p.Id))
                {
                    if (!isAdmin)
                    {
                        p.Hidden = true;
                    }
                }
                if (p.Children != null && p.Children.Any())
                {
                    PageRouteHandle(p.Children, authorityCorePage, isAdmin);
                }
            }
        }

        internal static PermissionOutput GetPermissionOutput(List<MenuConfig> menus, string roleAuthorityValueMenu, bool isAdmin)
        {
            var strMenuCategory = roleAuthorityValueMenu.Split('|');
            var pageWeight = strMenuCategory[2].ToBigInteger();
            var actionWeight = strMenuCategory[1].ToBigInteger();
            var authorityCoreLeafPage = new AuthorityCore(pageWeight);
            var authorityCoreActionPage = new AuthorityCore(actionWeight);
            var output = new PermissionOutput()
            {
                Action = new List<int>(),
                Page = new List<int>()
            };
            foreach (var p in menus)
            {
                if (authorityCoreLeafPage.Validation(p.Id) || isAdmin)
                {
                    output.Page.Add(p.Id);
                }
                if (p.ChildrenAction != null && p.ChildrenAction.Any())
                {
                    GetPermissionActionHandle(p.ChildrenAction, authorityCoreActionPage, output, isAdmin);
                }
                if (p.ChildrenPage != null && p.ChildrenPage.Any())
                {
                    GetPermissionPageHandle(p.ChildrenPage, authorityCoreLeafPage, authorityCoreActionPage, output, isAdmin);
                }
            }
            return output;
        }

        internal static PermissionOutput GetPermissionOutputH5(string roleAuthorityValueMenu, bool isAdmin)
        {
            var strMenuCategory = roleAuthorityValueMenu.Split('|');
            var pageWeight = strMenuCategory[2].ToBigInteger();
            var actionWeight = strMenuCategory[1].ToBigInteger();
            var authorityCoreLeafPage = new AuthorityCore(pageWeight);
            var authorityCoreActionPage = new AuthorityCore(actionWeight);
            var output = new PermissionOutput()
            {
                Action = new List<int>(),
                Page = new List<int>()
            };
            foreach (var mypage in PermissionDataH5.PageAllList)
            {
                if (authorityCoreLeafPage.Validation(mypage) || isAdmin)
                {
                    output.Page.Add(mypage);
                }
            }
            foreach (var myaction in PermissionDataH5.ActionAllList)
            {
                if (authorityCoreActionPage.Validation(myaction) || isAdmin)
                {
                    output.Action.Add(myaction);
                }
            }
            return output;
        }

        internal static void GetPermissionPageHandle(List<MenuConfig> menuConfigs, AuthorityCore authorityCoreLeafPage, AuthorityCore authorityCoreActionPage, PermissionOutput output, bool isAdmin)
        {
            foreach (var p in menuConfigs)
            {
                if (authorityCoreLeafPage.Validation(p.Id) || isAdmin)
                {
                    output.Page.Add(p.Id);
                }
                if (p.ChildrenAction != null && p.ChildrenAction.Any())
                {
                    GetPermissionActionHandle(p.ChildrenAction, authorityCoreActionPage, output, isAdmin);
                }
                if (p.ChildrenPage != null && p.ChildrenPage.Any())
                {
                    GetPermissionPageHandle(p.ChildrenPage, authorityCoreLeafPage, authorityCoreActionPage, output, isAdmin);
                }
            }
        }

        internal static void GetPermissionActionHandle(List<MenuConfig> menuConfigs, AuthorityCore authorityCoreActionPage, PermissionOutput output, bool isAdmin)
        {
            foreach (var p in menuConfigs)
            {
                if (authorityCoreActionPage.Validation(p.ActionId) || isAdmin)
                {
                    output.Action.Add(p.ActionId);
                }
            }
        }

        internal static string GetCouponsValueDesc(byte type, decimal value)
        {
            switch (type)
            {
                case EmCouponsType.Cash:
                    return value.ToDecimalDesc();
                case EmCouponsType.ClassTimes:
                    return $"{value.ToDecimalDesc()}课时";
                case EmCouponsType.Discount:
                    return $"{value.ToDecimalDesc()}折";
            }
            return string.Empty;
        }

        internal static string GetCouponsValueDesc2(byte type, decimal value)
        {
            switch (type)
            {
                case EmCouponsType.Cash:
                    return $"{value.ToDecimalDesc()}元";
                case EmCouponsType.ClassTimes:
                    return $"{value.ToDecimalDesc()}课时";
                case EmCouponsType.Discount:
                    return $"{value.ToDecimalDesc()}折";
            }
            return string.Empty;
        }

        internal static string GetCouponsEffectiveTimeDesc(CouponsStudentGetView couponsStudentGet)
        {
            if (couponsStudentGet.LimitUseTime == null && couponsStudentGet.ExpiredTime == null)
            {
                return "永久有效";
            }
            return $"{couponsStudentGet.LimitUseTime.EtmsToDateString2()}-{couponsStudentGet.ExpiredTime.EtmsToDateString2()}";
        }

        internal static Tuple<byte, string> GetCouponsLogStatusDesc(CouponsStudentGetView couponsStudentGet)
        {
            if (couponsStudentGet.Status == EmCouponsStudentStatus.Used)
            {
                return Tuple.Create(EmCouponsStudentStatus.Used, "已核销");
            }
            if (couponsStudentGet.ExpiredTime != null && DateTime.Now.Date > couponsStudentGet.ExpiredTime.Value)
            {
                return Tuple.Create(EmCouponsStudentStatus.Expired, "已过期");
            }
            return Tuple.Create(EmCouponsStudentStatus.Unused, "未使用");
        }
    }
}
