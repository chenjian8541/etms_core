using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class ActiveGrowthEditRequest : RequestBase
    {
        public long CId { get; set; }

        public string GrowthContent { get; set; }

        public List<string> GrowthMediasKeys { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据格式错误";
            }
            if (string.IsNullOrEmpty(GrowthContent) && (GrowthMediasKeys == null || GrowthMediasKeys.Count == 0))
            {
                return "请填写成长内容";
            }
            if (GrowthMediasKeys != null && GrowthMediasKeys.Count > 30)
            {
                return "最多保存30个媒体文件";
            }
            return string.Empty;
        }
    }
}
