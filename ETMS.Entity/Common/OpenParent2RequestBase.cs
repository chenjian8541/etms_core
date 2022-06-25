using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Common
{
    public abstract class OpenParent2RequestBase : IValidate
    {
        public string OpenId { get; set; }

        public string Unionid { get; set; }

        public long MiniPgmUserId { get; set; }

        public virtual string Validate()
        {
            if (string.IsNullOrEmpty(OpenId))
            {
                return "请求数据格式错误";
            }
            if (MiniPgmUserId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
