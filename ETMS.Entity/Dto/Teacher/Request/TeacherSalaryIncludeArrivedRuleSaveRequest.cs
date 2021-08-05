using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Teacher.Request
{
    public class TeacherSalaryIncludeArrivedRuleSaveRequest : RequestBase
    {
        /// <summary>
        /// 补课计入到课人次
        /// <see cref="EmBool"/>
        /// </summary>
        public byte IncludeArrivedMakeUpStudent { get; set; }

        /// <summary>
        /// 试听计入到课人次
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IncludeArrivedTryCalssStudent { get; set; }
    }
}
