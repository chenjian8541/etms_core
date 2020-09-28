using ETMS.Entity.Common;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesRuleDelRequest : RequestBase
    {
        public long ClassId { get; set; }

        public long RuleId { get; set; }

        public override string Validate()
        {
            if (ClassId <= 0 || RuleId <= 0)
            {
                return "请求数据不合法";
            }
            return string.Empty;
        }
    }
}