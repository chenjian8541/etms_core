using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Teacher.Request
{
    public class TeacherSalaryFundsItemsGetRequest : RequestBase
    {
        public bool IsGetDisable { get; set; }
    }
}
