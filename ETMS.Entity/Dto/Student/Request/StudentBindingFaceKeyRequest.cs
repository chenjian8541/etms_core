using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentBindingFaceKeyRequest : RequestBase
    {
        public long CId { get; set; }

        public string FaceImageBase64 { get; set; }

        /// <summary>
        /// 是否忽略其他学员
        /// </summary>
        public bool IsIgnoreSameStudent { get; set; }

        public byte HkFaceStatus { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据格式错误";
            }
            if (string.IsNullOrEmpty(FaceImageBase64))
            {
                return "请上传人脸图片";
            }
            return string.Empty;
        }
    }
}

