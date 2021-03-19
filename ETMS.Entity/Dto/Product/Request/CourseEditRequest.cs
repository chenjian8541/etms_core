using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ETMS.Entity.Dto.Product.Request
{
    public class CourseEditRequest : RequestBase
    {
        public long CId { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmCourseType"/>
        /// </summary>
        //public byte Type { get; set; }

        public string StyleColor { get; set; }

        public string Remark { get; set; }

        public string CheckPoints { get; set; }

        public CoursePriceRule CoursePriceRules { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据格式错误";
            }
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
