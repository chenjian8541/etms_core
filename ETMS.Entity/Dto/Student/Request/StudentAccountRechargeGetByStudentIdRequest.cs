using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentAccountRechargeGetByStudentIdRequest : RequestBase
    {
        public long StudentId { get; set; }

        public bool IsGetRelationStudent { get; set; }

        public override string Validate()
        {
            if (StudentId <= 0)
            {
                return "数据不合法";
            }
            return base.Validate();
        }
    }
}

