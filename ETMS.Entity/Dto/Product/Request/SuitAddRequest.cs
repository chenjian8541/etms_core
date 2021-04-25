using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETMS.Entity.Dto.Product.Request
{
    public class SuitAddRequest : RequestBase
    {
        public string Name { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public int Points { get; set; }

        public string Remark { get; set; }

        public List<SuitCourseInput> SuitCourse { get; set; }

        public List<SuitGoodsInput> SuitGoods { get; set; }

        public List<SuitCostInput> SuitCost { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "请输入套餐名称";
            }
            if ((SuitCourse == null || !SuitCourse.Any())
                && (SuitGoods == null || !SuitGoods.Any())
                && (SuitCost == null || !SuitCost.Any()))
            {
                return "请选择要添加的套餐内容";
            }
            if (SuitCourse != null && SuitCourse.Any())
            {
                foreach (var p in SuitCourse)
                {
                    var msg = p.Validate();
                    if (!string.IsNullOrEmpty(msg))
                    {
                        return msg;
                    }
                }
            }
            if (SuitGoods != null && SuitGoods.Any())
            {
                foreach (var p in SuitGoods)
                {
                    var msg = p.Validate();
                    if (!string.IsNullOrEmpty(msg))
                    {
                        return msg;
                    }
                }
            }
            if (SuitCost != null && SuitCost.Any())
            {
                foreach (var p in SuitCost)
                {
                    var msg = p.Validate();
                    if (!string.IsNullOrEmpty(msg))
                    {
                        return msg;
                    }
                }
            }

            return base.Validate();
        }
    }
}
