using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentCheckByFace2Request : RequestBase
    {
        public string FaceImageBase64 { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(FaceImageBase64))
            {
                return "请上传人脸图片";
            }
            return string.Empty;
        }
    }
}