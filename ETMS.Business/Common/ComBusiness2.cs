using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ETMS.IDataAccess;
using System.Threading.Tasks;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.Enum;
using ETMS.Utility;
using ETMS.Entity.Temp;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.Student.Request;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.View;
using ETMS.Entity.Dto.User.Output;

namespace ETMS.Business.Common
{
    public static class ComBusiness2
    {
        /// <summary>
        /// 为学员构建一对一班级
        /// </summary>
        /// <param name="course"></param>
        /// <param name="student"></param>
        /// <returns></returns>
        internal static OneToOneClass GetOneToOneClass(EtCourse course, EtStudent student)
        {
            return new OneToOneClass()
            {
                CourseId = course.Id,
                StudentNums = 1,
                Type = EmClassType.OneToOne,
                Name = $"{course.Name}_{student.Name}",
                Students = new List<OneToOneClassStudent>() {
                  new OneToOneClassStudent(){
                    CourseId = course.Id,
                    StudentId = student.Id
                  }
                 }
            };
        }

        internal static decimal GetStudentCourseDetailSurplusMoney(EtStudentCourseDetail detail)
        {
            if (detail.Status == EmStudentCourseStatus.EndOfClass)
            {
                return 0;
            }
            if (detail.DeType == EmDeClassTimesType.ClassTimes)
            {
                if (detail.SurplusQuantity == detail.BuyQuantity + detail.GiveQuantity)
                {
                    return detail.TotalMoney;
                }
                return detail.SurplusQuantity * detail.Price;
            }
            else
            {
                if (detail.StartTime != null && detail.EndTime != null)
                {
                    if (detail.EndTime <= DateTime.Now.Date)
                    {
                        return 0;
                    }
                    var tempStartDate = DateTime.Now.Date;
                    if (detail.StartTime > tempStartDate)
                    {
                        tempStartDate = detail.StartTime.Value;
                    }
                    var diffDay = (detail.EndTime.Value - tempStartDate).TotalDays;
                    return (int)diffDay * detail.Price;
                }
                else
                {
                    //未设置起止日期  
                    return detail.TotalMoney;
                    //var totalCount = detail.SurplusQuantity * SystemConfig.ComConfig.MonthToDay + detail.SurplusSmallQuantity;
                    //return totalCount * detail.Price;
                }
            }
        }

        internal static EtStudentCourseDetail GetStudentCourseDetail<T>(EtCourse course, T priceRule,
            EnrolmentCourse enrolmentCourse, string no, long studentId, int tenantId) where T : BaseCoursePrice
        {
            var buyQuantity = priceRule.Quantity > 1 ? priceRule.Quantity : enrolmentCourse.BuyQuantity;
            var deType = priceRule.PriceUnit == EmCourseUnit.ClassTimes ? EmDeClassTimesType.ClassTimes : EmDeClassTimesType.Day;
            var surplusQuantity = 0;
            var surplusSmallQuantity = 0;
            if (priceRule.PriceUnit == EmDeClassTimesType.Day)
            {
                surplusQuantity = 0;
                surplusSmallQuantity = buyQuantity;
            }
            else
            {
                surplusQuantity = buyQuantity;
                surplusSmallQuantity = 0;
            }
            var useUnit = priceRule.PriceUnit == EmCourseUnit.ClassTimes ? EmCourseUnit.ClassTimes : EmCourseUnit.Day;
            if (enrolmentCourse.GiveQuantity > 0)
            {
                if (priceRule.PriceUnit != EmCourseUnit.ClassTimes && enrolmentCourse.GiveUnit == EmCourseUnit.Day)
                {
                    surplusSmallQuantity += enrolmentCourse.GiveQuantity;
                }
                else
                {
                    surplusQuantity += enrolmentCourse.GiveQuantity;
                }
            }
            DateTime? startTime = null;
            DateTime? endTime = null;
            if (priceRule.PriceUnit == EmCourseUnit.ClassTimes && !string.IsNullOrEmpty(enrolmentCourse.ExOt))
            {
                endTime = Convert.ToDateTime(enrolmentCourse.ExOt).Date;
            }
            else if (priceRule.PriceUnit != EmCourseUnit.ClassTimes && enrolmentCourse.ErangeOt != null && enrolmentCourse.ErangeOt.Count == 2)
            {
                if (!string.IsNullOrEmpty(enrolmentCourse.ErangeOt[0]))
                {
                    startTime = Convert.ToDateTime(enrolmentCourse.ErangeOt[0]).Date;
                    endTime = Convert.ToDateTime(enrolmentCourse.ErangeOt[1]).Date;

                    if (endTime < DateTime.Now.Date)
                    {
                        surplusQuantity = 0;
                        surplusSmallQuantity = 0;
                    }
                    else
                    {
                        var testStart = DateTime.Now.Date;
                        var testEnd = DateTime.Now.Date;
                        if (surplusQuantity > 0)
                        {
                            testEnd = testEnd.AddMonths(surplusQuantity);
                        }
                        if (surplusSmallQuantity > 0)
                        {
                            testEnd = testEnd.AddDays(surplusSmallQuantity);
                        }

                        if (startTime != testStart || endTime != testEnd)
                        {
                            var jsStartDate = DateTime.Now.Date;
                            if (startTime > DateTime.Now.Date)
                            {
                                jsStartDate = startTime.Value;
                            }
                            var dffTime = EtmsHelper.GetDffTime(jsStartDate, endTime.Value);
                            surplusQuantity = dffTime.Item1;
                            surplusSmallQuantity = dffTime.Item2;
                        }
                    }
                }
            }
            return new EtStudentCourseDetail()
            {
                BugUnit = priceRule.PriceUnit,
                BuyQuantity = buyQuantity,
                CourseId = course.Id,
                StudentId = studentId,
                TenantId = tenantId,
                OrderNo = no,
                IsDeleted = EmIsDeleted.Normal,
                DeType = deType,
                EndCourseRemark = string.Empty,
                EndCourseTime = null,
                EndCourseUser = null,
                GiveQuantity = enrolmentCourse.GiveQuantity,
                GiveUnit = enrolmentCourse.GiveUnit,
                Price = GetOneClassDeSum(enrolmentCourse.ItemAptSum, deType, surplusQuantity, surplusSmallQuantity, startTime, endTime),
                StartTime = startTime,
                EndTime = endTime,
                Status = EmStudentCourseStatus.Normal,
                SurplusQuantity = surplusQuantity,
                SurplusSmallQuantity = surplusSmallQuantity,
                TotalMoney = enrolmentCourse.ItemAptSum,
                UseQuantity = 0,
                UseUnit = useUnit
            };
        }

        /// <summary>
        /// 消耗一节课大概的价格（课消金额）
        /// </summary>
        /// <param name="itemAptSum"></param>
        /// <param name="deType"></param>
        /// <param name="surplusQuantity"></param>
        /// <param name="surplusSmallQuantity"></param>
        /// <param name="startTime">按时间收费的起始时间</param>
        /// <param name="endTime">按时间收费的结束时间</param>
        /// <returns></returns>
        public static decimal GetOneClassDeSum(decimal itemAptSum, byte deType, int surplusQuantity, int surplusSmallQuantity,
            DateTime? startTime, DateTime? endTime)
        {
            var price = 0M;
            if (itemAptSum > 0)
            {
                var totalCount = 0;
                if (deType == EmDeClassTimesType.Day)
                {
                    //如何课程设置了起止时间，则按照起止时间来计算总天数，否则按每个月30天来计算总天数。重新设置起止时间时，再改变此值
                    if (startTime != null && endTime != null)
                    {
                        totalCount = (int)(endTime.Value - startTime.Value).TotalDays;
                    }
                    else
                    {
                        totalCount = surplusQuantity * SystemConfig.ComConfig.MonthToDay + surplusSmallQuantity;
                    }
                }
                else
                {
                    totalCount = surplusQuantity;
                }
                if (totalCount > 0)
                {
                    price = Math.Round(itemAptSum / totalCount, 4);
                }
            }
            return price;
        }

        public static decimal GetOneClassDeSumByDay(decimal itemAptSum, DateTime startTime, DateTime endTime)
        {
            var price = 0M;
            var totalCount = (int)(endTime - startTime).TotalDays;
            if (totalCount > 0)
            {
                price = Math.Round(itemAptSum / totalCount, 4);
            }
            return price;
        }

        internal static Tuple<EtOrderDetail, string> GetCourseOrderDetail<T>(EtCourse course, T priceRule,
            EnrolmentCourse enrolmentCourse, string no, DateTime ot, long userId, int tenantId, byte buyType,
            long studentId, int orderType) where T : BaseCoursePrice
        {
            var priceRuleDesc = ComBusiness.GetPriceRuleDesc(priceRule).Desc;
            var ruleDesc = $"{course.Name}  {priceRuleDesc}";
            var buyQuantity = priceRule.Quantity > 1 ? priceRule.Quantity : enrolmentCourse.BuyQuantity;
            var itemSum = priceRule.Quantity > 1 ? priceRule.TotalPrice : (buyQuantity * priceRule.Price).EtmsToRound();
            return Tuple.Create(new EtOrderDetail()
            {
                BugUnit = priceRule.PriceUnit,
                OrderNo = no,
                Ot = ot,
                Price = priceRule.Price,
                BuyQuantity = buyQuantity,
                DiscountType = enrolmentCourse.DiscountType,
                DiscountValue = enrolmentCourse.DiscountValue,
                GiveQuantity = enrolmentCourse.GiveQuantity,
                GiveUnit = enrolmentCourse.GiveUnit,
                IsDeleted = EmIsDeleted.Normal,
                ItemAptSum = enrolmentCourse.ItemAptSum,
                ItemSum = itemSum,
                PriceRule = priceRuleDesc,
                ProductId = course.Id,
                ProductType = EmProductType.Course,
                Remark = string.Empty,
                Status = EmOrderStatus.Normal,
                TenantId = tenantId,
                UserId = userId,
                InOutType = EmOrderInOutType.In,
                BuyType = buyType,
                StudentId = studentId,
                OrderType = orderType
            }, ruleDesc);
        }

        internal static string GetStudentRelationshipDesc(List<EtStudentRelationship> etStudentRelationships, long? id, string defaultDesc)
        {
            if (id == null || id == 0)
            {
                return defaultDesc;
            }
            var relationships = etStudentRelationships.FirstOrDefault(p => p.Id == id.Value);
            if (relationships == null)
            {
                return defaultDesc;
            }
            return relationships.Name;
        }

        internal static string GetParentTeacherName(EtUser user)
        {
            if (user == null)
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(user.NickName))
            {
                return user.Name;
            }
            return user.NickName;
        }

        internal static string GetParentTeacherName(NoticeUserView user)
        {
            if (user == null)
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(user.NickName))
            {
                return user.Name;
            }
            return user.NickName;
        }

        internal static string GetParentTeacherName(string name, string nickName)
        {
            if (string.IsNullOrEmpty(nickName))
            {
                return name;
            }
            return nickName;
        }

        internal static async Task<string> GetParentTeacherName(IUserDAL userDAL, long? userId)
        {
            if (userId == null)
            {
                return string.Empty;
            }
            var user = await userDAL.GetUser(userId.Value);
            if (user == null)
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(user.NickName))
            {
                return user.Name;
            }
            return user.NickName;
        }

        internal static bool CheckTenantCanLogin(SysTenant sysTenant, out string msg)
        {
            msg = string.Empty;
            if (sysTenant == null)
            {
                msg = "机构不存在,无法登陆";
                return false;
            }
            if (sysTenant.ExDate < DateTime.Now.Date)
            {
                msg = "机构已过期,无法登陆";
                return false;
            }
            if (sysTenant.Status == EmSysTenantStatus.IsLock)
            {
                msg = "机构已锁定,无法登陆";
                return false;
            }
            return true;
        }

        internal static bool CheckUserCanLogin(EtUser user, out string msg)
        {
            msg = string.Empty;
            if (user == null)
            {
                msg = "用户不存在,请重新登陆";
                return false;
            }
            if (user.JobType == EmUserJobType.Resignation)
            {
                msg = "您已离职，无法登陆";
                return false;
            }
            return true;
        }

        internal static bool CheckRoleCanLogin(RoleNoticeSettingOutput roleSetting, int clientType, out string msg)
        {
            msg = string.Empty;
            switch (clientType)
            {
                case EmUserOperationLogClientType.PC:
                    if (!roleSetting.IsAllowPCLogin)
                    {
                        msg = "您无权限登录，请联系管理员";
                        return false;
                    }
                    break;
                case EmUserOperationLogClientType.WeChat:
                    if (!roleSetting.IsAllowWebchatLogin)
                    {
                        msg = "您无权限登录，请联系管理员";
                        return false;
                    }
                    break;
                case EmUserOperationLogClientType.Android:
                    if (!roleSetting.IsAllowAppLogin)
                    {
                        msg = "您无权限登录，请联系管理员";
                        return false;
                    }
                    break;
            }
            return true;
        }

        internal static string GetDeClassTimesDesc(byte deType, decimal deClassTimes, decimal exceedClassTimes)
        {
            if (deType == EmDeClassTimesType.NotDe)
            {
                if (exceedClassTimes > 0)
                {
                    return $"记录超上{exceedClassTimes.EtmsToString()}课时";
                }
                else
                {
                    return "未扣";
                }
            }
            if (deType == EmDeClassTimesType.Day)
            {
                return "按天自动消耗";
            }
            var desc = new StringBuilder($"{deClassTimes.EtmsToString()}课时");
            if (exceedClassTimes > 0)
            {
                desc.Append($" (记录超上{exceedClassTimes.EtmsToString()}课时)");
            }
            return desc.ToString();
        }

        internal static bool CheckStudentCourseNeedRemind(List<EtStudentCourse> myStudentCourses, int studentCourseNotEnoughCount, int limitClassTimes, int limitDay)
        {
            if (myStudentCourses == null || myStudentCourses.Count == 0)
            {
                return false;
            }
            var notEnoughRemindCount = myStudentCourses[0].NotEnoughRemindCount;
            if (notEnoughRemindCount == -1) // -1 表示忽略提醒
            {
                return false;
            }
            if (notEnoughRemindCount >= studentCourseNotEnoughCount)
            {
                return false;
            }
            var lastRemindTime = myStudentCourses.FirstOrDefault(p => p.NotEnoughRemindLastTime != null);
            if (lastRemindTime != null && lastRemindTime.NotEnoughRemindLastTime.Value >= DateTime.Now.Date)
            {
                return false;
            }
            if (!myStudentCourses.Where(p => p.Status != EmStudentCourseStatus.EndOfClass).Any())
            {
                return false;
            }
            var isHasEnoughDeClassTimes = false;
            var isHasEnoughCourseDay = false;
            var deClassTimes = myStudentCourses.FirstOrDefault(p => p.DeType == EmDeClassTimesType.ClassTimes);
            if (deClassTimes != null && deClassTimes.SurplusQuantity > limitClassTimes && deClassTimes.Status != EmStudentCourseStatus.EndOfClass)
            {
                var date = DateTime.Now.AddDays(limitDay).Date;
                if (deClassTimes.EndTime == null || deClassTimes.EndTime > date)
                {
                    isHasEnoughDeClassTimes = true;
                }
            }

            var courseDay = myStudentCourses.FirstOrDefault(p => p.DeType == EmDeClassTimesType.Day);
            if (courseDay != null && (courseDay.SurplusQuantity > 0 || courseDay.SurplusSmallQuantity > limitDay)
                && courseDay.Status != EmStudentCourseStatus.EndOfClass)
            {
                isHasEnoughCourseDay = true;
            }
            if (!isHasEnoughDeClassTimes && !isHasEnoughCourseDay)
            {
                return true;
            }
            return false;
        }

        internal static bool CheckStudentCourseIsNotEnough(List<EtStudentCourse> myStudentCourses, int studentCourseNotEnoughCount, int limitClassTimes, int limitDay)
        {
            if (myStudentCourses == null || myStudentCourses.Count == 0)
            {
                return true;
            }
            if (!myStudentCourses.Where(p => p.Status != EmStudentCourseStatus.EndOfClass).Any())
            {
                return true;
            }
            var isHasEnoughDeClassTimes = false;
            var isHasEnoughCourseDay = false;
            var deClassTimes = myStudentCourses.FirstOrDefault(p => p.DeType == EmDeClassTimesType.ClassTimes);
            if (deClassTimes != null && deClassTimes.SurplusQuantity > limitClassTimes && deClassTimes.Status != EmStudentCourseStatus.EndOfClass)
            {
                var date = DateTime.Now.AddDays(limitDay).Date;
                if (deClassTimes.EndTime == null || deClassTimes.EndTime > date)
                {
                    isHasEnoughDeClassTimes = true;
                }
            }

            var courseDay = myStudentCourses.FirstOrDefault(p => p.DeType == EmDeClassTimesType.Day);
            if (courseDay != null && (courseDay.SurplusQuantity > 0 || courseDay.SurplusSmallQuantity > limitDay)
                && courseDay.Status != EmStudentCourseStatus.EndOfClass)
            {
                isHasEnoughCourseDay = true;
            }
            if (!isHasEnoughDeClassTimes && !isHasEnoughCourseDay)
            {
                return true;
            }
            return false;
        }

        internal static string GetStudentImage(string avatar, string faceKey)
        {
            if (string.IsNullOrEmpty(faceKey))
            {
                return avatar;
            }
            return faceKey;
        }

        internal static CourseCanReturnInfo GetCourseCanReturnInfo(EtOrderDetail orderDetail, EtStudentCourseDetail p)
        {
            if (orderDetail.Status == EmOrderStatus.Repeal)
            {
                return new CourseCanReturnInfo()
                {
                    BuyValidSmallQuantity = 0,
                    IsHas = false,
                    Price = 0,
                    SurplusQuantity = 0,
                    SurplusQuantityDesc = string.Empty
                };
            }
            var now = DateTime.Now.Date;
            var monthToDay = SystemConfig.ComConfig.MonthToDay;
            var tempValidSmallQuantity = 0; //按天或者课时
            if (p.BugUnit == EmCourseUnit.ClassTimes)
            {
                tempValidSmallQuantity = p.BuyQuantity + p.GiveQuantity;
            }
            else
            {
                if (p.StartTime != null && p.EndTime != null)
                {
                    tempValidSmallQuantity = (int)(p.EndTime.Value - p.StartTime.Value).TotalDays;
                }
                else
                {
                    var giveDay = 0;
                    if (p.GiveQuantity > 0)
                    {
                        if (p.GiveUnit == EmCourseUnit.Month)
                        {
                            giveDay = p.GiveQuantity * monthToDay;
                        }
                        else
                        {
                            giveDay = p.GiveQuantity;
                        }
                    }
                    if (p.BugUnit == EmCourseUnit.Month)
                    {
                        tempValidSmallQuantity = p.BuyQuantity * monthToDay + giveDay;
                    }
                    else
                    {
                        tempValidSmallQuantity = p.BuyQuantity + giveDay;
                    }
                }
                if (tempValidSmallQuantity == 0)
                {
                    tempValidSmallQuantity = 1;
                }
            }
            //计算单价，合计金额/总的有效数量(课时/天数)
            var tempCoursePrice = Math.Round(orderDetail.ItemAptSum / tempValidSmallQuantity, 2);

            var tempCourseSurplusQuantity = 0M;
            var tempCourseSurplusQuantityDesc = string.Empty;
            var tempIsHas = true;
            if (p.Status != EmStudentCourseStatus.EndOfClass &&
               (p.SurplusQuantity > 0 || p.SurplusSmallQuantity > 0))
            {
                tempCourseSurplusQuantityDesc = ComBusiness.GetSurplusQuantityDesc(p.SurplusQuantity, p.SurplusSmallQuantity, p.DeType);
                if (p.DeType == EmDeClassTimesType.ClassTimes)
                {
                    tempCourseSurplusQuantity = p.SurplusQuantity;
                }
                else
                {
                    if (p.StartTime != null && p.EndTime != null)
                    {
                        if (p.EndTime < now)
                        {
                            tempCourseSurplusQuantity = 0;
                        }
                        else if (p.StartTime.Value <= now)
                        {
                            tempCourseSurplusQuantity = (decimal)(p.EndTime.Value - now).TotalDays;
                        }
                        else
                        {
                            tempCourseSurplusQuantity = (decimal)(p.EndTime.Value - p.StartTime.Value).TotalDays;
                        }
                    }
                    else
                    {
                        tempCourseSurplusQuantity = p.SurplusQuantity * monthToDay + p.SurplusSmallQuantity;
                    }
                }
            }
            else
            {
                tempIsHas = false;
                if (p.DeType == EmDeClassTimesType.ClassTimes)
                {
                    tempCourseSurplusQuantityDesc = "0课时";
                }
                else
                {
                    tempCourseSurplusQuantityDesc = "0天";
                }
            }
            if (tempCourseSurplusQuantity <= 0)
            {
                tempIsHas = false;
            }
            return new CourseCanReturnInfo()
            {
                BuyValidSmallQuantity = tempValidSmallQuantity,
                IsHas = tempIsHas,
                Price = tempCoursePrice,
                SurplusQuantity = tempCourseSurplusQuantity,
                SurplusQuantityDesc = tempCourseSurplusQuantityDesc
            };
        }

        internal static string GetBuyCourseDesc(string courseName, byte priceUnit, int buyQuantity, int giveQuantity, byte GiveUnit)
        {
            var strDesc = new StringBuilder(courseName);
            strDesc.Append("(");
            switch (priceUnit)
            {
                case EmCourseUnit.ClassTimes:
                    strDesc.Append($"{buyQuantity}课时");
                    break;
                case EmCourseUnit.Month:
                    strDesc.Append($"{buyQuantity}个月");
                    break;
                case EmCourseUnit.Day:
                    strDesc.Append($"{buyQuantity}天");
                    break;
            }
            if (giveQuantity > 0)
            {
                if (priceUnit == EmCourseUnit.ClassTimes)
                {
                    strDesc.Append($"+{giveQuantity}课时");
                }
                else
                {
                    if (GiveUnit == EmCourseUnit.Month)
                    {
                        strDesc.Append($"+{giveQuantity}个月");
                    }
                    else
                    {
                        strDesc.Append($"+{giveQuantity}天");
                    }
                }
            }
            strDesc.Append(")");
            return strDesc.ToString();
        }

        internal static string GetBuyGoodsDesc(string goodsName, int buyQuantity)
        {
            return $"{goodsName}({buyQuantity}件)";
        }

        internal static string GetBuyCostDesc(string costName, int buyQuantity)
        {
            return $"{costName}({buyQuantity}笔)";
        }

        internal static string GetReturnCourseDesc(string courseName, byte useUnit, decimal returnCount)
        {
            switch (useUnit)
            {
                case EmCourseUnit.ClassTimes:
                    return $"{courseName}({returnCount.EtmsToString()}课时)";
                case EmCourseUnit.Month:
                    return $"{courseName}({returnCount.EtmsToString()}个月)";
                case EmCourseUnit.Day:
                    return $"{courseName}({returnCount.EtmsToString()}天)";
            }
            return string.Empty;
        }

        internal static List<string> GetParentStudentsDesc(IEnumerable<ParentStudentInfo> parentStudentInfos)
        {
            var result = new List<string>();
            if (parentStudentInfos == null || parentStudentInfos.Count() == 0)
            {
                return result;
            }
            foreach (var p in parentStudentInfos)
            {
                result.Add($"{p.Name}({p.Phone})");
            }
            return result;
        }

        internal static List<string> GetStudentsDesc(IEnumerable<StudentAccountRechargeBinderView> studentInfos)
        {
            var result = new List<string>();
            if (studentInfos == null || studentInfos.Count() == 0)
            {
                return result;
            }
            foreach (var p in studentInfos)
            {
                result.Add($"{p.StudentName}({p.StudentPhone})");
            }
            return result;
        }

        internal static string GetParentStudentsDesc2(IEnumerable<ParentStudentInfo> parentStudentInfos)
        {
            if (parentStudentInfos == null || parentStudentInfos.Count() == 0)
            {
                return string.Empty;
            }
            var str = new StringBuilder();
            foreach (var p in parentStudentInfos)
            {
                str.Append($"{p.Name},");
            }
            return str.ToString().TrimEnd(',');
        }

        internal static string GetStudentsDesc2(IEnumerable<StudentAccountRechargeBinderView> studentInfos)
        {
            if (studentInfos == null || studentInfos.Count() == 0)
            {
                return string.Empty;
            }
            var str = new StringBuilder();
            foreach (var p in studentInfos)
            {
                str.Append($"{p.StudentName},");
            }
            return str.ToString().TrimEnd(',');
        }
    }
}
