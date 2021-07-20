using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Product.Output
{
    public class CourseGetPagingOutput
    {
        public long CId { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmCourseType"/>
        /// </summary>
        public byte Type { get; set; }

        public string TypeDesc { get; set; }

        public byte Status { get; set; }

        public string StatusDesc { get; set; }

        public byte PriceType { get; set; }

        public string PriceTypeDesc { get; set; }

        public string Remark { get; set; }

        public List<PriceRuleDesc> PriceRuleDescs { get; set; }

        public long Value { get; set; }

        public string Label { get; set; }

        /// <summary>
        /// 考勤扣的课时
        /// </summary>
        public decimal StudentCheckDeClassTimes { get; set; }

        public int CheckPoints { get; set; }
    }

    public class PriceRuleDesc
    {
        public byte PriceType { get; set; }

        public string Desc { get; set; }

        public string PriceTypeDesc { get; set; }

        public long CId { get; set; }
        public CoursePriceRuleOut RuleValue { get; set; }
    }

    public class CoursePriceRuleOut
    {
        public long CId { get; set; }

        /// <summary>
        /// 收费类型 <see cref="ETMS.Entity.Enum.EmCoursePriceType"/>
        /// </summary>
        public byte PriceType { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 单价单位  <see cref="ETMS.Entity.Enum.EmCourseUnit"/>
        /// </summary>
        public byte PriceUnit { get; set; }

        public int Points { get; set; }
    }
}
