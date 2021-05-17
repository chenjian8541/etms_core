using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Temp
{
    public class TempStudentTypeCount
    {
        /// <summary>
        /// 学员类型   <see cref="ETMS.Entity.Enum.EmStudentType"/>
        /// </summary>
        public byte StudentType { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int MyCount { get; set; }
    }
}
