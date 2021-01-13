﻿using ETMS.Entity.Database.Source;
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

namespace ETMS.Business.Common
{
    internal static class ComBusiness2
    {
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
                isHasEnoughDeClassTimes = true;
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
                tempValidSmallQuantity = p.BuyQuantity * monthToDay + giveDay;
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
    }
}
