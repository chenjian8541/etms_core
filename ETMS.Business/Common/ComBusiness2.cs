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
            var deClassTimes = myStudentCourses.FirstOrDefault(p => p.DeType == EmDeClassTimesType.ClassTimes);
            if (deClassTimes != null && deClassTimes.SurplusQuantity <= limitClassTimes)
            {
                return true;
            }
            var courseDay = myStudentCourses.FirstOrDefault(p => p.DeType == EmDeClassTimesType.Day);
            if (courseDay != null && courseDay.SurplusQuantity == 0 && courseDay.SurplusSmallQuantity <= limitDay)
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
    }
}
