using ETMS.Authority;
using ETMS.Entity.Config.Menu;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Common;
using ETMS.Entity.Dto.Student.Request;
using ETMS.Entity.Dto.User.Output;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.View;
using ETMS.Entity.View.Database;
using ETMS.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Common
{
    internal static class ComBusiness4
    {
        internal static Tuple<string, string> GetTeacherSalaryContractPerformanceSetDetailDesc(EtTeacherSalaryContractPerformanceSetDetail item, decimal bascValue)
        {
            var computeValueType = item.ComputeValueType;
            var unitTag = string.Empty;
            if (computeValueType == EmTeacherSalaryComputeValueType.Proportion)
            {
                switch (item.ComputeMode)
                {
                    case EmTeacherSalaryComputeMode.TeacherClassTimes:
                        unitTag = "元/课时";
                        break;
                    case EmTeacherSalaryComputeMode.StudentAttendeesCount:
                        unitTag = "元/人次";
                        break;
                    case EmTeacherSalaryComputeMode.StudentDeSum:
                        unitTag = "%课消比";
                        break;
                    case EmTeacherSalaryComputeMode.StudentClassTimes:
                        unitTag = "元/课时";
                        break;
                }
            }
            else
            {
                unitTag = "元";
            }
            var strDesc = new StringBuilder();
            if (bascValue > 0)
            {
                strDesc.Append($"<div class='performance_set_rule_content_item'><span class='rule_interval_value'>每节课底薪：{bascValue}元</span></div>");
            }
            strDesc.Append($"<div class='performance_set_rule_content_item'><span class='rule_interval_value'>{item.ComputeValue}{unitTag}</span></div>");
            return Tuple.Create(strDesc.ToString(), EmTeacherSalaryComputeMode.GetTeacherSalaryComputeModeDesc(item.ComputeMode));
        }

        internal static Tuple<string, string> GetTeacherSalaryContractPerformanceSetDetailDesc(List<EtTeacherSalaryContractPerformanceSetDetail> items, decimal bascValue)
        {
            if (items == null || items.Count == 0)
            {
                return Tuple.Create(string.Empty, string.Empty);
            }
            var myItem = items.OrderBy(p => p.MinLimit);
            var firstItem = myItem.First();
            var unitTag = string.Empty;
            var computeValueType = firstItem.ComputeValueType;
            if (computeValueType == EmTeacherSalaryComputeValueType.Proportion)
            {
                switch (firstItem.ComputeMode)
                {
                    case EmTeacherSalaryComputeMode.TeacherClassTimes:
                        unitTag = "元/课时";
                        break;
                    case EmTeacherSalaryComputeMode.StudentAttendeesCount:
                        unitTag = "元/人次";
                        break;
                    case EmTeacherSalaryComputeMode.StudentDeSum:
                        unitTag = "%课消比";
                        break;
                    case EmTeacherSalaryComputeMode.StudentClassTimes:
                        unitTag = "元/课时";
                        break;
                }
            }
            else
            {
                unitTag = "元";
            }
            var strDesc = new StringBuilder();
            if (bascValue > 0)
            {
                strDesc.Append($"<div class='performance_set_rule_content_item'><span class='rule_interval_value'>每节课底薪：{bascValue}元</span></div>");
            }
            if (items.Count == 1) //无梯度
            {
                strDesc.Append($"<div class='performance_set_rule_content_item'><span class='rule_interval_value'>{firstItem.ComputeValue}{unitTag}</span></div>");
            }
            else
            {
                //采用前开后闭
                foreach (var p in items)
                {
                    if (p.MinLimit == null || p.MinLimit == 0) //第一项
                    {
                        strDesc.Append($"<div class='performance_set_rule_content_item'><span class='rule_interval_desc'>0＜X≤{p.MaxLimit}</span><span class='rule_interval_value'>{p.ComputeValue}{unitTag}</span></div>");
                        continue;
                    }
                    if (p.MaxLimit == null || p.MaxLimit == 0) //最后一项
                    {
                        strDesc.Append($"<div class='performance_set_rule_content_item'><span class='rule_interval_desc'>{p.MinLimit}＜X</span><span class='rule_interval_value'>{p.ComputeValue}{unitTag}</span></div>");
                        continue;
                    }
                    strDesc.Append($"<div class='performance_set_rule_content_item'><span class='rule_interval_desc'>{p.MinLimit}＜X≤{p.MaxLimit}</span><span class='rule_interval_value'>{p.ComputeValue}{unitTag}</span></div>");
                }
            }
            return Tuple.Create(strDesc.ToString(), EmTeacherSalaryComputeMode.GetTeacherSalaryComputeModeDesc(firstItem.ComputeMode));
        }

        internal static TenantAgtPayInfoView GetTenantAgtPayInfo(SysTenant tenant)
        {
            switch (tenant.AgtPayType)
            {
                case EmAgtPayType.Lcsw:
                    return new TenantAgtPayInfoView()
                    {
                        AgtPayType = tenant.AgtPayType,
                        AgtPayDesc = "扫呗支付",
                        AgtPayDesc2 = "扫呗",
                        IsOpenAgtPay = EmLcswApplyStatus.IsSuccess(tenant.LcswApplyStatus) && tenant.LcswOpenStatus == EmBool.True
                    };
                case EmAgtPayType.Fubei:
                    return new TenantAgtPayInfoView()
                    {
                        AgtPayType = tenant.AgtPayType,
                        AgtPayDesc = "付呗支付",
                        AgtPayDesc2 = "付呗",
                        IsOpenAgtPay = true
                    };
                case EmAgtPayType.Suixing:
                    return new TenantAgtPayInfoView()
                    {
                        AgtPayType = tenant.AgtPayType,
                        AgtPayDesc = "随行付",
                        AgtPayDesc2 = "随行付",
                        IsOpenAgtPay = true
                    };
            }
            return new TenantAgtPayInfoView()
            {
                AgtPayType = tenant.AgtPayType,
                AgtPayDesc = "扫呗支付",
                AgtPayDesc2 = "扫呗",
                IsOpenAgtPay = false
            };
        }

        internal static string GetLcsTerminalTime(DateTime time)
        {
            return time.ToString("yyyyMMddHHmmss");
        }

        internal static string CheckStudentCourseIsEnough(List<EtStudentCourseDetail> myCourseDetail, string studentName, DateTime classDate, decimal deClassTimes)
        {
            if (!string.IsNullOrEmpty(studentName))
            {
                studentName = $"[{studentName}]";
            }
            if (myCourseDetail == null || !myCourseDetail.Any())
            {
                return $"学员{studentName}未购买此课程，无法点名";
            }
            var dayCourseDetail = myCourseDetail.Where(p => p.DeType == EmDeClassTimesType.Day && (p.StartTime == null || classDate >= p.StartTime)
            && (p.EndTime == null || classDate <= p.EndTime) && p.Status == EmStudentCourseStatus.Normal);
            if (!dayCourseDetail.Any())
            {
                var timesCourseDetail = myCourseDetail.Where(p => p.DeType == EmDeClassTimesType.ClassTimes && p.Status == EmStudentCourseStatus.Normal
                && (p.EndTime == null || classDate <= p.EndTime) && p.SurplusQuantity >= deClassTimes);
                if (!timesCourseDetail.Any())
                {
                    var stopTimeCourseDetail = myCourseDetail.Where(p => p.DeType == EmDeClassTimesType.ClassTimes
                    && p.Status == EmStudentCourseStatus.StopOfClass && (p.EndTime == null || classDate <= p.EndTime) && p.SurplusQuantity >= deClassTimes);
                    if (stopTimeCourseDetail.Any())
                    {
                        return $"学员{studentName}已停课，无法点名";
                    }
                    var stopDayCourseDetail = myCourseDetail.Where(p => p.DeType == EmDeClassTimesType.Day && (p.StartTime == null || classDate >= p.StartTime)
                    && (p.EndTime == null || classDate <= p.EndTime) && p.Status == EmStudentCourseStatus.StopOfClass);
                    if (stopDayCourseDetail.Any())
                    {
                        return $"学员{studentName}已停课，无法点名";
                    }
                    return $"学员{studentName}剩余课时不足，无法点名";
                }
            }
            return string.Empty;
        }

        internal static string GetClassTimesOtDesc(EtClassTimes classTimes)
        {
            return $"{classTimes.ClassOt.EtmsToDateString()}(周{EtmsHelper.GetWeekDesc(classTimes.Week)}){EtmsHelper.GetTimeDesc(classTimes.StartTime, classTimes.EndTime, "-")}";
        }

        internal static Img GetImg(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }
            return new Img()
            {
                Key = key,
                Url = AliyunOssUtil.GetAccessUrlHttps(key)
            };
        }

        internal static List<Img> GetImgs(string keys)
        {
            if (string.IsNullOrEmpty(keys))
            {
                return new List<Img>();
            }
            var imgs = new List<Img>();
            var myMedias = keys.Split('|');
            foreach (var p in myMedias)
            {
                if (!string.IsNullOrEmpty(p))
                {
                    imgs.Add(new Img(p, AliyunOssUtil.GetAccessUrlHttps(p)));
                }
            }
            return imgs;
        }

        internal static string GetMediasKey(List<Img> imgs)
        {
            if (imgs == null || imgs.Count == 0)
            {
                return string.Empty;
            }
            var temp = imgs.Select(j => j.Key);
            return string.Join('|', temp);
        }

        internal static string GetSpecContent(List<MallGoodsSpecItem> specItems)
        {
            if (specItems == null || specItems.Count == 0)
            {
                return null;
            }
            return JsonConvert.SerializeObject(new MallGoodsSpec()
            {
                SpecItems = specItems
            });
        }

        internal static string GetTagContent(List<MallGoodsTagItem> tagItems)
        {
            if (tagItems == null || tagItems.Count == 0)
            {
                return null;
            }
            return JsonConvert.SerializeObject(new MallGoodsTag()
            {
                Items = tagItems
            });
        }

        internal static List<MallGoodsSpecItem> GetSpecView(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return new List<MallGoodsSpecItem>();
            }
            var view = JsonConvert.DeserializeObject<MallGoodsSpec>(content);
            return view.SpecItems;
        }

        internal static List<MallGoodsTagItem> GetTagView(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return new List<MallGoodsTagItem>();
            }
            var view = JsonConvert.DeserializeObject<MallGoodsTag>(content);
            return view.Items;
        }

        internal static Tuple<EtOrderDetail, string> GetGoodsOrderDetail(EtGoods goods, EnrolmentGoods enrolmentGoods, string no,
            DateTime ot, int tenantId, long userId, long studentId, int orderType)
        {
            var priceRuleDesc = $"{goods.Price}元/件";
            var ruleDesc = goods.Name;
            return new Tuple<EtOrderDetail, string>(new EtOrderDetail()
            {
                OrderNo = no,
                Ot = ot,
                Price = goods.Price,
                BuyQuantity = enrolmentGoods.BuyQuantity,
                BugUnit = 0,
                DiscountType = enrolmentGoods.DiscountType,
                DiscountValue = enrolmentGoods.DiscountValue,
                GiveQuantity = 0,
                GiveUnit = 0,
                IsDeleted = EmIsDeleted.Normal,
                ItemAptSum = enrolmentGoods.ItemAptSum,
                ItemSum = (enrolmentGoods.BuyQuantity * goods.Price).EtmsToRound(),
                PriceRule = priceRuleDesc,
                ProductId = goods.Id,
                ProductType = EmProductType.Goods,
                Remark = string.Empty,
                Status = EmOrderStatus.Normal,
                TenantId = tenantId,
                UserId = userId,
                StudentId = studentId,
                OrderType = orderType
            }, ruleDesc);
        }

        internal static Tuple<EtOrderDetail, string> GetCostOrderDetail(EtCost cost, EnrolmentCost enrolmentCost, string no,
            DateTime ot, int tenantId, long userId, long studentId, int orderType)
        {
            var priceRuleDesc = $"{cost.Price}元/笔";
            var ruleDesc = cost.Name;
            return new Tuple<EtOrderDetail, string>(new EtOrderDetail()
            {
                OrderNo = no,
                Ot = ot,
                Price = cost.Price,
                BuyQuantity = enrolmentCost.BuyQuantity,
                BugUnit = 0,
                DiscountType = enrolmentCost.DiscountType,
                DiscountValue = enrolmentCost.DiscountValue,
                GiveQuantity = 0,
                GiveUnit = 0,
                IsDeleted = EmIsDeleted.Normal,
                ItemAptSum = enrolmentCost.ItemAptSum,
                ItemSum = (enrolmentCost.BuyQuantity * cost.Price).EtmsToRound(),
                PriceRule = priceRuleDesc,
                ProductId = cost.Id,
                ProductType = EmProductType.Cost,
                Remark = string.Empty,
                Status = EmOrderStatus.Normal,
                TenantId = tenantId,
                UserId = userId,
                StudentId = studentId,
                OrderType = orderType
            }, ruleDesc);
        }

        internal static int GetFubeiPayStatus(string order_status)
        {
            order_status = order_status.Trim().ToUpper();
            var status = EmLcsPayLogStatus.Unpaid;
            switch (order_status)
            {
                case "USERPAYING": //用户支付中
                    status = EmLcsPayLogStatus.Unpaid;
                    break;
                case "SUCCESS": //支付成功
                    status = EmLcsPayLogStatus.PaySuccess;
                    break;
                case "REVOKED": //已撤销
                    status = EmLcsPayLogStatus.PayFail;
                    break;
                case "CLOSED": //已关闭
                    status = EmLcsPayLogStatus.PayFail;
                    break;
                case "REVOKING": //撤销中
                    status = EmLcsPayLogStatus.PayFail;
                    break;
            }
            return status;
        }

        internal static string GetStudentCourseExDateDesc(byte status, byte deType, DateTime? startTime, DateTime? endTime)
        {
            if (status == EmStudentCourseStatus.EndOfClass)
            {
                return string.Empty;
            }
            if (deType == EmDeClassTimesType.ClassTimes)
            {
                if (endTime == null)
                {
                    return "永久有效";
                }
                else
                {
                    return $"{endTime.EtmsToDateString()} 到期";
                }
            }
            else
            {
                if (startTime == null || endTime == null)
                {
                    return "未设置";
                }
                else
                {
                    return $"{startTime.EtmsToDateString()} 到 {endTime.EtmsToDateString()}";
                }
            }
        }

        internal static byte GetStudentCourseStatus(List<StudentCourseStatusView> myAllStatus)
        {
            if (myAllStatus.Exists(p => p.Status == EmStudentCourseStatus.StopOfClass))
            {
                return EmStudentCourseStatus.StopOfClass;
            }
            if (myAllStatus.Exists(p => p.Status == EmStudentCourseStatus.Normal))
            {
                return EmStudentCourseStatus.Normal;
            }
            return EmStudentCourseStatus.EndOfClass;
        }

        internal static string GetAuthorityValueMenu(List<int> pageMenus, List<int> actionMenus, List<int> pageRouteIds)
        {
            return $"{GetAuthorityValue(pageMenus.ToArray())}|{GetAuthorityValue(actionMenus.ToArray())}|{GetAuthorityValue(pageRouteIds.ToArray())}";
        }

        /// <summary>
        /// 通过选择的菜单ID，计算权值
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        internal static string GetAuthorityValue(int[] ids)
        {
            if (ids == null || !ids.Any())
            {
                return string.Empty;
            }
            var authorityCore = new AuthorityCore();
            var weightSum = authorityCore.AuthoritySum(ids);
            return weightSum.ToString();
        }

        internal static void MenuConfigsHandle(List<MenuConfig> myMenuConfigs, AuthorityCore authorityCorePage, AuthorityCore authorityCoreAction)
        {
            foreach (var p in myMenuConfigs)
            {
                if (p.Type == MenuType.Page)
                {
                    p.IsOwner = authorityCorePage.Validation(p.Id);
                }
                else
                {
                    p.IsOwner = authorityCoreAction.Validation(p.ActionId);
                }
                if (p.ChildrenPage != null && p.ChildrenPage.Any())
                {
                    MenuConfigsHandle(p.ChildrenPage, authorityCorePage, authorityCoreAction);
                }
                if (p.ChildrenAction != null && p.ChildrenAction.Any())
                {
                    MenuConfigsHandle(p.ChildrenAction, authorityCorePage, authorityCoreAction);
                }
            }
        }

        internal static List<RoleMenuViewOutput> GetRoleMenuViewOutputs(List<MenuConfig> menuConfigs)
        {
            var output = new List<RoleMenuViewOutput>();
            var index = 1;
            foreach (var p in menuConfigs)
            {
                var item = new RoleMenuViewOutput()
                {
                    ActionCheck = new List<int>(),
                    ActionItems = new List<RoleMenuItem>(),
                    PageCheck = new List<int>(),
                    PageItems = new List<RoleMenuItem>(),
                    Index = index
                };
                index++;
                var thisPageItem = new RoleMenuItem()
                {
                    Children = new List<RoleMenuItem>(),
                    Id = p.Id,
                    Label = p.Name,
                    Type = p.Type
                };
                item.PageItems.Add(thisPageItem);
                if (p.IsOwner)
                {
                    item.PageCheck.Add(p.Id);
                }
                if (p.ChildrenPage != null && p.ChildrenPage.Any())
                {
                    AddChildrenPage(p.ChildrenPage, thisPageItem, item);
                }
                if (p.ChildrenAction != null && p.ChildrenAction.Any())
                {
                    AddChildrenCheck(p.ChildrenAction, item);
                }
                output.Add(item);
            }
            return output;
        }

        internal static void AddChildrenCheck(List<MenuConfig> ChildrenAction, RoleMenuViewOutput itemOutput)
        {
            if (itemOutput.ActionItems.Count == 0)
            {
                itemOutput.ActionItems.Add(new RoleMenuItem()
                {
                    Children = new List<RoleMenuItem>(),
                    Id = 0,
                    Label = "全选",
                    Type = MenuType.Action
                });
            }
            foreach (var p in ChildrenAction)
            {
                itemOutput.ActionItems[0].Children.Add(new RoleMenuItem()
                {
                    Id = p.ActionId,
                    Label = p.Name,
                    Type = p.Type
                });
                if (p.IsOwner)
                {
                    itemOutput.ActionCheck.Add(p.ActionId);
                }
            }
        }

        internal static void AddChildrenPage(List<MenuConfig> childrenPage, RoleMenuItem item, RoleMenuViewOutput itemOutput)
        {
            foreach (var p in childrenPage)
            {
                var thisRoleMenuItem = new RoleMenuItem()
                {
                    Children = new List<RoleMenuItem>(),
                    Id = p.Id,
                    Label = p.Name,
                    Type = p.Type
                };
                item.Children.Add(thisRoleMenuItem);
                if (p.IsOwner)
                {
                    itemOutput.PageCheck.Add(p.Id);
                }
                if (p.ChildrenPage != null && p.ChildrenPage.Any())
                {
                    AddChildrenPage(p.ChildrenPage, thisRoleMenuItem, itemOutput);
                }
                if (p.ChildrenAction != null && p.ChildrenAction.Any())
                {
                    AddChildrenCheck(p.ChildrenAction, itemOutput);
                }
            }
        }

    }
}
