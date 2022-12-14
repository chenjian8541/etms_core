using ETMS.Authority;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Config;
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
        internal static PriceRuleDesc GetPriceRuleDesc<T>(T p) where T : BaseCoursePrice
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
                Points = p.Points,
                ExpiredType = p.ExpiredType,
                ExpiredValue = p.ExpiredValue
            };
            var givePointsDesc = string.Empty;
            if (p.Points > 0)
            {
                givePointsDesc = $",赠送{p.Points}积分";
            }
            var expirationDesc = string.Empty;
            if (p.PriceType == EmCoursePriceType.ClassTimes)
            {
                if (p.ExpiredType != null && p.ExpiredValue != null && p.ExpiredValue > 0)
                {
                    expirationDesc = $",有效期{p.ExpiredValue}{EmCoursePriceRuleExpiredType.GetCoursePriceRuleExpiredTypeDesc(p.ExpiredType.Value)}";
                }
            }
            var priceTypeDesc = EmCoursePriceType.GetGetCourseUnitDesc(p.PriceType);
            if (p.Quantity == 1)
            {
                return new PriceRuleDesc()
                {
                    PriceType = p.PriceType,
                    Desc = $"{p.Name}({p.TotalPrice.ToDecimalDesc()}元/{priceTypeDesc}){expirationDesc}{givePointsDesc}",
                    RuleValue = rule,
                    PriceTypeDesc = priceTypeDesc,
                    CId = p.Id
                };
            }
            return new PriceRuleDesc()
            {
                PriceType = p.PriceType,
                Desc = $"{p.Name}({p.TotalPrice.ToDecimalDesc()}元{p.Quantity}{priceTypeDesc}){expirationDesc}{givePointsDesc}",
                RuleValue = rule,
                PriceTypeDesc = priceTypeDesc,
                CId = p.Id
            };
        }

        internal static string GetDesc<T>(List<T> entitys, string strIds, string defaultStr = "") where T : Entity<long>, IHasName
        {
            if (string.IsNullOrEmpty(strIds))
            {
                return defaultStr;
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
            if (strDesc.Length == 0)
            {
                return defaultStr;
            }
            return strDesc.ToString().TrimEnd(',');
        }

        internal static string GetStudentCourseDesc(List<EtStudentCourse> studentCourse, bool isShowExceedTotalClassTimes = true,
            bool isStudentShowClassTimesUnit = false, decimal studentShowClassTimesUnitValue = 0)
        {
            var courseSurplusDesc = new StringBuilder();
            var exceedTotalClassTimes = 0M;
            if (studentCourse != null && studentCourse.Any())
            {
                var timesStudentCourse = studentCourse.FirstOrDefault(p => p.DeType == EmDeClassTimesType.ClassTimes);
                var dayStudentCourse = studentCourse.FirstOrDefault(p => p.DeType == EmDeClassTimesType.Day);
                if (timesStudentCourse != null)
                {
                    if (timesStudentCourse.SurplusQuantity > 0)
                    {
                        courseSurplusDesc.Append($"{timesStudentCourse.SurplusQuantity.EtmsToString()}课时 ");
                        if (isStudentShowClassTimesUnit && studentShowClassTimesUnitValue > 0)
                        {
                            var totalClassCount = timesStudentCourse.SurplusQuantity / studentShowClassTimesUnitValue;
                            courseSurplusDesc.Append($"({totalClassCount.EtmsToString()}节课)");
                        }
                    }
                    exceedTotalClassTimes = timesStudentCourse.ExceedTotalClassTimes;
                }
                if (dayStudentCourse != null)
                {
                    if (dayStudentCourse.SurplusQuantity > 0)
                    {
                        courseSurplusDesc.Append($"{dayStudentCourse.SurplusQuantity.EtmsToString()}个月 ");
                    }
                    if (dayStudentCourse.SurplusSmallQuantity > 0)
                    {
                        courseSurplusDesc.Append($"{dayStudentCourse.SurplusSmallQuantity.EtmsToString()}天 ");
                    }
                }
            }
            if (courseSurplusDesc.Length == 0)
            {
                if (isShowExceedTotalClassTimes && exceedTotalClassTimes > 0)
                {
                    return $"超上{exceedTotalClassTimes.EtmsToString()}课时";
                }
                return "0课时";
            }
            if (isShowExceedTotalClassTimes && exceedTotalClassTimes > 0)
            {
                courseSurplusDesc.Append($"(超上{exceedTotalClassTimes.EtmsToString()}课时)");
            }
            return courseSurplusDesc.ToString().TrimEnd();
        }

        internal static string GetStudentCourseExpireDateDesc(List<EtStudentCourseDetail> studentCourseDetail)
        {
            if (studentCourseDetail != null && studentCourseDetail.Any())
            {
                var normalCourse = studentCourseDetail.Where(p => p.Status == EmStudentCourseStatus.Normal);
                if (normalCourse.Any())
                {
                    var notExCourse = normalCourse.FirstOrDefault(p => (p.SurplusQuantity > 0 || p.SurplusSmallQuantity > 0) && p.EndTime == null);
                    if (notExCourse != null)
                    {
                        return string.Empty;
                    }
                    var hasExCourse = normalCourse.Where(p => (p.SurplusQuantity > 0 || p.SurplusSmallQuantity > 0) && p.EndTime != null).OrderByDescending(p => p.EndTime.Value).FirstOrDefault();
                    if (hasExCourse != null)
                    {
                        return hasExCourse.EndTime.Value.EtmsToDateString();
                    }
                }
            }
            return string.Empty;
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
            if (userId == null || userId == 0)
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
            if (userId == null || userId == 0)
            {
                return string.Empty;
            }
            var user = await tempBox.GetData(userId.Value, async () =>
            {
                return await userDAL.GetUser(userId.Value);
            });
            return user?.Name;
        }

        internal static async Task<string> GetUserNames(DataTempBox<EtUser> tempBox, IUserDAL userDAL, string users, string defaultStr = "")
        {
            if (string.IsNullOrEmpty(users))
            {
                return defaultStr;
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
            if (teacherDesc.Length == 0)
            {
                return defaultStr;
            }
            return teacherDesc.ToString().TrimEnd(',');
        }

        internal static async Task<string> GetParentTeachers(DataTempBox<EtUser> tempBox, IUserDAL userDAL,
            string users, string defaultDesc = "")
        {
            if (string.IsNullOrEmpty(users))
            {
                return defaultDesc;
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
            if (teacherDesc.Length == 0)
            {
                return defaultDesc;
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

        internal static async Task<EtCost> GetCost(DataTempBox<EtCost> tempBox, ICostDAL costDAL, long costId)
        {
            return await tempBox.GetData(costId, async () =>
            {
                return await costDAL.GetCost(costId);
            });
        }

        internal static async Task<EtGoods> GetGoods(DataTempBox<EtGoods> tempBox, IGoodsDAL goodsDAL, long goodsId)
        {
            return await tempBox.GetData(goodsId, async () =>
            {
                return await goodsDAL.GetGoods(goodsId);
            });
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
                if (deType == EmDeClassTimesType.ClassTimes)
                {
                    return "0课时";
                }
                else
                {
                    return "0天";
                }
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
            if (strDesc.Length == 0)
            {
                return "0课时";
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

        internal static string GetClassTimesEndTimeDesc(DateTime? endTime)
        {
            if (endTime == null)
            {
                return "永久有效";
            }
            return endTime.EtmsToDateString();
        }

        internal static string GetClassDeDayEndTimeDesc(DateTime? endTime)
        {
            if (endTime == null)
            {
                return "未设置";
            }
            return endTime.EtmsToDateString();
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

        internal static string GetBuyQuantityDesc(List<EtStudentCourse> myStudentCourse)
        {
            if (myStudentCourse == null || myStudentCourse.Count == 0)
            {
                return string.Empty;
            }
            var strDesc = new StringBuilder();
            var classTimesLog = myStudentCourse.FirstOrDefault(p => p.DeType == EmDeClassTimesType.ClassTimes);
            if (classTimesLog != null)
            {
                strDesc.Append(GetBuyQuantityDesc(classTimesLog.BuyQuantity, classTimesLog.BuySmallQuantity,
                    classTimesLog.BugUnit, EmProductType.Course));
            }
            var dayLog = myStudentCourse.FirstOrDefault(p => p.DeType == EmDeClassTimesType.Day);
            if (dayLog != null)
            {
                strDesc.Append(GetBuyQuantityDesc(dayLog.BuyQuantity, dayLog.BuySmallQuantity,
                    dayLog.BugUnit, EmProductType.Course));
            }
            return strDesc.ToString();
        }

        /// <summary>
        /// 此方法为了兼容 EtStudentCourse，单位永远是按月，则通过判断"buySmallQuantity"属性来判断具体是按月 还是按天 
        /// </summary>
        /// <param name="buyQuantity"></param>
        /// <param name="buySmallQuantity"></param>
        /// <param name="bugUnit"></param>
        /// <param name="productType"></param>
        /// <returns></returns>
        internal static string GetBuyQuantityDesc(int buyQuantity, int buySmallQuantity, byte bugUnit, byte productType)
        {
            if (buyQuantity == 0 && buySmallQuantity == 0)
            {
                return string.Empty;
            }
            if (productType == EmProductType.Goods)
            {
                return $"{buyQuantity}件";
            }
            if (productType == EmProductType.Cost)
            {
                return $"{buyQuantity}笔";
            }
            if (bugUnit == EmCourseUnit.ClassTimes)
            {
                return $"{buyQuantity}课时";
            }
            else
            {
                if (buySmallQuantity == 0)
                {
                    switch (bugUnit)
                    {
                        case EmCourseUnit.ClassTimes:
                            return $"{buyQuantity}课时";
                        case EmCourseUnit.Day:
                            return $"{buyQuantity}天";
                        case EmCourseUnit.Month:
                            return $"{buyQuantity}个月";
                    }
                    return string.Empty;
                }
                else
                {
                    var str = new StringBuilder();
                    if (buyQuantity > 0)
                    {
                        str.Append($"{buyQuantity}个月");
                    }
                    if (buySmallQuantity > 0)
                    {
                        str.Append($"{buySmallQuantity}天");
                    }
                    return str.ToString();
                }
            }
        }

        internal static string GetOutQuantityDesc(decimal outQuantity, byte bugUnit, byte productType)
        {
            var tempOutQuantity = outQuantity.EtmsToString();
            if (tempOutQuantity == "0")
            {
                return string.Empty;
            }
            if (productType == EmProductType.Goods)
            {
                return $"{tempOutQuantity}件";
            }
            if (productType == EmProductType.Cost)
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
            if (productType == EmProductType.Goods)
            {
                return $"{surplusQuantity}件";
            }
            if (productType == EmProductType.Cost)
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
            if (productType == EmProductType.Goods)
            {
                return $"{price.ToString("F2")}元/件";
            }
            if (productType == EmProductType.Cost)
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
                case EmDiscountType.Nothing:
                    return string.Empty;
                case EmDiscountType.DeductionMoney:
                    return $"直减{discountValue}";
                case EmDiscountType.Discount:
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

        internal static PermissionOutput GetPermissionOutput(List<MenuConfig> menus, AuthorityCore authorityCoreLeafPage,
            AuthorityCore authorityCoreActionPage, bool isAdmin)
        {
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

        internal static PermissionOutput GetPermissionOutput(List<MenuConfig> menus, string roleAuthorityValueMenu, bool isAdmin)
        {
            var strMenuCategory = roleAuthorityValueMenu.Split('|');
            var pageWeight = strMenuCategory[2].ToBigInteger();
            var actionWeight = strMenuCategory[1].ToBigInteger();
            var authorityCoreLeafPage = new AuthorityCore(pageWeight);
            var authorityCoreActionPage = new AuthorityCore(actionWeight);
            return GetPermissionOutput(menus, authorityCoreLeafPage, authorityCoreActionPage, isAdmin);
        }

        internal static Tuple<List<MenuH5Output>, PermissionOutput, bool> GetH5HomeMenuAndPermission(List<MenuConfig> menus,
            List<MenuConfigH5> allMenus, string roleAuthorityValueMenu, string userHomeMenu, bool isAdmin)
        {
            var strMenuCategory = roleAuthorityValueMenu.Split('|');
            var pageWeight = strMenuCategory[2].ToBigInteger();
            var actionWeight = strMenuCategory[1].ToBigInteger();
            var authorityCoreLeafPage = new AuthorityCore(pageWeight);
            var authorityCoreActionPage = new AuthorityCore(actionWeight);
            if (string.IsNullOrEmpty(userHomeMenu))
            {
                userHomeMenu = SystemConfig.ComConfig.UserH5HomeMenusDefault;
            }
            var authorityCoreHome = new AuthorityCore(userHomeMenu.ToBigInteger());
            var permissionOutput = GetPermissionOutput(menus, authorityCoreLeafPage, authorityCoreActionPage, isAdmin);

            var isShowMoreMenus = false;
            var menuH5Output = new List<MenuH5Output>();
            var allMenusCount = 0;
            foreach (var p in allMenus)
            {
                if (!isAdmin)
                {
                    if (p.PageId > 0 && !authorityCoreLeafPage.Validation(p.PageId))
                    {
                        continue;
                    }
                    if (p.ActionId > 0 && !authorityCoreActionPage.Validation(p.ActionId))
                    {
                        continue;
                    }
                }
                allMenusCount++;
                if (authorityCoreHome.Validation(p.Id))
                {
                    menuH5Output.Add(new MenuH5Output()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Sort = p.Sort,
                        IconUrl = p.Icon,
                        CategoryId = p.CategoryId,
                        CategoryName = p.CategoryName
                    });
                }
                else
                {
                    isShowMoreMenus = true;
                }
            }
            if (allMenusCount >= 7)
            {
                isShowMoreMenus = true;
            }
            return Tuple.Create(menuH5Output, permissionOutput, isShowMoreMenus);
        }

        internal static List<MenuH5Output> GetH5AllMenus(List<MenuConfigH5> allMenus, string roleAuthorityValueMenu, bool isAdmin)
        {
            var strMenuCategory = roleAuthorityValueMenu.Split('|');
            var pageWeight = strMenuCategory[2].ToBigInteger();
            var actionWeight = strMenuCategory[1].ToBigInteger();
            var authorityCoreLeafPage = new AuthorityCore(pageWeight);
            var authorityCoreActionPage = new AuthorityCore(actionWeight);

            var menuH5Output = new List<MenuH5Output>();
            foreach (var p in allMenus)
            {
                if (!isAdmin)
                {
                    if (p.PageId > 0 && !authorityCoreLeafPage.Validation(p.PageId))
                    {
                        continue;
                    }
                    if (p.ActionId > 0 && !authorityCoreActionPage.Validation(p.ActionId))
                    {
                        continue;
                    }
                }
                menuH5Output.Add(new MenuH5Output()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Sort = p.Sort,
                    IconUrl = p.Icon,
                    CategoryId = p.CategoryId,
                    CategoryName = p.CategoryName
                });
            }

            return menuH5Output;
        }

        internal static GetEditMenusH5Output GetEditMenusH5(List<MenuConfigH5> allMenus, string roleAuthorityValueMenu, string userHomeMenu, bool isAdmin)
        {
            var strMenuCategory = roleAuthorityValueMenu.Split('|');
            var pageWeight = strMenuCategory[2].ToBigInteger();
            var actionWeight = strMenuCategory[1].ToBigInteger();
            var authorityCoreLeafPage = new AuthorityCore(pageWeight);
            var authorityCoreActionPage = new AuthorityCore(actionWeight);
            if (string.IsNullOrEmpty(userHomeMenu))
            {
                userHomeMenu = SystemConfig.ComConfig.UserH5HomeMenusDefault;
            }
            var authorityCoreHome = new AuthorityCore(userHomeMenu.ToBigInteger());

            var allMenusOutput = new List<AllMenuH5Output>();
            var output = new GetEditMenusH5Output()
            {
                HomeMenus = new List<MenuH5Output>(),
                AllMenuCategorys = new List<AllMenuCategory>()
            };
            bool isHome;
            foreach (var p in allMenus)
            {
                if (!isAdmin)
                {
                    if (p.PageId > 0 && !authorityCoreLeafPage.Validation(p.PageId))
                    {
                        continue;
                    }
                    if (p.ActionId > 0 && !authorityCoreActionPage.Validation(p.ActionId))
                    {
                        continue;
                    }
                }
                isHome = false;
                if (authorityCoreHome.Validation(p.Id))
                {
                    output.HomeMenus.Add(new MenuH5Output()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Sort = p.Sort,
                        IconUrl = p.Icon,
                        CategoryId = p.CategoryId,
                        CategoryName = p.CategoryName
                    });
                    isHome = true;
                }
                allMenusOutput.Add(new AllMenuH5Output()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Sort = p.Sort,
                    IconUrl = p.Icon,
                    IsHome = isHome,
                    CategoryId = p.CategoryId,
                    CategoryName = p.CategoryName
                });
            }
            var allCategory = allMenusOutput.GroupBy(p => p.CategoryId).OrderBy(p => p.Key);
            foreach (var p in allCategory)
            {
                var thisItem = allMenusOutput.Where(j => j.CategoryId == p.Key).OrderBy(j => j.Sort);
                output.AllMenuCategorys.Add(new AllMenuCategory()
                {
                    CategoryId = p.Key,
                    CategoryName = thisItem.First().CategoryName,
                    MyMenus = thisItem
                });
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
