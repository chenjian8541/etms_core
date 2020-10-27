using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class UploadFileBase64Request : ParentRequestBase
    {
        public string FileData { get; set; }

        public byte FileType { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(FileData))
            {
                return "图片信息不正确";
            }
            return base.Validate();
        }
    }

    public struct UploadFileType
    {
        public const byte Image = 0;

        public const byte Video = 1;

        public const byte Audio = 2;
    }
}
