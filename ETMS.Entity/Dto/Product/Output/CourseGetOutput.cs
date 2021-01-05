using ETMS.Entity.Dto.Product.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Product.Output
{
    public class CourseGetOutput
    {
        public long CId { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmCourseType"/>
        /// </summary>
        public byte Type { get; set; }

        public string StyleColor { get; set; }

        public string Remark { get; set; }

        public byte Status { get; set; }

        public int CheckPoints { get; set; }

        public CoursePriceRuleOutput CoursePriceRules { get; set; }
    }
}
