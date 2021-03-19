using ETMS.Entity.Common;

namespace ETMS.Entity.Dto.Marketing.Request
{
    public class GiftDelRequest : RequestBase
    {
        public long CId { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}
