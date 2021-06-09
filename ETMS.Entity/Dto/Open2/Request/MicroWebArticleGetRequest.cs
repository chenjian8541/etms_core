using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Open2.Request
{
    public class MicroWebArticleGetRequest : Open2Base
    {
        public long ArticleId { get; set; }

        public override string Validate()
        {
            if (ArticleId <= 0)
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}
