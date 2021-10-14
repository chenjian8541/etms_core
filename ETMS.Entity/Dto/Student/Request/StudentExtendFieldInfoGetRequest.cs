using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentExtendFieldInfoGetRequest : RequestBase
    {
        public List<long> StudentIds { get; set; }

        public override string Validate()
        {
            if (StudentIds == null || StudentIds.Count == 0)
            {
                return "请选择学员";
            }
            if (StudentIds.Count > 100)
            {
                return "最多选择100个学员";
            }
            return base.Validate();
        }
    }
}
