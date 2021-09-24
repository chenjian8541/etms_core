using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class ActiveGrowthRecordStudentAddRequest : RequestBase
    {
        public long GrowingTag { get; set; }

        public List<long> StudentIds { get; set; }

        public byte SendType { get; set; }

        public string GrowthContent { get; set; }

        public List<string> GrowthMediasKeys { get; set; }

        public override string Validate()
        {
            if (StudentIds == null || StudentIds.Count == 0)
            {
                return "请选择学员";
            }
            if (GrowingTag <= 0)
            {
                return "请选择档案类型";
            }
            if (string.IsNullOrEmpty(GrowthContent) && (GrowthMediasKeys == null || GrowthMediasKeys.Count == 0))
            {
                return "请填写成长内容";
            }
            if (GrowthMediasKeys != null && GrowthMediasKeys.Count > 30)
            {
                return "最多保存30个媒体文件";
            }
            return base.Validate();
        }
    }
}