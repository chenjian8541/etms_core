using ETMS.Entity.Dto.Product.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.Tenant.Output
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
}
