using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Common;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.Temp.Compare;
using ETMS.Entity.View;
using ETMS.Entity.View.Activity;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Common
{
    internal static class ComBusiness5
    {
        public static CompareValue GetCompareValue(decimal startValue, decimal endValue)
        {
            if (startValue == 0 || endValue == 0)
            {
                return new CompareValue()
                {
                    Type = EmCompareValueType.Upward,
                    Desc = "0"
                };
            }
            if (endValue > startValue)
            {
                //增长
                var diff = endValue - startValue;
                var pro = diff / startValue;
                var value = pro.EtmsPercentage2();
                return new CompareValue()
                {
                    Type = EmCompareValueType.Upward,
                    Desc = value
                };
            }
            else
            {
                var diff2 = startValue - endValue;
                var pro2 = diff2 / startValue;
                var value2 = pro2.EtmsPercentage2();
                return new CompareValue()
                {
                    Type = EmCompareValueType.Downward,
                    Desc = value2
                };
            }
        }

        /// <summary>
        /// 学员所关联的员工
        /// 老师、跟进人、学管师
        /// </summary>
        /// <param name="student"></param>
        /// <param name="myClassList"></param>
        /// <returns></returns>
        public static IEnumerable<long> GetStudentRelationUser(EtStudent student, IEnumerable<EtClass> myClassList)
        {
            List<long> result = null;
            if (myClassList != null && myClassList.Any())
            {
                var allTeacherIds = string.Join(',', myClassList.Select(p => p.Teachers));
                result = EtmsHelper.AnalyzeMuIds(allTeacherIds);
            }
            else
            {
                result = new List<long>();
            }
            //if (student.TrackUser != null)
            //{
            //    result.Add(student.TrackUser.Value);
            //}
            if (student.LearningManager != null)
            {
                result.Add(student.LearningManager.Value);
            }
            return result.Distinct();
        }

        /// <summary>
        /// 获取需要通知的员工
        /// 老师和学管师
        /// </summary>
        /// <param name="classDAL"></param>
        /// <param name="userDAL"></param>
        /// <param name="student"></param>
        /// <param name="roleNoticeTagMy"></param>
        /// <param name="roleNoticeTagAll"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<NoticeUserView>> GetNoticeUser(IClassDAL classDAL, IUserDAL userDAL, EtStudent student,
            int roleNoticeTagMy, int roleNoticeTagAll)
        {
            var noticeUser = new List<NoticeUserView>();
            //通知设置的—>关联员工
            var myClassList = await classDAL.GetStudentClass(student.Id);
            var relationUserIds = ComBusiness5.GetStudentRelationUser(student, myClassList);
            if (relationUserIds.Any())
            {
                var trelationUsers = await userDAL.GetUserAboutNotice(roleNoticeTagMy, relationUserIds);
                if (trelationUsers != null && trelationUsers.Any())
                {
                    noticeUser.AddRange(trelationUsers);
                }
            }
            //通知设置的—>全体员工
            var noticeAllUsers = await userDAL.GetUserAboutNotice(roleNoticeTagAll);
            if (noticeAllUsers.Any())
            {
                noticeUser.AddRange(noticeAllUsers);
            }
            if (noticeUser.Any())
            {
                noticeUser = noticeUser.Distinct(new ComparerNoticeUserView()).ToList();
            }
            return noticeUser;
        }

        public static Tuple<int, byte> GetActivityRouteLimit(int minCount, int maxCount, int finishCount)
        {
            if (finishCount >= maxCount)
            {
                return Tuple.Create(0, EmActivityRouteCountStatus.CompleteFull);
            }
            if (finishCount >= minCount)
            {
                return Tuple.Create(maxCount - finishCount, EmActivityRouteCountStatus.CompleteItem);
            }
            return Tuple.Create(minCount - finishCount, EmActivityRouteCountStatus.None);
        }

        public static Tuple<int, byte> GetActivityRouteLimitHaggling(int limitCount, int finishCount)
        {
            if (finishCount >= limitCount)
            {
                return Tuple.Create(0, EmActivityRouteCountStatus.CompleteFull);
            }
            return Tuple.Create(limitCount - finishCount, EmActivityRouteCountStatus.None);
        }

        public static Tuple<string, string> GetActivityPayInfo(EtActivityMain p
            , ActivityOfGroupPurchaseRuleContentView ruleContent)
        {
            if (!p.IsOpenPay)
            {
                return Tuple.Create("团价", ruleContent.Item.First().Money.EtmsToString3());
            }
            if (ruleContent.Item.Count > 1) //多阶拼团 支付定金
            {
                return Tuple.Create("定金", p.PayValue.EtmsToString3());
            }
            if (p.PayType == EmActivityPayType.Type1)
            {
                return Tuple.Create("定金", p.PayValue.EtmsToString3());
            }
            return Tuple.Create("团价", ruleContent.Item.First().Money.EtmsToString3());
        }

        public static decimal GetActivityPayInfo2(EtActivityMain p, ActivityOfGroupPurchaseRuleContentView ruleContent)
        {
            if (!p.IsOpenPay)
            {
                return 0;
            }
            if (ruleContent.Item.Count > 1) //多阶拼团 支付定金
            {
                return p.PayValue;
            }
            if (p.PayType == EmActivityPayType.Type1)
            {
                return p.PayValue;
            }
            return ruleContent.Item.First().Money;
        }

        public static Tuple<string, string> GetActivityPayInfoHaggling(EtActivityMain p)
        {
            return Tuple.Create("砍后价", p.RuleEx1);
        }

        public static string GetProgressHaggling(int limitCount, int finishCount)
        {
            if (finishCount == 0)
            {
                return "0";
            }
            return ((finishCount / Convert.ToDecimal(limitCount)) * 100).ToString("F0") ;
        }

        public static int GetSysActivityRouteItemStatus(int minCount, int maxCount, int finishCount)
        {
            if (finishCount >= maxCount)
            {
                return EmSysActivityRouteItemStatus.FinishFull;
            }
            if (finishCount >= minCount)
            {
                return EmSysActivityRouteItemStatus.FinishItem;
            }
            return EmSysActivityRouteItemStatus.Going;
        }

        public static string GetSysActivityGroupPurchaseStatusDesc(int minCount, int maxCount, int finishCount)
        {
            if (finishCount >= maxCount)
            {
                return "已满团";
            }
            if (finishCount >= minCount)
            {
                return "已成团";
            }
            return "进行中";
        }

        public static string GetSysActivityHagglingStatusDesc(int limitCount, int finishCount)
        {
            return finishCount >= limitCount ? "已完成" : "进行中";
        }

        public static string GetStudentSelfHelpRegisterUrl(string registerUrl, string tenantNo, long userId)
        {
            return string.Format(registerUrl, $"{tenantNo}_{TenantLib.GetIdEncryptUrl(userId)}");
        }

        public static string GetStudentSelfHelpRegisterUrl(string registerUrl, string tenantNo)
        {
            return string.Format(registerUrl, tenantNo);
        }
    
    }
}
