using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class StudentRecommendConfigSave2Request : RequestBase
    {
        /// <summary>
        /// 规则描述(文字)
        /// </summary>
        public string RecommendDesText { get; set; }

        /// <summary>
        /// 规则描述(图片)
        /// </summary>
        public string RecommendDesImg { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(RecommendDesImg) && string.IsNullOrEmpty(RecommendDesText))
            {
                return "请填写规则说明";
            }
            return base.Validate();
        }
    }
}
