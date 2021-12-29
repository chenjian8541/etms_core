using ETMS.Entity.Common;

namespace ETMS.Entity.Dto.Open2.Request
{
    public class ShareContentGetRequest : Open2Base
    {
        public int UseType { get; set; }

        public override string Validate()
        {
            if (UseType <= 0)
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}
