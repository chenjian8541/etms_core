using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class StudentRecommendRuleGetOutput
    {
        /// <summary>
        /// 规则描述(文字)
        /// </summary>
        public string RecommendDesText { get; set; }

        /// <summary>
        /// 规则描述(图片)
        /// </summary>
        public string RecommendDesImgUrl { get; set; }
    }
}
