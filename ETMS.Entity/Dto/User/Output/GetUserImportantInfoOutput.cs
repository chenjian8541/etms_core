using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.User.Output
{
    public class GetUserImportantInfoOutput
    {
        public string SalaryThisMonth { get; set; }

        public bool IsShowTeacherSalary { get; set; }
    }
}
