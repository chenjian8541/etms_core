using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Output
{
    public class ClassTimesRuleGetOutput
    {
        public string RuleDesc { get; set; }

        public string DateDesc { get; set; }

        public string TimeDesc { get; set; }

        public string CourseDesc { get; set; }

        public string TeachersDesc { get; set; }

        public string ClassContent { get; set; }

        public string ClassRoomDesc { get; set; }

        public long ClassId { get; set; }

        public long RuleId { get; set; }

        /// <summary>
        /// 是否跳过节假日
        /// </summary>
        public bool IsJumpHoliday { get; set; }

        /// <summary>
        /// 预约类型  <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte ReservationType { get; set; }

        /// <summary>
        /// 数据类型 <see cref="ETMS.Entity.Enum.EmClassTimesDataType"/>
        /// </summary>
        public byte DataType { get; set; }

        public string DataTypeDesc { get; set; }
    }
}
