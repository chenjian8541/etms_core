using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.View
{
    public class ClassCanChooseView
    {
        public long Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 学员个数
        /// </summary>
        public int StudentNums { get; set; }

        /// <summary>
        /// 班级容量
        /// </summary>
        public int? LimitStudentNums { get; set; }

        /// <summary>
        /// 班级容量类型
        /// <see cref="ETMS.Entity.Enum.EmLimitStudentNumsType"/>
        /// </summary>
        public byte LimitStudentNumsType { get; set; }

        /// <summary>
        /// 老师，各老师Id之间以“,”隔开
        /// </summary>
        public string Teachers { get; set; }
    }
}
