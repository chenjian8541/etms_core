using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Parent5.Output
{
    public class StudentTryClassGetOutput
    {
        public long Id { get; set; }

        public string TitleDesc { get; set; }

        public string ClassOt { get; set; }

        public string CourseDesc { get; set; }

        /// <summary>
        /// 处理状态  <see cref="ETMS.Entity.Enum.EmTryCalssApplyHandleStatus"/>
        /// </summary>
        public byte HandleStatus { get; set; }

        public DateTime ApplyOt { get; set; }
    }
}
