using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Common
{
    internal static class ComBusiness4
    {
        internal static Tuple<string, string> GetTeacherSalaryContractPerformanceSetDetailDesc(List<EtTeacherSalaryContractPerformanceSetDetail> items)
        {
            if (items == null || items.Count == 0)
            {
                return Tuple.Create(string.Empty, string.Empty);
            }
            var myItem = items.OrderBy(p => p.MinLimit);
            var firstItem = myItem.First();
            var unitTag = string.Empty;
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
            var strDesc = new StringBuilder();
            if (items.Count == 1) //无梯度
            {
                strDesc.Append($"<div class='performance_set_rule_content_item'>{firstItem.ComputeValue}{unitTag}</div>");
            }
            else
            {
                //采用前开后闭
                foreach (var p in items)
                {
                    if (p.MinLimit == null || p.MinLimit == 0) //第一项
                    {
                        strDesc.Append($"<div class='performance_set_rule_content_item'><span class='rule_interval_desc'>0＜X≤{p.MaxLimit}<span>{p.ComputeValue}{unitTag}</div>");
                        continue;
                    }
                    if (p.MaxLimit == null || p.MaxLimit == 0) //最后一项
                    {
                        strDesc.Append($"<div class='performance_set_rule_content_item'><span class='rule_interval_desc'>{p.MinLimit}＜X<span>{p.ComputeValue}{unitTag}</div>");
                        continue;
                    }
                    strDesc.Append($"<div class='performance_set_rule_content_item'><span class='rule_interval_desc'>{p.MinLimit}＜X≤{p.MaxLimit}<span>{p.ComputeValue}{unitTag}</div>");
                }
            }
            return Tuple.Create(strDesc.ToString(), EmTeacherSalaryComputeMode.GetTeacherSalaryComputeModeDesc(firstItem.ComputeMode)); ;
        }
    }
}
