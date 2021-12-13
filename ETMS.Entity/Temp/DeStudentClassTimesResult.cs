using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Temp
{
    public class DeStudentClassTimesResult
    {
        /// <summary>
        /// 课消金额
        /// </summary>
        public decimal DeSum { get; set; }

        /// <summary>
        /// 课消类型
        /// </summary>
        public byte DeType { get; set; }

        /// <summary>
        /// 超上课时
        /// </summary>
        public decimal ExceedClassTimes { get; set; }

        public string OrderNo { get; set; }

        public long? OrderId { get; set; }

        public long? DeStudentCourseDetailId { get; set; }

        public decimal DeClassTimes { get; set; }

        public string Remrak { get; set; }

        public long DeCourseId { get; set; }

        public decimal Price { get; set; }

        public static DeStudentClassTimesResult GetNotDeEntity()
        {
            return new DeStudentClassTimesResult()
            {
                DeSum = 0,
                DeType = EmDeClassTimesType.NotDe,
                ExceedClassTimes = 0
            };
        }
    }
}
