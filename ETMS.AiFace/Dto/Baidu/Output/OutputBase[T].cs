using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.AiFace.Dto.Baidu.Output
{
    public class OutputBase
    {
        public string error_code { get; set; }

        public string error_msg { get; set; }

        public string log_id { get; set; }
    }

    public class OutputBase<T> : OutputBase where T : class
    {
        public T result { get; set; }
    }
}
