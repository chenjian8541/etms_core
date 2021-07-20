using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ETMS.Entity.Dto.Product.Request
{
    public class CourseAddRequest : RequestBase
    {
        public string Name { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmCourseType"/>
        /// </summary>
        public byte Type { get; set; }

        public string StyleColor { get; set; }

        public string Remark { get; set; }

        public string CheckPoints { get; set; }

        /// <summary>
        /// 考勤扣的课时
        /// </summary>
        public decimal StudentCheckDeClassTimes { get; set; }

        public CoursePriceRule CoursePriceRules { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "请输入课程名称";
            }
            if (CoursePriceRules == null)
            {
                return "请设置收费标准";
            }
            var msg = CoursePriceRules.Validate();
            if (!string.IsNullOrEmpty(msg))
            {
                return msg;
            }
            return string.Empty;
        }
    }
}
