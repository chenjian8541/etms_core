using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.OpenParent.Request
{
    public class StudentOpenRegisterInitRequest : IValidate
    {
        public string Tno { get; set; }

        public string Validate()
        {
            if (string.IsNullOrEmpty(Tno))
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
