using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesDelClassStudentRequest : RequestBase
    {
        public long ClassTimesId { get; set; }

        public long StudentId { get; set; }

        public string StudentName { get; set; }
        public override string Validate()
        {
            if (ClassTimesId <= 0 || StudentId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
