using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.AiFace.Dto.Baidu.Output
{
    public class TokenOutput
    {
        public string refresh_token { get; set; }

        public int expires_in { get; set; }

        public string scope { get; set; }

        public string session_key { get; set; }

        public string access_token { get; set; }

        public string session_secret { get; set; }

        public string error { get; set; }

        public string error_description { get; set; }
    }
}
