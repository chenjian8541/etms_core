using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Teacher.Output
{
    public class TeacherSalaryContractGetPagingTableHeadOutput
    {
        public long Id { get; set; }

        public string Label { get; set; }

        public int Index { get; set; }

        public string Property { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryFundsItemsType"/>
        /// </summary>
        public byte Type { get; set; }
    }
}
