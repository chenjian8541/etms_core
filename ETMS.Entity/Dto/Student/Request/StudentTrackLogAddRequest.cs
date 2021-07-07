using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentTrackLogAddRequest : RequestBase
    {
        public long StudentId { get; set; }

        public string TrackContent { get; set; }

        public List<string> TrackImgKey { get; set; }

        public DateTime? NextTrackTime { get; set; }

        public override string Validate()
        {
            if (StudentId <= 0)
            {
                return "请求数据格式错误";
            }
            if (string.IsNullOrEmpty(TrackContent))
            {
                return "请输入跟进内容";
            }
            if (TrackImgKey != null && TrackImgKey.Count > 10)
            {
                return "最多上传10张图片";
            }
            return base.Validate();
        }
    }
}
