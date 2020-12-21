using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentCheckByFaceRequest : RequestBase
    {
        public long StudentId { get; set; }

        public string FaceImageBase64 { get; set; }

        public bool ImageIsFaceWhite { get; set; }

        public override string Validate()
        {
            if (StudentId <= 0)
            {
                return "请选择学员";
            }
            if (string.IsNullOrEmpty(FaceImageBase64))
            {
                return "请上传人脸图片";
            }
            return string.Empty;
        }
    }
}
