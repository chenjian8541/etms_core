using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Teacher.Output
{
    public class TeacherSalaryPayrollGetOutput
    {
        public TeacherSalaryPayrollInfo BascInfo { get; set; }

        public List<PagingTableHeadOutput> PayrollUserTableHeads { get; set; }

        public List<TeacherSalaryPayrollUserInfo> PayrollUserList { get; set; }
    }

    public class TeacherSalaryPayrollInfo
    {
        public long CId { get; set; }

        public string Name { get; set; }

        public string DateDesc { get; set; }

        public int UserCount { get; set; }

        public string PayDateDesc { get; set; }

        public decimal PaySum { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryPayrollStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string StatusDesc { get; set; }

        public string OtDesc { get; set; }

        public long OpUserId { get; set; }

        public string OpUserName { get; set; }
    }
}
