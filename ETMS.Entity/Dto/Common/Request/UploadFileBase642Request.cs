using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Common.Request
{
    public class UploadFileBase642Request : RequestBase
    {
        public string FileData { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Dto.Parent.Request.UploadFileType"/>
        /// </summary>
        public byte FileType { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(FileData))
            {
                return "请选择需要上传的文件";
            }
            return base.Validate();
        }
    }
}
