using ETMS.Entity.Common;

namespace ETMS.Entity.Dto.SysCom.Request
{
    public class SysBulletinSetReadRequest : RequestBase
    {
        public int BulletinId { get; set; }

        public override string Validate()
        {
            if (BulletinId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}