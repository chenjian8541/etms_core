using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class ActiveGrowthRecordClassAddRequest : RequestBase
    {
        public long GrowingTag { get; set; }

        public List<long> ClassIds { get; set; }

        public byte SendType { get; set; }

        public string GrowthContent { get; set; }

        public List<string> GrowthMediasKeys { get; set; }

        public override string Validate()
        {
            if (ClassIds == null || ClassIds.Count == 0)
            {
                return "请选择班级";
            }
            if (GrowingTag <= 0)
            {
                return "请选择档案类型";
            }
            if (string.IsNullOrEmpty(GrowthContent) && (GrowthMediasKeys == null || GrowthMediasKeys.Count == 0))
            {
                return "请填写成长内容";
            }
            return base.Validate();
        }
    }
}
