using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class StudentPointsLogGetOutput
    {
        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// 学员名称
        /// </summary>
        public string StudentName { get; set; }

        /// <summary>
        /// 类型  <see cref=" ETMS.Entity.Enum.EmStudentPointsLogType"/>
        /// </summary>
        public int Type { get; set; }

        public string TypeDesc { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public string PointsDesc { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Ot { get; set; }
    }
}
