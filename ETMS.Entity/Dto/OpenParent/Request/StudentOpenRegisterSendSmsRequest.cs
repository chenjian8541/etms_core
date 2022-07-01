using ETMS.Entity.Common;

namespace ETMS.Entity.Dto.OpenParent.Request
{
    public class StudentOpenRegisterSendSmsRequest : IValidate
    {
        public string Tno { get; set; }

        public string Phone { get; set; }

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
