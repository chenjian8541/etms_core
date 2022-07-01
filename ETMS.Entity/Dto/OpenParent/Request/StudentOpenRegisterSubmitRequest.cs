using ETMS.Entity.Common;
using ETMS.Entity.Dto.Student.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.OpenParent.Request
{
    public class StudentOpenRegisterSubmitRequest : IValidate
    {
        public string Tno { get; set; }

        public string Phone { get; set; }

        public string Name { get; set; }

        public string SmsCode { get; set; }

        public string Address { get; set; }

        public string Remark { get; set; }

        public List<StudentExtendItem> StudentExtendItems { get; set; }

        public string Validate()
        {
            if (string.IsNullOrEmpty(Tno))
            {
                return "请求数据格式错误";
            }
            if (string.IsNullOrEmpty(Phone))
            {
                return "请输入手机号码";
            }
            return string.Empty;
        }
    }
}
