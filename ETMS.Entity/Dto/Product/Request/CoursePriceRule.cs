using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ETMS.Entity.Dto.Product.Request
{
    public class CoursePriceRule : IValidate
    {
        public bool IsByClassTimes { get; set; }

        public List<CoursePriceRuleItem> ByClassTimes { get; set; }

        public bool IsByMonth { get; set; }

        public List<CoursePriceRuleItem> ByMonth { get; set; }

        public string Validate()
        {
            if (!IsByClassTimes && !IsByMonth)
            {
                return "请设置收费标准";
            }
            if (IsByClassTimes)
            {
                if (ByClassTimes == null || !ByClassTimes.Any())
                {
                    return "请正确设置收费标准";
                }
                if (ByClassTimes.Count > 10)
                {
                    return "最多允许设置10种收费方式";
                }
                foreach (var rule in ByClassTimes)
                {
                    var msg = rule.Validate();
                    if (!string.IsNullOrEmpty(msg))
                    {
                        return msg;
                    }
                }
            }
            if (IsByMonth)
            {
                if (ByMonth == null || !ByMonth.Any())
                {
                    return "请正确设置收费标准";
                }
                if (ByMonth.Count > 10)
                {
                    return "最多允许设置10种收费方式";
                }
                foreach (var rule in ByMonth)
                {
                    var msg = rule.Validate();
                    if (!string.IsNullOrEmpty(msg))
                    {
                        return msg;
                    }
                }
            }
            return string.Empty;
        }
    }
}
