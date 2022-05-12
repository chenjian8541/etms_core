using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.ExternalService.ZhuTong.Response
{
    public class SendSmsPaRes
    {
        public int code { get; set; }

        public string msg { get; set; }

        public List<SendSmsPaResSuccess> successList { get; set; }

        public List<SendSmsPaResFail> invalidList { get; set; }

        public static bool IsSuccess(SendSmsPaRes res)
        {
            return res.code == 200;
        }
    }

    public class SendSmsPaResSuccess
    {
        public string mobile { get; set; }

        public string msgId { get; set; }

        public int contNum { get; set; }
    }

    public class SendSmsPaResFail
    {
        public string mobile { get; set; }

        public string msgId { get; set; }

        public int contNum { get; set; }

        public string code { get; set; }

        public string msg { get; set; }
    }
}
