using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Entity.View;
using ETMS.Utility;
using ETMS.IBusiness;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ETMS.Entity.Dto.Product.Output;
using ETMS.Entity.Dto.User.Output;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.BasicData.Output;

namespace ETMS.Business.Common
{
    internal static class ComBusiness3
    {
        internal static string GetStudentDescPC(EtStudent student)
        {
            return $"{student.Name}({student.Phone})";
        }

        internal static bool CheckStudentCourseHasSurplus(IEnumerable<EtStudentCourse> studentCourses)
        {
            if (studentCourses == null || !studentCourses.Any())
            {
                return false;
            }
            var now = DateTime.Now.Date;
            var deClassTimes = studentCourses.FirstOrDefault(p => p.DeType == EmDeClassTimesType.ClassTimes);
            if (deClassTimes != null && deClassTimes.SurplusQuantity > 0 && deClassTimes.Status != EmStudentCourseStatus.EndOfClass)
            {
                return true;
            }

            var courseDay = studentCourses.FirstOrDefault(p => p.DeType == EmDeClassTimesType.Day);
            if (courseDay != null && (courseDay.SurplusQuantity > 0 || courseDay.SurplusSmallQuantity > 0)
                && courseDay.Status != EmStudentCourseStatus.EndOfClass)
            {
                return true;
            }

            return false;
        }

        internal static ClassReservationSettingDesc GetClassReservationSettingDesc(ClassReservationSettingView rule)
        {
            var result = new ClassReservationSettingDesc();
            switch (rule.StartClassReservaLimitType)
            {
                case EmStartClassReservaLimitType.NotLimit:
                    result.RuleStartClassReservaLimitDesc = "不限制";
                    break;
                case EmStartClassReservaLimitType.LimitHour:
                    result.RuleStartClassReservaLimitDesc = $"上课前{rule.StartClassReservaLimitValue}小时内可预约";
                    break;
                case EmStartClassReservaLimitType.LimitDay:
                    result.RuleStartClassReservaLimitDesc = $"上课前{rule.StartClassReservaLimitValue}天内可预约";
                    break;
            }

            switch (rule.DeadlineClassReservaLimitType)
            {
                case EmDeadlineClassReservaLimitType.NotLimit:
                    result.RuleDeadlineClassReservaLimitDesc = "不限制";
                    break;
                case EmDeadlineClassReservaLimitType.LimitMinute:
                    result.RuleDeadlineClassReservaLimitDesc = $"上课前{rule.DeadlineClassReservaLimitValue}分钟截止预约";
                    break;
                case EmDeadlineClassReservaLimitType.LimitHour:
                    result.RuleDeadlineClassReservaLimitDesc = $"上课前{rule.DeadlineClassReservaLimitValue}小时截止预约";
                    break;
                case EmDeadlineClassReservaLimitType.LimitDay:
                    result.RuleDeadlineClassReservaLimitDesc = $"上课前{rule.DeadlineClassReservaLimitValue}天的{EtmsHelper.GetTimeDesc(rule.DeadlineClassReservaLimitDayTimeValue)}截止预约";
                    break;
            }

            switch (rule.MaxCountClassReservaLimitType)
            {
                case EmMaxCountClassReservaLimitType.NotLimit:
                    result.RuleMaxCountClassReservaLimitDesc = "不限制";
                    break;
                case EmMaxCountClassReservaLimitType.SameCourseLimit:
                    result.RuleMaxCountClassReservaLimitDesc = $"同一门课程最多可约{rule.MaxCountClassReservaLimitValue}节课";
                    break;
            }

            switch (rule.CancelClassReservaType)
            {
                case EmCancelClassReservaType.LimitMinute:
                    break;
                case EmCancelClassReservaType.LimitHour:
                    break;
                case EmCancelClassReservaType.LimitDay:
                    break;
            }

            return result;
        }

        internal static bool CheckStudentInClassTimes(EtClassTimes classTimes, long studentId)
        {
            if (!string.IsNullOrEmpty(classTimes.StudentIdsClass) && classTimes.StudentIdsClass.Split(',').FirstOrDefault(p => p == studentId.ToString()) != null)
            {
                return true;
            }
            if (!string.IsNullOrEmpty(classTimes.StudentIdsTemp) && classTimes.StudentIdsTemp.Split(',').FirstOrDefault(p => p == studentId.ToString()) != null)
            {
                return true;
            }
            if (!string.IsNullOrEmpty(classTimes.StudentIdsReservation) && classTimes.StudentIdsReservation.Split(',').FirstOrDefault(p => p == studentId.ToString()) != null)
            {
                return true;
            }
            return false;
        }

        internal static async Task<StudentAccountRechargeView> GetStudentAccountRechargeView(DataTempBox2<StudentAccountRechargeView> tempBox,
            IStudentAccountRechargeCoreBLL coreBLL, string phone, long id)
        {
            return await tempBox.GetData(id, async () =>
            {
                return await coreBLL.GetStudentAccountRechargeByPhone(phone);
            });
        }

        internal static string PhoneSecrecy(string phone, int loginClientType)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return string.Empty;
            }
            if (loginClientType == EmUserOperationLogClientType.WeChat)
            {
                try
                {
                    return Regex.Replace(phone, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[PhoneSecrecy]{phone}", ex, typeof(ComBusiness3));
                }
            }
            return phone;
        }

        internal static string GetName<T>(List<T> entitys, long? id) where T : Entity<long>, IHasName
        {
            if (id == null)
            {
                return string.Empty;
            }
            var myData = entitys.FirstOrDefault(p => p.Id == id.Value);
            return myData?.Name;
        }

        internal static List<string> GetMediasUrl(string workMedias)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(workMedias))
            {
                return result;
            }
            var myMedias = workMedias.Split('|');
            foreach (var p in myMedias)
            {
                if (!string.IsNullOrEmpty(p))
                {
                    result.Add(AliyunOssUtil.GetAccessUrlHttps(p));
                }
            }
            return result;
        }

        internal static List<PriceRuleDesc> GetPriceRuleDescs(List<EtCoursePriceRule> priceRules)
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

        internal static RoleNoticeSettingOutput AnalyzeNoticeSetting(string noticeSetting, bool isAdmin = false)
        {
            if (isAdmin)
            {
                return new RoleNoticeSettingOutput() //这里，一些接收微信消息的权限给了true，因为前端没用到暂且这么处理
                {
                    IsAllowAppLogin = true,
                    IsAllowLookStatistics = true,
                    IsReceiveInteractiveStudent = true,
                    IsStudentContractsNotArrived = true,
                    IsStudentLeaveApply = true,
                    IsTryCalssApply = true,
                    IsAllowPCLogin = true,
                    IsAllowWebchatLogin = true
                };
            }
            var output = new RoleNoticeSettingOutput()
            {
                IsAllowPCLogin = true,
                IsAllowWebchatLogin = true
            };
            if (string.IsNullOrEmpty(noticeSetting))
            {
                return output;
            }
            var settings = noticeSetting.Split(',');
            foreach (var p in settings)
            {
                if (string.IsNullOrEmpty(p))
                {
                    continue;
                }
                var temp = p.ToInt();
                switch (temp)
                {
                    case RoleOtherSetting.StudentLeaveApply:
                        output.IsStudentLeaveApply = true;
                        break;
                    case RoleOtherSetting.StudentContractsNotArrived:
                        output.IsStudentContractsNotArrived = true;
                        break;
                    case RoleOtherSetting.TryCalssApply:
                        output.IsTryCalssApply = true;
                        break;
                    case RoleOtherSetting.ReceiveInteractiveStudent:
                        output.IsReceiveInteractiveStudent = true;
                        break;
                    case RoleOtherSetting.AllowAppLogin:
                        output.IsAllowAppLogin = true;
                        break;
                    case RoleOtherSetting.AllowLookStatistics:
                        output.IsAllowLookStatistics = true;
                        break;
                    case RoleOtherSetting.AllowPCLogin:
                        output.IsAllowPCLogin = true;
                        break;
                    case RoleOtherSetting.AllowWebchatLogin:
                        output.IsAllowWebchatLogin = true;
                        break;
                    case RoleOtherSetting.StudentCheckOnWeChat:
                        output.IsStudentCheckOnWeChat = true;
                        break;
                }
            }
            return output;
        }

        internal static TenantConfigGetSimpleOutput GetTenantConfigGetSimple(TenantConfig config)
        {
            var output = new TenantConfigGetSimpleOutput()
            {
                IsCanDeDecimal = config.ClassCheckSignConfig.IsCanDeDecimal,
                IsEnableStudentCheckDeClassTimes = false
            };
            var cardCheckIn = config.StudentCheckInConfig.StudentUseCardCheckIn;
            var faceCheckIn = config.StudentCheckInConfig.StudentUseFaceCheckIn;
            if (cardCheckIn.IsRelationClassTimesCard == EmBool.True
                && cardCheckIn.RelationClassTimesCardType == EmAttendanceRelationClassTimesType.GoDeStudentCourse)
            {
                output.IsEnableStudentCheckDeClassTimes = true;
            }
            if (faceCheckIn.IsRelationClassTimesFace == EmBool.True
                && faceCheckIn.RelationClassTimesFaceType == EmAttendanceRelationClassTimesType.GoDeStudentCourse)
            {
                output.IsEnableStudentCheckDeClassTimes = true;
            }
            return output;
        }
    }
}
