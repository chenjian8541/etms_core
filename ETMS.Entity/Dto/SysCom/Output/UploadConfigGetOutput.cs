using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.SysCom.Output
{
    public class UploadConfigGetOutput
    {
        public string Region { get; set; }

        public string AccessKeyId { get; set; }

        public string AccessKeySecret { get; set; }

        public string SecurityToken { get; set; }

        public string Bucket { get; set; }

        public string Basckey { get; set; }

        public DateTime ExTime { get; set; }

        public string BascAccessUrlHttps { get; set; }

        public int FileLimitMB { get; set; }
    }
}
