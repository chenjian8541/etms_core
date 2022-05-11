using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Parent5.Request
{
    public class StudentReservation1v1InitRequest : ParentRequestBase
    {
        public long StudentId { get; set; }

        public override string Validate()
        {
            if (StudentId <= 0)
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}
