using ETMS.Entity.Common;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class MicroWebColumnArticleDelRequest : RequestBase
    {
        public long Id { get; set; }

        public override string Validate()
        {
            if (Id <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}