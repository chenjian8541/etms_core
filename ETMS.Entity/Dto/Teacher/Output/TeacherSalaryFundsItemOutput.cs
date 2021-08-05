using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Teacher.Output
{
    public class TeacherSalaryFundsItemOutput
    {
        public long Id { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryFundsItemsType"/>
        /// </summary>
        public byte Type { get; set; }

        public string TypeDesc { get; set; }

        public string Name { get; set; }

        public int OrderIndex { get; set; }

        /// <summary>
        /// <see cref="EmBool"/>
        /// 启用状态
        /// </summary>
        public byte Status { get; set; }
    }
}
